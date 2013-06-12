using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PhotoR
{
    public class PicManager
    {
        /// <summary>
        /// get the blob uri for the last 6 uploaded pictures
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetPics(int pageSize)
        {
            var container = StorageHelper.getPhotoContainer();
            var table = StorageHelper.getTable();
            var query = new TableQuery<PicEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "pics"));
            var result = table.ExecuteQuery(query)
                .OrderByDescending(x => x.DateCreated)
                .Take(pageSize)
                .Select(x => container.GetBlockBlobReference(x.RowKey).Uri.AbsoluteUri);
            return result;
        }

        /// <summary>
        /// upload the picture to blob storage and return the uri
        /// </summary>
        /// <param name="picStream"></param>
        /// <returns></returns>
        public Uri SavePic(Stream picStream)
        {
            var container = StorageHelper.getPhotoContainer();
            var id = Guid.NewGuid().ToString();
            var blob = container.GetBlockBlobReference(id);
            blob.UploadFromStream(picStream);

            var table = StorageHelper.getTable();
            var pic = new PicEntity(id);
            pic.DateCreated = DateTime.Now;
            var insertOp = TableOperation.Insert(pic);
            table.Execute(insertOp);

            return blob.Uri;
        }
    }


}