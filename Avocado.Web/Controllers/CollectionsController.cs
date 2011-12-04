using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avocado.Domain.Abstract;
using Avocado.Web.Models.Collections;

namespace Avocado.Web.Controllers
{
    public class CollectionsController : Controller
    {
        ICollectionService _collectionService;

        public CollectionsController(ICollectionService collectionService)
        {
            _collectionService = collectionService;
            ViewBag.ActiveTab = "collections-tab";
        }

        public ViewResult Index()
        {
            int accountId = Convert.ToInt32(User.Identity.Name.Split('|')[1]);

            MyCollectionsViewModel model = new MyCollectionsViewModel();
            model.MyLikes = _collectionService.getMyLikes(accountId);
            model.MySaves = _collectionService.GetMySaves(accountId);
            model.MyCreations = _collectionService.GetMyCreations(accountId);

            return View(model);
        }

    }
}
