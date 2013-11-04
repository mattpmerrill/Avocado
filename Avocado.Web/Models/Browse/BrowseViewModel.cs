using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avocado.Domain.Entities;
using System.Data.Entity;

namespace Avocado.Web.Models.Browse
{
    public class BrowseViewModel
    {
        public List<Post> Posts { get; set; }
        public string Category { get; set; }
        public string Tag { get; set; }
    }
}