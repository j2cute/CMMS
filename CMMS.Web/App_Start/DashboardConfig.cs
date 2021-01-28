using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Xml.Linq;
using ClassLibrary.Models;
using CMMS.Web.Helper;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DashboardWeb.Mvc;
using DevExpress.Data.Filtering;
 

namespace WebApplication
{
    public class DashboardConfig
    {
        private static string dashboardPath = $"~/{ConfigurationManager.AppSettings["DashboardPath"]?.ToString()}";
        private static CustomDashboardStorage customStorage = new CustomDashboardStorage(dashboardPath);

        public static void RegisterService(RouteCollection routes)
        {
            routes.MapDashboardRoute();

            DashboardConfigurator.Default.SetDashboardStorage(customStorage);
            DashboardConfigurator.Default.SetConnectionStringsProvider(new DevExpress.DataAccess.Web.ConfigFileConnectionStringsProvider());

            DashboardConfigurator.Default.CustomFilterExpression += Default_CustomFilterExpression;

        }

        private static void Default_CustomFilterExpression(object sender, CustomFilterExpressionWebEventArgs e)
        {
            try
            {
                var dashBoardXML = customStorage.LoadDashboard(e.DashboardId);

                // Check For Data Source

                var query = dashBoardXML.Descendants("SqlDataSource").Where(x => x.Attribute("Name").Value == e.DataSourceName);
                if (query.Any())
                {
                    var columns = query.Descendants("Columns");

                    var tables = query.Descendants("Tables").FirstOrDefault().Elements().Select(x => x.Attributes("Name"));

                    var applicableSites = ((IEnumerable<tbl_Unit>)System.Web.HttpContext.Current.Session[SessionKeys.ApplicableUnits]).Select(x => x.Id);

                    foreach (var table in tables)
                    {
                        if (table.Any())
                        {
                            var tableName = table.FirstOrDefault().Value; 
                            var columnsDetails = columns.FirstOrDefault().Elements().Where(x => x.Attribute("Table").Value == tableName);

                            foreach (var col in columnsDetails)
                            {
                                if (col.Attributes().Where(x => x.Name == "Name").Any())
                                {
                                    if (col.Attribute("Name").Value.Contains("SiteId"))
                                    {
                                        e.FilterExpression = new InOperator(tableName + ".SiteId", applicableSites);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
              
            
            }
            catch
            {

            }
        }
    }

    public class CustomDashboardStorage : IEditableDashboardStorage
    {
        public string WorkingDirectory { get; set; }
        protected virtual DirectoryInfo Directory
        {
            get
            {
                string absolutePath = Path.IsPathRooted(WorkingDirectory) ? WorkingDirectory : HttpContext.Current.Server.MapPath(WorkingDirectory);
                return new DirectoryInfo(absolutePath);
            }
        }

        public CustomDashboardStorage(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
        }

        Dashboard CreateIntance(XDocument dashboard)
        {
            Dashboard instance = null;
            try
            {
                instance = new Dashboard();
                instance.LoadFromXDocument(dashboard);
                return instance;
            }
            catch
            {
                if (instance != null)
                    instance.Dispose();
                return null;
            }
        }

        protected virtual string ResolveFileName(string dashboardID)
        {
            var dashboardFileName = string.Format("{0}.xml", dashboardID);
            dashboardFileName = Path.GetInvalidFileNameChars().Aggregate(dashboardFileName, (current, c) => current.Replace(c.ToString(), string.Empty));
            return Path.Combine(Directory.FullName, dashboardFileName);
        }


        public IEnumerable<DashboardInfo> GetAvailableDashboardsInfo()
        {
            IEnumerable<string> dashboardsID = Directory.GetFiles().Select(file => Path.GetFileNameWithoutExtension(file.Name)).ToList();
            return dashboardsID.Select(id =>
            {
                string name = null;
                using (Dashboard instance = CreateIntance(LoadDashboard(id)))
                {
                    if (instance != null)
                        name = instance.Title.Text;
                }
                return new DashboardInfo
                {
                    ID = id,
                    Name = name
                };
            });
        }
        public XDocument LoadDashboard(string dashboardID)
        {
            var fileName = ResolveFileName(dashboardID);
            if (File.Exists(fileName))
                return XDocument.Load(fileName);
            return null;
        }
        public void SaveDashboard(string dashboardID, XDocument dashboard)
        {
            var fileName = ResolveFileName(dashboardID);
            dashboard.Save(fileName);
        }
        public string AddDashboard(XDocument dashboard, string dashboardName)
        {
            using (Dashboard instance = CreateIntance(dashboard))
            {
                if (instance != null)
                {
                    instance.Title.Text = dashboardName;
                    dashboard = instance.SaveToXDocument();

                    var fileName = ResolveFileName(dashboardName);
                    dashboard.Save(fileName);
                }
            }
            return dashboardName;
        }

        public void DeleteDashboard(string dashboardID)
        {
            if (File.Exists(dashboardID))
            {
                File.Delete(dashboardID);
            }
        }
    }


    //public class CustomDataSourceStorage : IDataSourceStorage
    //{
    //    public XDocument GetDataSource()
    //    {
    //        XDocument.Load()
    //    }
    //}
}