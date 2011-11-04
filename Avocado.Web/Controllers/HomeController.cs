using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avocado.Domain.Abstract;
using Avocado.Web.Models.Browse;

namespace Avocado.Web.Controllers
{
    public class HomeController : Controller
    {
        private IBrowseService _browseServcie;

        public HomeController(IBrowseService browseService)
        {
            _browseServcie = browseService;
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Browse");
            }
            else
            {
                BrowseViewModel model = new BrowseViewModel();
                model.Posts = _browseServcie.getLatestItemsByDate().ToList();

                return View(model);
            }
        }
    }
}
