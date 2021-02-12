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
    [CustomAuthorization]
    public class MCATController : BaseController
    {
        // GET: AwardType
 
        public ActionResult Index()
        {
            using (WebAppDbContext db = new WebAppDbContext())
            {
                var abc = db.tbl_MCAT.ToList();
                return View(abc);
            }
        }

        // GET: AreaType/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (WebAppDbContext db = new WebAppDbContext())
            {
                var abc = db.tbl_MCAT.Where(x => x.MCAT_ID == id).FirstOrDefault();
                return PartialView("_Details", abc);
            }
        }

        // GET: AreaType/Create
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        // POST: AreaType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tbl_MCAT tbl_MCAT)
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
                    db.tbl_MCAT.Add(tbl_MCAT);
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

        // GET: AreaType/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (WebAppDbContext db = new WebAppDbContext())
            {
                var abc = db.tbl_MCAT.Where(x => x.MCAT_ID == id).FirstOrDefault();
                return PartialView("_Edit", abc);
            }
        }

        // POST: AreaType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, tbl_MCAT tbl_MCAT)
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
                    db.Entry(tbl_MCAT).State = System.Data.Entity.EntityState.Modified;
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

        // GET: AreaType/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (WebAppDbContext db = new WebAppDbContext())
            {
                var abc = db.tbl_MCAT.Where(x => x.MCAT_ID == id).FirstOrDefault();
                return PartialView("_Delete", abc);
            }
        }

        // POST: AreaType/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, tbl_MCAT tbl_MCAT)
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
                    var abc = db.tbl_MCAT.Where(x => x.MCAT_ID == id).FirstOrDefault();
                    db.tbl_MCAT.Remove(abc);
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