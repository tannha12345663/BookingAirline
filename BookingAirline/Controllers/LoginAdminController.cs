using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAirline.Models;

namespace BookingAirline.Controllers
{
    public class LoginAdminController : Controller
    {
        BookingAirLightEntities database = new BookingAirLightEntities();
        // GET: LoginAdmin
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(NhanVien _user)
        {
            var check = database.NhanViens.Where(s => s.IDNV == _user.IDNV && s.Password == _user.Password).FirstOrDefault();

            if (check != null)
            {
                ViewBag.ErrorInfo = "Info Error";
                return View("Index");
            }
            else
            {
                database.Configuration.ValidateOnSaveEnabled = false;
                Session["ID"] = _user.IDNV;
                Session["password"] = _user.Password;
                return RedirectToAction("Index", "Product");
            }
            
        }

        //Register
        public ActionResult Register() { return View(); }

        [HttpPost]
        public ActionResult Register(NhanVien _user)
        {
            if (ModelState.IsValid)
            {
                var check_ID = database.NhanViens.Where(s => s.IDNV == _user.IDNV).FirstOrDefault();
                if(check_ID == null) //chua co ID
                {
                    database.Configuration.ValidateOnSaveEnabled = false;
                    database.NhanViens.Add(_user);
                    database.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorRegister = "This ID is exixst";
                    return View();
                }
            }
            return View();
        }

        public ActionResult LogOut() 
        { 
            Session.Abandon();
            return RedirectToAction("Index", "LoginAdmin");
        }
        
    }
}