using Booking.Common;
using Booking.Utilities.Attributes;
using Booking.Utilities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Booking.Controllers
{
    [CustomAuthorize]
    public class HomeController : CustomController
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ViewList()
        {
            return View();
        }
        public ActionResult ViewGroup()
        {
            return View();
        }    
        public ActionResult Clause(string mode)
        {
            ViewBag.Mode = mode;
            return View();
        }        
    }
}