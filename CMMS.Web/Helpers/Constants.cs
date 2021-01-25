using ClassLibrary.Models;
using ILS.UserManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMMS.Web.Helper
{
    public static class SessionKeys
    {
        public static string SessionHelperInstance = "SessionHelperInstance";
        public static string UserId = "UserId";
        public static string UnitId = "UnitId";
    }

    public class SessionHelper
    {
        public SessionHelper()
        {

        }

        public SessionHelper(string siteId, string userId,string roleId, List<PermissionViewModel> permissions)
        {
            UserId = userId;
            SiteId = siteId;
            CurrentRoleId = roleId;
            CurrentRolePermissions = permissions;

            using (Entities _context = new Entities())
            {
                var roles = _context.tbl_User.FirstOrDefault(x => x.UserId == userId && x.IsActive != 0)?.tbl_UserRole.Select(x => x.tbl_Role).Where(x => x.IsDeleted != 1);

                if (roles.Any())
                {
                    UserRoles = roles?.Select(x => new RolesVM() { RoleId = x.RoleId,RoleName = x.Name, RoleDescription = x.Description }).ToList();
                }
            }
        }

        public string CurrentRoleId { get; set; }
        public List<PermissionViewModel> CurrentRolePermissions { get; set; }

        public List<RolesVM> UserRoles { get; set; }
        private string UserId { get; set; }
        public string SiteId { get; set; }

        public SessionHelper UpdateFields(string roleId, List<PermissionViewModel> permissionViewModels)
        {
            if (!string.IsNullOrWhiteSpace(roleId))
            {
                CurrentRoleId = roleId;
            }

            if (permissionViewModels != null)
            {
                CurrentRolePermissions = permissionViewModels;
            }

            return this;
        }
    }

    public class RolesVM
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
    }

}