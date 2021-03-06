﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avocado.Domain.Abstract;
using Avocado.Web.Models.People;

namespace Avocado.Web.Controllers
{
    public class PeopleController : Controller
    {
        private IPeopleService _peopleServcie;

        public PeopleController(IPeopleService peopleService)
        {
            _peopleServcie = peopleService;
            ViewBag.ActiveTab = "people-tab";
        }

        public ViewResult Index()
        {
            CreativesViewModel model = new CreativesViewModel();

            model.Accounts = _peopleServcie.getTopCreatives().ToList();
            int sparks = 0;

            foreach (var account in model.Accounts)
            {
                foreach (var post in account.Posts)
                {
                    sparks += post.OriginPost.Count();
                }
            }

            model.Sparks = sparks;

            return View(model);
        }

        public ViewResult ViewWork(string userName)
        {
            ViewWorkViewModel model = new ViewWorkViewModel();
            model.Posts = _peopleServcie.getLatestWorkByUserName(userName).ToList();
            model.UserAccount = _peopleServcie.getUserAccount(userName);
            int sparks = 0;

            foreach (var post in model.UserAccount.Posts)
            {
                sparks += post.OriginPost.Count();
            }

            model.Sparks = sparks;

            return View(model);
        }

        public ActionResult GetMoreWorkByUserName(string userName, int lastPostId)
        {
            ViewWorkViewModel model = new ViewWorkViewModel();
            model.UserAccount = _peopleServcie.getUserAccount(userName);
            model.Posts = _peopleServcie.getMoreWorkByUserName(userName, lastPostId).ToList();

            if (model.Posts.Count > 0)
                return PartialView("_AdditionalUserWork", model);
            else
                return null;
        }

        [HttpPost]
        public string Follow(string follower, string followed)
        {
            if (_peopleServcie.follow(follower, followed) > 0)
            {
                return "success";
            }
            else
            {
                return null;
            }

        }

        [HttpPost]
        public string UnFollow(string follower, string followed)
        {
            if (_peopleServcie.UnFollow(follower, followed) > 0)
            {
                return "success";
            }
            else
            {
                return null;
            }

        }

    }
}
