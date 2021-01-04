using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILS.UserManagement.Models
    {
    public class RolesModel
    {
        public string UpdatedBy { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> IsDefault { get; set; }
        public Nullable<decimal> IsDeleted { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string RoleId { get; set; }
        public bool Selected { get; set; }

    }
    [MetadataType(typeof(RolesModel))]
    public partial class Roles
    {
        public bool Selected { get; set; }

    }
}
