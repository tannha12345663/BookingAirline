using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAirline.Models;

namespace BookingAirline.Models
{
    public class CartItem
    {
        public Ve idVe { get; set; }

        public int soLuong { get; set; }
        public string CCCD { get; set; }
    }
    public class Cart
    {
        List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }

        //Thêm vé vào giỏ hàng
        public void Add(Ve mave, int sl, string cccd)
        {
            var item = Items.FirstOrDefault(s => s.idVe.MaVe == mave.MaVe);
            if (item == null)
            {
                items.Add(new CartItem
                {
                    idVe = mave,
                    soLuong = sl,
                    CCCD=cccd

                });
            }
            else
            {
                item.soLuong += sl;
            }
        }

        //Tính tổng số lượng trong giỏ
        public int TongSLTrongGio()
        {
            return items.Sum(s => s.soLuong);
        }

        //Tính thành tiền
        public double TongTien()
        {
            var tong = items.Sum(s => s.soLuong * s.idVe.GiaVe);
            return (double)tong;
        }

        //Cập nhật số lượng
        public void CapNhatSL(string id, int slmoi)
        {
            var item = items.Find(s => s.idVe.MaVe == id);
            if (item != null)
            {
                item.soLuong = slmoi;
            }
        }
        //Cập nhật CCCD
        public void CapNhatCCCD(string id,string cccd)
        {
            var item = items.Find(s => s.idVe.MaVe == id);
            if (item != null)
            {
                item.CCCD = cccd;
            }
        }
        //Xóa sản phẩm trong giỏ hàng
        public void XoaSP(string id)
        {
            items.RemoveAll(s => s.idVe.MaVe == id);
        }

        //Xóa sản sau khi đặt hàng
        public void XoaSauKhiDat()
        {
            items.Clear();
        }
    }
}
