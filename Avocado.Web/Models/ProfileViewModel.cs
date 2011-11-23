using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Avocado.Web.Models
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }
        public string PersonalUrl { get; set; }
        public string Bio { get; set; }
        public string ProfileImage { get; set; }
    }
}