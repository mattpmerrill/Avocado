using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avocado.Domain.Entities;

namespace Avocado.Web.Models.Feed
{
    public class FeedViewModel
    {
        public List<Post> MyFeed { get; set; }
    }
}