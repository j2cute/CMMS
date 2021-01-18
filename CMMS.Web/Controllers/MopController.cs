using ClassLibrary.Common;
using ClassLibrary.Models;
using ClassLibrary.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication.Helpers;
using static ClassLibrary.Common.Enums;

namespace WebApplication.Controllers
{
    //7796 7566

    //[Authorization]
    public partial class MopController : BaseController
    {

        private WebAppDbContext db = new WebAppDbContext();
        // GET: Mop
        public ActionResult Index()
        {
            var vm = new MopViewModels()
            {
                _M_PMS = db.M_PMS.ToList(),
                _tbl_Unit = db.tbl_Unit.ToList(),
            };
            return View("Index", vm);
        }

        [HttpPost]
        public JsonResult GetSiteConfig(int? siteId)
        {
            var vm = new ConfigViewModel();
            try
            {
                if (siteId != null)
                {
                    List<TreeViewNode> nodesSite = new List<TreeViewNode>();
                    foreach (C_Site_Config c in db.C_Site_Config.Where(x => x.SiteId == siteId))
                    {
                        if (c.PESWBS == "0") { c.PESWBS = "#"; }
                        nodesSite.Add(new TreeViewNode { id = c.ESWBS.ToString(), parent = c.PESWBS.ToString(), text = (c.ESWBS + " - " + c.Nomanclature), Name = c.Nomanclature });
                    }
                    vm.JsonSiteConfig = (new JavaScriptSerializer()).Serialize(nodesSite);
                }
                //Serialize to JSON string.                
                //Alert("Data Saved Sucessfully!!!", NotificationType.success);
                return Json(vm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Exception(ex);
                Alert("Their is something went wrong!!!", NotificationType.error);
                return Json(vm, JsonRequestBehavior.AllowGet);
            }

        }


        #region GetMOPData
        [HttpPost]
        public ActionResult GetMOPData(int? SiteId, string eswbs)
        {
            try
            {
                var vm = new MopViewModels();
                if (SiteId != null && eswbs != null)
                {
                    vm = GetMopData(SiteId, eswbs);
                }

                else
                {
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return Json(SiteId, JsonRequestBehavior.AllowGet);
                }
                return Json(new { model = vm.M_MOPModel_List ,pmsNo = vm.pmsNo}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Exception(ex);
                Alert("Their is something went wrong!!!", NotificationType.error);
                return Json(SiteId, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion  GetMOPData


        public ActionResult GetSelectedData(int? siteId, string pmsNo, string mopNo)
        {
            var vm = new MopViewModels();
            try
            {
                if (pmsNo != null && mopNo != null && siteId !=null)
                {
                    var result = db.M_MOP.Where(x => x.PMS_No == pmsNo && x.MOP_No == mopNo && x.SiteId==siteId).Select(x => new M_MOPModel
                    {
                        SiteId = x.SiteId,
                        PMS_No = x.PMS_No,
                        MOP_No = x.MOP_No,
                        MOP_Desc = x.MOP_Desc,
                        By_Whom = x.By_Whom,
                        Period = x.Period,
                        Periodicity = x.Periodicity,
                        Doc = x.Doc,
                        Task_Procedure = x.Task_Procedure,
                        Safety_Precautions = x.Safety_Precautions,
                    }).FirstOrDefault();
                    vm.M_MOPModel = result;
                }
                else
                {
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return Json(vm, JsonRequestBehavior.AllowGet);
                }
                return Json(vm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Exception(ex);
                Alert("Their is something went wrong!!!", NotificationType.error);
                return Json(vm, JsonRequestBehavior.AllowGet);
            }

        }
        #region AddChild   
        [ValidateAjax]
        [HttpPost]
        public ActionResult AddMOP(M_MOP model, int? SiteId, string eswbs)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                MopViewModels vm = new MopViewModels();
                try
                {
                    if (!ModelState.IsValid)
                    {

                        Alert("Their is something went wrong!!!", NotificationType.error);
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                    if (model.PMS_No != null && model.MOP_No != null && model.SiteId != 0)
                    {
                        db.M_MOP.Add(model);
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    if (SiteId != null && eswbs != null)
                    {
                        vm = GetMopData(SiteId, eswbs);
                    }
                    //  Alert("Data Saved Sucessfully!!!", NotificationType.success);
                    return Json(new { msg = "Data Saved Sucessfully!!!", model = vm.M_MOPModel_List}, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Exception(ex);
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return Json(vm.M_MOPModel_List);
                }
            }

        }
        #endregion EditChild


        #region EditChild  
        [ValidateAjax]
        [HttpPost]
        public ActionResult EditMOP(M_MOPModel model, int? SiteId, string eswbs)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                MopViewModels vm = new MopViewModels();
                try
                {
                    if (!ModelState.IsValid)
                    {

                        Alert("Their is something went wrong!!!", NotificationType.error);
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                    if (model.PMS_No != null && model.MOP_No != null && model.SiteId != 0)
                    {
                        var obj = new M_MOP()
                        {
                            SiteId=model.SiteId,
                            PMS_No = model.PMS_No,
                            MOP_No = model.MOP_No,
                            MOP_Desc = model.MOP_Desc,
                            By_Whom = model.By_Whom,
                            Periodicity = model.Periodicity,
                            Period = model.Period,
                            Doc = model.Doc,
                            Task_Procedure = model.Task_Procedure,
                            Safety_Precautions = model.Safety_Precautions,

                        };
                        db.Entry(obj).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    if (SiteId != null && eswbs != null)
                    {
                        vm = GetMopData(SiteId, eswbs);
                    }
                    //  Alert("Data Saved Sucessfully!!!", NotificationType.success);
                    // return Json(vm.M_MOPModel_List, JsonRequestBehavior.AllowGet);
                    return Json(new { msg = "Data Updated Sucessfully!!!", model = vm.M_MOPModel_List, pmsNo = vm.pmsNo }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Exception(ex);
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return Json(vm.M_MOPModel_List);
                }
            }

        }
        #endregion EditChild


        #region delChild
        [HttpPost]
        public ActionResult DeleteMOP(int? s_siteId, string pmsNo, string mopNo, int? SiteId, string eswbs)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var vm = new MopViewModels();
                try
                {
                    if (pmsNo != null && mopNo != null && s_siteId != null)
                    {
                        var result = db.M_MOP.Where(x => x.PMS_No == pmsNo && x.MOP_No == mopNo && x.SiteId == s_siteId).SingleOrDefault();
                        if (result != null)
                        {
                            db.M_MOP.Remove(result);
                            db.SaveChanges();
                            transaction.Commit();
                        }
                    }
                    if (SiteId != null && eswbs != null)
                    {
                        vm = GetMopData(SiteId, eswbs);
                    }
                    // Alert("Record Deleted Sucessfully!!!", NotificationType.success);
                    return Json(new { msg = "Record Deleted Sucessfully!!!", model = vm.M_MOPModel_List, pmsNo = vm.pmsNo }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Exception(ex);
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return Json(vm.M_MOPModel_List);
                }
            }
        }
        #endregion delChild

        #region MOP_ITEMS
        public ActionResult TaskMaterialList()
        {
            var vm = new MopViewModels()
            {
                _tbl_Unit = db.tbl_Unit.ToList(),
                _M_PMS = db.M_PMS.ToList(),
                tbl_Parts_list = db.tbl_Parts.Where(x => x.Status == "Active").ToList(),
            };
            return View("TaskMaterialList", vm);
        }


        [HttpPost]
        public ActionResult GetMOP(string PMS_No)
        {
            List<M_MOP> M_MOP = db.M_MOP.Where(x => x.PMS_No == PMS_No).ToList();
            ViewBag.M_MOP = new SelectList(M_MOP, "MOP_No", "MOP_No");
            return PartialView("_DisplayMOPs");
        }

        [HttpPost]
        public ActionResult TaskMaterialListData(int? siteId, string pmsNo, string mopNo)
        {
            var vm = new MopViewModels();
            try
            {
                if (siteId!= null && pmsNo != null && mopNo != null)
                {
                    vm = GetMopItemData(siteId, pmsNo, mopNo);
                }
                return Json(  new { model = vm.M_MOP_ItemsModelList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Exception(ex);
               // Alert("Their is something went wrong!!!", NotificationType.error);
                return Json(vm);
            }

        }
        #endregion MOP_ITEMS

 


        #region GetSelectedMopItemData

        public ActionResult GetMopItemSelectedData(string pmsNo, string mopNo)
        {
            var vm = new MopViewModels();
            try
            {
                if (pmsNo != null && mopNo != null)
                {
                    var result = db.M_MOP.Where(x => x.PMS_No == pmsNo && x.MOP_No == mopNo).Select(x => new M_MOPModel
                    {
                        PMS_No = x.PMS_No,
                        MOP_No = x.MOP_No,
                        MOP_Desc = x.MOP_Desc,
                        By_Whom = x.By_Whom,
                        Period = x.Period,
                        Periodicity = x.Periodicity,
                        Doc = x.Doc,
                        Task_Procedure = x.Task_Procedure,
                        Safety_Precautions = x.Safety_Precautions,
                    }).FirstOrDefault();
                    vm.M_MOPModel = result;
                }
                else
                {
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return Json(vm, JsonRequestBehavior.AllowGet);
                }
                return Json(vm, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Exception(ex);
                Alert("Their is something went wrong!!!", NotificationType.error);
                return Json(vm, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion GetSeleectedMopItemData

        #region AddMOPItem  
        [ValidateAjax]
        [HttpPost]
        public ActionResult AddMOPItem(M_MOP_ITEMS model, int? siteId, string pmsNo, string mopNo)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                MopViewModels vm = new MopViewModels();
                try
                {
                    if (!ModelState.IsValid)
                    {
                        Alert("Their is something went wrong!!!", NotificationType.error);
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                    if (model != null)
                    {
                        db.M_MOP_ITEMS.Add(model);
                        db.SaveChanges();
                        transaction.Commit();
                    }

                    if (siteId != null && pmsNo != null && mopNo != null)
                    {
                        vm = GetMopItemData(siteId, pmsNo, mopNo);
                    }

                    //  Alert("Data Saved Sucessfully!!!", NotificationType.success);
                    return Json(new { msg = "Data Saved Sucessfully!!!", model = vm.M_MOP_ItemsModelList }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Exception(ex);
                    //Alert("Their is something went wrong!!!", NotificationType.error);
                    return Json(vm);
                }
            }

        }
        #endregion AddMOPItem


        #region EditChild   
        [ValidateAjax]
        [HttpPost]
        public ActionResult EditMOPItem(M_MOP_ItemsModel model, int? siteId, string pmsNo, string mopNo)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                MopViewModels vm = new MopViewModels();
                try
                {
                    if (!ModelState.IsValid)
                    {
                        Alert("Their is something went wrong!!!", NotificationType.error);
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                    if (model != null)
                    {
                        var obj = new M_MOP_ITEMS()
                        {
                            MOP_ItemsId = model.MOP_ItemsId,
                            SiteId=Convert.ToInt32(model.SiteId),
                            SR_Qty = model.SR_Qty,
                            PMS_No = model.PMS_No,
                            MOP_No = model.MOP_No,
                            Part_No = model.Part_No,
                        };
                        if (model.NewSelectedPart_No != null)
                        {
                            obj.Part_No = model.NewSelectedPart_No.Trim();
                        }
                        db.Entry(obj).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    transaction.Commit();

                    if (siteId != null && pmsNo != null && mopNo != null)
                    {
                        vm = GetMopItemData(siteId, pmsNo, mopNo);
                    }

                    //  Alert("Data Saved Sucessfully!!!", NotificationType.success);
                    return Json(new { msg = "Record Updated Sucessfully!!!", model = vm.M_MOP_ItemsModelList }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Exception(ex);
                    //Alert("Their is something went wrong!!!", NotificationType.error);
                    return Json(vm);
                }
            }
        }
        #endregion EditChild



        #region delChild
        [HttpPost]
        public ActionResult DeleteMOPItem(int? mopItemId, int? siteId, string pmsNo, string mopNo)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var vm = new MopViewModels();
                try
                {
                    if (mopItemId != null)
                    {
                        var result = db.M_MOP_ITEMS.Where(x => x.MOP_ItemsId == mopItemId).SingleOrDefault();
                        if (result != null)
                        {
                            db.M_MOP_ITEMS.Remove(result);
                            db.SaveChanges();
                            transaction.Commit();
                        }
                    }
                    if (siteId !=null && pmsNo != null && mopNo != null)
                    {
                        vm = GetMopItemData(siteId, pmsNo, mopNo);
                    }
                    // Alert("Record Deleted Sucessfully!!!", NotificationType.success);
                    return Json(new { msg = "Record Deleted Sucessfully!!!", model = vm.M_MOP_ItemsModelList }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Exception(ex);
                    //Alert("Their is something went wrong!!!", NotificationType.error);
                    return Json(vm);
                }
            }

        }
        #endregion delChild

        #region utility


        public MopViewModels GetMopData(int? SiteId, string eswbs)
        {
            var vm = new MopViewModels();
            if (SiteId != null && eswbs != null)
            {
                var data = db.C_Site_Config.Where(x => x.SiteId == SiteId && x.ESWBS == eswbs).FirstOrDefault();
                

                if (data.PMS_No != null)
                {
                    var result = db.M_MOP.Where(x => x.PMS_No == data.PMS_No).ToList().Select(x => new M_MOPModel
                    {
                        SiteId=x.SiteId,
                        PMS_No = x.PMS_No,
                        MOP_No = x.MOP_No,
                        MOP_Desc = x.MOP_Desc,
                        By_Whom = x.By_Whom,
                        PeriodMonth = x.Periodicity + " " + x.Period,
                        mmsDoc = "MMS " + x.Doc,
                    });
                    vm.M_MOPModel_List = result.ToList();
                    vm.pmsNo = data.PMS_No;
                }

            }
            return vm;
        }

        public MopViewModels GetMopItemData(int? siteId, string pmsNo, string mopNo)
        {
            var vm = new MopViewModels();
            if (pmsNo != null && mopNo != null)
            {
                var mopItems = db.M_MOP_ITEMS.Where(x => x.PMS_No == pmsNo && x.MOP_No == mopNo && x.SiteId ==siteId).ToList().Select(x => new M_MOP_ItemsModel
                {
                    MOP_ItemsId = x.MOP_ItemsId,
                    SiteId= x.SiteId,
                    PMS_No = x.PMS_No,
                    MOP_No = x.MOP_No,
                    SR_Qty = x.SR_Qty,
                    Part_No = x.Part_No,

                });

                vm.M_MOP_ItemsModelList = mopItems.ToList();
            }
            return vm;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
      
        #endregion utility
    }

}