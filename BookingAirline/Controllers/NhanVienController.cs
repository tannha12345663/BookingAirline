using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookingAirline.App_Start;
using BookingAirline.Models;
using Newtonsoft.Json;

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
            //Kiểm tra chuyến bay 
            if (cb == null)
            {
                TempData["error"] = "Error";
                return RedirectToAction("Scheduleaflight");
            }
            //Thêm chuyến bay
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
                    return RedirectToAction("Scheduleaflight");
                }
                TempData["machuyenbay"] = cb.MaCB;
                TempData["messageAlert"] = "success";
                //Sinh ra vé tự động dựa vào số ghế của chuyến bay
                ThemVe((int)cb.SoLuongGheHang1, (int)cb.SoLuongGheHang2, (int)cb.SoLuongGheHang3, cb);
                return RedirectToAction("Scheduleaflight");
            }

        }
        public void ThemVe(int id1,int id2,int id3, ChuyenBay cb)
        {
            Ve ticket = new Ve();
            //Kiểm tra nếu số lượng
            if (id1 ==0 && id2 == 0 && id3==0)
            {
                return ;
            }
            else
            {
                int stt = 0;
                //RAndom ghế hạng 1
                for (int i = 1; i <= id1; i++)
                {
                    ticket = new Ve();
                    stt++;
                    ticket.MaVe = "VE" + stt;
                    ticket.MaCB = cb.MaCB;
                    ticket.MaHV = "HV01";
                    ticket.TinhTrang = "Chưa đặt chỗ";
                    ticket.GiaVe = Convert.ToDouble( Request["dongiaG1"]);
                    ticket.CCCD = "null";
                    database.Ves.Add(ticket);
                    database.SaveChanges();
                }
                for (int i = 1; i <= id2; i++)
                {
                    ticket = new Ve();
                    stt++;
                    ticket.MaVe = "VE" + stt;
                    ticket.MaCB = cb.MaCB;
                    ticket.MaHV = "HV02";
                    ticket.TinhTrang = "Chưa đặt chỗ";
                    ticket.GiaVe = Convert.ToDouble(Request["dongiaG2"]);
                    ticket.CCCD = "null";
                    database.Ves.Add(ticket);
                    database.SaveChanges();
                }
                for (int i = 1; i <= id3; i++)
                {
                    ticket = new Ve();
                    stt++;
                    ticket.MaVe = "VE" + stt;
                    ticket.MaCB = cb.MaCB;
                    ticket.MaHV = "HV03";
                    ticket.TinhTrang = "Chưa đặt chỗ";
                    ticket.GiaVe = Convert.ToDouble(Request["dongiaG3"]);
                    ticket.CCCD = "null";
                    database.Ves.Add(ticket);
                    database.SaveChanges();
                }
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
            var checkmacb = (string)Session["mcb"];
            if (checkmacb != null)
            {
                ct.MaCTCB = checkmacb;
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

        public ActionResult DetailFlight(string id)
        {
            if (id != null)
            {
                var thongtincb = database.ChuyenBays.Find(id);
                TempData["themthongtin"] = 2;
                return View(thongtincb);
            }
            else if(id == null)
            {
                return HttpNotFound();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetailFlight(ChuyenBay cb)
        {
            ChiTietChuyenBay ctcb = new ChiTietChuyenBay();
            ctcb.MaCTCB = cb.MaCB;
            ctcb.SanBayTrungGian = Request["SanBayTrungGian"];
            ctcb.ThoiGianDung = Request["ThoiGianDung"];
            ctcb.GhiChu = Request["GhiChu"];
            database.Entry(ctcb).State = (System.Data.Entity.EntityState)System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            database.Entry(cb).State = (System.Data.Entity.EntityState)System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            TempData["machuyenbay"] = cb.MaCB;
            TempData["messageAlert"] = "SuaThanhCong";
            return RedirectToAction("Scheduleaflight");
        }
        [HttpPost]
        public ActionResult DeleteFlight(string id)
        {
            var tb = database.ChuyenBays.Find(id);
            database.ChuyenBays.Remove(tb);
            database.SaveChanges();
            TempData["machuyenbay"] = tb.MaCB;
            TempData["messageAlert"] = "XoaThanhCong";
            //return RedirectToAction("Scheduleaflight");
            return Json(new { success = true });
        }
        public ActionResult AddSchedulea()
        {
            return View();
        }

        public ActionResult FlightRoute()
        {

            return View();
        }
        [HttpGet]
        public JsonResult GetData()
        {
            bool proxyCreation = database.Configuration.ProxyCreationEnabled;
            try
            {
                database.Configuration.ProxyCreationEnabled = false;
                var dstb = database.TuyenBays.ToList();
                return Json(new { Data=dstb,TotalItems=dstb.Count}, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(ex.Message);
            }
            finally
            {
                //restore ProxyCreation to its original state
                database.Configuration.ProxyCreationEnabled = proxyCreation;
            }
        }
        // Thêm AddAirport
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddAirport([Bind(Include = "MaSB,TenSB")] SanBay sb)
        //{
        //    if (sb == null)
        //    {
        //        TempData["error"] = "Error";
        //        return RedirectToAction("FlightRoute");
        //    }
        //    else
        //    {
        //        database.SanBays.Add(sb);
        //        database.SaveChanges();
        //        return RedirectToAction("FlightRoute");
        //    }
            
        //}
        //Áp dụng Ajax vào chức năng thêm mới sân bay
        [HttpPost]
        public ActionResult AddAirport(SanBay sb)
        {
            if (sb != null)
            {
                database.SanBays.Add(sb);
                try
                {
                    database.SaveChanges();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    TempData["error"] = "Error";
                    return Json(new { success = false });
                }
            }
            TempData["error"] = "Error";
            return Json(new { success = false}); // Khai báo trả lại status của json
        }

        [HttpPost]
        public ActionResult AddRoute(TuyenBay tb)
        {
            if (tb == null)
            {
                TempData["messageAlert"] = "Error";
                return RedirectToAction("FlightRoute");
            }
            else
            {
                if (tb.SanBayDi == tb.SanBayDen)
                {
                    TempData["messageAlert"] = "Error01";
                    return RedirectToAction("FlightRoute");
                }
                Random rd = new Random();
                var matb = "TB" + rd.Next(1, 1000);
                tb.MaTBay = matb;
                database.TuyenBays.Add(tb);
                database.SaveChanges();
                TempData["matuyenbay"] = tb.MaTBay;
                TempData["messageAlert"] = "success";
                //return RedirectToAction("FlightRoute");
                return Json(new { success = true });
            }

        }
        public ActionResult DetailFR(string id)
        {
            var ttFR = database.TuyenBays.Find(id);
            return View(ttFR);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetailFR(TuyenBay tb)
        {
            database.Entry(tb).State = (System.Data.Entity.EntityState)System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            TempData["matuyenbay"] = tb.MaTBay;
            TempData["messageAlert"] = "SuaTBTC";
            return RedirectToAction("FlightRoute");
        }
        [HttpPost]
        public ActionResult DeleteFR(string id)
        {
            var tb = database.TuyenBays.Find(id);
            database.TuyenBays.Remove(tb);
            database.SaveChanges();
            TempData["matuyenbay"] = tb.MaTBay;
            TempData["messageAlert"] = "XoaTBay";
            //return RedirectToAction("FlightRoute");
            return Json(new { success = true });
        }
        public ActionResult Plane()
        {
            return View();
        }
        //Thêm vé máy bay sau khi đã có chuyến bay
        public ActionResult TicketManagement()
        {
            var dsticket = database.Ves.ToList();
            return View(dsticket);
        }
        public ActionResult TicketList()
        {     
            return View();
        }
        public ActionResult TotalRevenue()
        {
            var dshd = database.HoaDons.ToList();
            return View(dshd);
        }
        public ActionResult DetailTotalRevenue(string id)
        {
            var cthd = database.ChiTietHDs.Where(s => s.MaHD == id).ToList();
            TempData["Mahd"] = id;
            return View(cthd);
        }
        public ActionResult Reports()
        {
            return View();
        }
    }
}