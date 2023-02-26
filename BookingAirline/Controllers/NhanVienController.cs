using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAirline.App_Start;
namespace BookingAirline.Controllers
{
    [AuthenticationNV]
    public class NhanVienController : Controller
    {
        // GET: NhanVien
        public ActionResult TrangChu()
        {
            return View();
        }
        public ActionResult Scheduleaflight()
        {
            return View();
        }
    }
}