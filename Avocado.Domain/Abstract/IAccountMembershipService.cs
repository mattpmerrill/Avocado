using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace Avocado.Domain.Abstract
{
    public interface IAccountMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        bool IsUserNameAvailable(string userName);
        MembershipCreateStatus CreateUser(string email, string password, string userName, string social, string token, string verifier);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        bool IsLinkedWithFacebook(string email);
        bool IsLinkedWithSocial(string id);
        string GetEmailFromSocialId(string socialId);
        int GetAccountId(string userName);
    }
}
