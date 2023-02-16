using BookingAirline.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BookingAirline.Controllers
{
    public class ProductController : Controller
    {
        BookingAirLightEntities database = new BookingAirLightEntities();
        // GET: Product
        public ActionResult Index()
        {
            return View(database.ChuyenBays.ToList());
        }
        #region Thêm vé
        //public ActionResult Create() 
        //{
        //    Ve pro = new Ve();
        //    return View(pro);
        //}

        //[HttpPost]
        //public ActionResult Create(Ve pro)
        //{
        //    try
        //    {
        //        if(pro.UploadIMG != null) 
        //        {
        //            string filename = Path.GetFileNameWithoutExtension(pro.UploadIMG.FileName);
        //            string extent = Path.GetExtension(pro.UploadIMG.FileName);
        //            filename = filename + extent;
        //            pro.IMG = "~/Content/images/" + filename;
        //            pro.UploadIMG.SaveAs(Path.Combine(Server.MapPath("~/Content/images/"), filename));
        //        }
        //        database.Ves.Add(pro);
        //        database.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    catch  
        //    {
        //        return View();
        //    }
        //
        //}
        #endregion
    }
}