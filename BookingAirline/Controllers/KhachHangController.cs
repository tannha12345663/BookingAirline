using BookingAirline.Models;
using System;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

namespace BookingAirline.Controllers
{
    public class KhachHangController : Controller
    {
        BookingAirLightEntities database = new BookingAirLightEntities();
        // GET: KhachHang
        public ActionResult TrangChu()
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
            if  (check == "one-way")
            {
                
                return RedirectToAction("DienThongTinKH");
            }
            else
            {
                var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                OrderStatu order = new OrderStatu();
                order.IDUser = uid;
                var count = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
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
            var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            OrderStatu order = new OrderStatu();
            order =database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
            order.MaCBve = id;
            database.Entry(order).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            
            Session["Mave"] = id;
            return View();
        }
        [HttpPost]
        public ActionResult DienThongTinKH()
        {
            var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
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
            ve.IDKH = "Vang Lai";
            ve.MaHV = "HV01";
            total +=(int) ve.GiaVe;
            database.Ves.Add(ve);
            database.SaveChanges();
            dsorder.MaCBdi = ve.MaVe;
            database.Entry(dsorder).State = System.Data.Entity.EntityState.Modified;
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
                ve.IDKH = "Vang Lai";
                ve.MaHV = "HV01";
                total += (int)ve.GiaVe;
                database.Ves.Add(ve);
                database.SaveChanges();
                dsorder.MaCBve = ve.MaVe;
                database.Entry(dsorder).State = System.Data.Entity.EntityState.Modified;
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
        [HttpGet]
        public ActionResult ThanhToan()
        {
           
            var stt = "Chưa thanh toán";
            var uid = "Vang Lai";
            var dsve = database.Ves.Where(s => s.IDKH == uid && s.TinhTrang == stt).ToList();
            
            return View(dsve);
        }
        public ActionResult ConfirmTT(string id)
        {
            var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            var kh = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
            var mavedi = database.Ves.Where(s => s.MaVe == kh.MaCBdi).FirstOrDefault();
            var maveve = database.Ves.Where(s => s.MaVe == kh.MaCBve).FirstOrDefault();
            mavedi.TinhTrang = "Đã thanh toán";
            database.Entry(mavedi).State = System.Data.Entity.EntityState.Modified;
            database.SaveChanges();
            if(maveve != null)
            {
                maveve.TinhTrang = "Đã thanh toán";
                database.Entry(maveve).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges();
            };
            var ttkh = (Order)Session["contacKH"];
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/Template/HtmlPage1.html"));
            content = content.Replace("{{CustomerName}}", ttkh.ShipName);
            content = content.Replace("{{Phone}}", ttkh.NumberPhone);
            content = content.Replace("{{Email}}", ttkh.ShipEmail);
            content = content.Replace("{{Total}}", Convert.ToString(ttkh.Total));
            string subject = "Đây là tin nhắn tự động từ hệ thống POS";
            WebMail.Send(ttkh.ShipEmail, subject, content, null, null, null, true, null, null, null, null, null, null);
            return RedirectToAction("ThankYou");
        }
        public ActionResult ThankYou()
        {
            return View();
        }
    }
}