using BookingAirline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAirline.App_Start;
using System.Web.Helpers;

namespace BookingAirline.Controllers
{
    [Authentication]
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
        [HttpGet]
        public ActionResult EditProfile(string id)
        {
            KhachHang kh = database.KhachHangs.Find(id);
            ViewBag.MaLKH = new SelectList(database.LoaiKHs, "MaLKH", "TenLKH");
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "IDKH,MaLKH,UserName,Password,TenKH,SDT,Email,GioiTinh,NgaySinh")] KhachHang kh )
        {
            //database.Entry(kh).State = System.Data.Entity.EntityState.Modified;
            //database.SaveChanges();
            //return RedirectToAction("ThongtinCaNhan");
            if (ModelState.IsValid)
            {
                database.Entry(database.KhachHangs.Find(kh.IDKH)).CurrentValues.SetValues(kh);
                database.SaveChanges();
                return View("ThongTinKH");
            }
            return View(kh);
        }

        public ActionResult Booking()
        {
            var user = (BookingAirline.Models.KhachHang)Session["userKH"];
            var hd = database.HoaDons.Where(s => s.IDKH== user.IDKH).ToList();
            return View(hd);
        }

        public ActionResult DetailBooking(string id)
        {
            var cthd = database.ChiTietHDs.Where(s => s.MaHD == id).ToList();
            TempData["mahd"] = id;
            return View(cthd);
        }
        public ActionResult Whishlist()
        {
            return View();
        }

        public ActionResult MyCard()
        {
            return View();
        }
        public ActionResult DSachCB()
        {
            Session["From"] = Request["from"];
            Session["To"] = Request["to"];
            Session["Trip"] = Request["trip"];

            //Lọc tìm kiếm chuyến bay
            var di = Request["From"].ToString();
            var chuyendi = database.TuyenBays.Where(s => s.SanBayDi == di).FirstOrDefault();
            var listdi = database.ChuyenBays.Where(s => s.MaTBay == chuyendi.MaTBay).ToList();
            //Hiển thị danh sách các chuyến bay
            return View(listdi);
        }
        public ActionResult DSachCBVe(string id)
        {

            //Kiểm tra nếu là khứ hồi
            if (Session["trip"] == null)
            {
                return RedirectToAction("TrangChu");
            }
            var check = Session["trip"].ToString();
            if (check == "one-way")
            {

                return RedirectToAction("DienThongTinKH");
            }
            else
            {
                var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
                OrderStatu order = new OrderStatu();
                order.IDUser = uid.IDKH;
                var count = database.OrderStatus.Where(s => s.IDUser == uid.IDKH).FirstOrDefault();
                if (count != null)
                {
                    database.OrderStatus.Remove(count);
                    database.SaveChanges();
                }
                order.MaCBdi = id;
                database.OrderStatus.Add(order);
                database.SaveChanges();
                //Kiểm tra và xuất dữ liệu vé theo trước
                var to = Session["To"].ToString();
                var chuyenve = database.TuyenBays.Where(s => s.SanBayDi == to).FirstOrDefault();
                var listcv = database.ChuyenBays.Where(s => s.MaTBay == chuyenve.MaTBay).ToList();
                return View(listcv);
            }

        }
        public ActionResult DienThongTinKH(string id)
        {
            var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
            OrderStatu order = new OrderStatu();
            order = database.OrderStatus.Where(s => s.IDUser == uid.IDKH).FirstOrDefault();
            order.MaCBve = id;
            database.Entry(order).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();

            Session["Mave"] = id;

            return View();
        }
        [HttpPost]
        public ActionResult DienThongTinKH()
        {
            var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid.IDKH).FirstOrDefault();
            Random rd = new Random();
            var total = 0;
            var mave = "VE" + rd.Next(1, 1000);
            //check ma ve duoi database
            //Tao ve moi
            Ve ve = new Ve();
            ve.MaVe = mave;
            ve.MaCB = dsorder.MaCBdi;
            ve.TinhTrang = "Chưa thanh toán";
            ve.MaPhi = "PS01";
            ve.GiaVe = 500000;
            ve.CCCD = Request["cccd"];
            ve.IDKH = uid.IDKH;
            ve.MaHV = "HV01";
            total += (int)ve.GiaVe;
            database.Ves.Add(ve);
            database.SaveChanges();
            //Kiểm tra nếu có khứ hồi 
            if (dsorder.MaCBve != null)
            {
                ve = new Ve();
                mave = "VE" + rd.Next(1, 1000);
                ve.MaVe = mave;
                ve.MaCB = dsorder.MaCBve;
                ve.TinhTrang = "Chưa thanh toán";
                ve.MaPhi = "PS01";
                ve.GiaVe = 500000;
                ve.CCCD = Request["cccd"];
                ve.IDKH = uid.IDKH;
                ve.MaHV = "HV01";
                total += (int)ve.GiaVe;
                database.Ves.Add(ve);
                database.SaveChanges();
            }
            var contact = new Order();
            contact.CreateDate = DateTime.Now;
            contact.ShipName = Request["name"];
            contact.ShipEmail = Request["email"];
            contact.NumberPhone = Request["number"];
            contact.Total = total;
            Session["contacKH"] = contact;
            return RedirectToAction("ThanhToan");
        }

        public ActionResult ThanhToan()
        {
            var stt = "Chưa thanh toán";
            var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
            var dsve = database.Ves.Where(s => s.IDKH == uid.IDKH && s.TinhTrang == stt).ToList();
            return View(dsve);
        }
        [HttpPost]
        public ActionResult ThanhToan01()
        {
            var ttkh = (Order)Session["contacKH"];
            var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
            //Lên hóa đơn mới
            HoaDon hd = new HoaDon();
            Random rd = new Random();
            hd.MaHD = "HD" + rd.Next(1, 1000);
            hd.IDNV = "HT";
            hd.TinhTrang = "Đã thanh toán";
            hd.NgayLap = System.DateTime.Now;
            hd.IDKH = uid.IDKH;
            hd.ThanhTien = ttkh.Total;
            database.HoaDons.Add(hd);
            database.SaveChanges();
            //Thêm chi tiết hóa đơn
            ChiTietHD cthd = new ChiTietHD();
            cthd.MaHD = hd.MaHD;
            var tt = "Chưa thanh toán";
            var Order = database.Ves.Where(s => s.IDKH == uid.IDKH && s.TinhTrang == tt).ToList() ;
            foreach(var item in Order)
            {
                cthd.MaVe = item.MaVe;
                cthd.SoLuong = 1;
                cthd.DonGia = item.GiaVe;
                cthd.TongTien = cthd.SoLuong * cthd.DonGia;
                database.ChiTietHDs.Add(cthd);
                database.SaveChanges();

                Ve stt = new Ve();
                stt.MaVe = item.MaVe;
                stt.MaCB = item.MaCB;
                stt.MaPhi = item.MaPhi;
                stt.MaHV = item.MaHV;
                stt.IDKH = item.IDKH;
                stt.TinhTrang = "Đã thanh toán";
                stt.GiaVe = item.GiaVe;
                stt.CCCD = item.CCCD;
                database.Entry(database.Ves.Find(item.MaVe)).CurrentValues.SetValues(stt);
                database.SaveChanges();
            }
            
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/Template/HtmlPage1.html"));
            content = content.Replace("{{CustomerName}}", ttkh.ShipName);
            content = content.Replace("{{Phone}}", ttkh.NumberPhone);
            content = content.Replace("{{Email}}", ttkh.ShipEmail);
            var total = string.Format("{0:0,0 vnđ}", ttkh.Total);
            content = content.Replace("{{Total}}", total);
            content = content.Replace("{{Thoigian}}",Convert.ToString(ttkh.CreateDate));
            string subject = "Đây là tin nhắn tự động từ hệ thống POS";
            WebMail.Send(ttkh.ShipEmail, subject, content, null, null, null, true, null, null, null, null, null, null);
            return RedirectToAction("ThankYou");
        }
        public ActionResult ConfirmTT()
        {

            return RedirectToAction("ThankYou");
        }
        public ActionResult ThankYou()
        {
            return View();
        }
    }
}