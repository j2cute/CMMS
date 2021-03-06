//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ILS.UserManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_Permission
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_Permission()
        {
            this.tbl_RolePermission = new HashSet<tbl_RolePermission>();
        }
    
        public string PermissionId { get; set; }
        public string DisplayName { get; set; }
        public string IconPath { get; set; }
        public string Heading { get; set; }
        public Nullable<decimal> IsActive { get; set; }
        public Nullable<decimal> IsDefault { get; set; }
        public Nullable<decimal> IsDelete { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public Nullable<decimal> PermissionLevel { get; set; }
        public Nullable<decimal> SoreOrder { get; set; }
        public string ToolTip { get; set; }
        public string Type { get; set; }
        public string URL { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_RolePermission> tbl_RolePermission { get; set; }
    }
}
