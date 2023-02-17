using BookingAirline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            else if (taikhoan != null)
            {
                //add session
                database.Configuration.ValidateOnSaveEnabled = false;
                Session["userKH"] = data;
                return RedirectToAction("TrangChu", "KhachHang");
            }
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
    }
}