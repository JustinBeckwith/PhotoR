using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Core;
using System.Configuration;

namespace PhotoR
{
    /// <summary>
    /// a simple class to get our photo container
    /// </summary>
    public class StorageHelper
    {
        public static CloudBlobContainer getPhotoContainer()
        {
            var storageAccount = StorageHelper.getAccount();
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("myphotos");
            container.CreateIfNotExists();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            return container;
        }

        public static CloudTable getTable()
        {
            var storageAccount = StorageHelper.getAccount();
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("pics");
            table.CreateIfNotExists();
            return table;
        }

        public static CloudStorageAccount getAccount()
        {
            var cred = new StorageCredentials(ConfigurationManager.AppSettings["storageName"], ConfigurationManager.AppSettings["storageKey"]);
            var storageAccount = new CloudStorageAccount(cred, false);
            return storageAccount;
        }
    }
}