using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avocado.Domain.Abstract;
using Avocado.Web.Models.Posts;

namespace Avocado.Web.Controllers
{
    public class PostsController : Controller
    {
        private IAuthorService _authorService;

        public PostsController(IAuthorService authorService)
        {
            _authorService = authorService;
            ViewBag.ActiveTab = "myposts-tab";
        }

        public ViewResult Index()
        {
            int accountId = Convert.ToInt32(User.Identity.Name.Split('|')[1]);
            MyPostsViewModel model = new MyPostsViewModel();
            model.MyPosts = _authorService.GetMyPosts(accountId);

            return View(model);
        }

    }
}
