using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILS.UserManagement.Models
    {
    public class UserRoleModel
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }
        public Nullable<decimal> IsDefault { get; set; }

    }
    [MetadataType(typeof(UserRoleModel))]
    public partial class tbl_UserRole
    {
        //public bool Selected { get; set; }

    }
}
