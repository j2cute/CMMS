using ClassLibrary.Common;
using ClassLibrary.Models;
using ClassLibrary.ViewModels;
using CMMS.Web.Helper;
using Microsoft.AspNet.Identity;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication.Helpers;
using static ClassLibrary.Common.Enums;

namespace WebApplication.Controllers
{

    public class ConfigController : BaseController
    {
        private static Logger _logger;

        public ConfigController()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        #region ConfigurationEquipment

        [CustomAuthorization]
        public ActionResult ConfigEquipIndex()
        {
            var vm = new ConfigViewModel();

            string actionName = "ConfigEquipIndex";
            _logger.Log(LogLevel.Trace, actionName + " :: started.");

            try
            {
                SessionKeys.LoadTablesInSession(SessionKeys.ApplicableUnits, "", "");

                var selectedSiteId = ((SessionHelper)Session[SessionKeys.SessionHelperInstance])?.SelectedSiteId;
                if (!String.IsNullOrWhiteSpace(selectedSiteId))
                {
                    int siteId = Convert.ToInt32(selectedSiteId);
                    vm._tbl_Unit = ((List<ClassLibrary.Models.tbl_Unit>)Session[SessionKeys.ApplicableUnits]).Where(x => x.Id == siteId);
                }
                else
                {
                    vm._tbl_Unit = ((List<ClassLibrary.Models.tbl_Unit>)Session[SessionKeys.ApplicableUnits]);

                }

                vm.selectedData = "0";

                using (var db = new WebAppDbContext())
                {
                    vm.tbl_Parts_list = db.tbl_Parts.Where(x => x.Status == "Active" && (x.PartTypeID == "X" || x.PartTypeID == "A")).ToList();
                    vm._M_PMS = db.M_PMS.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            _logger.Log(LogLevel.Trace, actionName + " :: Ended.");

            return View("ConfigEquipIndex", vm);
        }

        [HttpPost]
        //[CustomAuthorization]
        public JsonResult LoadData(int length, int start)
        {
            using (var db = new WebAppDbContext())
            {
                //search value
                string searchvalue = Request.Form["search[value]"];
                //Find Order Column
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                Expression<Func<tbl_Parts, object>> sortExpression;
                switch (sortColumn)
                {
                    case "CageCode":
                        sortExpression = (x => x.CageCode);
                        break;

                    case "Part_No":
                        sortExpression = (x => x.Part_No);
                        break;
                    case "PART_NAME":
                        sortExpression = (x => x.PART_NAME);
                        break;

                    case "PartTypeID":
                        sortExpression = (x => x.PartTypeID);
                        break;
                    default:
                        sortExpression = (x => x.CageCode);
                        break;
                }


                int filterrecord;
                List<PartsModel> Partdata = null;
                if (sortColumnDir == "asc")
                {
                    Partdata = db.tbl_Parts.Where(x => x.Status == "Active" && (x.PartTypeID == "X" || x.PartTypeID == "A") && (
                    x.CageCode.Contains(searchvalue)

                    || x.Part_No.Contains(searchvalue)
                    || x.PART_NAME.Contains(searchvalue)
                    || x.PartTypeID.Contains(searchvalue))
                    )
                    .OrderBy(sortExpression).Skip(start).Take(length)
                    .Select(x => new PartsModel
                    {
                        PartId = x.PartId,
                        CageId = x.CageId,
                        CageCode = x.CageCode,
                        Part_No = x.Part_No,
                        PART_NAME = x.PART_NAME,
                        PartTypeID = x.PartTypeID
                    }).ToList();

                }
                else
                {
                    Partdata = db.tbl_Parts.Where(x => x.Status == "Active" && (x.PartTypeID == "X" || x.PartTypeID == "A") && (
                  x.CageCode.Contains(searchvalue)

                    || x.Part_No.Contains(searchvalue)
                    || x.PART_NAME.Contains(searchvalue)
                    || x.PartTypeID.Contains(searchvalue))
                )
                .OrderByDescending(sortExpression).Skip(start).Take(length)
                .Select(x => new PartsModel
                {
                    PartId = x.PartId,
                    CageId = x.CageId,
                    CageCode = x.CageCode,
                    Part_No = x.Part_No,
                    PART_NAME = x.PART_NAME,
                    PartTypeID = x.PartTypeID
                }).ToList();
                }
                if (searchvalue != "")
                {
                    filterrecord = Partdata.Count();
                }
                else 
                { 
                    filterrecord = db.tbl_Parts.Where(x => x.Status == "Active" && (x.PartTypeID == "X" || x.PartTypeID == "A")).Count();
                }

                var recordcount = db.tbl_Parts.Where(x => x.Status == "Active" && (x.PartTypeID == "X" || x.PartTypeID == "A")).Count();
 

                var response = new { recordsTotal = recordcount, recordsFiltered = filterrecord, data = Partdata };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

        }

        //[CustomAuthorization]
        public JsonResult GetConfigEquipment(int? siteId)
        {
            try
            {
                ConfigViewModel vm = new ConfigViewModel();
                if (siteId != 0)
                {
                    vm = GetSiteConfigTree(siteId);
                }
                return Json(vm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Exception(ex);
                return Json(siteId, JsonRequestBehavior.AllowGet);
            }
        }

        #region GetConfigData
        [HttpPost]
        [CustomAuthorization]
        public JsonResult GetSelectedConfigData(string eswbs)
        {
            string actionName = "GetSelectedConfigData";
            _logger.Log(LogLevel.Trace, actionName + " :: started.");

            var type = "error";
            var msg = string.Empty;
            ConfigViewModel vm = new ConfigViewModel();
            int siteId;
            try
            {
                using (var db = new WebAppDbContext())
                {
                    var selectedSiteId = ((SessionHelper)Session[SessionKeys.SessionHelperInstance])?.SelectedSiteId;


                    if (!String.IsNullOrWhiteSpace(eswbs) && !String.IsNullOrWhiteSpace(selectedSiteId))
                    {
                        siteId = Convert.ToInt32(selectedSiteId);
                        var eswbsDetail = db.C_Site_Config.Where(x => x.ESWBS == eswbs && x.SiteId == siteId).Select(x => new C_SiteConfigModel
                        {
                            ESWBS = x.ESWBS,
                            PESWBS = x.PESWBS,
                            Nomanclature = x.Nomanclature,
                            SMR_Code = x.SMR_Code,
                            PMS_No = x.PMS_No,
                            Qty = x.Qty,
                            CageId = x.CageId,
                            PartId = x.PartId,

                        }).FirstOrDefault();

                        if (eswbsDetail != null)
                        {
                            type = "success";
                            if (eswbsDetail.CageId != null && eswbsDetail.PartId != null)
                            {
                                var partDetails = db.tbl_Parts.Where(x => x.CageId == eswbsDetail.CageId && x.PartId == eswbsDetail.PartId).Select(x => new PartsModel
                                {
                                    Part_No = x.Part_No,
                                    PART_NAME = x.PART_NAME,
                                    CageCode = x.CageCode,
                                    NSN = x.NSN,
                                    PartTypeID = x.PartTypeID,
                                    MCAT_ID = x.MCAT_ID,
                                    LENGTH = x.LENGTH,
                                    WIDTH = x.WIDTH,
                                    HEIGHT = x.HEIGHT,
                                    WEIGHT = x.WEIGHT,
                                    MTBF = x.MTBF,
                                    MTTR = x.MTTR,
                                    PART_CHARACTERISTIC = x.PART_CHARACTERISTIC,
                                    PICTURE_FILE_NAME = x.PICTURE_FILE_NAME,
                                    File_Path = x.File_Path,
                                }).FirstOrDefault();

                                if (partDetails != null)
                                {
                                    if (!String.IsNullOrWhiteSpace(partDetails.PICTURE_FILE_NAME) && !String.IsNullOrWhiteSpace(partDetails.File_Path))
                                    {
                                        var imgPath = partDetails.File_Path + partDetails.PICTURE_FILE_NAME;
                                        var imgsrc = ImageConverter.GetImageBase64(imgPath);
                                        partDetails.base64Pic = imgsrc;
                                    }

                                    vm.PartsModel = partDetails;

                                    if (!String.IsNullOrWhiteSpace(partDetails.CageCode))
                                    {
                                        var CageDetails = db.tbl_Cage.Where(x => x.CageCode == partDetails.CageCode).Select(x => new CageModel
                                        {
                                            CageCode = x.CageCode,
                                            CageName = x.CageName,
                                        }).FirstOrDefault();

                                        vm.CageModel = CageDetails;
                                    };
                                }
                            }

                            vm.C_SiteConfigModel = eswbsDetail;
                        }
                        else
                        {
                            msg = "ESWBS Not Found !!!";
                        }
                    }
                    else
                    {
                        msg = "Required Data Missing !!!";
                    }
                }
            }
            catch(Exception ex)
            {
                type = "error";
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
                Exception(ex);

                msg = "Something Went Wrong !!!";
             
            }

            if (type == "success")
            {

                _logger.Log(LogLevel.Error, actionName + " :: Ended. ");
                return Json(new
                {
                    type = type,
                    msg = msg,
                    vm = vm
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                _logger.Log(LogLevel.Error, actionName + " :: Ended. with Error :  " + msg);
                return Json(new
                {
                    type = type,
                    msg = msg
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion  GetConfigData

        #region AddCHILD

        [HttpPost]
        [CustomAuthorization]
        [ValidateAjax]
        public ActionResult AddChild(C_SiteConfigModel model)
        {
            string actionName = "AddChild";
            _logger.Log(LogLevel.Trace, actionName + " :: started.");

            var type = "error";
            var msg = string.Empty;

            ConfigViewModel vm = new ConfigViewModel();
            try
            {
                if (ModelState.IsValid)
                {
                    var selectedSiteId = ((SessionHelper)Session[SessionKeys.SessionHelperInstance])?.SelectedSiteId;
                    if (model.SiteId == Convert.ToInt64(selectedSiteId))
                    {
                        using (var db = new WebAppDbContext())
                        {
                            if (model.SiteId != 0 && !String.IsNullOrWhiteSpace(model.ESWBS))
                            {
                                if (!db.C_Site_Config.Any(x => x.SiteId == model.SiteId && model.ESWBS == x.ESWBS))
                                {

                                    var obj = new C_Site_Config()
                                    {
                                        SiteId = model.SiteId,
                                        ESWBS = model.ESWBS,
                                        PESWBS = model.PESWBS,
                                        Nomanclature = model.Nomanclature,
                                        Qty = model.Qty,
                                        PMS_No = model.PMS_No,
                                        SMR_Code = model.SMR_Code,
                                        CageId = model.CageId,
                                        PartId = model.PartId,
                                        Title = model.ESWBS + " - " + model.Nomanclature,
                                    };

                                    db.C_Site_Config.Add(obj);
                                    db.SaveChanges();

                                    vm = GetSiteConfigTree(model.SiteId);

                                    type = "success";
                                    msg = "ESWBS Added Successfully !! ";
                                }
                                else
                                {
                                    msg = "Duplicate ESWBS : " + model.ESWBS;
                                }


                                _logger.Log(LogLevel.Trace, actionName + " :: Ended.");

                            }
                            else
                            {
                                _logger.Log(LogLevel.Error, actionName + " :: Ended. Required data missing. ");
                                msg = "Required Data Missing !!!";
                            }
                        }
                    }
                    else
                    {
                        msg = "Invalid Site Selected !!!";
                    }
                }
                else
                {
                    _logger.Log(LogLevel.Error, actionName + " :: Ended. Model validation failed ");
                    Alert("Something Went Wrong !!", NotificationType.error);

                    msg = "Validation failed !!!";
                }
            }
            catch (Exception ex)
            {
                type = "error";
                Exception(ex);
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());

                msg = "Something Went Wrong !!!";
            }

            if (type == "success")
            {
                return Json(new
                {
                    type = type,
                    msg = msg,
                    vm = vm
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    type = type,
                    msg = msg,
                    vm = model
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion AddChild 

        #region EditChild   

        [HttpPost]
        [CustomAuthorization]
        [ValidateAjax]
        public ActionResult EditChild(C_SiteConfigModel model)
        {
            var type = "error";
            var msg = string.Empty;
            string actionName = "EditChild";

            _logger.Log(LogLevel.Trace, actionName + " :: started.");

            ConfigViewModel vm = new ConfigViewModel();
            
            try
            {
                if (ModelState.IsValid)
                {
                    var selectedSiteId = ((SessionHelper)Session[SessionKeys.SessionHelperInstance])?.SelectedSiteId;

                    if (model.SiteId == Convert.ToInt64(selectedSiteId))
                    {
                        using (var db = new WebAppDbContext())
                        {
                            if (model.SiteId != 0 && !String.IsNullOrWhiteSpace(model.ESWBS))
                            {
                                if (db.C_Site_Config.Any(x => x.SiteId == model.SiteId && model.ESWBS == x.ESWBS))
                                {

                                    var obj = new C_Site_Config()
                                    {
                                        SiteId = model.SiteId,
                                        ESWBS = model.ESWBS,
                                        PESWBS = model.PESWBS,
                                        Nomanclature = model.Nomanclature,
                                        Qty = model.Qty,
                                        PMS_No = model.PMS_No,
                                        SMR_Code = model.SMR_Code,
                                        CageId = model.CageId,
                                        PartId = model.PartId,
                                        Title = model.ESWBS + " - " + model.Nomanclature,
                                    };

                                    db.Entry(obj).State = EntityState.Modified;
                                    db.SaveChanges();
 
                                    vm = GetSiteConfigTree(model.SiteId);

                                    type = "success";
                                    msg = "ESWBS Updated Successfully !! ";
                                }
                                else
                                {
                                    msg = "ESWBS : " + model.ESWBS + " Not Found.";
                                    _logger.Log(LogLevel.Trace, actionName + " :: "+ msg);
                                }


                                _logger.Log(LogLevel.Trace, actionName + " :: Ended.");

                            }
                            else
                            {
                                _logger.Log(LogLevel.Error, actionName + " :: Ended. Required data missing. ");
                                msg = "Required Data Missing !!!";
                            }
                        }
                    }
                    else
                    {
                        msg = "Invalid Site Selected !!!";
                    }
                }
                else
                {
                    _logger.Log(LogLevel.Error, actionName + " :: Ended. Model validation failed ");
                    Alert("Something Went Wrong !!", NotificationType.error);

                    msg = "Validation failed !!!";
                }
            }
            catch (Exception ex)
            {
                type = "error";
                Exception(ex);
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());

                msg = "Something Went Wrong !!!";
            }

            if (type == "success")
            {
                return Json(new
                {
                    type = type,
                    msg = msg,
                    vm = vm
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    type = type,
                    msg = msg,
                    vm = model
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion EditChild

        #region delChild
        [HttpPost]
        [CustomAuthorization]
        public ActionResult DeleteChild(int? siteId, string eswbs)
        {
            using (var db = new WebAppDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var vm = new ConfigViewModel();
                    try
                    {
                        if (siteId != null && eswbs != null)
                        {
                            var result = db.C_Site_Config.Where(x => x.SiteId == siteId && x.ESWBS == eswbs).SingleOrDefault();
                            if (result != null)
                            {
                                db.C_Site_Config.Remove(result);
                                db.SaveChanges();
                                transaction.Commit();

                                if (siteId != 0)
                                {
                                    vm = GetSiteConfigTree(siteId);
                                }
                            }
                        }

                        // Alert("Record Deleted Sucessfully!!!", NotificationType.success);
                        return Json(new
                        {
                            msg = "Record Deleted Sucessfully!!!",
                            vm = vm
                        }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Exception(ex);
                        Alert("Their is something went wrong!!!", NotificationType.error);
                        return Json(vm);
                    }
                }
            }
        }
        #endregion delChild

        #endregion ConfigurationEquipment


        #region ConfigurationSystem
        [CustomAuthorization]
        public ActionResult Index()
        {
            string actionName = "Index";
            _logger.Log(LogLevel.Error, actionName + " :: started.");

            var vm = new ConfigViewModel();
            try
            {
                SessionKeys.LoadTablesInSession(SessionKeys.AllUnits, "", "");
                vm._tbl_Unit = ((List<ClassLibrary.Models.tbl_Unit>)Session[SessionKeys.AllUnits]);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());
            }

            _logger.Log(LogLevel.Error, actionName + " :: Ended.");

            return View("Index", vm);
        }

        [CustomAuthorization]
        public JsonResult GetConfig(int unitId)
        {
            string actionName = "GetConfig";
            _logger.Log(LogLevel.Error, actionName + " :: started.");

            ConfigViewModel vm = new ConfigViewModel();
            try
            {
                using (var db = new WebAppDbContext())
                {
                    List<TreeViewNode> nodesMaster = new List<TreeViewNode>();
                    foreach (C_Config_Master c in db.C_Config_Master)
                    {
                        if (c.PESWBS == "0") { c.PESWBS = "#"; }
                        nodesMaster.Add(new TreeViewNode { id = c.ESWBS.ToString(), parent = c.PESWBS.ToString(), text = c.Title, Name = c.Nomanclature });
                    }

                    vm.JsonMasterConfig = (new JavaScriptSerializer()).Serialize(nodesMaster);

                    if (unitId > 0)
                    {
                        List<TreeViewNode> nodesSite = new List<TreeViewNode>();
                        foreach (C_Site_Config c in db.C_Site_Config.Where(x => x.SiteId == unitId))
                        {
                            if (c.PESWBS == "0") { c.PESWBS = "#"; }
                            if (c.ESWBS.Length < 5)
                            {
                                nodesSite.Add(new TreeViewNode { id = c.ESWBS.ToString(), parent = c.PESWBS.ToString(), text = (c.ESWBS + " - " + c.Nomanclature), Name = c.Nomanclature });
                            }
                        }

                        vm.JsonSiteConfig = (new JavaScriptSerializer()).Serialize(nodesSite);

                    }

                    _logger.Log(LogLevel.Error, actionName + " :: Ended.");
                    return Json(vm, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException?.ToString());

                Exception(ex);
                return Json(unitId, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [CustomAuthorization]
        public JsonResult GetSiteConfig(string selectedItems, int unitId)
        {
            using (var db = new WebAppDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    List<TreeViewNode> parentItems = new List<TreeViewNode>();

                    var vm = new ConfigViewModel();
                    try
                    {
                        if (unitId != 0)
                        {
                            List<TreeViewNode> items = (new JavaScriptSerializer()).Deserialize<List<TreeViewNode>>(selectedItems);

                            List<string> newListOfParent = new List<string>();

                            foreach (var temp in items.Select(x => x.parents))
                            {
                                newListOfParent = newListOfParent.Concat(temp).ToList();
                            }
                            newListOfParent = newListOfParent.Distinct().ToList();

                            foreach (var selected in items)
                            {
                                if (selected.parent == "#")
                                {
                                    selected.parent = "0";
                                }

                                if (!db.C_Site_Config.Where(x => x.SiteId == unitId).Any(x => x.ESWBS == selected.id))
                                {
                                    var model = new C_Site_Config
                                    {
                                        SiteId = unitId,
                                        ESWBS = selected.id,
                                        PESWBS = selected.parent,
                                        Title = selected.text,
                                        Nomanclature = selected.text.Substring(selected.text.IndexOf('-') + 2),
                                    };
                                    db.C_Site_Config.Add(model);
                                }
                            }

                            foreach (var selected in newListOfParent)
                            {
                                if (!items.Where(x => x.id == selected).Any() && selected != "#")
                                {
                                    C_Config_Master master = db.C_Config_Master.Where(x => x.ESWBS == selected).FirstOrDefault();

                                    if (!db.C_Site_Config.Where(x => x.SiteId == unitId).Any(x => x.ESWBS == selected))
                                    {
                                        var model = new C_Site_Config
                                        {
                                            SiteId = unitId,
                                            ESWBS = selected,
                                            PESWBS = master.PESWBS,
                                            Title = master.Nomanclature,
                                            Nomanclature = master.Nomanclature,
                                        };
                                        db.C_Site_Config.Add(model);
                                    }
                                }
                            }

                            db.SaveChanges();
                            transaction.Commit();
                            vm = GetSiteConfigTree(unitId);
                        }
                        //Serialize to JSON string.                
                        Alert("Data Saved Sucessfully!!!", NotificationType.success);
                        return Json(vm, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Exception(ex);
                        Alert("Their is something went wrong!!!", NotificationType.error);
                        return Json(vm, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        #endregion ConfigurationSystem


        #region EquipmentRequest    
        [CustomAuthorization]
        public ActionResult RequestForm()
        {
            using (var db = new WebAppDbContext())
            {
                var vm = new ConfigViewModel
                {
                    _tbl_Country = db.tbl_Country.ToList(),
                    _tbl_Currency = db.tbl_Currency.ToList()

                };
                return View("RequestForm", vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequestForm(tbl_Request tbl_Request, FileHelper FileHelper)
        {
            using (var db = new WebAppDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (!ModelState.IsValid)
                        {
                            var vm = new ConfigViewModel
                            {
                                _tbl_Country = db.tbl_Country.ToList(),
                                _tbl_Currency = db.tbl_Currency.ToList()
                            };
                            return View("RequestForm", vm);
                        }

                        if (FileHelper.File != null)
                        {
                            string savaPath = "";
                            string tempPath = "";
                            tempPath = "~/Images/Config";
                            savaPath = Server.MapPath(tempPath);
                            if (!Directory.Exists(savaPath))
                            {
                                Directory.CreateDirectory(savaPath);
                            }

                            var fileName = Path.GetFileNameWithoutExtension(FileHelper.File.FileName);
                            var fileExtension = Path.GetExtension(FileHelper.File.FileName);
                            string image = @"\" + fileName + DateTime.Now.Ticks + fileExtension;

                            tbl_Request.PICTURE_FILE_NAME = image;
                            tbl_Request.FileExtension = fileExtension;
                            tbl_Request.File_Path = savaPath;
                            FileHelper.File.SaveAs(savaPath + image);
                        }
                        // Get Current user Id
                        var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                        tbl_Request.CreatedByUser = userId;
                        tbl_Request.ModifiedByUser = userId;

                        //Get Current Date & Time.
                        tbl_Request.CreatedOnDate = DateTime.Now;
                        tbl_Request.ModifiedOnDate = DateTime.Now;
                        tbl_Request.Status = 1;


                        db.tbl_Request.Add(tbl_Request);
                        db.SaveChanges();

                        tbl_RequestLog req = new tbl_RequestLog();
                        req.RequestId = tbl_Request.RequestId;
                        req.Status = 1;
                        req.Remarks = "NA";
                        req.CurrentDateTime = DateTime.Now;
                        req.UserId = userId;

                        db.tbl_RequestLog.Add(req);
                        db.SaveChanges();


                        transaction.Commit();
                        Alert("Request Submitted Sucessfully!!!", NotificationType.success);
                        return RedirectToAction("RequestForm");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Exception(ex);
                        Alert("Their is something went wrong!!!", NotificationType.error);
                        return RedirectToAction("RequestForm");
                    }
                }
            }
        }

        public ActionResult Inbox()
        {
            using (var db = new WebAppDbContext())
            {
                var vm = new ConfigViewModel
                {
                    tbl_Request_list = db.tbl_Request.Where(x => x.Status == 1).ToList(),
                    _tbl_Country = db.tbl_Country.ToList(),
                    _tbl_Currency = db.tbl_Currency.ToList()
                };
                return View("Inbox", vm);
            }
        }

        #endregion EquipmentRequest


        [HttpPost]
        [CustomAuthorization]
        public ActionResult GetSelectedRequestForm(int? reqId)
        {
            using (var db = new WebAppDbContext())
            {
                var vm = new ConfigViewModel();
                try
                {
                    if (reqId != null)
                    {
                        var result = db.tbl_Request.Where(x => x.RequestId == reqId).Select(x => new RequestModel
                        {
                            RequestId = x.RequestId,
                            Part_No = x.Part_No,
                            Equipment_Name = x.Equipment_Name,
                            PMS_No = x.PMS_No,
                            NSN = x.NSN,
                            Country = x.Country,
                            ManufacturerName = x.ManufacturerName,
                            CageCode = x.CageCode,
                            Qty = x.Qty,
                            UNIT_PRICE = x.UNIT_PRICE,
                            MTBF = x.MTBF,
                            BRF = x.BRF,
                            LOCATIONS = x.LOCATIONS,
                            SERIAL_NO = x.SERIAL_NO,
                            DateOfInstallation = x.DateOfInstallation,
                            LENGTH = x.LENGTH,
                            WIDTH = x.WIDTH,
                            HEIGHT = x.HEIGHT,
                            DIAMETER = x.DIAMETER,
                            WEIGHT = x.WEIGHT,
                            JUSTIFICATION = x.JUSTIFICATION,
                            DETAIL_REMARKS = x.DETAIL_REMARKS,
                            NAME = x.NAME,
                            DESIGNATION = x.DESIGNATION,
                            CONTACT = x.CONTACT,

                        }).FirstOrDefault();
                        vm.RequestModel = result;
                    }

                    // Alert("Record Deleted Sucessfully!!!", NotificationType.success);
                    return Json(vm, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {

                    Exception(ex);
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return Json(vm);
                }
            }
        }

        public ActionResult RequestReturn()
        {
            using (var db = new WebAppDbContext())
            {
                var vm = new ConfigViewModel
                {
                    tbl_Request_list = db.tbl_Request.Where(x => x.Status == 3).ToList(),
                    _tbl_Country = db.tbl_Country.ToList(),
                    _tbl_Currency = db.tbl_Currency.ToList()
                };
                return View("RequestReturn", vm);
            }
        }

        [HttpPost]
        [CustomAuthorization]
        public ActionResult ReturnRequest(int? reqId, string remarks)
        {
            using (var db = new WebAppDbContext())
            {
                try
                {
                    if (reqId != null)
                    {
                        var result = db.tbl_Request.Where(x => x.RequestId == reqId).FirstOrDefault();
                        if (result != null)
                        {
                            result.Status = 3;
                            db.Entry(result).Property(x => x.Status).IsModified = true;
                  
                            db.SaveChanges();

                        }

                        tbl_RequestLog req = new tbl_RequestLog();
                        req.RequestId = reqId;
                        req.Status = 3;
                        req.Remarks = remarks;
                        req.CurrentDateTime = DateTime.Now;
                        req.UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();

                        db.tbl_RequestLog.Add(req);
                        db.SaveChanges();
                    }

                    Alert("Request return Sucessfully!!!", NotificationType.success);
                    return RedirectToAction("Inbox");
                }
                catch (Exception ex)
                {

                    Exception(ex);
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return RedirectToAction("Inbox");
                }
            }
        }

        [HttpPost]
        [CustomAuthorization]
        public ActionResult RegisterRequest(int? reqId)
        {
            using (var db = new WebAppDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (reqId != null)
                        {
                            var result = db.tbl_Request.Where(x => x.RequestId == reqId).FirstOrDefault();
                            if (result != null)
                            {
                                result.Status = 2;
                                db.Entry(result).State = EntityState.Modified;
                                db.SaveChanges();

                            }

                            tbl_RequestLog req = new tbl_RequestLog();
                            req.RequestId = reqId;
                            req.Status = 2;
                            req.Remarks = "Registered Successfully!!!";
                            req.CurrentDateTime = DateTime.Now;
                            req.UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();

                            db.tbl_RequestLog.Add(req);
                            db.SaveChanges();
                            transaction.Commit();
                        }

                        // Alert("Record Deleted Sucessfully!!!", NotificationType.success);
                        return RedirectToAction("Inbox");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Exception(ex);
                        Alert("Their is something went wrong!!!", NotificationType.error);
                        return RedirectToAction("Inbox");
                    }
                }
            }
        }

        [HttpPost]
        [CustomAuthorization]
        public ActionResult ReturnRequestSubmit(int? reqId)
        {
            using (var db = new WebAppDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var vm = new ConfigViewModel();
                    try
                    {
                        if (reqId != null)
                        {
                            var result = db.tbl_Request.Where(x => x.RequestId == reqId).FirstOrDefault();
                            if (result != null)
                            {
                                result.Status = 1;
                                db.SaveChanges();
                                transaction.Commit();
                            }


                            tbl_RequestLog req = new tbl_RequestLog();
                            req.RequestId = reqId;
                            req.Status = 1;
                            req.Remarks = "Again Submitted";
                            req.CurrentDateTime = DateTime.Now;
                            req.UserId = System.Web.HttpContext.Current.User.Identity.GetUserId();

                            db.tbl_RequestLog.Add(req);
                            db.SaveChanges();
                        }

                        // Alert("Record Deleted Sucessfully!!!", NotificationType.success);
                        return RedirectToAction("RequestReturn");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Exception(ex);
                        Alert("Their is something went wrong!!!", NotificationType.error);
                        return RedirectToAction("RequestReturn");
                    }
                }
            }
        }

        #region utilities
        public ConfigViewModel GetSiteConfigTree(int? Id)
        {
            using (var db = new WebAppDbContext())
            {
                var viewModel = new ConfigViewModel();
                List<TreeViewNode> nodesSite = new List<TreeViewNode>();
                foreach (C_Site_Config c in db.C_Site_Config.Where(x => x.SiteId == Id))
                {
                    if (c.PESWBS == "0") { c.PESWBS = "#"; }
                    nodesSite.Add(new TreeViewNode { id = c.ESWBS.ToString(), parent = c.PESWBS.ToString(), text = (c.ESWBS + " - " + c.Nomanclature), Name = c.Nomanclature });
                }
                //Serialize to JSON string.
                viewModel.JsonSiteConfig = (new JavaScriptSerializer()).Serialize(nodesSite);
                return viewModel;
            }
        }
        #endregion utilities
    }
}
