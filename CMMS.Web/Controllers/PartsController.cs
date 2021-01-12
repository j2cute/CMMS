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
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication.Helpers;
using static ClassLibrary.Common.Enums;

namespace WebApplication.Controllers
{
    //[Authorization]
    public class PartsController : BaseController
    {
        private WebAppDbContext db = new WebAppDbContext();
        // GET: Parts

        [HttpPost]
        public JsonResult PartNoCheck(string CageCode, string PartNo)
        {
            var SearchData = db.tbl_Parts.Where(x => x.Part_No == PartNo && x.CageCode == CageCode).SingleOrDefault();
            if (SearchData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
        public ActionResult Index()
        {
            var vm = new PartsViewModels
            {
                //tbl_Parts_list = db.tbl_Parts.Where(x => x.Status == "Active").ToList(),
                cageActiveCount = db.tbl_Cage.Where(x => x.Status == "Active").Count(),
                partsCount = db.tbl_Parts.Count(),
                partTypeCount = db.tbl_PartType.Count(),
            };

            List<GraphData> data = new List<GraphData>();
            foreach (var item in vm._tbl_PartType = db.tbl_PartType.ToList())
            {
                GraphData details = new GraphData();
                details.label = item.PartName;
                var id = item.PartTypeID;
                details.value = db.tbl_Parts.Where(x => x.PartTypeID == id && x.Status == "Active").Count();
                data.Add(details);
            }
            vm.dataList = data;
            return View(vm);
        }


        public ActionResult LoadPartsCount()
        {
            var output =  db.tbl_Parts.Count();
            return Json(output, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult LoadData(int length, int start)
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
                    sortExpression = (x => x.CageCode)
                        ;
                    break;
                case "Part_No":
                    sortExpression = (x => x.Part_No);
                    break;
                case "PART_NAME":
                    sortExpression = (x => x.PART_NAME);
                    break;
                case "NSN":
                    sortExpression = (x => x.NSN);
                    break;
                case "PartTypeName":
                    sortExpression = (x => x.tbl_PartType.PartName);
                    break;
                default:
                    sortExpression = (x => x.CageCode);
                    break;
            }


            int filterrecord;
            List<PartsModel> Partdata = null;
            if (sortColumnDir == "asc")
            {
                Partdata = db.tbl_Parts.Where(x => x.Status == "Active" && (x.CageCode.Contains(searchvalue)
                || x.Part_No.Contains(searchvalue)
                || x.PART_NAME.Contains(searchvalue)
                || x.NSN.Contains(searchvalue)
                || x.tbl_PartType.PartName.Contains(searchvalue))
                )
                .OrderBy(sortExpression).Skip(start).Take(length)
                .Select(x => new PartsModel
                {
                    PartId = x.PartId,
                    CageCode = x.CageCode,
                    Part_No = x.Part_No,
                    PART_NAME = x.PART_NAME,
                    NSN = x.NSN,
                    PartTypeName = x.tbl_PartType.PartName
                }).ToList();
            }
            else
            {
                Partdata = db.tbl_Parts.Where(x => x.Status == "Active" && (x.CageCode.Contains(searchvalue)
            || x.Part_No.Contains(searchvalue)
            || x.PART_NAME.Contains(searchvalue)
            || x.NSN.Contains(searchvalue)
            || x.tbl_PartType.PartName.Contains(searchvalue))
            )
            .OrderByDescending(sortExpression).Skip(start).Take(length)
            .Select(x => new PartsModel
            {
                PartId = x.PartId,
                CageCode = x.CageCode,
                Part_No = x.Part_No,
                PART_NAME = x.PART_NAME,
                NSN = x.NSN,
                PartTypeName = x.tbl_PartType.PartName
            }).ToList();
            }
            if (searchvalue != "")
            { filterrecord = Partdata.Count(); }
            else { filterrecord = db.tbl_Parts.Where(x => x.Status == "Active").Count(); }
            var recordcount = db.tbl_Parts.Where(x => x.Status == "Active").Count();

            List<PartsModel> dataItems = new List<PartsModel>();
            foreach (var item in Partdata)
            {
                dataItems.Add(item);
            }

            var response = new { recordsTotal = recordcount, recordsFiltered = filterrecord, data = dataItems };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index2()
        {
            var vm = new PartsViewModels
            {
                tbl_Parts_list = db.tbl_Parts.Where(x => x.Status == "InActive").ToList(),
            };
            return View(vm);
        }
        // GET: Parts/Edit/id
        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartsViewModels objPartsViewModels = GetParts(id);
            return PartialView("_Details", objPartsViewModels);
        }

        public ActionResult Create()
        {
            var vm = new PartsViewModels
            {
                _tbl_Cage = db.tbl_Cage.ToList(),
                _tbl_PartType = db.tbl_PartType.ToList(),
                _tbl_MCAT = db.tbl_MCAT.ToList(),
                _tbl_Currency = db.tbl_Currency.ToList(),
            };
            return PartialView("_Create", vm);
        }

        //POST: Parts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tbl_Parts tbl_Parts, FileHelper FileHelper)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        var vm = new PartsViewModels
                        {
                            _tbl_Cage = db.tbl_Cage.ToList(),
                            _tbl_PartType = db.tbl_PartType.ToList(),
                            _tbl_MCAT = db.tbl_MCAT.ToList(),
                            _tbl_Currency = db.tbl_Currency.ToList(),
                        };
                        return PartialView("_Create", vm);
                    }

                    if (FileHelper.File != null)
                    {
                        string savaPath = "";
                        string tempPath = "";
                        tempPath = "~/Images/Parts";
                        savaPath = Server.MapPath(tempPath);
                        if (!Directory.Exists(savaPath))
                        {
                            Directory.CreateDirectory(savaPath);
                        }

                        var fileName = Path.GetFileNameWithoutExtension(FileHelper.File.FileName);
                        var fileExtension = Path.GetExtension(FileHelper.File.FileName);
                        string image = @"\" + fileName + DateTime.Now.Ticks + fileExtension;

                        tbl_Parts.PICTURE_FILE_NAME = image;
                        tbl_Parts.FileExtension = fileExtension;
                        tbl_Parts.File_Path = savaPath;
                        FileHelper.File.SaveAs(savaPath + image);
                    }

                    var cage = db.tbl_Cage.Where(x => x.CageCode == tbl_Parts.CageCode).FirstOrDefault();
                    tbl_Parts.CageId = cage.CageId;
                    // Get Current user Id
                    var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    tbl_Parts.CreatedByUser = userId;
                    tbl_Parts.ModifiedByUser = userId;

                    //Get Current Date & Time.
                    tbl_Parts.CreatedOnDate = DateTime.Now;
                    tbl_Parts.ModifiedOnDate = DateTime.Now;
                    tbl_Parts.Status = "Active";


                    db.tbl_Parts.Add(tbl_Parts);
                    db.SaveChanges();

                    transaction.Commit();
                    Alert("Data Saved Sucessfully!!!", NotificationType.success);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                    transaction.Rollback();
                    Exception(ex);
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return RedirectToAction("Index");
                }
            }
        }
        // GET: Parts/Edit/id
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PartsViewModels objPartsViewModels = GetParts(id);
            return PartialView("_Edit", objPartsViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, tbl_Parts tbl_Parts, FileHelper FileHelper, PartsViewModels vm)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (id == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    if (!ModelState.IsValid)
                    {
                        PartsViewModels objPartsViewModels = GetParts(id);
                        return PartialView("_Edit", objPartsViewModels);
                    }

                    if (FileHelper.File != null)
                    {

                        string savaPath = "";
                        string tempPath = "";
                        tempPath = "~/Images/Parts";
                        savaPath = Server.MapPath(tempPath);
                        if (!Directory.Exists(savaPath))
                        {
                            Directory.CreateDirectory(savaPath);
                        }

                        if (tbl_Parts.File_Path != null && tbl_Parts.PICTURE_FILE_NAME != null)
                        {
                            var imgPath = tbl_Parts.File_Path + tbl_Parts.PICTURE_FILE_NAME;

                            if (System.IO.File.Exists(imgPath))
                            {
                                System.IO.File.Delete(imgPath);
                            }
                        }
                        var fileName = Path.GetFileNameWithoutExtension(FileHelper.File.FileName);
                        var fileExtension = Path.GetExtension(FileHelper.File.FileName);
                        string image = @"\" + fileName + DateTime.Now.Ticks + fileExtension;

                        tbl_Parts.PICTURE_FILE_NAME = image;
                        tbl_Parts.FileExtension = fileExtension;
                        tbl_Parts.File_Path = savaPath;
                        FileHelper.File.SaveAs(savaPath + image);
                    }
                    // Get Current user Id
                    var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    tbl_Parts.ModifiedByUser = userId;

                    //Get Current Date & Time.
                    tbl_Parts.ModifiedOnDate = DateTime.Now;
                    tbl_Parts.Status = "Active";

                    db.Entry(tbl_Parts).State = EntityState.Modified;
                    db.SaveChanges();

                    transaction.Commit();
                    Alert("Record Updated Sucessfully!!!", NotificationType.success);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Exception(ex);
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return RedirectToAction("Index");
                }
            }
        }

        // GET: Parts/Delete/id
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return PartialView("_Delete");
        }

        // POST: Parts/Delete/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (id == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    var result = db.tbl_Parts.Where(x => x.PartId == id).SingleOrDefault();
                    if (result != null)
                    {
                        result.Status = "InActive";
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    Alert("Record Deleted Sucessfully!!!", NotificationType.success);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Exception(ex);
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return RedirectToAction("Index");
                }
            }
        }


        // GET: Parts/Delete/id
        public ActionResult Delete2(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return PartialView("_Delete2");
        }

        // POST: Parts/Delete/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete2(int id, FormCollection collection)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (id == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    var result = db.tbl_Parts.Where(x => x.PartId == id).SingleOrDefault();
                    if (result != null)
                    {
                        //Remove from award
                        db.tbl_Parts.Remove(result);
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    Alert("Record Deleted Permanently!!!", NotificationType.success);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Exception(ex);
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return RedirectToAction("Index");
                }
            }
        }

        //Restore
        public ActionResult Restore(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return PartialView("_Restore");
        }

        // POST: Parts/Delete/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Restore(int id, FormCollection collection)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (id == 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    var result = db.tbl_Parts.Where(x => x.PartId == id).SingleOrDefault();
                    if (result != null)
                    {
                        //Remove from award
                        result.Status = "Active";
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    Alert("Record Restore Sucessfully!!!", NotificationType.success);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Exception(ex);
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return RedirectToAction("Index");
                }
            }
        }



        // new child part
        public ActionResult ChildPart()
        {

            var vm = new PartsViewModels
            {
                tbl_Parts_list = db.tbl_Parts.Where(x => x.Status == "Active").ToList(),
            };
            return View(vm);
        }


        [HttpPost]
        public ActionResult GetSelectedData(int parentPartId)
        {
            var vm = new PartsViewModels();
            try
            {

                if (parentPartId != 0)
                {
                    vm = GetData(parentPartId);
                }

                else
                {
                    Alert("Their is something went wrong!!!", NotificationType.error);
                    return Json(vm, JsonRequestBehavior.AllowGet);
                }
                return Json(vm.ChildPartsModel_list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Exception(ex);
                Alert("Their is something went wrong!!!", NotificationType.error);
                return Json(vm, JsonRequestBehavior.AllowGet);
            }
        }

        public PartsViewModels GetData(int? parentPartId)
        {
            var vm = new PartsViewModels();
            if (parentPartId != 0)
            {
                var result = db.tbl_ChildParts.Where(x => x.ParentPartId == parentPartId).Select(x => new ChildPartsModel
                {
                    ParentPartId = x.ParentPartId,
                    ChildPartId = x.ChildPartId,
                    ChildPartName = x.tbl_Parts.PART_NAME,
                    ChildPartNo = x.tbl_Parts.Part_No,
                    Qty = x.Qty
                }).ToList();
                vm.ChildPartsModel_list = result;
            }
            return vm;
        }



        #region AddMOPItem  
        [HttpPost]
        public ActionResult AddChildPart(tbl_ChildParts model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                PartsViewModels vm = new PartsViewModels();
                try
                {
                    if (!ModelState.IsValid)
                    {
                        Alert("Their is something went wrong!!!", NotificationType.error);
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                    if (model.ParentPartId != 0 && model.ChildPartId != 0)
                    {
                        db.tbl_ChildParts.Add(model);
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    int? parentPartId = model.ParentPartId;
                    if (parentPartId != 0)
                    {
                        vm = GetData(parentPartId);
                    }

                    //  Alert("Data Saved Sucessfully!!!", NotificationType.success);
                    return Json(vm.ChildPartsModel_list, JsonRequestBehavior.AllowGet);
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
        #endregion AddMOPItem
        #region EditChild   
        [HttpPost]
        public ActionResult EditChildPart(ChildPartsModel model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                PartsViewModels vm = new PartsViewModels();
                try
                {
                    if (!ModelState.IsValid)
                    {
                        Alert("Their is something went wrong!!!", NotificationType.error);
                        return Json(model, JsonRequestBehavior.AllowGet);
                    }
                    if (model.ParentPartId != 0 && model.ChildPartId != 0)
                    {
                        var obj = new tbl_ChildParts()
                        {
                            ParentPartId = model.ParentPartId,
                            ChildPartId = model.ChildPartId,
                            Qty = model.Qty,
                        };
                        db.Entry(obj).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    transaction.Commit();

                    int? parentPartId = model.ParentPartId;
                    if (parentPartId != 0)
                    {
                        vm = GetData(parentPartId);
                    }

                    //  Alert("Data Saved Sucessfully!!!", NotificationType.success);
                    return Json(vm.ChildPartsModel_list, JsonRequestBehavior.AllowGet);
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
        #endregion EditChild

        #region delChild
        [HttpPost]
        public ActionResult DeleteChildPart(int? childPartId, int? parentPartId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var vm = new PartsViewModels();
                try
                {
                    if (childPartId != null)
                    {
                        var result = db.tbl_ChildParts.Where(x => x.ChildPartId == childPartId).SingleOrDefault();
                        if (result != null)
                        {
                            db.tbl_ChildParts.Remove(result);
                            db.SaveChanges();
                            transaction.Commit();
                        }
                    }
                    if (parentPartId != 0)
                    {
                        vm = GetData(parentPartId);
                    }
                    // Alert("Record Deleted Sucessfully!!!", NotificationType.success);
                    return Json(vm.ChildPartsModel_list, JsonRequestBehavior.AllowGet);
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
        #endregion delChild

        #region Utilities

        private PartsViewModels GetParts(int id)
        {
            var viewModel = new PartsViewModels()
            {
                tbl_Parts = db.tbl_Parts.Where(x => x.PartId == id).SingleOrDefault(),
                _tbl_Cage = db.tbl_Cage.ToList(),
                _tbl_PartType = db.tbl_PartType.ToList(),
                _tbl_MCAT = db.tbl_MCAT.ToList(),
                _tbl_Currency = db.tbl_Currency.ToList(),
            };
            return viewModel;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}

