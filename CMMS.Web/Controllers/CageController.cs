using ClassLibrary.Common;
using ClassLibrary.Models;
using ClassLibrary.ViewModels;
using CMMS.Web.Helper;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication.Helpers;
using static ClassLibrary.Common.Enums;

namespace WebApplication.Controllers
{

    [HandleError]
    public class CageController : BaseController
    {
        private static Logger _logger;

        public CageController()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        [CustomAuthorization]
        public ActionResult Index()
        {
            string actionName = "Index";
            _logger.Log(LogLevel.Error, actionName + " :: started.");

            CageViewModels cageVM = new CageViewModels();
            try
            {
                using (var db = new WebAppDbContext())
                {
                    List<GraphData> data = new List<GraphData>();

                    var cages = db.tbl_Cage.ToList();
                    var countries = cages?.Select(x => x.Country).Distinct();

                    cageVM.cageCount = cages?.Count();
                    cageVM.cageActiveCount = cages.Where(x => x.Status == "Active")?.Count();
                    cageVM.CageCountryCount = countries?.Count();

                    foreach (var item in countries)
                    {
                        GraphData details = new GraphData();
                        details.label = item;
                        details.value = cages.Where(x => x.Country == item && x.Status == "Active")?.Count();
                        data.Add(details);
                    }

                    cageVM.dataList = data;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");

            return View(cageVM);
        }

        [HttpPost]
        [ValidateAjax]
        [ValidateLoadActions]
        public JsonResult LoadData(int length, int start)
        {
            string actionName = "LoadData";
            int recordcount = 0, filterrecord = 0;
            List<CageModel> Cagedata = null;

            _logger.Log(LogLevel.Trace, actionName + " :: started.");

            try
            {
                using (var db = new WebAppDbContext())
                {
                    //search value
                    string searchvalue = Request.Form["search[value]"];
                    //Find Order Column
                    var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                    var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                    Expression<Func<tbl_Cage, object>> sortExpression;
                    switch (sortColumn)
                    {
                        case "CageCode":
                            sortExpression = (x => x.CageCode);
                            break;
                        case "CageName":
                            sortExpression = (x => x.CageName);
                            break;
                        case "Country":
                            sortExpression = (x => x.Country);
                            break;
                        case "Status":
                            sortExpression = (x => x.Status);
                            break;
                        default:
                            sortExpression = (x => x.CageCode);
                            break;
                    }

                     if (sortColumnDir == "asc")
                    {
                        Cagedata = db.tbl_Cage.Where(x => x.CageCode.Contains(searchvalue)
                     || x.CageName.Contains(searchvalue)
                     || x.Country.Contains(searchvalue)
                     || x.Status.Contains(searchvalue))
                     .OrderBy(sortExpression).Skip(start).Take(length)
                     .Select(x => new CageModel
                     {
                         CageId = x.CageId,
                         CageCode = x.CageCode,
                         CageName = x.CageName,
                         Country = x.Country,
                         Status = x.Status,
                     }).ToList();
                    }
                    else
                    {
                        Cagedata = db.tbl_Cage.Where(x => x.CageCode.Contains(searchvalue)
                       || x.CageName.Contains(searchvalue)
                       || x.Country.Contains(searchvalue)
                       || x.Status.Contains(searchvalue))
                       .OrderByDescending(sortExpression).Skip(start).Take(length)
                       .Select(x => new CageModel
                       {
                           CageId = x.CageId,
                           CageCode = x.CageCode,
                           CageName = x.CageName,
                           Country = x.Country,
                           Status = x.Status,
                       }).ToList();
                    }

              
                    recordcount = db.tbl_Cage.Count();

                    if (searchvalue != "")
                    {
                        filterrecord = Cagedata.Count();
                    }
                    else
                    {
                        filterrecord = recordcount;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");

            var response = new { recordsTotal = recordcount, recordsFiltered = filterrecord, data = Cagedata };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorization]
        public ActionResult Create()
        {
            string actionName = "Create";
            _logger.Log(LogLevel.Trace, actionName + " :: started.");
            CageViewModels vm = new CageViewModels();

            try
            {
                SessionKeys.LoadTablesInSession(SessionKeys.Countries, "", "");
                vm._tbl_Country = ((List<ClassLibrary.Models.tbl_Country>)Session[SessionKeys.Countries]);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            return PartialView("_Create", vm);
        }

        [HttpPost]
        [ValidateAjax]
        [ValidateLoadActions]
        public JsonResult CageCodeCheck(string CageCode)
        {
            string actionName = "CageCodeCheck";
            int response = 0;

            _logger.Log(LogLevel.Trace, actionName + " :: started.");

            try
            {
                if (!String.IsNullOrWhiteSpace(CageCode))
                {
                    using (var db = new WebAppDbContext())
                    {
                        if (db.tbl_Cage.Any(x => x.CageCode == CageCode))
                        {
                            response = 1;
                        }
                    }
                }
                else
                {
                    _logger.Log(LogLevel.Error, actionName + " :: Cage Code is empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            return Json(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorization]
        public ActionResult Create(tbl_Cage tbl_Cage)
        {
            string actionName = "Create";
            _logger.Log(LogLevel.Trace, actionName + " :: started.");

            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new WebAppDbContext())
                    {
                        if (!db.tbl_Cage.Any(x => x.CageCode == tbl_Cage.CageCode))
                        {

                            // Get Current user Id
                            var userId = Session[SessionKeys.UserId]?.ToString();
                            tbl_Cage.CreatedByUser = userId;
                            tbl_Cage.ModifiedByUser = userId;

                            //Get Current Date & Time.
                            tbl_Cage.CreatedOnDate = DateTime.Now;
                            tbl_Cage.ModifiedOnDate = DateTime.Now;
                            tbl_Cage.Status = "Active";

                            db.tbl_Cage.Add(tbl_Cage);
                            db.SaveChanges();


                            Alert("Record Added Successfully !! ", NotificationType.success);
                         }
                        else
                        {
                            _logger.Log(LogLevel.Trace, actionName + " :: Cage code : " + tbl_Cage.CageCode + " already exist.");
                            Alert("Cage code already exist.", NotificationType.error);
                        }
                    }
                }
                else
                {
                    CageViewModels vm = new CageViewModels();

                    SessionKeys.LoadTablesInSession(SessionKeys.Countries, "", "");
                    vm._tbl_Country = ((List<ClassLibrary.Models.tbl_Country>)Session[SessionKeys.Countries]);

                    _logger.Log(LogLevel.Trace, actionName + " :: Model state not valid.");
                    return PartialView("_Create", vm);
                }
            }
            catch (Exception ex)
            {
                Alert("Their is something went wrong!!!", NotificationType.error);
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
                Exception(ex);
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");

            return RedirectToAction("Index");
        }

        [CustomAuthorization]
        public ActionResult Details(int id)
        {
            string actionName = "Details";
            _logger.Log(LogLevel.Trace, actionName + " :: started.");
            CageViewModels objCageViewModels = new CageViewModels();
            try
            {
                if (id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                objCageViewModels = GetCage(id);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");

            return PartialView("_Details", objCageViewModels);
        }
        
        [CustomAuthorization]
        public ActionResult Edit(int id)
        {
            string actionName = "Edit";
            _logger.Log(LogLevel.Trace, actionName + " :: started.");
            CageViewModels objCageViewModels = new CageViewModels();
            try
            {
                if (id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                objCageViewModels = GetCage(id);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");

            return PartialView("_Edit", objCageViewModels);
        }


        [HttpPost]
        [CustomAuthorization]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, tbl_Cage tbl_Cage)
        {
            string actionName = "Edit";
            _logger.Log(LogLevel.Trace, actionName + " :: started.");

            try
            {
                if (ModelState.IsValid)
                {
                    if (id <= 0)
                    {
                        Alert("Invalid Cage Selected.", NotificationType.error);
                        _logger.Log(LogLevel.Trace, actionName + " :: Ended. Invalid cage id : " + id);
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    using (var db = new WebAppDbContext())
                    {
                        var cage = db.tbl_Cage.FirstOrDefault(x => x.CageId == id);
                        if (cage != null && (cage?.CageCode == tbl_Cage.CageCode))
                        {
                            cage.CageName = tbl_Cage.CageName;
                            cage.Address = tbl_Cage.Address; 
                            cage.Country = tbl_Cage.Country;
                            cage.City = tbl_Cage.City;
                            cage.PostalCode = tbl_Cage.PostalCode;
                            cage.Status = tbl_Cage.Status;

                            cage.ModifiedByUser = Session[SessionKeys.UserId]?.ToString();
                            cage.ModifiedOnDate = DateTime.Now;

                            db.Entry(cage).State = EntityState.Modified;
                            db.SaveChanges();

                            Alert("Record Updated Successfully.", NotificationType.success);
                        }
                        else
                        {
                            Alert("Cage Not Found.", NotificationType.error);
                            _logger.Log(LogLevel.Trace, actionName + " :: Ended. Cage not found for cage id : " + id);
                        }
                    }
                }
                else
                {
                    _logger.Log(LogLevel.Trace, actionName + " :: Ended. Model state is not valid for cage id : " + id);
                    CageViewModels objCageViewModels = GetCage(id);
                    return PartialView("_Edit", objCageViewModels);
                }
            }
            catch (Exception ex)
            {
                Alert("Something Went Wrong !!!", NotificationType.error);
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
                Exception(ex);
            }

            return RedirectToAction("Index");
        }

        [CustomAuthorization]
        public ActionResult Delete(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return PartialView("_Delete");
        }

        [HttpPost]
        [CustomAuthorization]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            string actionName = "Delete";
            string msg = String.Empty;
            _logger.Log(LogLevel.Trace, actionName + " :: started.");
            try
            {
                if (id <= 0)
                {
                    Alert("Invalid Cage Selected.", NotificationType.error);
                    _logger.Log(LogLevel.Trace, actionName + " :: Ended. Invalid cage id : " + id);
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    using (var db = new WebAppDbContext())
                    {
                        var result = db.tbl_Cage.FirstOrDefault(x => x.CageId == id);
                        if (result != null)
                        {
                            result.Status = "InActive";
                            db.Entry(result).State = EntityState.Modified;
                            db.SaveChanges();

                            msg = "Cage Deleted Successfully.";
                            Alert(msg, NotificationType.success);
                        }
                        else
                        {
                            msg = "Cage Not Found.";
                            Alert(msg, NotificationType.error);
                            _logger.Log(LogLevel.Trace, actionName + " :: Ended. Cage id not found : " + id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exception(ex);
                msg = "Something Went Wrong !!! ";
                Alert(msg, NotificationType.error);
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            return RedirectToAction("Index");
        }


        #region Utilities

        private CageViewModels GetCage(int id)
        {
            SessionKeys.LoadTablesInSession(SessionKeys.Countries);
            using (var db = new WebAppDbContext())
            {
                var viewModel = new CageViewModels()
                {
                    tbl_Cage = db.tbl_Cage.FirstOrDefault(x => x.CageId == id),
                    _tbl_Country = ((List<ClassLibrary.Models.tbl_Country>)Session[SessionKeys.Countries]),
                };
                return viewModel;
            }
        }

        protected override void Dispose(bool disposing)
        {
            using (var db = new WebAppDbContext())
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        #endregion
    }
}

