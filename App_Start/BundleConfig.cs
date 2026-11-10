using System.Web.Optimization;

namespace POMS
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            ScriptBundle scriptBndl = new ScriptBundle("~/Scripts/forlayout");
            string[] scripts = new string[5];
            scripts[0] = "~/Scripts/jquery-1.12.0.js";
            scripts[1] = "~/design/js/bootstrap.min.js";
            scripts[2] = "~/Scripts/tinymce/js/tinymce/tinymce.min.js";
            scripts[3] = "~/Scripts/tinymce/js/tinymce/plugins/textcolor/plugin.min.js";
            scripts[4] = "~/Scripts/MenuJS.js";
           
            scriptBndl.Include(scripts);
          
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                                                    "~/design/css/bootstrap.min.css",
                                                    "~/design/css/bootstrap-theme.min.css",
                                                    "~/design/css/owl.carousel.css",
                                                    "~/design/css/owl.theme.css",
                                                    "~/design/css/theme.css",
                                                    "~/css/build.css"
                                                   
                                                ));

           
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
            bundles.Add(scriptBndl);
            BundleTable.EnableOptimizations = true;

        }
    }
}