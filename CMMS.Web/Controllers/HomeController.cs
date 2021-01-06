using ClassLibrary.Common;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication.Helpers;

namespace WebApplication.Controllers
{
    [Authorization]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult start()
        {
            return View();
        }
        //public ActionResult Index()
        //{
        //    var vm = new ConfigViewModel
        //    { _tbl_Unit = db.tbl_Unit.ToList(), };
        //    List<TreeViewNode> nodes = new List<TreeViewNode>();
        //    foreach (C_Config_Master c in db.C_Config_Master)
        //    {
        //        if (c.PESWBS == "0") { c.PESWBS = "#"; }
        //        nodes.Add(new TreeViewNode { id = c.ESWBS.ToString(), parent = c.PESWBS.ToString(), text = c.Nomanclature });
        //    }
        //    //Serialize to JSON string.
        //    ViewBag.JsonMasterConfig = (new JavaScriptSerializer()).Serialize(nodes);
        //    List<TreeViewNode> nodesSite = new List<TreeViewNode>();
        //    foreach (C_Site_Config c in db.C_Site_Config)
        //    {
        //        if (c.PESWBS == "0") { c.PESWBS = "#"; }
        //        nodesSite.Add(new TreeViewNode { id = c.ESWBS.ToString(), parent = c.PESWBS.ToString(), text = c.Nomanclature });
        //    }
        //    //Serialize to JSON string.
        //    ViewBag.SiteJson = (new JavaScriptSerializer()).Serialize(nodesSite);
        //    return View("Index", vm);
        //}
    }
}