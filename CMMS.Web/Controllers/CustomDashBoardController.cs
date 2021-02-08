using ClassLibrary.Models;
using ClassLibrary.ViewModels;
using CMMS.Web.Helper;
using ILS.UserManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class CustomDashboardController : Controller
    {
        private const string AllRoles = "AllRoles";
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ClientMode()
        {
            return View("ClientMode");
        }

        public ActionResult DesignerMode()
        {
            return View("DesignerMode");
        }

        public ActionResult UserDashboards()
        {
            return View("UserDashboards");
        }

        public ActionResult LoadUsers(int length, int start)
        {
            int recordCount = 0, filterrecord = 0;
            dynamic userDashboardMappingData = null;

            try
            {
                using (var db = new Entities())
                {
                    string searchvalue = Request.Form["search[value]"];
                    var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                    var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                    Expression<Func<tbl_User, object>> sortExpression;
                    switch (sortColumn)
                    {
                        case "UserId":
                            sortExpression = (x => x.UserId);
                            break;
                        case "UserName":
                            sortExpression = (x => x.Name);
                            break;
                        default:
                            sortExpression = (x => x.UserId);
                            break;
                    }

                    if (sortColumnDir == "asc")
                    {
                        userDashboardMappingData = db.tbl_User.Where(x => x.UserId.Contains(searchvalue)
                     || x.Name.Contains(searchvalue))
                     .OrderBy(sortExpression).Skip(start).Take(length).Where(x => x.IsActive != 0).ToList().Select(y => new { UserId = y.UserId, UserName = y.Name });
                    }
                    else
                    {
                        userDashboardMappingData = db.tbl_User.Where(x => x.UserId.Contains(searchvalue)
                    || x.Name.Contains(searchvalue))
                    .OrderByDescending(sortExpression).Skip(start).Take(length).Where(x => x.IsActive != 0).ToList().Select(y => new { UserId = y.UserId, UserName = y.Name });
                    }

                    var recordcount = db.tbl_User.Count();

                    if (searchvalue != "")
                    {
                        filterrecord = userDashboardMappingData.Count();
                    }
                    else
                    {
                        filterrecord = recordcount;
                    }
                }
            }
            catch
            {

            }

            var response = new { recordsTotal = recordCount, recordsFiltered = filterrecord, data = userDashboardMappingData };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LinkedDashboards(string userId)
        {
            TempData["UserId"] = userId;
            ViewBag.UserId = userId;
            return View("UserDashboards");
        }

        [HttpPost]
        public ActionResult LoadUserDashboard(int length, int start)
        {
            string userId = TempData["UserId"]?.ToString();
            int recordCount = 0, filterrecord = 0;
            List<UserDashboardMapping> userDashboardMappingData = new List<UserDashboardMapping>();

            try
            {
                using (WebAppDbContext db = new WebAppDbContext())
                {
                    string searchvalue = Request.Form["search[value]"];
                    var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                    var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                    Expression<Func<UserDashboardMapping, object>> sortExpression;
                    switch (sortColumn)
                    {
                        case "UserId":
                            sortExpression = (x => x.UserId);
                            break;
                        default:
                            sortExpression = (x => x.UserId);
                            break;
                    }

                    if (sortColumnDir == "asc")
                    {
                        userDashboardMappingData = db.UserDashboardMappings.Where(x => x.UserId.Contains(userId))
                       .OrderBy(sortExpression).Skip(start).Take(length).ToList();
                    }
                    else
                    {
                        userDashboardMappingData = db.UserDashboardMappings.Where(x => x.UserId.Contains(userId))
                        .OrderByDescending(sortExpression).Skip(start).Take(length).ToList();
                    }

                    var recordcount = db.UserDashboardMappings.Where(x => x.UserId == userId).Count();

                    if (searchvalue != "")
                    {
                        filterrecord = userDashboardMappingData.Count();
                    }
                    else
                    {
                        filterrecord = recordcount;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            var response = new { recordsTotal = recordCount, recordsFiltered = filterrecord, data = userDashboardMappingData };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUserDashboard(string userId)
        {
            ViewBag.PageMode = PageMode.Add;
            return RedirectToAction("PopulateUserData", new { userId = userId, roleId = "", dashboardId = "", pageMode = PageMode.Add });
        }

        [HttpPost]
        public ActionResult EditUserDashboard(string userId,string dashboardId,string roleId)
        {
            ViewBag.PageMode = PageMode.Edit;
            return RedirectToAction("PopulateUserData", new { userId = userId,roleId = roleId, dashboardId = dashboardId, pageMode = PageMode.Edit });
        }

        [HttpPost]
        public ActionResult DeleteDashboard(string DashboardID)
        {
            var dashboardPath = $"~/{ConfigurationManager.AppSettings["DashboardPath"]?.ToString()}";
            CustomDashboardStorage newDashboardStorage = new CustomDashboardStorage(dashboardPath);

            newDashboardStorage.DeleteDashboard(HttpContext.Request.MapPath(Path.Combine(dashboardPath, $"{DashboardID}.xml")));
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult GetRoles(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                using (var context = new Entities())
                {
                    var userRoles = context.tbl_UserRole.Where(y => y.UserId == id && y.IsDeleted != 1).Select(z => z.RoleId);
                    var response = context.tbl_Role.Where(x => userRoles.Contains(x.RoleId) && x.IsDeleted != 1).Select(x => new
                    {
                        RoleId = x.RoleId,
                        RoleName = x.Name
                    }).ToList();

                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetAvailableDashboards(string userId, string roleId)
        {
            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(roleId))
            {
                using (var context = new WebAppDbContext())
                {
                    var response = GetDashboards(userId, roleId);
                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(string userId, string roleId, string dashboardId,string isDefault, string pageMode)
        {
            var type = "success";
            var msg = pageMode == PageMode.Add.ToString() ? "Plan added successfully." : "Plan updated successfully.";

            isDefault = String.IsNullOrWhiteSpace(isDefault) ? "0" : isDefault;
            try
            {
                if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(roleId) && !string.IsNullOrWhiteSpace(dashboardId))
                {
                    using (var context = new WebAppDbContext())
                    {

                        if (roleId == AllRoles)
                        {
                            context.UserDashboardMappings.RemoveRange(context.UserDashboardMappings.Where(x => x.UserId == userId));

                            var roles = GetUserRoles(userId);
                            List<UserDashboardMapping> models = new List<UserDashboardMapping>();
                            foreach (var role in roles)
                            {
                                UserDashboardMapping model = new UserDashboardMapping()
                                {
                                    UserId = userId,
                                    RoleId = role.RoleId,
                                    DashboardId = dashboardId,
                                    InsertedBy = Session[SessionKeys.UserId].ToString(),
                                    InsertionDateTime = DateTime.Now,
                                    IsDefault = isDefault
                                };
                                models.Add(model);
                            }

                            context.UserDashboardMappings.AddRange(models);
                            context.SaveChanges();
                        }

                        else
                        {
                            UserDashboardMapping model = null;

                            if (pageMode == PageMode.Add.ToString())
                            {
                                model = new UserDashboardMapping()
                                {
                                    UserId = userId,
                                    RoleId = roleId,
                                    DashboardId = dashboardId,
                                    InsertedBy = Session[SessionKeys.UserId].ToString(),
                                    InsertionDateTime = DateTime.Now,
                                    IsDefault = isDefault
                                };

                                context.UserDashboardMappings.Add(model);
                            }
                            else if (pageMode == PageMode.Edit.ToString())
                            {
                                var dashboard = context.UserDashboardMappings.FirstOrDefault(x => x.RoleId == roleId && x.UserId == userId && x.DashboardId == dashboardId);
                                if (dashboard != null)
                                {
                                    dashboard.IsDefault = isDefault;
                                    context.UserDashboardMappings.Attach(dashboard);
                                    context.Entry(dashboard).State = EntityState.Modified;
                                }
                            }
                         
                            if (isDefault == "1")
                            {
                                var previousDefault = context.UserDashboardMappings.FirstOrDefault(x => x.RoleId == roleId && x.UserId == userId && x.IsDefault == "1");
                                if (previousDefault != null)
                                {
                                    if (previousDefault.RoleId != roleId)
                                    {
                                        previousDefault.IsDefault = "0";
                                        context.UserDashboardMappings.Attach(previousDefault);
                                        context.Entry(previousDefault).State = EntityState.Modified;
                                    }
                                }
                            }

                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                type = "error";
                msg = ex.ToString();
            }

            return Json(new
            {
                msg = msg,
                type = type
            }, JsonRequestBehavior.AllowGet);
        }

        private List<RoleDetails> GetUserRoles(string userId)
        {
            using (var context = new Entities())
            {
                var userRoles = context.tbl_UserRole.Where(y => y.UserId == userId && y.IsDeleted != 1).Select(z => z.RoleId);
                var response = context.tbl_Role.Where(x => userRoles.Contains(x.RoleId) && x.IsDeleted != 1).Select(x => new RoleDetails
                {
                    RoleId = x.RoleId,
                    RoleName = x.Name
                }
                ).ToList();



                return response;
            }
        }

        public ActionResult PopulateUserData(string userId, string roleId , string dashboardId , string pageMode)
        {
            var type = "success";
            string msg = string.Empty;

            try
            {
                if (!String.IsNullOrWhiteSpace(userId))
                {

                    dynamic details = new ExpandoObject();
                    string userName = string.Empty;
                    string roleName = string.Empty;

                    using (var context = new WebAppDbContext())
                    {
                        if (pageMode == PageMode.Add.ToString())
                        {
                            ViewBag.PageMode = PageMode.Add;
                            using (var db = new Entities())
                            {
                                var user = db.tbl_User.FirstOrDefault(x => x.UserId == userId && x.IsActive != 0);

                                if (user != null)
                                {
                                    var roles = GetUserRoles(user.UserId);

                                    roles.Add(new RoleDetails() { RoleId = AllRoles, RoleName = "All" });

                                    details.UserDetails = new SelectList(new List<dynamic>{                                        new
                                        {
                                            Id = user.UserId, Name = user.Name
                                        }
                                    }, "Id", "Name");

                                    details.IsDefault = new SelectList(new List<dynamic>{
                                           new{  Id = "0", Name = "False" },
                                           new {  Id = "1", Name = "True"  }
                                    }, "Id", "Name");

                                    details.Roles = new SelectList(roles, "RoleId", "RoleName");

                                    ViewBag.Details = details;

                                    return PartialView("_AddEditUserDashboardMapping");
                                }
                            }

                        }
                        else if (pageMode == PageMode.Edit.ToString())
                        {
                            ViewBag.PageMode = PageMode.Edit;
                            var response = context.UserDashboardMappings.Where(x => x.UserId == userId && x.DashboardId == dashboardId && roleId == x.RoleId);

                            if (response.Any())
                            {
                                var userDashboard = response.FirstOrDefault();
                                using (var entities = new Entities())
                                {
                                    userName = entities.tbl_User.FirstOrDefault(x => x.UserId == userDashboard.UserId).Name;
                                }

                                var roles = GetUserRoles(userDashboard.UserId);
                                roleName = roles.FirstOrDefault(x => x.RoleId == userDashboard.RoleId).RoleName;


                                details.UserDetails = new SelectList(new List<dynamic>{new
                                        {
                                            Id = userDashboard.UserId, Name = userName
                                        }
                                    }, "Id", "Name");

                                details.Roles = new SelectList(new List<dynamic>{new
                                        {
                                            RoleId = userDashboard.RoleId, RoleName = roleName
                                        }
                                    }, "RoleId", "RoleName");

                                details.IsDefault = new SelectList(new List<dynamic>{ 
                                           new{  Id = "0", Name = "False" },
                                           new {  Id = "1", Name = "True"  }
                                    }, "Id", "Name", userDashboard.IsDefault);

                                details.DashboardDetails = new SelectList(new List<dynamic> { new { DashboardId = userDashboard.DashboardId, DashboardName = userDashboard.DashboardId } }, "DashboardId", "DashboardName", userDashboard.DashboardId);

                                ViewBag.Details = details;

                                return PartialView("_AddEditUserDashboardMapping");
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return Json(new
            {
                msg = msg,
                type = type
            }, JsonRequestBehavior.AllowGet);
        }

        public dynamic GetDashboards(string userId, string roleId)
        {
            dynamic response = null;
            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(roleId))
            {
                using (var context = new WebAppDbContext())
                {
                    CustomDashboardStorage dashboardStorage = new CustomDashboardStorage();
                    var dashboardsInfo = dashboardStorage.GetAvailableDashboardsInfo();

                    if (roleId == AllRoles)
                    {
                        response = dashboardsInfo.Select(x => new
                        {
                            DashId = x.ID,
                            DashName = x.Name
                        }).ToList();
                    }
                    else
                    {
                        response = dashboardsInfo.Where(y => !context.UserDashboardMappings.Where(x => x.RoleId == roleId && x.UserId == userId).Select(x => x.DashboardId).Contains(y.ID)).Select(y => new
                        {
                            DashId = y.ID,
                            DashName = y.Name
                        }).ToList();
                    }
                }
            }

            return response;
        }

        [HttpPost]
        public ActionResult DeleteUserDashboard(string userId, string dashboardId, string roleId)
        {
            var type = "success";
            string msg = string.Empty;

            try
            {
                using (var context = new WebAppDbContext())
                {
                    var userDashboards = context.UserDashboardMappings.Where(x => x.UserId == userId && x.RoleId == roleId).ToList();

                    var toBeDeleted = userDashboards.First(x => x.DashboardId == dashboardId);

                    if (toBeDeleted.IsDefault == "1")
                    {
                        var remainingDashboards = userDashboards.Where(x => x.DashboardId != dashboardId);
                        if (remainingDashboards.Any())
                        {
                            var newDefaultDashboard = remainingDashboards.FirstOrDefault();
                            newDefaultDashboard.IsDefault = "1";

                            context.UserDashboardMappings.Add(newDefaultDashboard);
                            context.Entry(newDefaultDashboard).State = EntityState.Modified;

                        }
                    }

                    context.UserDashboardMappings.Attach(toBeDeleted);
                    context.Entry(toBeDeleted).State = EntityState.Deleted;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                type = "error";
                msg = ex.ToString();
            }

            return Json(new
            {
                msg = msg,
                type = type
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ClientDashboards()
        {
            TempData["UserId"] = Session[SessionKeys.UserId].ToString();
            ViewBag.UserId = Session[SessionKeys.UserId].ToString();
            return View("UserDashboards_ClientView");
        }

        [HttpPost]
        public ActionResult LoadUserDashboardClientView(int length, int start)
        {
            string userId = Session[SessionKeys.UserId].ToString();
            int recordCount = 0, filterrecord = 0;
            List<UserDashboardMapping> userDashboardMappingData = new List<UserDashboardMapping>();

            try
            {
                using (WebAppDbContext db = new WebAppDbContext())
                {
                    string searchvalue = Request.Form["search[value]"];
                    var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                    var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                    Expression<Func<UserDashboardMapping, object>> sortExpression;
                    switch (sortColumn)
                    {
                        case "UserId":
                            sortExpression = (x => x.UserId);
                            break;
                        default:
                            sortExpression = (x => x.UserId);
                            break;
                    }

                    if (sortColumnDir == "asc")
                    {
                        userDashboardMappingData = db.UserDashboardMappings.Where(x => x.UserId.Contains(userId))
                       .OrderBy(sortExpression).Skip(start).Take(length).ToList();
                    }
                    else
                    {
                        userDashboardMappingData = db.UserDashboardMappings.Where(x => x.UserId.Contains(userId))
                        .OrderByDescending(sortExpression).Skip(start).Take(length).ToList();
                    }

                    var recordcount = db.UserDashboardMappings.Where(x => x.UserId == userId).Count();

                    if (searchvalue != "")
                    {
                        filterrecord = userDashboardMappingData.Count();
                    }
                    else
                    {
                        filterrecord = recordcount;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            var response = new { recordsTotal = recordCount, recordsFiltered = filterrecord, data = userDashboardMappingData };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditClientDashboard(string userId,string roleId,string dashboardId)
        {
            using(var context = new WebAppDbContext())
            {
                var response = context.UserDashboardMappings.FirstOrDefault(x => x.UserId == userId && x.RoleId == roleId && x.DashboardId == dashboardId);
                return View("ClientMode", response);
            }
        }

        [HttpPost]
        public JsonResult SaveAsDashboard(string dashboardId,string userId,string roleId)
        {
            var type = "success";
            var msg = "Dashboard added successfully";
            try
            {
                using(var context = new WebAppDbContext())
                {
                    UserDashboardMapping model = new UserDashboardMapping()
                    {
                        DashboardId = dashboardId,
                        UserId = userId,
                        RoleId = roleId,
                        IsDefault = "0",
                        InsertionDateTime = DateTime.Now,
                        InsertedBy = Session[SessionKeys.UserId].ToString()
                    };

                    context.UserDashboardMappings.Add(model);
                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                type = "error";
                msg = ex.ToString();
            }

            return Json(new { type = type, msg = msg }, JsonRequestBehavior.AllowGet);
        }
    }


    public class RoleDetails
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}