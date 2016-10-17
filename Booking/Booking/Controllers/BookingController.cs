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
    public class BookingController : CustomController
    {
        // GET: Booking
        public ActionResult Write()
        {
            return View();
        }
    }
}