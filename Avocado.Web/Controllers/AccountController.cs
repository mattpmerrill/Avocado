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
        private IAuthorService _authorService;

        public AccountController(IAccountMembershipService membershipService, IFormsAuthenticationService authenticationService, IAuthorService authorService)
        {
            _membershipService = membershipService;
            _authenticationService = authenticationService;
            _authorService = authorService;
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
                if (_authenticationService.AuthenticateUser(model.SignInUserName, model.SignInPassword))
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
                    newAccountModel.Gender = me.gender;
                    newAccountModel.locale = me.locale;
                    if (me.location != null)
                    {
                        newAccountModel.City = ((String)me.location["name"]).Split(',')[0].Trim();
                        newAccountModel.State = ((String)me.location["name"]).Split(',')[1].Trim();
                    }

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
                string twitterUserId = Convert.ToString(responseToken.UserId);

                if (_membershipService.IsLinkedWithSocial(Convert.ToString(responseToken.UserId)))
                {
                    string ticket = _membershipService.GetAuthTicketFromSocialId(Convert.ToString(responseToken.UserId));
                    _authenticationService.LogIn(ticket, createPersistentCookie: true);
                }
                else
                {
                    //create a new account based on Twitter account
                    string redirect = "/Account/CreateAccount?socialNetwork=twitter&at=" + accessToken + "&as=" + accessSecret + "&uid=" + twitterUserId;
                    return Redirect(redirect);
                }
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //return user back to login screen
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult ModalLogIn(SignInViewModel model)
        {
            var data = new { name = model.SignInUserName, password = model.SignInPassword };
            return Json(data);
        }

        public ViewResult CreateAccount()
        {
            string socialNetwork = Request.QueryString["socialNetwork"];

            if (socialNetwork == "twitter")
            {
                string token = Request.QueryString["at"];
                string secret = Request.QueryString["as"];
                string twitterUserId = Request.QueryString["uid"];

                CreateAccountViewModel model = new CreateAccountViewModel();
                model = GetTwitterUserData(model, token, secret, twitterUserId);
                return View(model);
            }
            else
            {
                return View();
            }
        }

        public ActionResult LogOut()
        {
            _authenticationService.LogOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult CreateAccount(CreateAccountViewModel model)
        {
            string socialNetwork = Request.QueryString["socialNetwork"];
            string token = Request.QueryString["at"];
            string secret = Request.QueryString["as"];
            string twitterUserId = Request.QueryString["uid"];

            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus = _membershipService.CreateUser(model.Email, model.Password, model.UserName, socialNetwork, token, secret, model.FirstName, model.LastName, model.FacebookId, model.TwitterId, model.Gender, model.City, model.State, model.locale);

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

            //If error occured during account creation return to the registration page with model errors displayed
            if (socialNetwork == "Facebook")
            {
                return View("RegisterWithFacebook", model);
            }
            else
            {
                return View("CreateAccount", model);
            }
        }

        private CreateAccountViewModel GetTwitterUserData(CreateAccountViewModel model, string token, string secret, string twitterUserId)
        {
            OAuthTokens tokens = new OAuthTokens();
            tokens.AccessToken = token;
            tokens.AccessTokenSecret = secret;
            tokens.ConsumerKey = ConfigurationManager.AppSettings["consumerKey"];
            tokens.ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"];

            TwitterResponse<TwitterUser> user = TwitterUser.Show(tokens, Convert.ToDecimal(twitterUserId));

            model.TwitterId = twitterUserId;
            model.UserName = user.ResponseObject.ScreenName;
            
            return model;
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

        public ViewResult EditProfile()
        {
            ProfileViewModel model = new ProfileViewModel();
            var account = _membershipService.GetAccount(User.Identity.Name.Split('|')[0]);
            var profile = _membershipService.GetProfile(Convert.ToInt32(User.Identity.Name.Split('|')[1]));

            if (account != null)
            {
                model.ProfileImage = account.ProfileImage;
                model.FirstName = account.FirstName;
                model.LastName = account.LastName;
            }

            if (profile != null)
            {
                model.Bio = profile.Bio;
                model.PersonalUrl = profile.PersonalUrl;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateProfile(ProfileViewModel model)
        {
            int accountId = Convert.ToInt32(User.Identity.Name.Split('|')[1]);
            if (_membershipService.IsProfileUpdated(accountId, model.FirstName, model.LastName, model.ProfileImage.Replace(" ", ""), model.Bio, model.PersonalUrl))
                return Json(new { success = true, message = "Your profile has been updated" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { success = false, message = "An error occurred while trying to update your profile" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFiles(string qqfile)
        {
            string userName = User.Identity.Name.Split('|')[0];
            string newFilePath = string.Empty;
            string newLittleFilePath = string.Empty;
            //var path = @"C:\\Temp\\100\\";
            //var file = string.Empty;

            try
            {
                var stream = Request.InputStream;
                if (String.IsNullOrEmpty(Request["qqfile"]))
                {
                    // IE
                    HttpPostedFileBase postedFile = Request.Files[0];
                    stream = postedFile.InputStream;
                    //file = Path.Combine(path, System.IO.Path.GetFileName(Request.Files[0].FileName));
                }
                else
                {
                    //Webkit, Mozilla
                    //file = Path.Combine(path, qqfile);
                }

                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                if (stream.Length > 0)
                {
                    stream.Position = 0;
                    newFilePath = _authorService.SaveProfileImage(userName, stream, qqfile, "profilePics");
                }

                //System.IO.File.WriteAllBytes(file, buffer);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, "application/json");
            }

            newFilePath = ConfigurationManager.AppSettings["AzureStorageUri"] + userName + "/" + newFilePath;
            newLittleFilePath = ConfigurationManager.AppSettings["AzureStorageUri"] + userName + "/thumb/profile-pic";

            return Json(new { success = true, message = "sweet", imgPath = newFilePath, littleImgPath = newLittleFilePath}, JsonRequestBehavior.AllowGet);
        }
    }
}
