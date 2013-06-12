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
        /// <summary>
        /// show the top 6 images on the home page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var pageSize = 6;
            var picManager = new PicManager();
            ViewBag.pics = picManager.GetPics(pageSize).ToList();
            return View();
        }        

        /// <summary>
        /// upload the new pic to blob storage and add a new row in the table
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            try
            {
                var picManager = new PicManager();
                var uri = picManager.SavePic(Request.Files[0].InputStream);
                Response.Write(uri);
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
