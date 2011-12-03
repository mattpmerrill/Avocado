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

        public IQueryable<Entities.Post> GetSaves(int accountId)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Entities.Post> GetMyCreations(int accountId)
        {
            throw new NotImplementedException();
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
