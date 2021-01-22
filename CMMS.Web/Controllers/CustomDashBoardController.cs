using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class CustomDashBoardController : Controller
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