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
    
    public partial class DoanhThuThang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DoanhThuThang()
        {
            this.DoanhThuNams = new HashSet<DoanhThuNam>();
        }
    
        public string MaDTThang { get; set; }
        public string MaHD { get; set; }
        public Nullable<int> SoLuongVeBan { get; set; }
        public Nullable<double> DoanhThu { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DoanhThuNam> DoanhThuNams { get; set; }
        public virtual HoaDon HoaDon { get; set; }
    }
}
