using Booking.Models;
using Booking.Utilities.CustomUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Booking.Utilities.Base
{
    public class CustomController : Controller
    {
        public virtual new CustomPrincipal User
        {
            get { return HttpContext.User as CustomPrincipal; }
        }
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies["IsContinue"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-1);
                cookie.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Add(cookie);
            }
            else
            {
                // clear authentication cookie
                HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                cookie1.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie1);

                // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
                HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
                cookie2.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(cookie2);
            }
        }
    }
}