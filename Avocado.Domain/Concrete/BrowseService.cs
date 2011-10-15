using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avocado.Domain.Abstract;
using Avocado.Domain.Entities;
using System.Collections;

namespace Avocado.Domain.Concrete
{
    public class BrowseService: IBrowseService
    {
        private AvocadoEntities _data;

        public BrowseService(string connectionString)
        {
            _data = new AvocadoEntities(connectionString);
        }

        public IQueryable<Post> getLatestItemsByCategory(string category)
        {
            return _data.Posts.Where(x => x.Category.Name == category).OrderByDescending(x => x.PostId).Take(15);
        }

        public IQueryable<Post> getLatestItemsFromAllCategories()
        {
            IQueryable<Post> posts = _data.Posts.Where(x => x.Category.CategoryId == 1).OrderByDescending(x => x.PostId).Take(3)
                .Concat(_data.Posts.Where(x => x.Category.CategoryId == 2).OrderByDescending(x => x.PostId).Take(3)
                .Concat(_data.Posts.Where(x => x.Category.CategoryId == 3).OrderByDescending(x => x.PostId).Take(3)
                .Concat(_data.Posts.Where(x => x.Category.CategoryId == 4).OrderByDescending(x => x.PostId).Take(3)
                .Concat(_data.Posts.Where(x => x.Category.CategoryId == 5).OrderByDescending(x => x.PostId).Take(3)
                .Concat(_data.Posts.Where(x => x.Category.CategoryId == 6).OrderByDescending(x => x.PostId).Take(3)
                .Concat(_data.Posts.Where(x => x.Category.CategoryId == 7).OrderByDescending(x => x.PostId).Take(3)
                .Concat(_data.Posts.Where(x => x.Category.CategoryId == 8).OrderByDescending(x => x.PostId).Take(3)
                .Concat(_data.Posts.Where(x => x.Category.CategoryId == 9).OrderByDescending(x => x.PostId).Take(3)
                .Concat(_data.Posts.Where(x => x.Category.CategoryId == 10).OrderByDescending(x => x.PostId).Take(3))))))))));

            return posts;
        }

        public IEnumerable<Post> getAdditionalPostsFromCategory(string category, int lastPostId)
        {
            IEnumerable<Post> posts = _data.Posts.Where(x => x.Category.Name == category).OrderByDescending(x => x.PostId).Where(x => x.PostId < lastPostId).Take(15);

            return posts;
        }

        public IQueryable<Post> getLatestPostsByAuthor(int accountId)
        {
            throw new NotImplementedException();
        }
    }
}
