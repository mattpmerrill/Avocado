﻿using System;
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

        public MembershipCreateStatus CreateUser(string email, string password, string userName, string socialNetwork, string token, string secret, string firstName = null, string lastName = null, string FacebookId = null, string TwitterId = null, string gender = null, string city = null, string state = null, string locale = null)
        {
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            MembershipCreateStatus status;
            MembershipUser newUser = _provider.CreateUser(username: userName, password: password, email: email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out status);

            if (status == MembershipCreateStatus.Success) 
            {
                if (socialNetwork == "twitter")
                {
                    string consumerKey = ConfigurationManager.AppSettings["consumerKey"];
                    string consumerSecret = ConfigurationManager.AppSettings["consumerSecret"];

                    if (SaveTwitterData(newUser.ProviderUserKey, consumerKey, consumerSecret, token, secret, userName, TwitterId))
                        status = MembershipCreateStatus.Success;
                    else
                        status = MembershipCreateStatus.ProviderError;
                }
                else
                {
                    if (SaveUserSettings(newUser.ProviderUserKey, userName, firstName, lastName, FacebookId, TwitterId, gender, city, state, locale))
                        status = MembershipCreateStatus.Success;
                    else
                        status = MembershipCreateStatus.ProviderError;
                }
            }

            return status;
        }

        public bool IsUserNameAvailable(string userName)
        {
            MembershipUser user = _provider.GetUser(userName, false);

            if (user != null)
                return false;
            else
                return true;
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
            if (_data.Accounts.Any(x => x.TwitterUserId == id || x.FacebookUserId == id))
                return true;
            else
                return false;
        }

        public string GetAuthTicketFromSocialId(string socialId)
        {
            string ticket = string.Empty;
            Account userAccount = _data.Accounts.Where(x => x.TwitterUserId == socialId || x.FacebookUserId == socialId).FirstOrDefault();

            if (userAccount != null)
            {
                string userName = userAccount.UserName;
                int accountId = userAccount.AccountId;

                ticket = userName + "|" + accountId.ToString();
            }
            
            return ticket;
        }

        public string GetAuthTicketFromEmail(string email)
        {
            string ticket = string.Empty;
            Account userAccount = _data.aspnet_Membership.Where(x => x.Email == email).FirstOrDefault().aspnet_Users.Accounts.FirstOrDefault();

            if (userAccount != null)
            {
                string userName = userAccount.UserName;
                int accountId = userAccount.AccountId;

                ticket = userName + "|" + accountId.ToString();
            }

            return ticket;
        }

        public string GetEmailFromSocialId(string socialId)
        {
            return _data.Accounts.Where(x => x.TwitterUserId == socialId).Select(x => x.aspnet_Users.UserId).ToString();
        }

        private bool SaveTwitterData(object userId, string consumerKey, string consumerSecret, string token, string secret, string userName, string twitterUserId)
        {
            try
            {
                Account newAccount = new Account() { UserId = (Guid)userId, TwitterAccessToken = token, TwitterAccessSecret = secret, UserName = userName, TwitterUserId = twitterUserId };
                _data.Accounts.AddObject(newAccount);
                _data.Profiles.AddObject(new Profile() { AccountId = newAccount.AccountId });
                _data.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool SaveUserSettings(object userId, string userName, string firstName = null, string lastName = null, string FacebookId = null, string TwitterId = null, string gender = null, string city = null, string state = null, string locale = null)
        {
            try
            {
                string fullName = ((firstName + " " + lastName).Length > 100) ? (firstName + " " + lastName).Substring(0, 100) : (firstName + " " + lastName);
                Account newAccount = new Account() { UserId = (Guid)userId, UserName = userName, FirstName = firstName, LastName = lastName, FullName = fullName, FacebookUserId = FacebookId, TwitterUserId = TwitterId, Gender = gender, City = city, State = state, Locale = locale };
                _data.Accounts.AddObject(newAccount);
                _data.Profiles.AddObject(new Profile() { AccountId = newAccount.AccountId });
                _data.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int GetAccountId(string userName)
        {
            return _data.Accounts.Where(x => x.aspnet_Users.LoweredUserName == userName.ToLower()).Select(x => x.AccountId).FirstOrDefault();
        }

        public Account GetAccount(string userName)
        {
            return _data.Accounts.Where(x => x.UserName == userName).FirstOrDefault();
        }

        public Profile GetProfile(int accountId)
        {
            return _data.Profiles.Where(x => x.AccountId == accountId).FirstOrDefault();
        }

        public bool IsProfileUpdated(int accountId, string firstName, string lastName, string profileImage, string bio, string personalUrl)
        {
            try
            {
                var account = _data.Accounts.Where(x => x.AccountId == accountId).FirstOrDefault();
                account.FirstName = firstName;
                account.LastName = lastName;
                account.ProfileImage = profileImage;

                var profile = _data.Profiles.Where(x => x.AccountId == accountId).FirstOrDefault();
                profile.Bio = bio;
                profile.PersonalUrl = personalUrl;

                _data.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                //TODO: log exceptions with ELMAH
                return false;
            }
        }
    }
}
