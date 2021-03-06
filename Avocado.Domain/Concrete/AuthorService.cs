﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avocado.Domain.Abstract;
using System.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.IO;
using ImageResizer;
using Avocado.Domain.Entities;

namespace Avocado.Domain.Concrete
{
    public class AuthorService:IAuthorService
    {
        private AvocadoEntities _data;

        public AuthorService(string connectionString)
        {
            _data = new AvocadoEntities(connectionString);
        }

        /// <summary>
        /// Save images uploaded into Azure Blob storage
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <param name="fileLocation"></param>
        public string SaveProfileImage(string userName, System.IO.Stream fileStream, string fileName, string fileLocation)
        {
            var account = _data.Accounts.Where(x => x.UserName == userName).FirstOrDefault();

            // Setup the connection to Windows Azure Storage
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + ConfigurationManager.AppSettings["AzureStorageAccount"] + ";AccountKey=" + ConfigurationManager.AppSettings["AzureStorageKey"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // For large file copies you need to set up a custom timeout period
            // and using parallel settings appears to spread the copy across multiple threads
            blobClient.Timeout = new System.TimeSpan(1, 0, 0);
            blobClient.ParallelOperationThreadCount = 2;

            // Get and create the container
            CloudBlobContainer blobContainer = null;
            blobContainer = blobClient.GetContainerReference(userName); //("publicfiles");
            blobContainer.CreateIfNotExist();

            // Setup the permissions on the container to be public
            var permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            blobContainer.SetPermissions(permissions);

            //ImageResizer
            //The resizing settings can specify any of 30 commands.. See http://imageresizing.net for details.
            ResizeSettings resizeCropSettings = new ResizeSettings("width=150&height=150&format=jpg&crop=auto");
            ResizeSettings resizeCropSettingsThumb = new ResizeSettings("width=55&height=55&format=jpg&crop=auto");

            //Let the image builder add the correct extension based on the output file type (which may differ).  
            MemoryStream ms = new MemoryStream();
            MemoryStream msThumb = new MemoryStream();
            ImageBuilder.Current.Build(fileStream, ms, resizeCropSettings, false);
            fileStream.Position = 0;
            ImageBuilder.Current.Build(fileStream, msThumb, resizeCropSettingsThumb, false);


            //Generate a file path
            string fileNameProfile = "profile/" + fileName.Replace(" ", "");
            string fileNameThumb = "thumb/profile-pic"; //+ fileName.Replace(" ", "");

            //if there are existing profile and thumb images delete them
            if (account.ProfileImage != null)
            {
                var oldProfile = blobContainer.GetBlobReference("profile/" + account.ProfileImage);
                oldProfile.Delete();
                var oldThumb = blobContainer.GetBlobReference("thumb/profile-pic");
                oldThumb.Delete();
            }

            // Create the Blob and upload the file
            var blob = blobContainer.GetBlobReference(fileNameProfile);
            var blobThumb = blobContainer.GetBlobReference(fileNameThumb);
            ms.Position = 0;
            msThumb.Position = 0;
            blob.UploadFromStream(ms);
            blobThumb.UploadFromStream(msThumb);
            ms.Close();
            msThumb.Close();

            //update account table with profile image filename
            try
            {
                account.ProfileImage = fileName.Replace(" ", "");
                _data.SaveChanges();
            }
            catch
            {
                //todo: log exceptions with ELMAH
            }

            return fileNameProfile;
        }

        /// <summary>
        /// upload the images for new posts
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string SavePostImage(string userName, Stream fileStream, string fileName)
        {
            var account = _data.Accounts.Where(x => x.UserName == userName).FirstOrDefault();

            // Setup the connection to Windows Azure Storage
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=http;AccountName=" + ConfigurationManager.AppSettings["AzureStorageAccount"] + ";AccountKey=" + ConfigurationManager.AppSettings["AzureStorageKey"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // For large file copies you need to set up a custom timeout period
            // and using parallel settings appears to spread the copy across multiple threads
            blobClient.Timeout = new System.TimeSpan(1, 0, 0);
            blobClient.ParallelOperationThreadCount = 2;

            // Get and create the container
            CloudBlobContainer blobContainer = null;
            blobContainer = blobClient.GetContainerReference(userName); //("publicfiles");
            blobContainer.CreateIfNotExist();

            // Setup the permissions on the container to be public
            var permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            blobContainer.SetPermissions(permissions);

            //ImageResizer
            //The resizing settings can specify any of 30 commands.. See http://imageresizing.net for details.
            ResizeSettings resizeCropSettings = new ResizeSettings("width=575&format=jpg&crop=auto");
            ResizeSettings resizeCropSettingsMedium = new ResizeSettings("width=250&format=jpg&crop=auto");
            ResizeSettings resizeCropSettingsSmall = new ResizeSettings("width=218&height=170&format=jpg&crop=auto");

            //Let the image builder add the correct extension based on the output file type (which may differ).  
            MemoryStream ms = new MemoryStream();
            MemoryStream msMedium = new MemoryStream();
            MemoryStream msSmall = new MemoryStream();
            ImageBuilder.Current.Build(fileStream, ms, resizeCropSettings, false);
            fileStream.Position = 0;
            ImageBuilder.Current.Build(fileStream, msMedium, resizeCropSettingsMedium, false);
            fileStream.Position = 0;
            ImageBuilder.Current.Build(fileStream, msSmall, resizeCropSettingsSmall, false);


            //Generate a file path
            string mainFileName = "main/" + fileName.Replace(" ", "");
            string mediumFileName = "medium/" + fileName.Replace(" ", "");
            string smallFileName = "small/" + fileName.Replace(" ", "");

            //if there are existing profile and thumb images delete them
            //if (account.ProfileImage != null)
            //{
            //    var oldProfile = blobContainer.GetBlobReference("profile/" + account.ProfileImage);
            //    oldProfile.Delete();
            //    var oldThumb = blobContainer.GetBlobReference("thumb/profile-pic");
            //    oldThumb.Delete();
            //}

            // Create the Blob and upload the file
            var blob = blobContainer.GetBlobReference(mainFileName);
            var blobThumb = blobContainer.GetBlobReference(mediumFileName);
            var blobSmall = blobContainer.GetBlobReference(smallFileName);
            ms.Position = 0;
            msMedium.Position = 0;
            msSmall.Position = 0;
            blob.UploadFromStream(ms);
            blobThumb.UploadFromStream(msMedium);
            blobSmall.UploadFromStream(msSmall);
            ms.Close();
            msMedium.Close();
            msSmall.Close();

            ////update account table with profile image filename
            //try
            //{
            //    account.ProfileImage = fileName.Replace(" ", "");
            //    _data.SaveChanges();
            //}
            //catch
            //{
            //    //todo: log exceptions with ELMAH
            //}

            return mainFileName;
        }

        public IQueryable<Post> GetMyPosts(int accountId)
        {
            return _data.Posts.Where(x => x.AccountId == accountId);
        }

        public bool IsNewPostSaved(int accountId, string title, string description, int categoryId, string imagePath)
        {
            try
            {
                Post newPost = new Post();
                newPost.AccountId = accountId;
                newPost.Title = title;
                newPost.Description = description;
                newPost.CategoryId = categoryId;
                newPost.MainImage = imagePath;
                newPost.InsertDate = DateTime.UtcNow;

                _data.Posts.AddObject(newPost);
                _data.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                //todo: log errors with ELMAH
                return false;
            }
        }

        public IQueryable<Category> GetAllCategories()
        {
            return _data.Categories.AsQueryable();
        }
    }
}
