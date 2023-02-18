using BookingAirline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingAirline.Controllers
{
    public class KhachHangHAController : Controller
    {
        BookingAirLightEntities database = new BookingAirLightEntities();
        // GET: KhachHangHA
        public ActionResult TrangChu()
        {
            return View();
        }
        public ActionResult ThongTinKH()
        {
            return View();
        }
        public ActionResult ThongtinCaNhan()
        {
            var user = (BookingAirline.Models.KhachHang)Session["userKH"];
            var data = database.KhachHangs.Where(s => s.IDKH == user.IDKH).FirstOrDefault();
            return View(data);
        }
        public ActionResult Booking()
        {
            return View();
        }
        public ActionResult Whishlist()
        {
            return View();
        }
        public ActionResult MyCard()
        {
            return View();
        }
    }
}