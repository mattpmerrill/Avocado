using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avocado.Domain.Abstract;

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

        public ActionResult Index()
        {
            return View();
        }

    }
}
