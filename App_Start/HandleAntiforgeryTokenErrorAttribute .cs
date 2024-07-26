using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Aavaya_CM_CDR_Analyzer.App_Start
{
    public class HandleAntiforgeryTokenErrorAttribute : HandleErrorAttribute
    {
        //public override void OnException(ExceptionContext filterContext)
        //{
        //    filterContext.ExceptionHandled = true;
        //    filterContext.Result = new RedirectToRouteResult(
        //        new RouteValueDictionary(new { action = "Login", controller = "Account" }));
        //}
    }
}