using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avocado.Domain.Abstract;
using System.Web.Security;
using Avocado.Web.Utilities;
using Avocado.Web.Models;

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
