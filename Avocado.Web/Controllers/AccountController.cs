using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avocado.Domain.Abstract;
using System.Web.Security;
using Avocado.Web.Utilities;
using Avocado.Web.Models;
using Twitterizer;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;
using Facebook;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace Avocado.Web.Controllers
{
    public class AccountController : Controller
    {
        private IAccountMembershipService _membershipService;
        private IFormsAuthenticationService _authenticationService;

        public AccountController(IAccountMembershipService membershipService, IFormsAuthenticationService authenticationService)
        {
            _membershipService = membershipService;
            _authenticationService = authenticationService;
        }

        public ViewResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(SignInViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_authenticationService.AuthenticateUser(model.SignInUserName, model.Password))
                {
                    //get accountId for this user
                    int accountId = _membershipService.GetAccountId(model.SignInUserName);
                    //create the authTicket that will hold user data
                    string ticket = model.SignInUserName + "|" + accountId;

                    _authenticationService.LogIn(ticket, createPersistentCookie: true);


                    if (!String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect username or password");
                    return View();
                }
            }
            else
                return View();
        }

        /// <summary>
        /// Check if facebook or twitter id has been registered on our site yet
        /// If not, send to registration page to create an account
        /// otherwise, sign in
        /// </summary>
        /// <param name="socialId"></param>
        /// <returns></returns>
        public ActionResult LogInWithSocial(string socialId, string socialNetwork, string accessToken = null)
        {
            if (_membershipService.IsLinkedWithSocial(socialId))
            {
                //get a user auth ticket based on socialId
                string ticket = _membershipService.GetAuthTicketFromSocialId(socialId);

                _authenticationService.LogIn(ticket, createPersistentCookie: true);
            }
            else
            {
                if (socialNetwork == "Facebook")
                {
                    CreateAccountViewModel newAccountModel = new CreateAccountViewModel();
                    var client = new FacebookClient(accessToken);
                    dynamic me = client.Get("me");
                    newAccountModel.FirstName = me.first_name;
                    newAccountModel.LastName = me.last_name;
                    newAccountModel.Email = me.email;
                    newAccountModel.FacebookId = me.id;

                    //redirect to facebook registration page
                    return View("RegisterWithFacebook", newAccountModel);
                }
                else
                {
                    return RedirectToAction("CreateAccount", "Account");
                }
            }
            
            return RedirectToAction("Index", "Browse");
        }

        public ViewResult RegisterWithFacebook(CreateAccountViewModel model)
        {
            return View(model);
        }

        /// <summary>
        /// Facebook registration redirects here.
        /// Pull the registration data and create registration for MTA
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifyFacebookRegistration()
        {
            var signedRequest = Request.Form["signed_request"];
            var fap = FacebookApplication.Current; //the Facebook app seceret & id are in the web.config
            var signedRequestObject = FacebookSignedRequest.Parse(fap, signedRequest);
            JObject jData = new JObject();

            if (signedRequestObject != null)
            {
                jData = JObject.Parse(signedRequestObject.Data.ToString());
            }

            CreateAccountViewModel model = new CreateAccountViewModel();
            string json = jData["registration"].ToString();
            Dictionary<string, string> RegistrationValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            model.Email = RegistrationValues["email"];
            model.Password = RegistrationValues["password"];
            model.UserName = RegistrationValues["username"];
            model.FirstName = RegistrationValues["first_name"];
            model.LastName = RegistrationValues["last_name"];
            model.FacebookId = (string)jData.SelectToken("user_id");

            //register the new account in our system
            return CreateAccount(model);
        }

        public ActionResult SignInWithTwitter()
        {
            string consumerKey = ConfigurationManager.AppSettings["consumerKey"];
            string consumerSecrect = ConfigurationManager.AppSettings["consumerSecret"];
            OAuthTokenResponse reqToken = OAuthUtility.GetRequestToken(consumerKey, consumerSecrect, "http://localhost:58828/Account/TwitterAuth");

            string twitterUrl = "https://api.twitter.com/oauth/authenticate?oauth_token=" + reqToken.Token;

            return Redirect(twitterUrl);
        }

        public ActionResult TwitterAuth()
        {
            if (Request.QueryString["denied"] == null)
            {
                string consumerKey = ConfigurationManager.AppSettings["consumerKey"];
                string consumerSecret = ConfigurationManager.AppSettings["consumerSecret"];
                string requestToken = Request.QueryString["oauth_token"];
                string requestVerifier = Request.QueryString["oauth_verifier"];

                OAuthTokenResponse responseToken = OAuthUtility.GetAccessToken(consumerKey, consumerSecret, requestToken, requestVerifier);

                string accessToken = responseToken.Token;
                string accessSecret = responseToken.TokenSecret;

                if (_membershipService.IsLinkedWithSocial(Convert.ToString(responseToken.UserId)))
                {
                    string email = _membershipService.GetEmailFromSocialId(Convert.ToString(responseToken.UserId));
                    FormsAuthentication.SetAuthCookie(email, false);
                }
                else
                {
                    string redirect = "/Account/CreateAccount?social=twitter&at=" + accessToken + "&as=" + accessSecret;
                    return Redirect(redirect);
                }
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //return user back to login screen
                return View("LogIn");
            }
        }

        [HttpPost]
        public ActionResult ModalLogIn(SignInViewModel model)
        {
            var data = new { name = model.SignInUserName, password = model.Password };
            return Json(data);
        }

        public ViewResult CreateAccount()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            _authenticationService.LogOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult CreateAccount(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                string social = Request.QueryString["social"];
                string token = Request.QueryString["at"];
                string secret = Request.QueryString["as"];

                MembershipCreateStatus createStatus = _membershipService.CreateUser(model.Email, model.Password, model.UserName, social, token, secret, model.FirstName, model.LastName, model.FacebookId, model.TwitterId);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    string ticket = _membershipService.GetAuthTicketFromEmail(model.Email);
                    _authenticationService.LogIn(ticket: ticket, createPersistentCookie: true);
                    return RedirectToAction("Index", "Browse");
                }
                else
                {
                    ModelState.AddModelError("", ValidationHelpers.AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            return View("CreateAccount", model);
        }

        public JsonResult CheckUserNameAvailability(string userName)
        {
            if (!_membershipService.IsUserNameAvailable(userName))
            {
                var jsonData = new { error = "taken" };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var jsonData = new { error = "" };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
