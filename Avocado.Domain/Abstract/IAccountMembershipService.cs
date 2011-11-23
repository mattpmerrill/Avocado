using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using Avocado.Domain.Entities;

namespace Avocado.Domain.Abstract
{
    public interface IAccountMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        bool IsUserNameAvailable(string userName);
        MembershipCreateStatus CreateUser(string email, string password, string userName, string social, string token, string verifier, string firstName=null, string lastName=null, string FacebookId=null, string TwitterId=null, string gender=null, string city = null, string state = null, string locale = null);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        bool IsLinkedWithFacebook(string email);
        bool IsLinkedWithSocial(string id);
        string GetEmailFromSocialId(string socialId);
        int GetAccountId(string userName);
        string GetAuthTicketFromSocialId(string socialId);
        string GetAuthTicketFromEmail(string email);
        Account GetAccount(string userName);
        Profile GetProfile(int accountId);
        bool IsProfileUpdated(int accountId, string firstName, string lastName, string profileImage, string bio, string personalUrl);
    }
}
