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
            var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            OrderStatu order = new OrderStatu();
            order.IDUser = uid;
            var count = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
            if (count != null)
            {
                database.OrderStatus.Remove(count);
                database.SaveChanges();
            }
            //Kiểm tra nếu là khứ hồi
            if (Session["trip"] == null)
            {
                return RedirectToAction("TrangChu");
            }
            var check = Session["trip"].ToString();
            if  (check == "oneway")
            {
                order.MaCBdi = id;
                database.OrderStatus.Add(order);
                database.SaveChanges();
                return RedirectToAction("ChooseSeat");
            }
            else
            {
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
        [HttpGet]
        public ActionResult DSachCBVe01(string id)
        {
            var check = Session["trip"].ToString();
            if (check == "round")
            {
                var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                OrderStatu order = new OrderStatu();
                order = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
                order.MaCBve = id;
                database.Entry(order).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges();
                Session["Mave"] = id;
                return RedirectToAction("ChooseSeat");
            }
            return View();
        }
        //Version 1.0
        public ActionResult DienThongTinKH(string id)
        {
            
            return View();
        }
        
        [HttpPost]
        public ActionResult DienThongTinKH()
        {
            var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
            Random rd = new Random();
            var total = 0;
            #region Bancu
            //var mave = "VE" + rd.Next(1, 1000);
            //check ma ve duoi database
            //Tao ve moi
            //Ve ve = new Ve();
            //ve.MaVe = mave;
            //ve.MaCB = dsorder.MaCBdi;
            //ve.TinhTrang = "Chưa thanh toán";
            //ve.GiaVe = 500000;
            //ve.CCCD = Request["cccd"];
            //ve.IDKH = "Vang Lai";
            //ve.MaHV = "HV01";
            //total +=(int) ve.GiaVe;
            //database.Ves.Add(ve);
            //database.SaveChanges();
            //dsorder.MaCBdi = ve.MaVe;
            //database.Entry(dsorder).State = System.Data.Entity.EntityState.Modified;
            //database.SaveChanges();
            ////Kiểm tra nếu có khứ hồi 
            //if (dsorder.MaCBve != null)
            //{
            //    ve = new Ve();
            //    mave = "VE" + rd.Next(1, 1000);
            //    ve.MaVe = mave;
            //    ve.MaCB = dsorder.MaCBve;
            //    ve.TinhTrang = "Chưa thanh toán";
            //    ve.GiaVe = 500000;
            //    ve.CCCD = Request["cccd"];
            //    ve.IDKH = "Vang Lai";
            //    ve.MaHV = "HV01";
            //    total += (int)ve.GiaVe;
            //    database.Ves.Add(ve);
            //    database.SaveChanges();
            //    dsorder.MaCBve = ve.MaVe;
            //    database.Entry(dsorder).State = System.Data.Entity.EntityState.Modified;
            //    database.SaveChanges();
            //}
            #endregion
            var contact = new Order();
            contact.CreateDate = DateTime.Now;
            contact.ShipName = Request["name"];
            contact.ShipEmail = Request["email"];
            contact.NumberPhone = Request["number"];
            contact.CCCD = Request["cccd"];
            contact.Total = total;
            Session["contacKH"] = contact;
            return RedirectToAction("ThanhToan");
        }
        //Version 2.0
        //Chọn chỗ ngồi lúc đi
        [HttpGet]
        public ActionResult ChooseSeat()
        {
            var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
            var dsve = database.Ves.Where(s => s.MaCB == dsorder.MaCBdi).ToList();
            ViewData["MaCB"] = dsorder.MaCBdi;
            return View(dsve);
        }
        [HttpPost]
        public ActionResult ChooseSeat(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
            {
                cart.XoaSauKhiDat();
            }
            int check = 0;
            var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
            var dsve = database.Ves.Where(s => s.MaCB == dsorder.MaCBdi).ToList();
            for(int i =1; i <= dsve.Count(); i++)
            {
                if( Request["Ma" + i] != null)
                {
                    //14/03/2023 Suy nghĩ làm thêm luồn xử lý dữ liệu
                    //Add thông tin vé vào giỏ hàng
                    var ticket = Request["Ma" + i];
                    var detailtic = database.Ves.Where(s => s.MaCB == dsorder.MaCBdi && s.MaVe == ticket).FirstOrDefault();
                    GetCart().Add(detailtic,1);
                    
                    check++;
                    if(check == id)
                    {
                        break;
                    }
                }
                
            }
            var ktkh = Session["trip"].ToString();
            if (ktkh == "round")
            {
                return RedirectToAction("ChooseSeatVe");
            }
            return RedirectToAction("DienThongTinKH");
                
        }

        //Chọn chỗ ngồi lúc về
        [HttpGet]
        public ActionResult ChooseSeatVe()
        {
            var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
            var dsve = database.Ves.Where(s => s.MaCB == dsorder.MaCBve).ToList();
            ViewData["MaCB"] = dsorder.MaCBve;
            return View(dsve);
        }
        [HttpPost]
        public ActionResult ChooseSeatVe(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            int check = 0;
            var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
            var dsve = database.Ves.Where(s => s.MaCB == dsorder.MaCBve).ToList();
            for (int i = 1; i <= dsve.Count(); i++)
            {
                if (Request["Ma" + i] != null)
                {
                    //Add thông tin vé vào giỏ hàng
                    var ticket = Request["Ma" + i];
                    var detailtic = database.Ves.Where(s => s.MaCB == dsorder.MaCBve && s.MaVe == ticket).FirstOrDefault();
                    GetCart().Add(detailtic, 1);

                    check++;
                    if (check == id)
                    {
                        break;
                    }
                }

            }
            return RedirectToAction("DienThongTinKH");
        }
        // Tạo mới giỏ hàng
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
        [HttpGet]
        public ActionResult ThanhToan()
        {
            if(Session["Cart"] == null)
            {
                return View();
            }
            Cart cart = Session["Cart"] as Cart;
            var tt = cart.TongTien();
            var contact = (Order)Session["contacKH"];
            contact.Total = tt;
            Session["contacKH"] = contact;
            return View(cart);
        }
        public ActionResult ConfirmTT(string id)
        {
            var uid = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            var kh = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
            
            var maveve = database.Ves.Where(s => s.MaCB == kh.MaCBve).FirstOrDefault();
            var ttkh = (Order)Session["contacKH"]; // Thông tin liên lạc của KH
            var tongtien = string.Format("{0:0,0 vnđ}", ttkh.Total);
            //mavedi.TinhTrang = "Đã thanh toán";
            //database.Entry(mavedi).State = System.Data.Entity.EntityState.Modified;
            //database.SaveChanges();

            //Thêm lưu xuất ra hóa đơn
            Random rd = new Random();
            HoaDon themhd = new HoaDon();
            themhd.MaHD = "HD" + rd.Next(1, 100) + rd.Next(1, 100);
            themhd.TinhTrang = "Đã thanh toán";
            themhd.NgayLap = System.DateTime.Now;
            themhd.ThanhTien = ttkh.Total;
            themhd.CCCD = ttkh.CCCD;
            database.HoaDons.Add(themhd);
            database.SaveChanges();

            //Thêm chi tiết hóa đơn
            ChiTietHD cthd = new ChiTietHD();
            cthd.MaHD = themhd.MaHD;

            Cart cart = Session["Cart"] as Cart;
            var dsorder = cart.Items;
            foreach (var item in dsorder)
            {
                var mavedi = database.Ves.Where(s => s.MaVe == item.idVe.MaVe && s.MaCB == item.idVe.MaCB).FirstOrDefault();
                mavedi.TinhTrang = "Đã thanh toán";
                mavedi.CCCD = ttkh.CCCD;
                database.Entry(mavedi).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges();

                cthd.MaVe = item.idVe.MaVe;
                cthd.SoLuong = item.soLuong;
                cthd.DonGia = item.idVe.GiaVe;
                cthd.MaCB = item.idVe.MaCB;
                cthd.TongTien = (item.soLuong) * (item.idVe.GiaVe);
                database.ChiTietHDs.Add(cthd);
                database.SaveChanges();
            }
            if (maveve != null)
            {
                maveve.TinhTrang = "Đã thanh toán";
                database.Entry(maveve).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges();
            };

            string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/Template/HtmlPage1.html"));
            content = content.Replace("{{CustomerName}}", ttkh.ShipName);
            content = content.Replace("{{Phone}}", ttkh.NumberPhone);
            content = content.Replace("{{Email}}", ttkh.ShipEmail);
            content = content.Replace("{{Total}}", tongtien);
            content = content.Replace("{{Thoigian}}", Convert.ToString(ttkh.CreateDate));
            string subject = "Đây là tin nhắn tự động từ hệ thống POS";
            WebMail.Send(ttkh.ShipEmail, subject, content, null, null, null, true, null, null, null, null, null, null);
            cart.XoaSauKhiDat();
            var count = database.OrderStatus.Where(s => s.IDUser == uid).FirstOrDefault();
            database.OrderStatus.Remove(count);
            database.SaveChanges();
            return RedirectToAction("ThankYou");
        }
        public ActionResult ThankYou()
        {
            return View();
        }
        public ActionResult DSChuyenBayTest()
        {
            return View();
        }
    }
}