using BookingAirline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAirline.App_Start;
using System.Web.Helpers;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using BookingAirline.Models.VNPay;

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
        public void LuuAnh(KhachHang kh, HttpPostedFileBase HinhAnh)
        {
            #region Hình ảnh
            //Xác định đường dẫn lưu file : Url tương đói => tuyệt đói
            var urlTuongdoi = "/Data/Avatar/";
            var urlTuyetDoi = Server.MapPath(urlTuongdoi);// Lấy đường dẫn lưu file trên server

            //Check trùng tên file => Đổi tên file  = tên file cũ (ko kèm đuôi)
            //Ảnh.jpg = > ảnh + "-" + 1 + ".jpg" => ảnh-1.jpg

            string fullDuongDan = urlTuyetDoi + HinhAnh.FileName;
            int i = 1;
            while (System.IO.File.Exists(fullDuongDan) == true)
            {
                // 1. Tách tên và đuôi 
                var ten = Path.GetFileNameWithoutExtension(HinhAnh.FileName);
                var duoi = Path.GetExtension(HinhAnh.FileName);
                // 2. Sử dụng biến i để chạy và cộng vào tên file mới
                fullDuongDan = urlTuyetDoi + ten + "-" + i + duoi;
                i++;
                // 3. Check lại 
            }
            #endregion
            //Lưu file (Kiểm tra trùng file)
            HinhAnh.SaveAs(fullDuongDan);
            kh.HinhAnh = urlTuongdoi + Path.GetFileName(fullDuongDan);
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
        public ActionResult EditProfile([Bind(Include = "IDKH,MaLKH,UserName,Password,TenKH,SDT,Email,GioiTinh,NgaySinh,HinhAnh")] KhachHang kh, HttpPostedFileBase HinhAnh)
        {
            //database.Entry(kh).State = System.Data.Entity.EntityState.Modified;
            //database.SaveChanges();
            //return RedirectToAction("ThongtinCaNhan");
            if (ModelState.IsValid)
            {
                if (HinhAnh != null)
                {
                    LuuAnh(kh, HinhAnh);
                    database.Entry(database.KhachHangs.Find(kh.IDKH)).CurrentValues.SetValues(kh);
                    database.SaveChanges();
                }
                else
                {
                    var checkkh = database.KhachHangs.Find(kh.IDKH);
                    kh.HinhAnh = checkkh.HinhAnh;
                    database.Entry(database.KhachHangs.Find(kh.IDKH)).CurrentValues.SetValues(kh);
                    database.SaveChanges();
                }
                return View("ThongTinKH");
            }
            return View(kh);
        }

        public ActionResult Booking()
        {
            var user = (BookingAirline.Models.KhachHang)Session["userKH"];
            var hd = database.HoaDons.Where(s => s.IDKH == user.IDKH).ToList();
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
            Session["Return"] = Request["return"];
            DateTime ngaykh = Convert.ToDateTime(Request["deparure"]);
            var month = ngaykh.ToString("MM");
            var Day = ngaykh.ToString("dd");
            var year = ngaykh.ToString("yyyy");
            //Lọc tìm kiếm chuyến bay
            var di = Request["From"].ToString();
            var den = Request["to"].ToString();
            var chuyendi = database.TuyenBays.Where(s => s.SanBayDi == di && s.SanBayDen ==den ).FirstOrDefault();
            //var listdi = database.ChuyenBays.Where(s => s.MaTBay == chuyendi.MaTBay && Convert.ToDateTime(s.NgayGio).ToString("dd")== Day ).ToList();
            var test = database.ChuyenBays.SqlQuery
                ("Select * from ChuyenBay where YEAR(NgayGio)= @year and DAY (NgayGio) = @day and MONTH(NgayGio)= @month and MaTbay=@chuyendi",
                new SqlParameter("@year", year),
                new SqlParameter("@day", Day),
                new SqlParameter("@month", month),
                new SqlParameter("@chuyendi",chuyendi.MaTBay)
                ).ToList();
            //Hiển thị danh sách các chuyến bay
            return View(test);
        }
        public ActionResult DSachCBVe(string id)
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
            //Kiểm tra nếu là khứ hồi
            if (Session["trip"] == null)
            {
                return RedirectToAction("TrangChu");
            }
            var check = Session["trip"].ToString();
            if (check == "one-way")
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

                DateTime ngaykh = Convert.ToDateTime(Session["Return"]);
                var month = ngaykh.ToString("MM");
                var Day = ngaykh.ToString("dd");
                var year = ngaykh.ToString("yyyy");

                //Kiểm tra và xuất dữ liệu vé theo trước
                var di = Session["To"].ToString();
                var den = Session["From"].ToString();
                var chuyenve = database.TuyenBays.Where(s => s.SanBayDi == di && s.SanBayDen == den).FirstOrDefault();
                //var listcv = database.ChuyenBays.Where(s => s.MaTBay == chuyenve.MaTBay).ToList();
                var test = database.ChuyenBays.SqlQuery
                    ("Select * from ChuyenBay where YEAR(NgayGio)= @year and DAY (NgayGio) = @day and MONTH(NgayGio)= @month and MaTBay = @chuyenve",
                        new SqlParameter("@year", year),
                        new SqlParameter("@day", Day),
                        new SqlParameter("@month", month),
                        new SqlParameter("@chuyenve",chuyenve.MaTBay)
                        ).ToList();
                return View(test);
            }

        }

        [HttpGet]
        public ActionResult DSachCBVe01(string id)
        {
            var check = Session["trip"].ToString();
            if (check == "round")
            {
                var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
                OrderStatu order = new OrderStatu();
                order = database.OrderStatus.Where(s => s.IDUser == uid.IDKH).FirstOrDefault();
                order.MaCBve = id;
                database.Entry(order).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges();
                Session["Mave"] = id;
                return RedirectToAction("ChooseSeat");
            }
            return View();
        }
        //Điền thông tin khách hàng
        public ActionResult DienThongTinKH(string id)
        {
            Session["SLKH"] = null;
            Cart cart = Session["Cart"] as Cart;
            var check = Session["trip"].ToString();
            if (check == "round")
            {
                Session["SLKH"] = (cart.Items.Count() / 2);
            }
            else if(check == "one-way")
            {
                Session["SLKH"] = cart.Items.Count();
            }
            return View(cart);
        }
        //Nhân thông tin khi người dùng nhập thông tin khách hàng tương ứng với vé
        [HttpPost]
        public ActionResult DienThongTinKH()
        {
            var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid.IDKH).FirstOrDefault();
            //Lấy thông tin khách hàng khi có nhiều vé
            var di = Session["From"].ToString();
            var den = Session["To"].ToString();
            var checkkhuhoi = Session["Return"];
            Cart cart = Session["Cart"] as Cart;
            var stt = 0;
            if (checkkhuhoi == null)
            {
                foreach (var item01 in cart.Items)
                {
                    var mave1 = item01.idVe.MaVe;
                    var cccd = Request["cccd_" + stt];
                    cart.CapNhatCCCD(mave1, cccd);
                    stt++;
                }
            }
            else
            {
                var cbdi = dsorder.MaCBdi;
                var cbden = dsorder.MaCBve;
                var number = 0;
                var number2 = 0;
                foreach (var item01 in cart.Items)
                {

                    //Chép cccd vào vé lúc đi của khách khàng
                    if (item01.idVe.MaCB == cbdi)
                    {
                        var mave1 = item01.idVe.MaVe;
                        var cccd = Request["cccd_" + number];
                        cart.CapNhatCCCD(mave1, cccd);
                    }
                    //Chép cccd vào vè lúc về của khách hàng
                    else if (item01.idVe.MaCB == cbden)
                    {
                        
                        var mave2 = item01.idVe.MaVe;
                        var cccd2 = Request["cccd_" + number2];
                        cart.CapNhatCCCD(mave2, cccd2);
                        number2++;
                    }
                    number++;
                }
            }

            Random rd = new Random();
            var total = 0;
            #region Ban cu
            var mave = "VE" + rd.Next(1, 1000);
            //check ma ve duoi database
            //Tao ve moi
            //Ve ve = new Ve();
            //ve.MaVe = mave;
            //ve.MaCB = dsorder.MaCBdi;
            //ve.TinhTrang = "Chưa thanh toán";
            //ve.GiaVe = 500000;
            //ve.CCCD = Request["cccd"];
            //ve.IDKH = uid.IDKH;
            //ve.MaHV = "HV01";
            //total += (int)ve.GiaVe;
            //database.Ves.Add(ve);
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
            //    ve.IDKH = uid.IDKH;
            //    ve.MaHV = "HV01";
            //    total += (int)ve.GiaVe;
            //    database.Ves.Add(ve);
            //    database.SaveChanges();
            //}
            #endregion
            var contact = new Order();
            contact.CreateDate = DateTime.Now;
            contact.ShipName = Request["name"];
            contact.ShipEmail = Request["email"];
            contact.NumberPhone = Request["number"];
            contact.CCCD = Request["cccd_0"];
            contact.Total = total;
            Session["contacKH"] = contact;
            return RedirectToAction("ThanhToan");
        }

        //Version 2.0
        [HttpGet]
        public ActionResult ChooseSeat()
        {
            var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid.IDKH).FirstOrDefault();
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
            var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid.IDKH).FirstOrDefault();
            var dsve = database.Ves.Where(s => s.MaCB == dsorder.MaCBdi).ToList();
            for (int i = 1; i <= dsve.Count(); i++)
            {
                if (Request["Ma" + i] != null)
                {
                    //14/03/2023 Suy nghĩ làm thêm luồn xử lý dữ liệu
                    //Add thông tin vé vào giỏ hàng
                    var ticket = Request["Ma" + i];
                    var detailtic = database.Ves.Where(s => s.MaCB == dsorder.MaCBdi && s.MaVe == ticket).FirstOrDefault();
                    GetCart().Add(detailtic, 1,null);
                    check++;
                    if (check == id)
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
            var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid.IDKH).FirstOrDefault();
            var dsve = database.Ves.Where(s => s.MaCB == dsorder.MaCBve).ToList();
            ViewData["MaCB"] = dsorder.MaCBve;
            return View(dsve);
        }
        [HttpPost]
        public ActionResult ChooseSeatVe(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            int check = 0;
            var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
            var dsorder = database.OrderStatus.Where(s => s.IDUser == uid.IDKH).FirstOrDefault();
            var dsve = database.Ves.Where(s => s.MaCB == dsorder.MaCBve).ToList();
            for (int i = 1; i <= dsve.Count(); i++)
            {
                if (Request["Ma" + i] != null)
                {
                    //Add thông tin vé vào giỏ hàng
                    var ticket = Request["Ma" + i];
                    var detailtic = database.Ves.Where(s => s.MaCB == dsorder.MaCBve && s.MaVe == ticket).FirstOrDefault();
                    GetCart().Add(detailtic, 1,null);
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
        //Tạo mới thông tin khách hàng
        public DanhSachKH GetCustomer()
        {
            DanhSachKH customer = Session["Customer"] as DanhSachKH;
            if (customer == null || Session["Customer"] == null)
            {
                customer = new DanhSachKH();
                Session["Customer"] = customer;
            }
            return customer;
        }

        public ActionResult ThanhToan()
        {
            if (Session["Cart"] == null)
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



        [HttpGet]
        public ActionResult ThanhToan01()
        {
            var uid = (BookingAirline.Models.KhachHang)Session["userKH"];
            var kh = database.OrderStatus.Where(s => s.IDUser == uid.IDKH).FirstOrDefault();

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
            themhd.IDKH = uid.IDKH;
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
                mavedi.CCCD = item.CCCD;
                mavedi.IDKH = uid.IDKH;
                database.Entry(mavedi).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges();

                cthd.MaVe = item.idVe.MaVe;
                cthd.SoLuong = item.soLuong;
                cthd.DonGia = item.idVe.GiaVe;
                cthd.MaCB = item.idVe.MaCB;
                cthd.TongTien = (item.soLuong) * (item.idVe.GiaVe);
                database.ChiTietHDs.Add(cthd);
                database.SaveChanges();

                //Tiến hành thiết lập tạo phiếu đặt chỗ cho khách hàng
                PhieuDatCho pdc = new PhieuDatCho();
                pdc.MaPhieu = "PDC" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                pdc.MaCB = cthd.MaCB;
                pdc.CCCD = themhd.CCCD;
                pdc.NgayDat = System.DateTime.Now;
                pdc.SoGhe = cthd.MaVe;
                database.PhieuDatChoes.Add(pdc);
                database.SaveChanges();
            }
            if (maveve != null)
            {
                maveve.TinhTrang = "Đã thanh toán";
                database.Entry(maveve).State = System.Data.Entity.EntityState.Modified;
                database.SaveChanges();
            };
            //Render form gửi email về cho khách hàng
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/Template/HtmlPage1.html"));
            content = content.Replace("{{CustomerName}}", ttkh.ShipName);
            content = content.Replace("{{Phone}}", ttkh.NumberPhone);
            content = content.Replace("{{Email}}", ttkh.ShipEmail);
            content = content.Replace("{{Total}}", tongtien);
            content = content.Replace("{{Thoigian}}", Convert.ToString(ttkh.CreateDate));
            content = content.Replace("{{Invoice}}", themhd.MaHD);
            string subject = "Đây là tin nhắn tự động từ hệ thống POS";
            WebMail.Send(ttkh.ShipEmail, subject, content, null, null, null, true, null, null, null, null, null, null);
            cart.XoaSauKhiDat();
            var count = database.OrderStatus.Where(s => s.IDUser == uid.IDKH).FirstOrDefault();
            database.OrderStatus.Remove(count);
            database.SaveChanges();
            return RedirectToAction("ThankYou");
            //return RedirectToAction("Payment");
        }
        public ActionResult ConfirmTT()
        {

            return RedirectToAction("ThankYou");
        }

        //Chức năng thanh toán VNPAY
        public ActionResult Payment()
        {
            Cart cart = Session["Cart"] as Cart;
            var tt = cart.TongTien().ToString();

            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", tt+"00"); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toán vé máy bay"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return RedirectToAction("ThanhToan01");
        }


        public ActionResult ThankYou()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}