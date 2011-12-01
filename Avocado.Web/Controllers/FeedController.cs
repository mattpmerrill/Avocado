using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avocado.Domain.Abstract;
using Avocado.Web.Models.Feed;

namespace Avocado.Web.Controllers
{
    public class FeedController : Controller
    {
        private IFeedService _feedService;

        public FeedController(IFeedService feedService)
        {
            _feedService = feedService;
            ViewBag.ActiveTab = "feed-tab";
        }

        public ActionResult Index()
        {
            int accountId = Convert.ToInt32(User.Identity.Name.Split('|')[1]);
            FeedViewModel model = new FeedViewModel();
            model.MyFeed = _feedService.GetMyFeed(accountId).ToList();

            return View(model);
        }

    }
}
