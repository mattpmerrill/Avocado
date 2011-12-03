using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avocado.Domain.Entities;

namespace Avocado.Domain.Abstract
{
    public interface ICollectionService
    {
        IQueryable<Post> GetSaves(int accountId);
        IQueryable<Post> GetMyCreations(int accountId);
        IQueryable<Post> getMyLikes(int accountId);
    }
}
