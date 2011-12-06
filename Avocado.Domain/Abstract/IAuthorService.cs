using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Avocado.Domain.Entities;

namespace Avocado.Domain.Abstract
{
    public interface IAuthorService
    {
        string SaveProfileImage(string userName, Stream fileStream, string fileName, string fileLocation);
        IQueryable<Post> GetMyPosts(int accountId);
        string SavePostImage(string userName, Stream fileStream, string fileName);
        bool IsNewPostSaved(int accountId, string title, string description, int categoryId, string imagePath);
        IQueryable<Category> GetAllCategories();
    }
}
