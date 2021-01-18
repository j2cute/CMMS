using ClassLibrary.Models;
using ClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public partial class MopController : BaseController
    {
        private DateTime SlidingWindow = DateTime.Now.AddMonths(Convert.ToInt32(ConfigurationManager.AppSettings["SlidingWindowForRoutine"].ToString()));
        public ActionResult Scheduler()
        {
            return View("MopPlanFilter");
        }
        public ActionResult LoadSchedule()
        {
            IEnumerable<DataModel> result = LoadDataForScheduler();
            return View("LoadMopPlan", result);
        }
        public IEnumerable<DataModel> LoadDataForScheduler()
        {
            Queue<DataModel> queue = new Queue<DataModel>();
            try
            {
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
                            
                            foreach(var history in item.History)
                            {
                                var model = new DataModel()
                                {
                                    Period = item.MOP?.Period,
                                    Text = item.MOP?.MOP_Desc,
                                    IsPastDate = true,
                                    StartDate = history.DoneDate.ToString(),
                                    EndDate = history.DoneDate.ToString()
                                };

                                queue.Enqueue(model);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return queue.ToList();
        }
        public DataModel CalculateRoutineSchedule(Queue<DataModel> queue, M_MOP mop, DateTime nextDueDate, double totalDays)
        {
            DataModel model;

            if (nextDueDate.CompareTo(DateTime.Now) <= 0)
            {
                model = new DataModel()
                {
                    Period = mop.Period,
                    Text = mop.MOP_Desc,
                    StartDate = nextDueDate.ToString(),
                    EndDate = nextDueDate.AddHours(8).ToString(),
                    IsPastDate = true,
                    Color = "#808080"
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
                    Period = mop.Period,
                    Text = mop.MOP_Desc,
                    StartDate = nextDueDate.ToString(),
                    EndDate = nextDueDate.AddHours(8).ToString()
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