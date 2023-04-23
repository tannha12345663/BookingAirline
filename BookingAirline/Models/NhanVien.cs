//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookingAirline.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NhanVien
    {
        BookingAirLightEntities db = new BookingAirLightEntities();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhanVien()
        {
            this.HoaDons = new HashSet<HoaDon>();
        }
    
        public string IDNV { get; set; }
        public string MaCV { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TenNV { get; set; }
        public string SDT_NV { get; set; }
        public string Email_NV { get; set; }
        public string GioiTinh { get; set; }
        public Nullable<System.DateTime> NgaySinh { get; set; }
    
        public virtual ChucVu ChucVu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HoaDon> HoaDons { get; set; }
        public bool Update(NhanVien nv)
        {
            try
            {
                var nhanvien = db.NhanViens.Find(nv.IDNV);
                nhanvien.IDNV = nv.IDNV;
                nhanvien.UserName = nv.UserName;
                nhanvien.Password = nv.Password;
                nhanvien.TenNV = nv.TenNV;
                nhanvien.SDT_NV = nv.SDT_NV;
                nhanvien.Email_NV = nv.Email_NV;
                nhanvien.NgaySinh = nv.NgaySinh;
                nhanvien.GioiTinh = nv.GioiTinh;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public NhanVien ViewDetail(string id)
        {
            return db.NhanViens.Find(id);
        }
    }
}
