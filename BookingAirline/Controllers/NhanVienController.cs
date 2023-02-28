using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAirline.App_Start;
using BookingAirline.Models;

namespace BookingAirline.Controllers
{
    [AuthenticationNV]
    public class NhanVienController : Controller
    {
        BookingAirLightEntities database = new BookingAirLightEntities();
        // GET: NhanVien
        public ActionResult TrangChu()
        {
            return View();
        }
        
        public ActionResult Scheduleaflight()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSchulea(ChuyenBay cb)
        {
            if (cb == null)
            {
                TempData["error"] = "Error";
                return RedirectToAction("Scheduleaflight");
            }
            else
            {
                Random rd = new Random();
                var macb = "VN" + rd.Next(1, 1000);
                cb.MaCB = macb;
                database.ChuyenBays.Add(cb);
                database.SaveChanges();
                var check = Convert.ToInt32(Request["checkbox01"]) ;
                if ( check == 1)
                {
                    Session["mcb"] = cb.MaCB;
                    TempData["themthongtin"] = check;
                }
                TempData["machuyenbay"] = cb.MaCB;
                TempData["messageAlert"] = "success";
                return RedirectToAction("Scheduleaflight");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPlan ([Bind(Include = "MaMB,LoaiMayBay")] MayBay mb)
        {
            if (mb == null)
            {
                TempData["error"] = "Error";
                return RedirectToAction("Scheduleaflight");
            }
            else
            {
                database.MayBays.Add(mb);
                database.SaveChanges();
                return RedirectToAction("Scheduleaflight");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdddetailSchulea(ChiTietChuyenBay ct)
        {
            var checkmacb = Session["mcb"];
            if (checkmacb != null)
            {
                ct.MaCTCB = (string)checkmacb;
            }
            else
            {
                TempData["messageAlert"] = "erorr";
                return RedirectToAction("Scheduleaflight");
            }
            TempData["machuyenbay"] = ct.MaCTCB;
            TempData["messageAlert"] = "success";
            database.ChiTietChuyenBays.Add(ct);
            database.SaveChanges();
            return RedirectToAction("Scheduleaflight");
        }
        public ActionResult AddSchedulea()
        {
            return View();
        }

        public ActionResult FlightRoute()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAirport([Bind(Include = "MaSB,TenSB")] SanBay sb)
        {
            if (sb == null)
            {
                TempData["error"] = "Error";
                return RedirectToAction("FlightRoute");
            }
            else
            {
                database.SanBays.Add(sb);
                database.SaveChanges();
                return RedirectToAction("FlightRoute");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddRoute(TuyenBay tb)
        {
            if (tb == null)
            {
                TempData["error"] = "Error";
                return RedirectToAction("FlightRoute");
            }
            else
            {
                Random rd = new Random();
                var matb = "TB" + rd.Next(1, 1000);
                tb.MaTBay = matb;
                database.TuyenBays.Add(tb);
                database.SaveChanges();
                return RedirectToAction("FlightRoute");
            }

        }
        public ActionResult Plane()
        {
            return View();
        }
        public ActionResult TicketList()
        {
            return View();
        }
        public ActionResult TotalRevenue()
        {
            return View();
        }
        public ActionResult Reports()
        {
            return View();
        }
    }
}