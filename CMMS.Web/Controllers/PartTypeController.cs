using ClassLibrary.Models;
using NLog;
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
    public class PartTypeController : BaseController
    {
        private static Logger _logger;

        public PartTypeController()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public ActionResult Index()
        {
            string actionName = "Index";
            List<tbl_PartType> response = new List<tbl_PartType>();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
                using (WebAppDbContext db = new WebAppDbContext())
                {
                    response = db.tbl_PartType.ToList();
                }
                _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }
            return View(response);
        }
 
        public ActionResult Details(string id)
        {
            string actionName = "Details";
            tbl_PartType response = new tbl_PartType();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");

                if (String.IsNullOrWhiteSpace(id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    using (WebAppDbContext db = new WebAppDbContext())
                    {
                        response = db.tbl_PartType.FirstOrDefault(x => x.PartTypeID == id);
                    }
                }
                _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            return PartialView("_Details", response);
        }

        // GET: AreaType/Create
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        // POST: AreaType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tbl_PartType tbl_PartType)
        {
            string actionName = "Create";
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
                 
                if(ModelState.IsValid)
                {
                    using (WebAppDbContext db = new WebAppDbContext())
                    {
                        db.tbl_PartType.Add(tbl_PartType);
                        db.SaveChanges();
                    }
                }
                else
                {
                    _logger.Log(LogLevel.Trace, actionName + " :: ended.");
                    return PartialView("_Create");
                }
                
                _logger.Log(LogLevel.Trace, actionName + " :: ended.");

                Alert("Data Saved Sucessfully!!!", NotificationType.success);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
                Alert("Their is something went wrong!!!", NotificationType.error);
                return RedirectToAction("Index");
            }
        }
 
        public ActionResult Edit(string id)
        {
            string actionName = "Create";
            tbl_PartType response = new tbl_PartType();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
                if (String.IsNullOrWhiteSpace(id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    using (WebAppDbContext db = new WebAppDbContext())
                    {
                        response = db.tbl_PartType.FirstOrDefault(x => x.PartTypeID == id);
                    }
                }
                _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }
    
            return PartialView("_Edit", response);
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, tbl_PartType tbl_PartType)
        {
            string actionName = "Edit";
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
                if (String.IsNullOrWhiteSpace(id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    if(ModelState.IsValid)
                    {
                        using (WebAppDbContext db = new WebAppDbContext())
                        {
                            db.Entry(tbl_PartType).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        _logger.Log(LogLevel.Trace, actionName + " :: ended.");
                        return PartialView("_Edit");
                    }
                }

                _logger.Log(LogLevel.Trace, actionName + " :: ended.");

                Alert("Record Updated Sucessfully!!!", NotificationType.success);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
                Alert("Their is something went wrong!!!", NotificationType.error);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Delete(string id)
        {
            string actionName = "Delete";
            tbl_PartType response = new tbl_PartType();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
                if (String.IsNullOrWhiteSpace(id))
                {
                    _logger.Log(LogLevel.Trace, actionName + " :: ended.");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    using (WebAppDbContext db = new WebAppDbContext())
                    {
                        response = db.tbl_PartType.FirstOrDefault(x => x.PartTypeID == id);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }
            _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            return PartialView("_Delete", response);
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, tbl_PartType tbl_PartType)
        {
            string actionName = "Delete";
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
                if (String.IsNullOrWhiteSpace(id))
                {
                    _logger.Log(LogLevel.Trace, actionName + " :: ended.");
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    // TODO: Add delete logic here
                    using (WebAppDbContext db = new WebAppDbContext())
                    {
                        var response = db.tbl_PartType.FirstOrDefault(x => x.PartTypeID == id);
                        if (response != null)
                        {
                            db.tbl_PartType.Remove(response);
                            db.SaveChanges();
                        }
                        else
                        {
                            _logger.Log(LogLevel.Trace, actionName + " :: ended.");
                            Alert("Record not found!!!", NotificationType.error);
                            return RedirectToAction("Index");
                        }
                    }

                    _logger.Log(LogLevel.Trace, actionName + " :: ended.");
                    Alert("Record Deleted Sucessfully!!!", NotificationType.success);
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
                Alert("Their is something went wrong!!!", NotificationType.error);
                return RedirectToAction("Index");
            }
        }

    }
}