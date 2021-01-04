using ILS.UserManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILS.UserManagement.ViewModels
{
    public class RolesViewModel
    {
        public tbl_Role tbl_Role { get; set; }
     
        public List<tbl_Role> tbl_Role_list { get; set; }
        public string PermissionsList { get; set; }

    }
    public class UserRoleViewModel
    {
        public tbl_User tbl_User { get; set; }
        public List<tbl_User> tbl_User_list { get; set; }
        public tbl_Role tbl_Role { get; set; }
        public UserRoleModel UserRoleModel { get; set; }
        public List<tbl_Role> tbl_Role_list { get; set; }
        public List<RolesModel> RolesModel_list { get; set; }
        public List<UserRoleModel> userRoleModel_list { get; set; }
        public IEnumerable<tbl_Role> _tbl_Role { get; set; }
        public string PermissionsList { get; set; }
        public IEnumerable<tbl_Unit> _tbl_Unit { get; set; }

    }
}