using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avocado.Domain.Abstract;
using System.Web.Security;

namespace Avocado.Domain.Concrete
{
    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void LogIn(string ticket, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(ticket)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            FormsAuthentication.SetAuthCookie(ticket, createPersistentCookie);
        }

        public void LogOut()
        {
            FormsAuthentication.SignOut();
        }

        public bool AuthenticateUser(string userName, string password)
        {
            if (Membership.ValidateUser(userName, password))
                return true;
            else
                return false;
        }
    }
}
