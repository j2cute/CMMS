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
    public class PMSController : BaseController
    {
        private static Logger _logger;

        public PMSController()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public ActionResult Index()
        {
            string actionName = "Index";
            List<M_PMS> response = new List<M_PMS>();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");

                using (WebAppDbContext db = new WebAppDbContext())
                {
                    response = db.M_PMS.ToList(); 
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
            M_PMS response = new M_PMS();
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
                        response = db.M_PMS.FirstOrDefault(x => x.PMS_NO == id);
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

     
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(M_PMS M_PMS)
        {
            string actionName = "Create";
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
          
                if (ModelState.IsValid)
                {
                    using (WebAppDbContext db = new WebAppDbContext())
                    {
                        db.M_PMS.Add(M_PMS);
                        db.SaveChanges();
                    }

                    _logger.Log(LogLevel.Trace, actionName + " :: ended.");

                    Alert("Data Saved Sucessfully!!!", NotificationType.success);
                    return RedirectToAction("Index");
                }
                else
                {

                    _logger.Log(LogLevel.Trace, actionName + " :: ended.");
                    return PartialView("_Create");
                }
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
            string actionName = "Edit";
            M_PMS response = new M_PMS();
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
                        response = db.M_PMS.FirstOrDefault(x => x.PMS_NO == id);
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
        public ActionResult Edit(string id, M_PMS M_PMS)
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
                            db.Entry(M_PMS).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
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
            M_PMS response = new M_PMS();
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
                        response = db.M_PMS.FirstOrDefault(x => x.PMS_NO == id);
                    }
                }
                _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            return PartialView("_Delete", response);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, M_PMS M_PMS)
        {
            string actionName = "Delete";
            M_PMS response = new M_PMS();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
                if (String.IsNullOrWhiteSpace(id))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                using (WebAppDbContext db = new WebAppDbContext())
                {
                    response = db.M_PMS.FirstOrDefault(x => x.PMS_NO == id);
                    if(response != null)
                    {
                        db.M_PMS.Remove(response);
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
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
                Alert("Their is something went wrong!!!", NotificationType.error);
                return RedirectToAction("Index");
            }
        }

    }
}