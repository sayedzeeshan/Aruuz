using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;
using System.Web;
using System.Web.Optimization;

namespace Aruuz.Website
{
     
    public class BundleConfig
    {
        public const string BootstrapPath = "~/Bundles/Bootstrap";
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Scripts/jquery-1.9.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            bundles.Add(new ScriptBundle("~/Content/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/Content/create").Include(
                "~/Content/create.js"));

            bundles.Add(new ScriptBundle("~/Content/cssloader").Include(
                "~/Content/cssloader.js"));


            bundles.Add(new ScriptBundle("~/bundles/Editor").Include(
                "~/Scripts/jquery.urdueditor.js"));
           
            bundles.Add(new ScriptBundle("~/Content/keyboard").Include(
                "~/Content/keyboard.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/css_custom").Include(
                     "~/Content/customnoto.css"));


            var commonStylesBundle = new CustomStyleBundle(BootstrapPath);
            commonStylesBundle.Orderer = new NullOrderer();
            commonStylesBundle.Include("~/Content/bootstrap.less");
            bundles.Add(commonStylesBundle);
        }
    }
}
