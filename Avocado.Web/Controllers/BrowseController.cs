﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avocado.Domain.Abstract;
using Avocado.Web.Models.Browse;
using Avocado.Domain.Entities;

namespace Avocado.Web.Controllers
{
    public class BrowseController : Controller
    {
        private IBrowseService _browseServcie;

        public BrowseController(IBrowseService browseService)
        {
            _browseServcie = browseService;
        }

        public ViewResult Index()
        {
            BrowseViewModel model = new BrowseViewModel();
            model.Posts = _browseServcie.getLatestItemsFromAllCategories().ToList();

            return View(model);
        }

        public ViewResult Category(string category)
        {
            BrowseViewModel model = new BrowseViewModel();
            model.Category = category;
            model.Posts = _browseServcie.getLatestItemsByCategory(category).ToList();

            return View(model);
        }

        public ViewResult CategoryAndTag(string category, string tag)
        {
            BrowseViewModel model = new BrowseViewModel();
            model.Category = category;
            model.Tag = tag;
            model.Posts = _browseServcie.getLatestItemsByCategoryAndTag(category, tag).ToList();

            return View("Category", model);
        }

        public ActionResult GetMorePosts(string category, int lastId)
        {
            BrowseViewModel model = new BrowseViewModel();
            model.Category = category;
            model.Posts = _browseServcie.getAdditionalPostsFromCategory(category, lastId).ToList();

            if (model.Posts.Count > 0)
                return PartialView("_AdditionalPosts", model);
            else
                return null;
        }

        public ViewResult GetPostDetails(int postId)
        {
            PostDetailsViewModel model = new PostDetailsViewModel();
            model.Post = _browseServcie.getPost(Convert.ToInt32(postId));

            return View("Details", model);
        }
    }
}