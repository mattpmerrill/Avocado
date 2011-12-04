using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avocado.Domain.Abstract;
using Avocado.Domain.Entities;

namespace Avocado.Domain.Concrete
{
    public class CollectionService:ICollectionService
    {
        private AvocadoEntities _data;

        public CollectionService(string connectionString)
        {
            _data = new AvocadoEntities(connectionString);
        }

        public IQueryable<Entities.Post> GetMySaves(int accountId)
        {
            var mySaves = from p in _data.Posts
                          join s in _data.Saves on p.PostId equals s.PostId
                          where s.AccountId == accountId
                          select p;
            return mySaves;
        }

        public IQueryable<Entities.Post> GetMyCreations(int accountId)
        {
            return _data.Posts.Where(x => x.AccountId == accountId);
        }

        public IQueryable<Post> getMyLikes(int accountId)
        {
            var myLikes = from p in _data.Posts
                          join l in _data.Likes on p.PostId equals l.PostId
                          where l.AccountId == accountId
                          select p;
            return myLikes;
        }
    }
}
