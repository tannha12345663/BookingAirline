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
        public ActionResult Login(string user , string password)
        {
            var data = database.KhachHangs.Where(s => s.UserName == user && s.Password == password).FirstOrDefault();
            var taikhoan = database.KhachHangs.SingleOrDefault(s => s.UserName ==  user && s.Password == password);
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
            var checkuser = database.KhachHangs.FirstOrDefault(s => s.UserName == user);
            var checkemail = database.KhachHangs.FirstOrDefault(s => s.Email == email);
            if (checkuser != null)
            {
                TempData["error01"] = "User này đã có vui lòng đổi user khác !";
                return View("SignUp");
            }
            else if (checkemail !=null)
            {
                TempData["error02"] = "Email này đã tồn tại! ";
                return View("SignUp");
            }
            else if (password !=  confirm)
            {
                TempData["error03"] = "Vui lòng nhập lại xác nhận password ";
                return View("SignUp");
            }
            else if (password == confirm)
            {
                try
                {
                    Random rd = new Random();
                    var makh = "KH" + rd.Next(1, 1000);
                    KhachHang kh = new KhachHang();
                    kh.IDKH = makh;
                    kh.MaLKH = "Normal";
                    kh.UserName = user;
                    kh.Password = password;
                    kh.Email = email;
                    database.KhachHangs.Add(kh);
                    database.SaveChanges();
                }
                catch
                {
                    return View();
                }
                
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
            var data = database.KhachHangs.FirstOrDefault(s => s.Email == email);
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
    }
}