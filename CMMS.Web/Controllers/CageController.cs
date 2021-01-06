using ClassLibrary.Common;
using ClassLibrary.Models;
using ClassLibrary.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication.Helpers;
using static ClassLibrary.Common.Enums;

namespace WebApplication.Controllers
{
    [Authorization]
    public class CageController : BaseController
    {
        private WebAppDbContext db = new WebAppDbContext();

        [HttpPost]
        public JsonResult CageCodeCheck(string CageCode)
        {
            var SearchData = db.tbl_Cage.Where(x => x.CageCode == CageCode).SingleOrDefault();
            if (SearchData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }

        // GET: Cage

        [AuthorizationAttribute]
        public ActionResult Index()
        {
            var vm = new CageViewModels()
            {
               _tbl_Country = db.tbl_Country.ToList(),
                cageCount = db.tbl_Cage.Count(),
                cageActiveCount = db.tbl_Cage.Where(x => x.Status == "Active").Count(),
                CageCountryCount = db.tbl_Country.Count(),

            };
            List<GraphData> data = new List<GraphData>();
            foreach (var item in vm._tbl_Country)
            {
                GraphData details = new GraphData();
                details.label = item.CountryName;
                details.value = db.tbl_Cage.Where(x => x.Country == details.label && x.Status == "Active").Count();
                data.Add(details);
            }
            vm.dataList = data;
            return View(vm);
        }

        [HttpPost]
        public JsonResult LoadData(int length, int start)
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
          

            int filterrecord;
            List<CageModel> Cagedata = null;
            if (sortColumnDir == "asc")
            {
                Cagedata = db.tbl_Cage.Where(x => x.CageCode.Contains(searchvalue)
             || x.CageName.Contains(searchvalue)
             || x.Country.Contains(searchvalue)
             || x.Status.Contains(searchvalue))
             .OrderBy(sortExpression).Skip(start).Take(length)
             .Select(x => new CageModel
             {
                 CageId= x.CageId,
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
                 Status=x.Status,
             }).ToList();
            }
            if (searchvalue != "")
            { filterrecord = Cagedata.Count(); }
            else { filterrecord = db.tbl_Cage.Count(); }
            var recordcount = db.tbl_Cage.Count();

            List<CageModel> dataItems = new List<CageModel>();
            foreach (var item in Cagedata)
            { dataItems.Add(item);    }

            var response = new { recordsTotal = recordcount, recordsFiltered = filterrecord, data = dataItems };
            return Json(response, JsonRequestBehavior.AllowGet);


            //       if (sortColumn == "CageCode")
            //       {
            //           if (sortColumnDir == "desc")
            //           {
            //      Cagedata = db.tbl_Cage.Where(x => x.CageCode.Contains(searchvalue)
            //     || x.CageName.Contains(searchvalue)
            //     || x.Country.Contains(searchvalue)).OrderByDescending(x => x.CageCode).Skip(start).Take(length)
            //     .Select(x => new CageModel
            //     {
            //         CageCode = x.CageCode,
            //         CageName = x.CageName,
            //         Country = x.Country,
            //     }).ToList();
            //           }
            //           else
            //           {
            //        Cagedata = db.tbl_Cage.Where(x => x.CageCode.Contains(searchvalue)
            // || x.CageName.Contains(searchvalue)
            // || x.Country.Contains(searchvalue)).OrderBy(x => x.CageCode).Skip(start).Take(length)
            //.Select(x => new CageModel
            //{
            //    CageCode = x.CageCode,
            //    CageName = x.CageName,
            //    Country = x.Country,
            //}).ToList();
            //           }

            //       }
            //       else if(sortColumn == "CageName")
            //       {
            //           if (sortColumnDir == "desc")
            //           {
            //               Cagedata = db.tbl_Cage.Where(x => x.CageCode.Contains(searchvalue)
            //              || x.CageName.Contains(searchvalue)
            //              || x.Country.Contains(searchvalue)).OrderByDescending(x => x.CageName).Skip(start).Take(length)
            //              .Select(x => new CageModel
            //              {
            //                  CageCode = x.CageCode,
            //                  CageName = x.CageName,
            //                  Country = x.Country,
            //              }).ToList();
            //           }
            //           else
            //           {
            //               Cagedata = db.tbl_Cage.Where(x => x.CageCode.Contains(searchvalue)
            //        || x.CageName.Contains(searchvalue)
            //        || x.Country.Contains(searchvalue)).OrderBy(x => x.CageName).Skip(start).Take(length)
            //       .Select(x => new CageModel
            //       {
            //           CageCode = x.CageCode,
            //           CageName = x.CageName,
            //           Country = x.Country,
            //       }).ToList();
            //           }
            //       }
            //       else if (sortColumn == "Country")
            //       {
            //           if (sortColumnDir == "desc")
            //           {
            //               Cagedata = db.tbl_Cage.Where(x => x.CageCode.Contains(searchvalue)
            //              || x.CageName.Contains(searchvalue)
            //              || x.Country.Contains(searchvalue)).OrderByDescending(x => x.Country).Skip(start).Take(length)
            //              .Select(x => new CageModel
            //              {
            //                  CageCode = x.CageCode,
            //                  CageName = x.CageName,
            //                  Country = x.Country,
            //              }).ToList();
            //           }
            //           else
            //           {
            //               Cagedata = db.tbl_Cage.Where(x => x.CageCode.Contains(searchvalue)
            //        || x.CageName.Contains(searchvalue)
            //        || x.Country.Contains(searchvalue)).OrderBy(x => x.Country).Skip(start).Take(length)
            //       .Select(x => new CageModel
            //       {
            //           CageCode = x.CageCode,
            //           CageName = x.CageName,
            //           Country = x.Country,
            //       }).ToList();
            //           }
            //       }


        }

        public ActionResult Create()
        {
            var vm = new CageViewModels
            {
                _tbl_Country = db.tbl_Country.ToList(),
            };
            return PartialView("_Create", vm);
        }

        public ActionResult Details(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CageViewModels objCageViewModels = GetCage(id);
            return PartialView("_Details", objCageViewModels);
        }

        //POST: Cage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tbl_Cage tbl_Cage)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        var vm = new CageViewModels
                        {
                            tbl_Cage_list = db.tbl_Cage.ToList(),
                        };
                        return PartialView("_Create", vm);
                    }
                    // Get Current user Id
                    var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    tbl_Cage.CreatedByUser = userId;
                    tbl_Cage.ModifiedByUser = userId;

                    //Get Current Date & Time.
                    tbl_Cage.CreatedOnDate = DateTime.Now;
                    tbl_Cage.ModifiedOnDate = DateTime.Now;
                    tbl_Cage.Status = "Active";

                    db.tbl_Cage.Add(tbl_Cage);
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
            CageViewModels objCageViewModels = GetCage(id);
            return PartialView("_Edit", objCageViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, tbl_Cage tbl_Cage)
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
                        CageViewModels objCageViewModels = GetCage(id);
                        return PartialView("_Edit", objCageViewModels);
                    }
                    // Get Current user Id
                    var userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                    tbl_Cage.ModifiedByUser = userId;

                    //Get Current Date & Time.
                    tbl_Cage.ModifiedOnDate = DateTime.Now;
                    //tbl_Cage.Status = "Active";

                    db.Entry(tbl_Cage).State = EntityState.Modified;
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
        public ActionResult Delete(string id)
        {
            if (id == null)
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

                    var result = db.tbl_Cage.Where(x => x.CageId == id).SingleOrDefault();
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


        #region Utilities

        private CageViewModels GetCage(int id)
        {
            var viewModel = new CageViewModels()
            {
                tbl_Cage = db.tbl_Cage.Where(x => x.CageId == id).SingleOrDefault(),
                _tbl_Country = db.tbl_Country.ToList(),
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





        //#region utilities
        //public JsonResult CustomServerSideSearchAction(DataTableAjaxPostModel model)
        //{
        //    // action inside a standard controller
        //    int filteredResultsCount;
        //    int totalResultsCount;
        //    var res = YourCustomSearchFunc(model, out filteredResultsCount, out totalResultsCount);

        //    var result = new List<CageModel>(res.Count);
        //    foreach (var m in res)
        //    {
        //        // simple remapping adding extra info to found dataset
        //        result.Add(new CageModel
        //        {
        //            CageCode = m.CageCode,
        //            CageName = m.CageName,
        //            Country = m.Country

        //        });
        //    };

        //    return Json(new
        //    {
        //        // this is what datatables wants sending back
        //        draw = model.draw,
        //        recordsTotal = totalResultsCount,
        //        recordsFiltered = filteredResultsCount,
        //        data = result
        //    });
        //}


        //public IList<CageModel> YourCustomSearchFunc(DataTableAjaxPostModel model, out int filteredResultsCount, out int totalResultsCount)
        //{
        //    var searchBy = (model.search != null) ? model.search.value : null;
        //    var take = model.length;
        //    var skip = model.start;

        //    string sortBy = "";
        //    bool sortDir = true;

        //    if (model.order != null)
        //    {
        //        // in this example we just default sort on the 1st column
        //        sortBy = model.columns[model.order[0].column].data;
        //        sortDir = model.order[0].dir.ToLower() == "asc";
        //    }

        //    // search the dbase taking into consideration table sorting and paging
        //    var result = GetDataFromDbase(searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount);
        //    if (result == null)
        //    {
        //        // empty collection...
        //        return new List<CageModel>();
        //    }
        //    return result;
        //}


        //public List<CageModel> GetDataFromDbase(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount)
        //{
        //    // the example datatable used is not supporting multi column ordering
        //    // so we only need get the column order from the first column passed to us.        
        //    var db = new WebAppDbContext();
        //    var whereClause = BuildDynamicWhereClause(db, searchBy);

        //    if (String.IsNullOrEmpty(searchBy))
        //    {
        //        // if we have an empty search then just order the results by Id ascending
        //        sortBy = "Id";
        //        sortDir = true;
        //    }

        //    var result = db.tbl_Cage
        //                   .AsExpandable()
        //                   .Where(whereClause)
        //                   .Select(m => new CageModel
        //                   {
        //                       CageCode = m.CageCode,
        //                       CageName = m.CageName,
        //                       Country = m.Country


        //                   })
        //                   .OrderBy(sortBy, sortDir) // have to give a default order when skipping .. so use the PK
        //                   .Skip(skip)
        //                   .Take(take)
        //                   .ToList();

        //    // now just get the count of items (without the skip and take) - eg how many could be returned with filtering
        //    filteredResultsCount = db.tbl_Cage.AsExpandable().Where(whereClause).Count();
        //    totalResultsCount = db.tbl_Cage.Count();

        //    return result;
        //}

        //private Expression<Func<Person, bool>> BuildDynamicWhereClause(WebAppDbContext entities, string searchValue)
        //{
        //    // simple method to dynamically plugin a where clause
        //    var predicate = PredicateBuilder.New<Person>(true); // true -where(true) return all
        //    if (String.IsNullOrWhiteSpace(searchValue) == false)
        //    {
        //        // as we only have 2 cols allow the user type in name 'firstname lastname' then use the list to search the first and last name of dbase
        //        var searchTerms = searchValue.Split(' ').ToList().ConvertAll(x => x.ToLower());

        //        predicate = predicate.Or(s => searchTerms.Any(srch => s.FirstName.ToLower().Contains(srch)));
        //        predicate = predicate.Or(s => searchTerms.Any(srch => s.LastName.ToLower().Contains(srch)));
        //        predicate = predicate.Or(s => searchTerms.Any(srch => s.MiddleName.ToLower().Contains(srch)));
        //    }
        //    return predicate;
        //}
        //#endregion utilities
    }
}

