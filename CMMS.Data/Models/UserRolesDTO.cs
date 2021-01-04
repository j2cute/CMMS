using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class ExpandedUserDTO
    {
        [Key] 
        [Display(Name = "User Name")]
        [Required]
        [MaxLength(20, ErrorMessage = "Maximum Length Should be Equal to 20 Characters")]
        [RegularExpression("^([a-zA-Z0-9]+)$", ErrorMessage = "Only Alphabets and Numbers allowed.")]

        public string UserName { get; set; }
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Invalid Password!", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        //[Display(Name = "Lockout End Date Utc")]
        //public DateTime? LockoutEndDateUtc { get; set; }
        //public int AccessFailedCount { get; set; }
        //public string PhoneNumber { get; set; }
        public List<ExpandedUserDTO> expandedUserDTO_List { get; set; }
        // public IEnumerable<UserRolesDTO> Roles { get; set; }
    }

    public class UserRolesDTO
    {
        [Key]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class UserRoleDTO
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class RoleDTO
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class UserAndRolesDTO
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<UserRoleDTO> colUserRoleDTO { get; set; }
    }
}
