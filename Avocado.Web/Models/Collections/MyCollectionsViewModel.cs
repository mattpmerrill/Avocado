using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avocado.Domain.Entities;

namespace Avocado.Web.Models.Collections
{
    public class MyCollectionsViewModel
    {
        public IQueryable<Post> MyLikes { get; set; }
    }
}