using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAirline.Models;

namespace BookingAirline.Models
{

    //Khai bao thuoc tinh dong san pham items trong lop Cart
    public class CartItem
    {
        //Lop CartItem chua cac dong san pham trong gio hang
        public ChuyenBay _product { get; set; }
        public int _quantity { get; set; }
    }
    //public class Cart
    //{
    //    List<CartItem> items = new List<CartItem>();
    //    public IEnumerable<CartItem> Items
    //    {
    //        get { return items; }
    //    }


    //    //Ham lay san pham bo vao gio hang
    //    public void Add_Product_Cart(ChuyenBay _pro, int _quan = 1)
    //    {
    //        var item = Items.FirstOrDefault(s => s._product.MaCB == _pro.MaCB);
    //        if (item == null) //Neu gio hang rong thi them dong hang moi vao gio
    //            items.Add(new CartItem
    //            {
    //                _product = _pro,
    //                _quantity = _quan
    //            });
    //        else
    //            item._quantity += _quan; //Tong so luong trong gio hang duoc cong don
    //    }

    //    //Ham tinh tong so luong trong gio hang
    //    public int Total_quantity()
    //    {
    //        return items.Sum(s => s._quantity);
    //    }

    //    //Ham tinh thanh tien cho moi dong san pham trong gio hang
    //    public int Total_money()
    //    {
    //        var total = items.Sum(s => s._quantity * s._product.GiaVe);
    //        return (int)total;
    //    }

    //    //Ham cap nhat lai so luong san pham o moi dong san pham khi khach hang muon dat mua them 
    //    public void Update_quantity(string id, int _new_quan)
    //    {
    //        var item = items.Find(s => s._product.MaVe == id);
    //        if (item != null)
    //            item._quantity = _new_quan;
    //    }

    //    //Ham xoa san pham trong gio hang
    //    public void Remove_CartItem(string id)
    //    {
    //        items.RemoveAll(s => s._product.MaVe == id);
    //    }

    //    //Ham xoa san pham khi khach hang thanh toan
    //    public void ClearCart()
    //    {
    //        items.Clear();
    //    }

    //}

    
}