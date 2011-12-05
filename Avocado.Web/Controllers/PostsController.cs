using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avocado.Domain.Abstract;
using Avocado.Web.Models.Posts;
using System.Configuration;

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

        [HttpPost]
        public ActionResult UploadFiles(string qqfile)
        {
            string userName = User.Identity.Name.Split('|')[0];
            string newFilePath = string.Empty;
            string newLittleFilePath = string.Empty;
            //var path = @"C:\\Temp\\100\\";
            //var file = string.Empty;

            try
            {
                var stream = Request.InputStream;
                if (String.IsNullOrEmpty(Request["qqfile"]))
                {
                    // IE
                    HttpPostedFileBase postedFile = Request.Files[0];
                    stream = postedFile.InputStream;
                    //file = Path.Combine(path, System.IO.Path.GetFileName(Request.Files[0].FileName));
                }
                else
                {
                    //Webkit, Mozilla
                    //file = Path.Combine(path, qqfile);
                }

                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                if (stream.Length > 0)
                {
                    stream.Position = 0;
                    newFilePath = _authorService.SavePostImage(userName, stream, qqfile);
                }

                //System.IO.File.WriteAllBytes(file, buffer);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, "application/json");
            }

            newFilePath = ConfigurationManager.AppSettings["AzureStorageUri"] + userName + "/" + newFilePath;
            //newLittleFilePath = ConfigurationManager.AppSettings["AzureStorageUri"] + userName + "/thumb/profile-pic";

            return Json(new { success = true, message = "sweet", imgPath = newFilePath }, JsonRequestBehavior.AllowGet);
        }

    }
}
