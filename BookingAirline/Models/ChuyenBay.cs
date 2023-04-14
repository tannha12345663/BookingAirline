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
    
    public partial class ChuyenBay
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChuyenBay()
        {
            this.ChiTietChuyenBays = new HashSet<ChiTietChuyenBay>();
            this.PhieuDatChoes = new HashSet<PhieuDatCho>();
            this.Ves = new HashSet<Ve>();
        }
    
        public string MaCB { get; set; }
        public string MaMB { get; set; }
        public string MaTBay { get; set; }
        public Nullable<System.DateTime> NgayGio { get; set; }
        public string ThoiGianBay { get; set; }
        public Nullable<int> SoLuongGheHang1 { get; set; }
        public Nullable<int> SoLuongGheHang2 { get; set; }
        public Nullable<int> SoLuongGheHang3 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietChuyenBay> ChiTietChuyenBays { get; set; }
        public virtual MayBay MayBay { get; set; }
        public virtual TuyenBay TuyenBay { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhieuDatCho> PhieuDatChoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ve> Ves { get; set; }
    }
}
