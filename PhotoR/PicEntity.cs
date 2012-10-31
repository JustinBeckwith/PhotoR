using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoR
{
    public class PicEntity : TableEntity
    {
        public PicEntity(string BlobId)
        {
            this.RowKey = BlobId;
            this.PartitionKey = "pics";
        }

        public PicEntity()
        {

        }
        
        public DateTime DateCreated { get; set; }

    }
}