using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avocado.Domain.Entities;

namespace Avocado.Domain.Abstract
{
    public interface IFeedService
    {
        IQueryable<Post> GetMyFeed(int accountId);
    }
}
