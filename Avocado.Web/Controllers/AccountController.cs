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
        public ActionResult LogIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_authenticationService.AuthenticateUser(model.Email, model.Password))
                {
                    _authenticationService.LogIn(model.Email, createPersistentCookie: false);
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

        public ActionResult LogInWithSocial(string socialId)
        {
            //TODO:Check if facebook email has been registered on our site yet
            //If not, send to registration page to create an account
            //otherwise, log in with the facebook email address
            if (_membershipService.IsLinkedWithSocial(socialId))
            {
                FormsAuthentication.SetAuthCookie(socialId, false);
            }
            else
            {
                return RedirectToAction("CreateAccount", "Account");
            }
            
            return RedirectToAction("Index", "Home");
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
            var data = new { name = model.Email, password = model.Password };
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

                MembershipCreateStatus createStatus = _membershipService.CreateUser(model.Email, model.Password, model.UserName, social, token, secret);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    _authenticationService.LogIn(userName: model.Email, createPersistentCookie: true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ValidationHelpers.AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            return View(model);
        }

        public JsonResult CheckUserNameAvailability(string userName)
        {
            int Taken = 0;
            
            if (!_membershipService.IsUserNameAvailable(userName))
            {
                Taken = 1;
            }

            return Json(Taken);
        }
    }
}
