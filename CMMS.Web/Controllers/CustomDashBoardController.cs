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
                List<string> assignedUserIds = new List<string>();
                using (var context = new WebAppDbContext())
                {
                    assignedUserIds = context.UserDashboardMappings.Select(x => x.UserId).ToList();
                }

                using (var context = new Entities())
                {
                   var response = context.tbl_User.Where(p => assignedUserIds.All(p2 => p2 != p.UserId))
                    .Select(x => new
                    {
                        Name = x.Name,
                        Id = x.UserId
                    }).ToList();

                    ViewBag.UserList = new SelectList(response.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name}), "Value", "Text");

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
        public ActionResult DeleteDashboard(string DashboardID)
        {
            var dashboardPath = $"~/{ConfigurationManager.AppSettings["DashboardPath"]?.ToString()}";
            CustomDashboardStorage newDashboardStorage = new CustomDashboardStorage(dashboardPath);

            newDashboardStorage.DeleteDashboard(HttpContext.Request.MapPath(Path.Combine(dashboardPath, $"{DashboardID}.xml")));
            return new EmptyResult();
        }
    }
}