using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avocado.Domain.Abstract;
using Avocado.Domain.Entities;

namespace Avocado.Domain.Concrete
{
    public class FeedService : IFeedService
    {
        private AvocadoEntities _data;

        public FeedService(string connectionString)
        {
            _data = new AvocadoEntities(connectionString);
        }

        public IQueryable<Post> GetMyFeed(int accountId)
        {
            var myFollows = _data.Follows.Where(x => x.FollowerAccountId == accountId);

            var myFeed = (from p in _data.Posts
                          join mf in myFollows on p.AccountId equals mf.FollowedAccountId
                          select p)
                         .Union(from pp in _data.Posts
                                where pp.AccountId == accountId
                                select pp);

            return myFeed;
        }
    }
}
