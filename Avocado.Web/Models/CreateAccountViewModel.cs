using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace Avocado.Web.Models
{
    public class CreateAccountViewModel
    {
        [Required(ErrorMessage="username required")]
        [DisplayName("create a username")]
        public string UserName { get; set; }

        [Required(ErrorMessage="email required")]
        [DisplayName("email")]
        [RegularExpression(@"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage="password required")]
        [DisplayName("password")]
        [DataType(DataType.Password)]
        [RegularExpression(@".[\S]{5,20}", ErrorMessage = "Passwords must be at least 6 characters")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[DisplayName("Confirm Password")]
        //[Compare("Password", ErrorMessage = "The Password and Confirmation Password must match.")]
        //public string ConfirmPassword { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FacebookId { get; set; }
        public string TwitterId { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string locale { get; set; }
    }
}