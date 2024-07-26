using CallBackUtility.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CallBackUtility
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_End()
       {//  RouteTable.Routes.MapHubs(); 
        //   SqlDependency.Stop(ConfigurationManager.ConnectionStrings["CS"].ConnectionString);
            if (Session!=null)
            {
                Session.Abandon();
            } 
                 Response.Clear();
                  Server.ClearError();
        }
        //protected void Application_Error()
        //{
        //    string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
        //    var ex = Server.GetLastError();
        //    //log the error!
        //    LogsManager.Logs(LogPath,"Start Up Error: " + ex.Message);
        //}
        protected void Application_Error(object sender, EventArgs e)
        {
            string LogPath = Server.MapPath(Convert.ToBoolean(ConfigurationManager.AppSettings["isProduction"].ToString()) ? ConfigurationManager.AppSettings["ProductionLogsPath"].ToString() : ConfigurationManager.AppSettings["LabLogsPath"].ToString());
            Exception exception = Server.GetLastError();
            Response.Clear();
            LogsManager.Logs(LogPath, "Error: " + exception.Message);
            var httpException = exception as HttpException;
            if (httpException != null)
            {
                int errorCode = httpException.GetHttpCode();
                switch (errorCode)
                {
                    case 404:
                        Response.Redirect("~/Error/NotFound");
                        break;
                    // You can add more cases for specific HTTP error codes if needed
                    default:
                        Response.Redirect("~/Error");
                        break;
                }
            }

            // Clear the error from server
            Server.ClearError();
        }
        protected void Application_Start()
        {
         
            //SqlDependency.Start(ConfigurationManager.ConnectionStrings["CS"].ConnectionString);
            AppUserManager.SeedUser();
           // AppUserManager.SeedApplication();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            Application["TotalOnlineUsers"] = Application["TotalOnlineUsers"] == null ? 0 : (int)Application["TotalOnlineUsers"] + 1;
            
            Application.UnLock();

        }
        void Session_End(object sender, EventArgs e)
        {
          
            // Code that runs when a session ends.
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer
            // or SQLServer, the event is not raised.
            Application.Lock();
            Application["TotalOnlineUsers"] = Application["TotalOnlineUsers"] == null ? 0 : (int)Application["TotalOnlineUsers"] - 1;
           
            Application.UnLock();
        }
        //private void Application_Error(object sender, EventArgs e)
        //{
        //    Exception ex = Server.GetLastError();

        //    if (ex is HttpAntiForgeryException)
        //    {
        //        Session.Abandon();
        //        Response.Clear();
        //        Server.ClearError(); //make sure you log the exception first
        //        Response.Redirect("/error/UnhandledException/"+ex, true);
        //    }
        //}
    }
}
