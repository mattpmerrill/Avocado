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
        [Required]
        [DisplayName("user name")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("email")]
        [RegularExpression(@"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        //[Required]
        //[DisplayName("Full Name")]
        //public string FullName { get; set; }

        [Required]
        [DisplayName("password")]
        [DataType(DataType.Password)]
        [RegularExpression(@".[\S]{5,20}", ErrorMessage = "Passwords must be at least 6 characters")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[DisplayName("Confirm Password")]
        //[Compare("Password", ErrorMessage = "The Password and Confirmation Password must match.")]
        //public string ConfirmPassword { get; set; }
    }
}