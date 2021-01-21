using System;
using System.Collections.Generic;
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
    }
}