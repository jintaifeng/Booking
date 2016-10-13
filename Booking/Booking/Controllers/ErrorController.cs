using Booking.Utilities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Booking.Controllers
{
    public class ErrorController : CustomController
    {

        [PreventDirectAccess]
        public ActionResult ServerError()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult Authorize()
        {
            return View();
        }
        [PreventDirectAccess]
        public ActionResult OtherHttpStatusCode(int httpStatusCode)
        {
            ViewBag.StatusCode = httpStatusCode.ToString();
            return View();
        }

        private class PreventDirectAccessAttribute : FilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
                object value = filterContext.RouteData.Values["fromAppErrorEvent"];
                if (!(value is bool && (bool)value))
                    filterContext.Result = new ViewResult { ViewName = "NotFound" };
            }
        }
    }
}