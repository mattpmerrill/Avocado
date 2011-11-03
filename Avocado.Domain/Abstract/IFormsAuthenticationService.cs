using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Avocado.Domain.Abstract
{
    public interface IFormsAuthenticationService
    {
        bool AuthenticateUser(string userName, string password);
        void LogIn(string ticket, bool createPersistentCookie);
        void LogOut();
    }
}
