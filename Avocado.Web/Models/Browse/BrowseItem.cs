using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avocado.Web.Models.Browse
{
    public class BrowseItem
    {
        public int PostId { get; set; }
        public string PostImage { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Likes { get; set; }
        public int views { get; set; }
        public string Category { get; set; }
        public string tags { get; set; }
        public string Author { get; set; }
        public string AuthorImage { get; set; }
    }
}