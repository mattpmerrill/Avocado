using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avocado.Domain.Entities;

namespace Avocado.Web.Models.People
{
    public class CreativesViewModel
    {
        public List<Account> Accounts { get; set; }
        public int Sparks { get; set; }
    }
}