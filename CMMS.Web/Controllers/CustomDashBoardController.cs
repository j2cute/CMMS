using ClassLibrary.Models;
using CMMS.Web.Helper;
using ILS.UserManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class CustomDashboardController : Controller
    {
        
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

 
        public ActionResult AddUserDashboard()
        {
            ViewBag.PageMode = PageMode.Add;

            var type = "success";
            string msg = string.Empty;
            
            try
            {
               // List<string> assignedUserIds = new List<string>();
                //using (var context = new WebAppDbContext())
                //{
                //    assignedUserIds = context.UserDashboardMappings.Select(x => x.UserId).ToList();
                //}

                using (var context = new Entities())
                {
                    //.Where(p => assignedUserIds.All(p2 => p2 != p.UserId))
                   var response = context.tbl_User
                    .Select(x => new
                    {
                        Name = x.Name,
                        Id = x.UserId
                    }).ToList();

                    ViewBag.UserList = response;

                    return PartialView("_AddEditUserDashboardMapping", response);
                }

            }
            catch(Exception ex)
            {
                
            }

            return Json(new
            {
                msg = msg,
                type = type
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetRoles(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                using (var context = new Entities())
                {
                    var userRoles = context.tbl_UserRole.Where(y => y.UserId == id && y.IsDeleted != 1).Select(z => z.RoleId);
                    var response = context.tbl_Role.Where(x=> userRoles.Contains(x.RoleId) && x.IsDeleted != 1).Select(x=> new
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
        public ActionResult GetAvailableDashboards(string userId,string roleId)
        {
            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(roleId))
            {
                using (var context = new WebAppDbContext())
                {
                    CustomDashboardStorage dashboardStorage = new CustomDashboardStorage();
                    var dashboardsInfo = dashboardStorage.GetAvailableDashboardsInfo();

                    var response = dashboardsInfo.Where(y=> !context.UserDashboardMappings.Where(x => x.RoleId == roleId && x.UserId == userId).Select(x=>x.DashboardId).Contains(y.ID)).Select(y=> new
                    {
                        DashId= y.ID,
                        DashName = y.Name
                    }).ToList();

                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadAssignedDashboards(int length, int start)
        {
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
                        case "RoleId":
                            sortExpression = (x => x.RoleId);
                            break;
                        case "DashboardName":
                            sortExpression = (x => x.DashboardId);
                            break;
                        case "InsertedBy":
                            sortExpression = (x => x.InsertedBy);
                            break;
                        case "LastUpdatedBy":
                            sortExpression = (x => x.LastUpdatedBy);
                            break;
                        default:
                            sortExpression = (x => x.UserId);
                            break;
                    }

                    if (sortColumnDir == "asc")
                    {
                        userDashboardMappingData = db.UserDashboardMappings.Where(x => x.UserId.Contains(searchvalue)
                     || x.RoleId.Contains(searchvalue)
                     || x.DashboardId.Contains(searchvalue)
                     || x.InsertedBy.Contains(searchvalue) || x.LastUpdatedBy.Contains(searchvalue))
                     .OrderBy(sortExpression).Skip(start).Take(length).ToList();
                    }
                    else
                    {
                       userDashboardMappingData = db.UserDashboardMappings.Where(x => x.UserId.Contains(searchvalue)
                      || x.RoleId.Contains(searchvalue)
                      || x.DashboardId.Contains(searchvalue)
                      || x.InsertedBy.Contains(searchvalue) || x.LastUpdatedBy.Contains(searchvalue))
                        .OrderByDescending(sortExpression).Skip(start).Take(length).ToList();
                    }

                    var recordcount = db.UserDashboardMappings.Count();

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

        [HttpPost]
        public ActionResult Save(string userId,string roleId,string dashboardId)
        {
            var type = "success";
            var msg = "Plan edited successfully.";

            if (!string.IsNullOrWhiteSpace(userId) || !string.IsNullOrWhiteSpace(roleId) || !string.IsNullOrWhiteSpace(dashboardId))
            {
                using (var context = new Entities())
                {
                  
                }
            }
            return Json(new
            {
                msg = msg,
                type = type
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteDashboard(string DashboardID)
        {
            var dashboardPath = $"~/{ConfigurationManager.AppSettings["DashboardPath"]?.ToString()}";
            CustomDashboardStorage newDashboardStorage = new CustomDashboardStorage(dashboardPath);

            newDashboardStorage.DeleteDashboard(HttpContext.Request.MapPath(Path.Combine(dashboardPath, $"{DashboardID}.xml")));
            return new EmptyResult();
        }
    }
}