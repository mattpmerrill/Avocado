using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avocado.Domain.Abstract;
using System.Web.Security;
using Avocado.Domain.Entities;
using Twitterizer;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Configuration;

namespace Avocado.Domain.Concrete
{
    public class AccountMembershipService : IAccountMembershipService
    {
        private readonly MembershipProvider _provider;
        private AvocadoEntities _data;

        public AccountMembershipService(string connectionString)
        {
            _data = new AvocadoEntities(connectionString);
            _provider = Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string email, string password, string fullName, string social, string token, string secret)
        {
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            MembershipCreateStatus status;
            MembershipUser newUser = _provider.CreateUser(username: email, password: password, email: email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out status);

            if (status == MembershipCreateStatus.Success) 
            {
                if (social == "twitter")
                {
                    string consumerKey = ConfigurationManager.AppSettings["consumerKey"];
                    string consumerSecret = ConfigurationManager.AppSettings["consumerSecret"];

                    if (SaveTwitterData(newUser.ProviderUserKey, consumerKey, consumerSecret, token, secret, fullName))
                        status = MembershipCreateStatus.Success;
                    else
                        status = MembershipCreateStatus.ProviderError;
                }
                else
                {
                    if (SaveUserSettings(newUser.ProviderUserKey, fullName))
                        status = MembershipCreateStatus.Success;
                    else
                        status = MembershipCreateStatus.ProviderError;
                }
            }

            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }

        public bool IsLinkedWithFacebook(string email)
        {
            if (_data.aspnet_Users.Any(x => x.UserName == email))
                return true;
            else
                return false;
        }

        public bool IsLinkedWithSocial(string id)
        {
            if (_data.UserSettings.Any(x => x.TwitterUserId == id || x.FacebookUserId == id))
                return true;
            else
                return false;
        }

        public string GetEmailFromSocialId(string socialId)
        {
            return _data.UserSettings.Where(x => x.TwitterUserId == socialId).Select(x => x.aspnet_Users.UserId).ToString();
        }

        private bool SaveTwitterData(object userId, string consumerKey, string consumerSecret, string token, string secret, string fullName)
        {
            try
            {
                _data.UserSettings.AddObject(new UserSetting() { UserId = (Guid)userId, TwitterAccessToken = token, TwitterAccessSecret = secret, FullName = fullName });
                _data.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool SaveUserSettings(object userId, string FullName)
        {
            try
            {
                _data.UserSettings.AddObject(new UserSetting() { UserId = (Guid)userId, FullName = FullName });
                _data.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
