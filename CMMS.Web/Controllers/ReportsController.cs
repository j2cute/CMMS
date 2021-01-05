using ClassLibrary.Models;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using WebApplication.Helpers;

namespace WebApplication.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private ReportDocument _reportDocument;
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MMS5()
        {
            try
            {
                using (WebAppDbContext _entities = new WebAppDbContext())
                {
                    var data = _entities.V_MMS5.ToList().Select(x =>
                     new
                     {
                         SHIP_NAME = x.Name,
                         PMS_NO = x.PMS_No,
                         MOP_NO = x.MOP_No,
                         DOC = x.Doc,
                         JIC = x.Jic,
                         BY_WHOM = x.By_Whom,
                         PERIODICITY = x.Periodicity?.ToString(),
                         EQUIPMENT_NAME = x.PART_NAME,
                         MODEL = x.Model,
                         DESCRIPTION = x.MOP_Desc
                     }).ToList();

                    return LoadReport(Reports.MMS5, data);
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }
        
        public ActionResult JIC()
        {
            try
            {
                using (WebAppDbContext _entities = new WebAppDbContext())
                {
                     var data = _entities.V_MMS5.ToList().Select(x =>
                     new
                     {
                         SHIP_ID = x.Id?.ToString(),
                         PMS_NO = x.PMS_No,
                         MOP_NO = x.MOP_No,
                         DOC = x.Doc,
                         JIC = x.Jic,
                         BY_WHOM = x.By_Whom,
                         PERIODICITY = x.Periodicity?.ToString(),
                         EQUIP_NAME = x.PART_NAME,
                         MODEL = x.Model,
                         DESCRIPTION = x.MOP_Desc,
                         ADD_REFERENCE = x.AddReference,
                         PMS_ID = x.PMS_ID,
                         PROCEDURE = x.Task_Procedure,
                         PRECAUTIONS = x.Safety_Precautions,
                         ISSUE_DATE = x.IssueDate?.ToString(),
                     }).ToList();


                     var subReportData = _entities.V_MSD.ToList().Select(x =>
                     new
                     {
                         MOP_NO =  x.MOP_No,
                         NSN= x.NSN,
                         PART_NO = x.Part_No,
                         ITEM_NAME = x.PART_NAME,
                         PMS_ID =  x.PMS_ID,
                         SR_QTY = x.SR_Qty?.ToString(),
                         UR_QTY = x.UR_Qty?.ToString(),
                         PART_TYPE = x.PartTypeID
                     }).ToList();

                    Dictionary<string, dynamic> subReports = new Dictionary<string, dynamic>();
                    subReports.Add(Reports.MSD.Item1, subReportData);
                    subReports.Add(Reports.MSDSpares.Item1, subReportData);

                    return LoadReport(Reports.JIC, data,subReports);
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        private FileStreamResult LoadReport(Tuple<string,string> reportDetails,dynamic dataSource, Dictionary<string, dynamic> subReports = null, CrystalDecisions.Shared.ExportFormatType exportType = CrystalDecisions.Shared.ExportFormatType.PortableDocFormat)
        {
            try
            {
                var reportPath = $"~/{ConfigurationManager.AppSettings["ReportsPath"]?.ToString()}/{reportDetails.Item1}";
                _reportDocument = new ReportDocument();
                _reportDocument.Load(Server.MapPath(reportPath));

                if(subReports != null && subReports.Any())
                {
                    foreach(var subreport in subReports)
                    {
                        _reportDocument.Subreports[subreport.Key].SetDataSource(subreport.Value);
                    }
                }

                _reportDocument.SetDataSource(dataSource);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                //_reportDocument.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                //_reportDocument.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(5, 5, 5, 5));
                //_reportDocument.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA5;

                Stream stream = _reportDocument.ExportToStream(exportType);
                stream.Seek(0, SeekOrigin.Begin);

                return File(stream, "application/pdf", reportDetails.Item2);
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
    public static class Reports

    {
        public static Tuple<string,string> MMS5 = new Tuple<string,string>("rptMMS5.rpt","MMS5") ;
        public static Tuple<string,string> JIC = new Tuple<string,string>("rptJIC.rpt", "JIC");
        public static Tuple<string, string> MSD = new Tuple<string, string>("rptJicMsd.rpt", "MSD");
        public static Tuple<string, string> MSDSpares = new Tuple<string, string>("rptJicMsdSpare.rpt", "MSDSpares");
    }
}