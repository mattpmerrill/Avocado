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
        [DisplayName("Email Address")]
        public string Email { get; set; }

        [Required]
        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [Required]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "The Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }
    }
}