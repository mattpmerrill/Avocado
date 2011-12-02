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
    }
}
