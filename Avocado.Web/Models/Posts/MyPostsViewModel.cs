using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avocado.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Avocado.Web.Models.Posts
{
    public class MyPostsViewModel
    {
        public IQueryable<Post> MyPosts { get; set; }

        [Required(ErrorMessage="Please provide a title")]
        public string NewTitle { get; set; }
        [Required(ErrorMessage="Please provide a description")]
        public string NewDescription { get; set; }
        [Required(ErrorMessage="Please upload an image for your post")]
        public string NewMainImage { get; set; }
    }
}