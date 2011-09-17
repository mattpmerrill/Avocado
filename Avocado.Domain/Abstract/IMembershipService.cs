using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Avocado.Domain.Abstract
{
    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string email, string password);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        bool IsLinkedWithFacebook(string email);
    }
}
