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
    
    public partial class M_MOP_PLAN
    {
        public int SiteId { get; set; }
        public string MOP_No { get; set; }
        public string PMS_No { get; set; }
        public string ESWBS { get; set; }
        public Nullable<System.DateTime> DoneDate { get; set; }
        public Nullable<System.DateTime> NextDueDate { get; set; }
        public string DoneBy { get; set; }
    }
}
