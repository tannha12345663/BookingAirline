using BookingAirline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BookingAirline.Controllers
{
    public class LoginUserController : Controller
    {
        BookingAirLightEntities database = new BookingAirLightEntities();
        // GET: LoginUser
        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(string user, string password)
        {
            var data = database.KhachHang.Where(s => s.UserName == user && s.Password == password).FirstOrDefault();
            var taikhoan = database.KhachHang.SingleOrDefault(s => s.UserName == user && s.Password == password);
            if (data == null)
            {
                TempData["error"] = "Tài khoản đăng nhập không đúng";
                return View("Login");
            }
            else if (data != null)
            {
                //add session
                database.Configuration.ValidateOnSaveEnabled = false;
                Session["userKH"] = data;
                return RedirectToAction("TrangChu", "KhachHangHA");
            }
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(string user, string email, string password, string confirm)
        {
            var checkuser = database.KhachHang.FirstOrDefault(s => s.UserName == user);
            var checkemail = database.KhachHang.FirstOrDefault(s => s.Email == email);
            if (checkuser != null)
            {
                TempData["error01"] = "User này đã có vui lòng đổi user khác !";
                return View("SignUp");
            }
            else if (checkemail != null)
            {
                TempData["error02"] = "Email này đã tồn tại! ";
                return View("SignUp");
            }
            else if (password != confirm)
            {
                TempData["error03"] = "Vui lòng nhập lại xác nhận password ";
                return View("SignUp");
            }
            else if (password == confirm)
            {
                Random rd = new Random();
                var makh = "KH" + rd.Next(1, 1000);
                KhachHang kh = new KhachHang();
                kh.IDKH = makh;
                kh.MaLKH = "Normal";
                kh.UserName = user;
                kh.Password = password;
                kh.Email = email;
                database.KhachHang.Add(kh);
                database.SaveChanges();
                return RedirectToAction("TrangChu", "KhachHang");
            }
            return View();
        }
        public ActionResult ForgotPassword()
        {

            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var data = database.KhachHang.FirstOrDefault(s => s.Email == email);
            if (data == null)
            {
                TempData["error04"] = "Không tồn tại email này !";
                return View();

            }
            else
            {
                TempData["Succeed"] = "Đã gửi yêu cầu đặt lại mật khẩu thông qua email của bạn !";
                return View();
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult ResetPass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPass(string email, string username)
        {

            var mail = database.KhachHang.Where(s => s.Email == email).FirstOrDefault();
            var user = database.KhachHang.Where(s => s.UserName == username).FirstOrDefault();
            
            if(mail != null && user != null)
            {
                var kh = database.KhachHang.Where(s => s.Email == email).FirstOrDefault();
                
                    kh.Password = "12345678";
                    database.Entry(kh).State = System.Data.Entity.EntityState.Modified;
                    database.SaveChanges();
                
                    return RedirectToAction("Login");
               
                
            }
            else
            {
                TempData["error"] = "Tài khoản không tồn tại";
                return View();
            }


            return View();

        }
    }
}






