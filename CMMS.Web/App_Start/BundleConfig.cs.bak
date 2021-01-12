using System.Web;
using System.Web.Optimization;

namespace WebApplication
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
             "~/Scripts/jquery-3.5.1.js",
             "~/Scripts/bootstrap.min.js",
             "~/Scripts/jquery.slimscroll.min.js",                 
             "~/Scripts/fastclick.js",
            "~/Scripts/adminlte.min.js"
        
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/moment.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/All").Include(
                            "~/Scripts/jquery.dataTables.min.js",
                            "~/Scripts/dataTables.bootstrap.min.js",
                            "~/Scripts/jquery-ui-1.12.1.min.js",
                            "~/Scripts/jquery-ui-timepicker-addon.min.js",
                            "~/Scripts/dataTables.buttons.min.js",
                            "~/Scripts/buttons.flash.min.js",
                            "~/Scripts/jszip.min.js",
                            "~/Scripts/pdfmake.min.js",
                            "~/Scripts/vfs_fonts.js",
                            "~/Scripts/buttons.print.min.js",
                            "~/Scripts/buttons.html5.min.js"
                     ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/jquery.validate.min.js",
                       "~/Scripts/jquery.dataTables.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/fonts/font-awesome.min.css",
                      "~/Content/ionicons.min.css",
                      "~/Content/datatables.min.css",
                      "~/Content/dataTables.bootstrap.min.css",
                      "~/Content/buttons.dataTables.min.css",
                      "~/Content/AdminLTE.min.css",
                      "~/Content/_all-skins.min.css",
                      "~/Content/Site.css"

                       ));


            //BundleTable.EnableOptimizations = true;
        }
    }
}
