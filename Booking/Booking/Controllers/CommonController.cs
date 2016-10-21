using Booking.Utilities;
using Booking.Utilities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Booking.Controllers
{
    public class CommonController : CustomController
    {
        public ActionResult GetServerDate()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            return Json(new
            {
                ServerDate = date
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Code(string sid)
        {
            return new CodeResult(sid);
        }
        public ActionResult VaildateCaptcha(string sid, string captcha) {
            var isSuccess = CodeResult.VaildateCaptcha(sid, captcha);
            return Json(new { isSuccess= isSuccess }, JsonRequestBehavior.AllowGet);
        }
        
    }
}