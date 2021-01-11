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
    }

    public class SessionHelper
    {
        public SessionHelper()
        {

        }

        public SessionHelper(string userId,string roleId, List<PermissionViewModel> permissions)
        {
            UserId = userId;
            CurrentRoleId = roleId;
            RolePermissions = permissions;
        }
        public string CurrentRoleId { get; set; }
        public List<PermissionViewModel> RolePermissions { get; set; }
        private string UserId { get; set; }

        public SessionHelper UpdateFields(string roleId, List<PermissionViewModel> permissionViewModels)
        {
            if (!string.IsNullOrWhiteSpace(roleId))
            {
                CurrentRoleId = roleId;
            }

            if (permissionViewModels != null)
            {
                RolePermissions = permissionViewModels;
            }

            return this;
        }
    }

}