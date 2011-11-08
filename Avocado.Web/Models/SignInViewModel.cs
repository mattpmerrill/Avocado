using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Avocado.Web.Models
{
    public class SignInViewModel
    {
        [Required]
        [Display(Name="username")]
        public string SignInUserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name="password")]
        public string SignInPassword { get; set; }
    }
}