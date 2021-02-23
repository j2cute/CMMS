using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using ClassLibrary.Models;
using static ClassLibrary.Common.Enums;
using ILS.UserManagement.Models;
using CMMS.Web.Helper;
using WebApplication.Helpers;
using System.Text.RegularExpressions;
using NLog;
using System.Configuration;
using System.Data.Linq;

namespace WebApplication.Controllers
{
    public class AdminController : BaseController
    {
        #region Declaration

        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        private static Logger _logger;

        public AdminController()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Dashboard

        [CheckUserSession]
        public ActionResult UnitSelection()
        {
            string actionName = "UnitSelection";
            DashboardViewModel vm = new DashboardViewModel();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");

                SessionKeys.LoadTablesInSession(SessionKeys.AllUnits);
                SessionKeys.LoadTablesInSession(SessionKeys.UnitTypes);

                if (Session[SessionKeys.UserId] != null)
                {

                    var userId = Session[SessionKeys.UserId]?.ToString();

                    using (Entities _context = new Entities())
                    {
                        var data = _context.tbl_User.Where(x => x.UserId == userId && x.IsActive != 0 && x.IsDeleted != 1).FirstOrDefault();

                        if (data != null)
                        {
                            Session[SessionKeys.UserUnitId] = data.UnitId;

                            var allUnits = ((List<ClassLibrary.Models.tbl_Unit>)Session[SessionKeys.AllUnits]);
                            var unit = allUnits.Where(x => x.Id == data.UnitId).FirstOrDefault();

                            if (unit != null && unit.UnitTypeId != null)
                            {
                                var unitLevel = ((List<ClassLibrary.Models.tbl_UnitType>)Session[SessionKeys.UnitTypes]).Where(x => x.UnitTypeId == unit.UnitTypeId).Select(x => x.UnitTypeLevel).FirstOrDefault();

                                IEnumerable<ClassLibrary.Models.tbl_Unit> UnitList;

                                if (unitLevel == 0)
                                {
                                    UnitList = allUnits;
                                }
                                else if (unitLevel == 1)
                                {
                                    var lookup = allUnits.ToLookup(x => x.ParentUnitId);
                                    var res = lookup[data.UnitId].SelectRecursive(x => lookup[x.Id]).ToList();
                                    res.Add(unit);
                                    UnitList = res;
                                }
                                else
                                {
                                    UnitList = allUnits.Where(x => x.Id == data.UnitId).ToList();
                                }

                                Session[SessionKeys.ApplicableUnits] = UnitList;
                                vm._tbl_Unit = UnitList;
                            }
                            else
                            {
                                _logger.Log(LogLevel.Error, actionName + " :: Ended.. Unit Id : " + data.UnitId + " not found.");
                                RedirectToAction("Login", "Account");
                            }
                        }
                        else
                        {
                            _logger.Log(LogLevel.Error, actionName + " ::  Ended.. User not found.");
                            RedirectToAction("Login", "Account");
                        }
                    }

                }
                else
                {
                    _logger.Log(LogLevel.Error, actionName + " :: Ended.. session user is null or empty.");
                    RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }
            _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            return PartialView("_UnitSelection", vm);
        }

        [HttpPost]
        [CheckUserSession]
        [ValidateAntiForgeryToken]
        public ActionResult Dashboard(string siteId, string inRoleId = null)
        {
            string actionName = "Dashboard";
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");

                if (!string.IsNullOrWhiteSpace(siteId))
                {
                    if (Regex.IsMatch(siteId, RegexHelper.NumberOnly))
                    {
                        var applicableUnits = ((List<ClassLibrary.Models.tbl_Unit>)Session[SessionKeys.ApplicableUnits]);
                        int intSiteId = Convert.ToInt32(siteId);

                        if (applicableUnits != null && applicableUnits.Where(x => x.Id == intSiteId).Any())
                        {

                            DashboardViewModel dashboard = new DashboardViewModel();
                            List<PermissionViewModel> ListPermissionViewModel = new List<PermissionViewModel>();

                            dashboard._tbl_Unit = ((List<ClassLibrary.Models.tbl_Unit>)Session[SessionKeys.AllUnits]);

                            string defaultRoleId = !String.IsNullOrWhiteSpace(inRoleId) ? inRoleId : string.Empty;

                            dashboard.datetime = DateTime.Now.ToString();
                            string loginUserId = Session[SessionKeys.UserId]?.ToString();

                            using (Entities _context = new Entities())
                            {
                                if (string.IsNullOrWhiteSpace(defaultRoleId))
                                {
                                    defaultRoleId = _context.tbl_UserRole.Where(x => x.UserId == loginUserId && x.IsDefault == 1 && x.IsActive != 0 && x.IsDeleted != 1)?.FirstOrDefault().RoleId;
                                }
                                else
                                {
                                    defaultRoleId = _context.tbl_UserRole.Where(x => x.UserId == loginUserId && x.RoleId == inRoleId && x.IsActive != 0 && x.IsDeleted != 1)?.FirstOrDefault().RoleId;
                                }

                                if (!String.IsNullOrWhiteSpace(defaultRoleId))
                                {
                                    SessionKeys.LoadTablesInSession(SessionKeys.RolePermissions, "", defaultRoleId);
                                    var permissions = ((List<tbl_Permission>)Session[SessionKeys.RolePermissions]);

                                    foreach (var item in permissions)
                                    {
                                        PermissionViewModel permissionViewModel = new PermissionViewModel()
                                        {
                                            PermissionId = item.PermissionId,
                                            DisplayName = item.DisplayName,
                                            Level = item.PermissionLevel?.ToString(),
                                            ParentId = item.ParentId,
                                            URL = item.URL,
                                            IconPath = item.IconPath
                                        };

                                        ListPermissionViewModel.Add(permissionViewModel);
                                    }

                                    Session[SessionKeys.SessionHelperInstance] = new SessionHelper(loginUserId, defaultRoleId, ListPermissionViewModel);
                                    dashboard.DashboardId = GetDashboardId(loginUserId);


                                    return View(dashboard);
                                }
                                else
                                {
                                    _logger.Log(LogLevel.Error, actionName + " :: Role id not found.");
                                }
                            }
                        }
                        else
                        {
                            _logger.Log(LogLevel.Error, actionName + " :: Please select valid site id.");
                        }
                    }
                    else
                    {
                        _logger.Log(LogLevel.Error, actionName + " :: Invalid input in site id.");
                    }
                }
                else
                {
                    _logger.Log(LogLevel.Error, actionName + " :: Site id empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }
            return RedirectToAction("UnitSelection");
        }

        //[HttpGet]
        //[CheckUserSession]
        //public ActionResult Dashboard()
        //{
        //    string actionName = "Get Dashboard";
        //    try
        //    {
        //        _logger.Log(LogLevel.Trace, actionName + " :: started.");

        //        var sessionVariables = ((SessionHelper)Session[SessionKeys.SessionHelperInstance]);
        //        if (sessionVariables != null)
        //        {

        //            DashboardViewModel dashboard = new DashboardViewModel();
        //            List<PermissionViewModel> ListPermissionViewModel = new List<PermissionViewModel>();

        //            dashboard._tbl_Unit = ((List<ClassLibrary.Models.tbl_Unit>)Session[SessionKeys.AllUnits]);

        //            dashboard.datetime = DateTime.Now.ToString();
        //            string loginUserId = Session[SessionKeys.UserId]?.ToString();

        //            string defaultRoleId = string.Empty;

        //            using (Entities _context = new Entities())
        //            {
        //                if (string.IsNullOrWhiteSpace(sessionVariables.CurrentRoleId))
        //                {
        //                    defaultRoleId = _context.tbl_UserRole.Where(x => x.UserId == loginUserId && x.IsDefault == 1 && x.IsActive != 0 && x.IsDeleted != 1)?.FirstOrDefault().RoleId;
        //                }
        //                else
        //                {
        //                    defaultRoleId = sessionVariables.CurrentRoleId;
        //                }

        //                if (!String.IsNullOrWhiteSpace(defaultRoleId))
        //                {
        //                    var permissions = ((List<tbl_Permission>)Session[SessionKeys.RolePermissions]);

        //                    foreach (var item in permissions)
        //                    {
        //                        PermissionViewModel permissionViewModel = new PermissionViewModel()
        //                        {
        //                            PermissionId = item.PermissionId,
        //                            DisplayName = item.DisplayName,
        //                            Level = item.PermissionLevel?.ToString(),
        //                            ParentId = item.ParentId,
        //                            URL = item.URL,
        //                            IconPath = item.IconPath
        //                        };

        //                        ListPermissionViewModel.Add(permissionViewModel);
        //                    }


        //                    Session[SessionKeys.SessionHelperInstance] = new SessionHelper(loginUserId, defaultRoleId, ListPermissionViewModel);

        //                    dashboard.DashboardId = GetDashboardId(loginUserId);
        //                    return View(dashboard);
        //                }
        //                else
        //                {
        //                    _logger.Log(LogLevel.Error, actionName + " :: Role id not found.");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            _logger.Log(LogLevel.Error, actionName + " :: Session Expired.");
        //            RedirectToAction("LogOff", "Account");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
        //    }

        //    _logger.Log(LogLevel.Trace, actionName + " :: ended.");

        //    return RedirectToAction("UnitSelection");
        //}


        [HttpGet]
        [CheckUserSession]
        public ActionResult Dashboard()
        {
            string actionName = "Dashboard";
            string inRoleId = string.Empty;
            try
            {
                string siteId = Session[SessionKeys.UserUnitId]?.ToString();

                _logger.Log(LogLevel.Trace, actionName + " :: started.");

                if (!string.IsNullOrWhiteSpace(siteId))
                {
                    if (Regex.IsMatch(siteId, RegexHelper.NumberOnly))
                    {
                        var applicableUnits = ((List<ClassLibrary.Models.tbl_Unit>)Session[SessionKeys.ApplicableUnits]);
                        int intSiteId = Convert.ToInt32(siteId);

                        if (applicableUnits != null && applicableUnits.Where(x => x.Id == intSiteId).Any())
                        {

                            DashboardViewModel dashboard = new DashboardViewModel();
                            List<PermissionViewModel> ListPermissionViewModel = new List<PermissionViewModel>();

                            dashboard._tbl_Unit = ((List<ClassLibrary.Models.tbl_Unit>)Session[SessionKeys.AllUnits]);

                            string defaultRoleId = !String.IsNullOrWhiteSpace(inRoleId) ? inRoleId : string.Empty;

                            dashboard.datetime = DateTime.Now.ToString();
                            string loginUserId = Session[SessionKeys.UserId]?.ToString();

                            using (Entities _context = new Entities())
                            {
                                if (string.IsNullOrWhiteSpace(defaultRoleId))
                                {
                                    defaultRoleId = _context.tbl_UserRole.Where(x => x.UserId == loginUserId && x.IsDefault == 1 && x.IsActive != 0 && x.IsDeleted != 1)?.FirstOrDefault().RoleId;
                                }
                                else
                                {
                                    defaultRoleId = _context.tbl_UserRole.Where(x => x.UserId == loginUserId && x.RoleId == inRoleId && x.IsActive != 0 && x.IsDeleted != 1)?.FirstOrDefault().RoleId;
                                }

                                if (!String.IsNullOrWhiteSpace(defaultRoleId))
                                {
                                    SessionKeys.LoadTablesInSession(SessionKeys.RolePermissions, "", defaultRoleId);
                                    var permissions = ((List<tbl_Permission>)Session[SessionKeys.RolePermissions]);

                                    foreach (var item in permissions)
                                    {
                                        PermissionViewModel permissionViewModel = new PermissionViewModel()
                                        {
                                            PermissionId = item.PermissionId,
                                            DisplayName = item.DisplayName,
                                            Level = item.PermissionLevel?.ToString(),
                                            ParentId = item.ParentId,
                                            URL = item.URL,
                                            IconPath = item.IconPath
                                        };

                                        ListPermissionViewModel.Add(permissionViewModel);
                                    }

                                    Session[SessionKeys.SessionHelperInstance] = new SessionHelper(loginUserId, defaultRoleId, ListPermissionViewModel);
                                    dashboard.DashboardId = GetDashboardId(loginUserId);


                                    return View(dashboard);
                                }
                                else
                                {
                                    _logger.Log(LogLevel.Error, actionName + " :: Role id not found.");
                                }
                            }
                        }
                        else
                        {
                            _logger.Log(LogLevel.Error, actionName + " :: Please select valid site id.");
                        }
                    }
                    else
                    {
                        _logger.Log(LogLevel.Error, actionName + " :: Invalid input in site id.");
                    }
                }
                else
                {
                    _logger.Log(LogLevel.Error, actionName + " :: Site id empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }
            return RedirectToAction("UnitSelection");
        }

        [CheckUserSession]
        private string GetDashboardId(string inUserId)
        {
            string actionName = "GetDashboardId";
            UserDashboardMapping dashboard = null;

            var id = ConfigurationManager.AppSettings["DefaultDashboardId"].ToString();

            _logger.Log(LogLevel.Trace, actionName + " :: started");

            try
            {
                using (var _context = new WebAppDbContext())
                {
                    var sessionDetails = (SessionHelper)Session[SessionKeys.SessionHelperInstance];
                    var dashboards = _context.UserDashboardMappings.Where(x => x.UserId == inUserId && x.RoleId == sessionDetails.CurrentRoleId);
                    if (dashboards.Any())
                    {
                        dashboard = dashboards.FirstOrDefault(x => x.IsPrefered == "1");

                        if (dashboard == null)
                        {
                            dashboard = dashboards.FirstOrDefault(x => x.IsDefault == "1");
                        }

                        id = dashboard != null ? dashboard.DashboardId : id;
                    }
                    else
                    {
                        _logger.Log(LogLevel.Trace, actionName + " :: No dashboard linked with user.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }
            _logger.Log(LogLevel.Trace, actionName + " :: ended");
            return id;
        }

        #endregion

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (var parent in source)
            {
                yield return parent;

                var children = selector(parent);
                foreach (var child in SelectRecursive(children, selector))
                    yield return child;
            }
        }
    }

}