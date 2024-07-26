using System.Web;
using System.Web.Optimization;

namespace CallBackUtility
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Content/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Content/Scripts/modernizr-*"));
            //bundles.Add(new ScriptBundle("~/scripts/vendor").Include(

            //      //"~/Content/vendor/chart.js/chart.umd.js"
            //     "~/Content/vendor/echarts/echarts.js"
            //      ,"~/Content/vendor/quill/quill.js"
            //       ,"~/Content/vendor/tinymce/tinymce.js"
            //       ));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                       "~/Content/Scripts/bootstrap.js",
                      "~/Content/Scripts/main.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/style.css"));
            //  bundles.Add(new StyleBundle("~/Content/vendor").Include(
            //          "~/Content/vendor/bootstrap-icons/bootstrap-icons.css",
            //       "~/Content/vendor/bootstrap-icons/boxicons.min.css",
            //         "~/Content/vendor/quill/quill.snow.css",
            //          "~/Content/vendor/quill/quill.bubble.css",
            //           "~/Content/vendor/remixicon/remixicon.css"
            //        ));

        }
    }
}
