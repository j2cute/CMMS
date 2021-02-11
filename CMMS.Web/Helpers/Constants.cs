﻿using ClassLibrary.Models;
using ILS.UserManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace CMMS.Web.Helper
{
    public static class SessionKeys
    {
        public static string SessionHelperInstance = "SessionHelperInstance";
        public static string UserId = "UserId";
        public static string UserUnitId = "UserUnitId";
        public static string ApplicableUnits = "ApplicableUnits";

        public static  string AllUnits = "AllUnits";
        public static string UnitTypes = "UnitTypes";
        public static string RolePermissions = "RolePermissions";

        public static void LoadTablesInSession(string tableName,string userId = null,string roleId = null)
        {
            using (var db = new WebAppDbContext())
            {
                switch (tableName)
                {
                    case "AllUnits":
                        if (HttpContext.Current.Session[AllUnits] == null)
                        {
                            HttpContext.Current.Session[AllUnits] = db.tbl_Unit.ToList();
                        }
                       
                    break;

                    case "UnitTypes":
                        if (HttpContext.Current.Session[UnitTypes] == null)
                        {
                            HttpContext.Current.Session[UnitTypes] = db.tbl_UnitType.ToList(); 
                        }

                        break;

                    case "RolePermissions":
                        if (HttpContext.Current.Session[RolePermissions] == null)
                        {
                            using (var _context = new Entities())
                            {
                                HttpContext.Current.Session[RolePermissions] = (from permission in _context.tbl_Permission
                                                                               join rolePermission in _context.tbl_RolePermission on permission.PermissionId equals rolePermission.PermissionId
                                                                               where rolePermission.RoleId == roleId
                                                                               select permission).ToList();
                            }
                        }
                        break;
                }
            }
        }
    }

    public class SessionHelper
    {
        public SessionHelper()
        {

        }

        public SessionHelper(string siteId, string userId,string roleId, List<PermissionViewModel> permissions)
        {
            UserId = userId;
            SelectedSiteId = siteId;
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
        public string SelectedSiteId { get; set; }
        public IEnumerable<ClassLibrary.Models.tbl_Unit> ApplicableUnits { get; set; }


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


 
    public enum PageMode
    {
        Add = 1,
        Edit = 2
    }

    public static class RegexHelper
    {
        public static string NumberOnly = "[0-9]";
        public static string AlphabetsOnly = "";
        public static string Alphanumeric = "";
        public static string AlphanumericSpecialCharacter = "";
    }
}