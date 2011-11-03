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
        [Display(Name="User Name")]
        public string SignInUserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}