using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Helpers;
using static ClassLibrary.Common.Enums;

namespace WebApplication.Controllers
{
    [Authorization]
    public class PMSController : BaseController
    {
        public ActionResult Index()
        {
            using (WebAppDbContext db = new WebAppDbContext())
            {
                var abc = db.M_PMS.ToList();
                return View(abc);
            }
        }


        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (WebAppDbContext db = new WebAppDbContext())
            {
                var abc = db.M_PMS.Where(x => x.PMS_NO == id).FirstOrDefault();
                return PartialView("_Details", abc);
            }
        }

     
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(M_PMS M_PMS)
        {
            try
            {
                // TODO: Add insert logic here
                using (WebAppDbContext db = new WebAppDbContext())
                {
                    if (!ModelState.IsValid)
                    {
                        return PartialView("_Create");
                    }
                    db.M_PMS.Add(M_PMS);
                    db.SaveChanges();
                }
                Alert("Data Saved Sucessfully!!!", NotificationType.success);
                return RedirectToAction("Index");
            }
            catch
            {
                Alert("Their is something went wrong!!!", NotificationType.error);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (WebAppDbContext db = new WebAppDbContext())
            {
                var abc = db.M_PMS.Where(x => x.PMS_NO == id).FirstOrDefault();
                return PartialView("_Edit", abc);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, M_PMS M_PMS)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                // TODO: Add update logic here
                using (WebAppDbContext db = new WebAppDbContext())
                {
                    if (!ModelState.IsValid)
                    {
                        return PartialView("_Edit");
                    }
                    db.Entry(M_PMS).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                Alert("Record Updated Sucessfully!!!", NotificationType.success);
                return RedirectToAction("Index");
            }
            catch
            {
                Alert("Their is something went wrong!!!", NotificationType.error);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (WebAppDbContext db = new WebAppDbContext())
            {
                var abc = db.M_PMS.Where(x => x.PMS_NO == id).FirstOrDefault();
                return PartialView("_Delete", abc);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, M_PMS M_PMS)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                // TODO: Add delete logic here
                using (WebAppDbContext db = new WebAppDbContext())
                {
                    var abc = db.M_PMS.Where(x => x.PMS_NO == id).FirstOrDefault();
                    db.M_PMS.Remove(abc);
                    db.SaveChanges();
                }
                Alert("Record Deleted Sucessfully!!!", NotificationType.success);
                return RedirectToAction("Index");
            }
            catch
            {
                Alert("Their is something went wrong!!!", NotificationType.error);
                return RedirectToAction("Index");
            }
        }

    }
}