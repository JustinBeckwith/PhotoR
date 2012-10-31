using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoR.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            var container = StorageHelper.getPhotoContainer();
            var table = StorageHelper.getTable();            
            var query = new TableQuery<PicEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "pics"));
            var result = table.ExecuteQuery(query)
                .OrderByDescending(x => x.DateCreated)
                .Take(6)
                .Select(x => container.GetBlockBlobReference(x.RowKey).Uri.AbsoluteUri);
            ViewBag.pics = result.ToList();
            return View();
        }        


        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            try
            {                                         
                var container = StorageHelper.getPhotoContainer();
                var id = Guid.NewGuid().ToString();
                var blob = container.GetBlockBlobReference(id);                                
                blob.UploadFromStream(Request.Files[0].InputStream);

                var table = StorageHelper.getTable();
                var pic = new PicEntity(id);
                pic.DateCreated = DateTime.Now;
                var insertOp = TableOperation.Insert(pic);
                table.Execute(insertOp);
                
                Response.Write(blob.Uri);
                Response.End();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Reset()
        {            
            var table = StorageHelper.getTable();
            table.DeleteIfExists();         
            return View();
        }        


    }
}
