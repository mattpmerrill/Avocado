using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avocado.Domain.Entities;

namespace Avocado.Domain.Abstract
{
    public interface IPeopleService
    {
        IQueryable<Account> getTopCreatives();
        IQueryable<Post> getLatestWorkByUserName(string userName);
        IQueryable<Post> getMoreWorkByUserName(string userName, int lastPostId);
        Account getUserAccount(string userName);
        int follow(string followerUserName, string followedUserId);
    }
}
