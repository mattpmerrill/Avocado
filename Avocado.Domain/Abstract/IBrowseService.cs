using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Avocado.Domain.Entities;

namespace Avocado.Domain.Abstract
{
    public interface IBrowseService
    {
        IQueryable<Post> getAllLatestItems();
        IQueryable<Post> getAdditionalPostsFromAllLatest(int lastPostId);
        IQueryable<Post> getLatestItemsByCategory(string category);
        IQueryable<Post> getLatestItemsByDate();
        IQueryable<Post> getLatestItemsByCategoryAndTag(string category, string tag);
        IQueryable<Post> getLatestItemsFromAllCategories();
        IEnumerable<Post> getAdditionalPostsFromCategory(string category, int lastPostId);
        IQueryable<Post> getLatestPostsByAuthor(int accountId);
        Post getPost(int postId);
    }
}
