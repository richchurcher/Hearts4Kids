using System.Web;
using System.Web.Optimization;

namespace Hearts4Kids
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/h4kutilities").Include(
            "~/Scripts/singlePage.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/mvcFileUploader").Include(
                "~/Scripts/mvcfileupload/blueimp/jquery.ui.widget.js",
                "~/Scripts/mvcfileupload/blueimp/tmpl.min.js",
                //< !--The Load Image plugin is included for the preview images and image resizing functionality-- >
                "~/Scripts/mvcfileupload/blueimp/load-image.min.js",
                //< !--The Canvas to Blob plugin is included for image resizing functionality-- >
                "~/Scripts/mvcfileupload/blueimp/canvas-to-blob.min.js",
                "~/Scripts/mvcfileupload/blueimp/jquery.iframe-transport.js",
                "~/Scripts/mvcfileupload/blueimp/jquery.fileupload.js",
                "~/Scripts/mvcfileupload/blueimp/jquery.fileupload-process.js",
                "~/Scripts/mvcfileupload/blueimp/jquery.fileupload-image.js",
                "~/Scripts/mvcfileupload/blueimp/jquery.fileupload-validate.js",
                "~/Scripts/mvcfileupload/blueimp/jquery.fileupload-ui.js"));
        }
    }
}

