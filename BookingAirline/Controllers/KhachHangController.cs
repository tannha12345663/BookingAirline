using BookingAirline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            if (Session["From"] !=null )
            {
                Session.Remove("From");
                Session.Remove("To");
            }
            else
            {
                Session["From"] = Request["from"];
                Session["To"] = Request["to"];
                Session["Trip"] = Request["trip"];
            }
            
            //Lọc tìm kiếm chuyến bay

            //Hiển thị danh sách các chuyến bay
            return View(database.ChuyenBays.ToList());
        }

        public ActionResult DSachCBVe(string id)
        {
            //Kiểm tra nếu là khứ hồi
            var check = Session["trip"].ToString();
            if  (check == "one-way")
            {
                
                return RedirectToAction("DienThongTinKH");
            }
            else
            {
                return View(database.ChuyenBays.ToList());
            }

        }
        public ActionResult DienThongTinKH()
        {
            
            return View();
        }
    }
}