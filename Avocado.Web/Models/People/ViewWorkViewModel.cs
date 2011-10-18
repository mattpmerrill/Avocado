using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avocado.Domain.Entities;

namespace Avocado.Web.Models.People
{
    public class ViewWorkViewModel
    {
        public List<Post> Posts;
        public Account UserAccount { get; set; }
    }
}