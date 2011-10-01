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

namespace Avocado.Web.Controllers
{
    public class AccountController : Controller
    {
        private IMembershipService _membershipService;
        private IFormsAuthenticationService _authenticationService;

        public AccountController(IMembershipService membershipService, IFormsAuthenticationService authenticationService)
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

        public ActionResult LogInWithFacebook(string facebookEmail)
        {
            //TODO:Check if facebook email has been registered on our site yet
            //If not, send to registration page to create an account
            //otherwise, log in with the facebook email address
            if (_membershipService.IsLinkedWithFacebook(facebookEmail))
            {
                FormsAuthentication.SetAuthCookie(facebookEmail, false);
            }
            else
            {
                return RedirectToAction("CreateAccount", "Account");
            }
            
            return RedirectToAction("Index", "Home");
        }

        public ActionResult SignInWithTwitter()
        {
            string consumerKey = "ep22vRe4UBW4VlYa3b6odQ";
            string consumerSecrect = "Tmni15oL2XCuDGBTdgtOIjRHvfWhNGjl5xT9RqcCj3A";
            OAuthTokenResponse reqToken = OAuthUtility.GetRequestToken(consumerKey, consumerSecrect, "http://localhost:58828/Account/TwitterAuth");

            string twitterUrl = "https://api.twitter.com/oauth/authenticate?oauth_token=" + reqToken.Token;

            return Redirect(twitterUrl);
        }

        public ActionResult TwitterAuth()
        {
            if (Request.QueryString["denied"] == null)
            {
                string consumerKey = "ep22vRe4UBW4VlYa3b6odQ";
                string consumerSecret = "Tmni15oL2XCuDGBTdgtOIjRHvfWhNGjl5xT9RqcCj3A";
                string requestToken = Request.QueryString["oauth_token"];
                string requestVerifier = Request.QueryString["oauth_verifier"];

                OAuthTokenResponse responseToken = OAuthUtility.GetAccessToken(consumerKey, consumerSecret, requestToken, requestVerifier);

                OAuthTokens accessToken = new OAuthTokens();
                accessToken.AccessToken = responseToken.Token;
                accessToken.AccessTokenSecret = responseToken.TokenSecret;
                accessToken.ConsumerKey = consumerKey;
                accessToken.ConsumerSecret = consumerSecret;

                //todo: 1.save the accesstoken in the database
                //      2.authenticate with membership services
                //      3.redirect user to home page

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
                MembershipCreateStatus createStatus = _membershipService.CreateUser(model.Email, model.Password);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    //todo: create the user in [User] table
                    _authenticationService.LogIn(model.Email, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ValidationHelpers.AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            return View(model);
        }

    }
}
