using ClassLibrary.Models;
using ClassLibrary.ViewModels;
using CMMS.Web.Helper;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;

using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public partial class MopController : BaseController
    {
        private static Logger _logger;

        public MopController()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }
        private Dictionary<string, string> PlanActions = new Dictionary<string, string>()
        {
            {"Deferred","Deferred"},
            {"Done","Done"}
        };
        private DateTime SlidingWindow = DateTime.Now.AddMonths(Convert.ToInt32(ConfigurationManager.AppSettings["SlidingWindowForRoutine"].ToString()));

        public ActionResult MopPlans()
        {
            return View("MopPlanList");
        }

        [HttpPost]
        public JsonResult LoadPlans(int length, int start)
        {
            string actionName = "LoadPlans";
            int recordCount = 0, filterrecord = 0;

            List<M_MOP_PLAN> model = new List<M_MOP_PLAN>();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
                using (var db = new WebAppDbContext())
                {
                    //search value
                    string searchvalue = Request.Form["search[value]"];
                    //Find Order Column
                    var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                    var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                    Expression<Func<M_MOP_PLAN, object>> sortExpression;
                    switch (sortColumn)
                    {
                        case "SiteId":
                            sortExpression = (x => x.SiteId);
                            break;
                        case "MOP_No":
                            sortExpression = (x => x.MOP_No);
                            break;
                        case "PMS_No":
                            sortExpression = (x => x.PMS_No);
                            break;
                        case "ESWBS":
                            sortExpression = (x => x.ESWBS);
                            break;
                        case "DoneDate":
                            sortExpression = (x => x.DoneDate);
                            break;
                        case "NextDueDate":
                            sortExpression = (x => x.NextDueDate);
                            break;
                        case "DoneBy":
                            sortExpression = (x => x.DoneBy);
                            break;
                        default:
                            sortExpression = (x => x.MOP_No);
                            break;
                    }

                    List<M_MOP_PLAN> mopPlans = null;
                    if (sortColumnDir == "asc")
                    {
                        mopPlans = db.M_MOP_PLAN.Where
                        (x => x.MOP_No.Contains(searchvalue)
                                || x.PMS_No.Contains(searchvalue)
                                || x.ESWBS.Contains(searchvalue)
                                || x.DoneBy.Contains(searchvalue)
                        )
                        .OrderBy(sortExpression).Skip(start).Take(length)
                        .ToList();
                    }
                    else
                    {
                        mopPlans = db.M_MOP_PLAN.Where
                        (x => x.MOP_No.Contains(searchvalue)
                                || x.PMS_No.Contains(searchvalue)
                                || x.ESWBS.Contains(searchvalue)
                                || x.DoneBy.Contains(searchvalue))
                        .OrderByDescending(sortExpression).Skip(start).Take(length)
                        .ToList();
                    }

                    recordCount = db.M_MOP.Count();

                    if (searchvalue != "")
                    {
                        filterrecord = mopPlans.Count();
                    }
                    else
                    {
                        filterrecord = recordCount;
                    }


                    foreach (var item in mopPlans)
                    {
                        model.Add(item);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException.ToString());
            }
            _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            var response = new { recordsTotal = recordCount, recordsFiltered = filterrecord, data = model };
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MopPlanData(M_MOP_PLAN data)
        {
            string actionName = "MopPlanData";
            M_MOP_PLAN model = new M_MOP_PLAN();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
                if (data.SiteId == 0 || string.IsNullOrWhiteSpace(data.PMS_No)
                   || string.IsNullOrWhiteSpace(data.MOP_No) || string.IsNullOrWhiteSpace(data.ESWBS))
                {

                }
                else
                {
                    using (var context = new WebAppDbContext())
                    {
                        var result = context.M_MOP_PLAN.Where(x => x.SiteId == data.SiteId && x.PMS_No == data.PMS_No
                                                     && x.MOP_No == data.MOP_No && x.ESWBS == data.ESWBS).FirstOrDefault();

                        if (result != null)
                        {
                            result.Actions = PlanActions;
                            model = result;
                        }
                        else
                        {

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException.ToString());
            }
            _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            return PartialView("_MopPlan", model);
        }

        [HttpPost]
        public JsonResult EditPlan(M_MOP_PLAN plan)
        {
            string actionName = "MopPlanData";

            var type = "success";
            var msg = "Plan edited successfully.";

            if (plan.SelectedDate != null)
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");
                using (var db = new WebAppDbContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            using (var context = new WebAppDbContext())
                            {
                                var data = context.M_MOP_PLAN.Where(x => x.SiteId == plan.SiteId && x.PMS_No == plan.PMS_No
                                                                && x.MOP_No == plan.MOP_No && x.ESWBS == plan.ESWBS).FirstOrDefault();

                                if (data.NextDueDate == null && data.DoneDate == null)
                                {
                                    data.NextDueDate = plan.SelectedDate;
                                }
                                else
                                {
                                    if (!String.IsNullOrWhiteSpace(plan.Status))
                                    {
                                        if (plan.Status == "Deferred")
                                        {
                                            data.NextDueDate = plan.SelectedDate;
                                        }
                                        else if (plan.Status == "Done")
                                        {
                                            var mop = context.M_MOP.Where(x => x.MOP_No == plan.MOP_No && x.PMS_No == plan.PMS_No && x.SiteId == plan.SiteId).FirstOrDefault();

                                            data.DoneDate = plan.SelectedDate;
                                            data.NextDueDate = DateTime.Now.AddDays(GetDaysViaPeriod(mop.Period, Convert.ToInt32(mop.Periodicity)));

                                            db.M_MOP_PLAN_HISTORY.Add(new M_MOP_PLAN_HISTORY()
                                            {
                                                SiteId = plan.SiteId,
                                                PMS_No = plan.PMS_No,
                                                MOP_No = plan.MOP_No,
                                                ESWBS = plan.ESWBS,
                                                DoneBy = Session[SessionKeys.UserId]?.ToString(),
                                                DoneDate = plan.SelectedDate,
                                                NextDueDate = data.NextDueDate
                                            });
                                        }
                                        else
                                        {
                                            type = "error";
                                            msg = "Invalid data entered.";
                                        }
                                    }
                                    else
                                    {
                                        type = "error";
                                        msg = "Please fill complete data.";
                                    }
                                }

                                db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                transaction.Commit();

                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException.ToString());

                            type = "failure";
                            msg = "Internal server error.";
                            transaction.Rollback();
                        }
                    }
                }
            }
            else
            {
                type = "error";
                msg = "Please fill complete data.";
            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");
            return Json(new
            {
                msg = msg,
                type = type
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadSchedule()
        {
            IEnumerable<DataModel> result = LoadDataForScheduler();
            return View("LoadMopPlan", result);
        }

        public IEnumerable<DataModel> LoadDataForScheduler()
        {
            string actionName = "LoadDataForScheduler";

            Queue<DataModel> queue = new Queue<DataModel>();
            try
            {
                _logger.Log(LogLevel.Trace, actionName + " :: started.");

                using (var context = new WebAppDbContext())
                {
                    var finalResult = from mop in context.M_MOP
                                      join mopHistory in context.M_MOP_PLAN_HISTORY
                                      on
                                      new { mop.SiteId, mop.PMS_No, mop.MOP_No }
                                      equals
                                      new { mopHistory.SiteId, mopHistory.PMS_No, mopHistory.MOP_No }
                                      into history
                                      from result2 in history.DefaultIfEmpty()
                                      join plan in context.M_MOP_PLAN
                                      on
                                      new { mop.SiteId, mop.PMS_No, mop.MOP_No }
                                      equals
                                      new { plan.SiteId, plan.PMS_No, plan.MOP_No }
                                      into query
                                      from result in query.DefaultIfEmpty()
                                      select new { MOP = mop, Plan = query, History = history };


                    foreach (var item in finalResult.ToList().Where(x => x.Plan.Any()))
                    {
                        if (item != null)
                        {
                            double totalDaysTillNextDueDate = GetDaysViaPeriod(item.MOP?.Period, Convert.ToInt32(item.MOP?.Periodicity));
                            CalculateRoutineSchedule(queue, item.MOP, item.Plan.FirstOrDefault().NextDueDate.Value, totalDaysTillNextDueDate);

                            foreach (var history in item.History)
                            {
                                var model = new DataModel()
                                {
                                    Id = "[1]",
                                    Period = item.MOP?.Period,
                                    Text = item.MOP?.MOP_Desc,
                                    IsPastDate = true,
                                    StartDate = history.DoneDate.ToString(),
                                    EndDate = history.DoneDate.ToString(),
                                    Color = "#cb6bb2"
                                };

                                queue.Enqueue(model);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, actionName + " EXCEPTION :: " + ex.ToString() + " INNER EXCEPTION :: " + ex.InnerException.ToString());

            }

            _logger.Log(LogLevel.Trace, actionName + " :: ended.");

            return queue.ToList();
        }

        public DataModel CalculateRoutineSchedule(Queue<DataModel> queue, M_MOP mop, DateTime nextDueDate, double totalDays)
        {
            DataModel model;

            if (nextDueDate.CompareTo(DateTime.Now) <= 0)
            {
                model = new DataModel()
                {
                    Id = "[1]",
                    Period = mop.Period,
                    Text = mop.MOP_Desc,
                    StartDate = nextDueDate.ToString(),
                    EndDate = nextDueDate.AddHours(8).ToString(),
                    IsPastDate = true,
                    Color = "#cb6bb2"
                };
            }
            else
            {
                if (nextDueDate.CompareTo(SlidingWindow) > 0)
                {
                    return null;
                }

                model = new DataModel()
                {
                    Id = "[1]",
                    Period = mop.Period,
                    Text = mop.MOP_Desc,
                    StartDate = nextDueDate.ToString(),
                    EndDate = nextDueDate.AddHours(8).ToString(),
                    Color = "#cb6bb2"
                };
            }

            queue.Enqueue(model);
            return CalculateRoutineSchedule(queue, mop, nextDueDate.AddDays(totalDays), totalDays);
        }
        
        private double GetDaysViaPeriod(string period, int periodicity)
        {
            double totalDaysTillNextDueDate = 0.0;
            switch (period)
            {
                case Periods.Daily:
                    totalDaysTillNextDueDate = 1;
                    break;

                case Periods.Weekly:
                    totalDaysTillNextDueDate = 7;
                    break;

                case Periods.Monthly:
                    totalDaysTillNextDueDate = (DateTime.Now.AddMonths(periodicity) - DateTime.Now).TotalDays;
                    break;

                case Periods.Quaterly:
                    totalDaysTillNextDueDate = (DateTime.Now.AddMonths(periodicity * 3) - DateTime.Now).TotalDays;
                    break;

                case Periods.Annualy:
                    totalDaysTillNextDueDate = (DateTime.Now.AddMonths(periodicity * 12) - DateTime.Now).TotalDays;
                    break;
            }

            return totalDaysTillNextDueDate;
        }
    }

    public class DataModel
    {
        public string Id { get; set; }
        public string Period { get; set; }
        public string Text { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsPastDate { get; set; }
        public string Color { get; set; }
    }
    public static class Periods
    {
        public const string Daily = "D";
        public const string Weekly = "W";
        public const string Monthly = "M";
        public const string Quaterly = "Q";
        public const string Annualy = "Y";
    }
}