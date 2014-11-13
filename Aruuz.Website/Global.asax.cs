using Aruuz.Controllers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using Aruuz.Website.Controllers;
using Aruuz.Website.App_Start;

namespace Aruuz.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BootstrapEditorTemplatesConfig.RegisterBundles();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        private void Application_BeginRequest(Object source, EventArgs e)
        {
            // Create HttpApplication and HttpContext objects to access
            // request and response properties.
          /*  string urlReferrer = "#@$@#%@$^$@#!@@#!";
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;
            MySqlConnection myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();

            try
            {
                urlReferrer = Request.UrlReferrer.ToString();
            }
            catch
            {

            }
            cmd.CommandText = "select id from blacklist where ip like '" + Request.UserHostAddress + "' or referrer like '%" + urlReferrer + "%';";

            MySqlDataReader dataReader = cmd.ExecuteReader();
            int id3 = 0;
            while (dataReader.Read())
            {
                id3 = dataReader.GetInt32(0);
            }
            myConn.Close();
            if (id3 != 0)
            {
                Response.End();
            }*/
        }
    }
}
