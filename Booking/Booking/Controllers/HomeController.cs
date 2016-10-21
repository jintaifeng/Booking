using Booking.Common;
using Booking.DacLayer;
using Booking.Models;
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
        BookingDac booking = new BookingDac();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ViewList(BookingOrderListQuery orderQuery)
        {
            BookingOrderList list = new BookingOrderList();
            ViewBag.Place = booking.GetGolfPlace();
            ViewBag.Query = orderQuery;

            DateTime dt = DateTime.Now;
            if (orderQuery.BeginDate.Year<2010)
            {
                orderQuery.BeginDate = dt.AddDays(1 - dt.Day).AddMonths(-1);
            }
            if (orderQuery.EndDate.Year < 2010)
            {
                orderQuery.EndDate = dt.AddDays(1 - dt.Day).AddMonths(2).AddDays(-1);
            }
            list = booking.GetBookingOrderList(orderQuery);
            if (list.BaseResult.Code != 0)
            {
                return Content("<script>alert('조회 실패하였습니다.');window.location.href='/Home/Index';</script>");
            }
            return View(list);
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