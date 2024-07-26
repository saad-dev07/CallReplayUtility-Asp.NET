using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CallBackUtility.Controllers
{
    [Authorize]
    public class ErrorController : Controller
    {
        // GET: Error
      
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult UnhandledException(Exception ex)
        {
            return View(ex.Message);
        }
    }
}