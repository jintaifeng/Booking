using Booking.DacLayer;
using Booking.Models;
using Booking.Utilities;
using Booking.Utilities.Attributes;
using Booking.Utilities.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Booking.Controllers
{
    public class CommonController : CustomController
    {
        BookingDac booking = new BookingDac();
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

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult AddGolfPlace(GolfPlace place)
        {
            var result = booking.AddGolfPlace(place);
            if (result.Code == 0)
                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult UpdateGolfPlace(GolfPlace place)
        {
            var result = booking.UpdateGolfPlace(place);
            if (result.Code == 0)
                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }

    }
}