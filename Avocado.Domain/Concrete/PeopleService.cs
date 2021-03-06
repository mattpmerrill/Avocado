﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avocado.Domain.Abstract;
using Avocado.Domain.Entities;

namespace Avocado.Domain.Concrete
{
    public class PeopleService:IPeopleService
    {
        private AvocadoEntities _data;

        public PeopleService(string connectionString)
        {
            _data = new AvocadoEntities(connectionString);
        }

        public IQueryable<Entities.Account> getTopCreatives()
        {
            IQueryable<Account> topCreatives = _data.Accounts.Where(x => x.Posts.Count() > 0).Take(10);
            return topCreatives;
        }

        public IQueryable<Post> getLatestWorkByUserName(string userName)
        {
            IQueryable<Post> latestWork = _data.Posts.Where(x => x.Account.UserName == userName).OrderByDescending(x => x.PostId).Take(15);
            return latestWork;
        }

        public IQueryable<Post> getMoreWorkByUserName(string userName, int lastPostId)
        {
            IQueryable<Post> moreWork = _data.Posts.Where(x => x.Account.UserName == userName).OrderByDescending(x => x.PostId).Where(x => x.PostId < lastPostId).Take(15);
            return moreWork;
        }

        public Account getUserAccount(string userName)
        {
            return _data.Accounts.FirstOrDefault(x => x.UserName == userName);
        }

        public int follow(string followerUserName, string followedUserId)
        {
            try
            {
                Guid follower = new Guid(followedUserId);
                int followerAccountId = _data.Accounts.Where(x => x.aspnet_Users.UserName == followerUserName).Select(a => a.AccountId).SingleOrDefault();
                int followedAccountId = _data.Accounts.Where(x => x.UserId == follower).Select(a => a.AccountId).SingleOrDefault();

                Follow newFollow = new Follow();
                newFollow.FollowedAccountId = followedAccountId;
                newFollow.FollowerAccountId = followerAccountId;
                newFollow.InsertDate = DateTime.UtcNow;

                _data.Follows.AddObject(newFollow);
                return _data.SaveChanges();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int UnFollow(string followerUserName, string followedUserId)
        {
            try
            {
                Guid follower = new Guid(followedUserId);
                int followerAccountId = _data.Accounts.Where(x => x.aspnet_Users.UserName == followerUserName).Select(a => a.AccountId).SingleOrDefault();
                int followedAccountId = _data.Accounts.Where(x => x.UserId == follower).Select(a => a.AccountId).SingleOrDefault();

                Follow followToDelete = _data.Follows.Where(x => x.FollowerAccountId == followerAccountId && x.FollowedAccountId == followedAccountId).SingleOrDefault();
                _data.Follows.DeleteObject(followToDelete);
                return _data.SaveChanges();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
