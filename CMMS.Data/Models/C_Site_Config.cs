//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClassLibrary.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class C_Site_Config
    {
        public int SiteId { get; set; }
        public string ESWBS { get; set; }
        public string PESWBS { get; set; }
        public string Nomanclature { get; set; }
        public string Title { get; set; }
        public Nullable<int> Qty { get; set; }
        public string PMS_No { get; set; }
        public string SMR_Code { get; set; }
        public Nullable<int> PartId { get; set; }
        public Nullable<int> CageId { get; set; }
    
        public virtual tbl_Unit tbl_Unit { get; set; }
    }
}
