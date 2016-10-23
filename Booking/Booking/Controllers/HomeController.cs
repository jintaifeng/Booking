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
            string userid = "";
            if (User.RoleName != "Admin")
                userid = User.UserId;
            List<BookingGroup> list = new List<BookingGroup>();
            list = booking.GetTodayBookingStatistics(userid);
            return View(list);
        }
        public ActionResult ViewList(BookingOrderListQuery orderQuery ,PageData page)
        {
            BookingOrderList list = new BookingOrderList();
            ViewBag.Place = booking.GetGolfPlace();

            if (string.IsNullOrEmpty(orderQuery.DateType)) {
                orderQuery.DateType = "booking_date";
            }
                DateTime dt = DateTime.Now;
            if (orderQuery.BeginDate.Year<2010)
            {
                orderQuery.BeginDate = dt.AddDays(1 - dt.Day).AddMonths(-1);
            }
            if (orderQuery.EndDate.Year < 2010)
            {
                orderQuery.EndDate = dt.AddDays(1 - dt.Day).AddMonths(2).AddDays(-1);
            }
            if (User.RoleName == "Admin")
                orderQuery.IsAdmin = true;
            else
                orderQuery.UserId = User.UserId;
            ViewBag.Query = orderQuery;
            list = booking.GetBookingOrderList(orderQuery, page);
            return View(list);
        }
        public ActionResult ViewGroup(BookingOrderListQuery orderQuery)
        {
            List<BookingStatistics> list = new List<BookingStatistics>();
            ViewBag.Place = booking.GetGolfPlace();

            if (string.IsNullOrEmpty(orderQuery.DateType))
            {
                orderQuery.DateType = "booking_date";
            }
            if (string.IsNullOrEmpty(orderQuery.SortFiled))
            {
                orderQuery.SortFiled = "booking_date";
            }
            if (string.IsNullOrEmpty(orderQuery.SortType))
            {
                orderQuery.SortType = "ASC";
            }
            DateTime dt = DateTime.Now;
            if (orderQuery.BeginDate.Year < 2010)
            {
                orderQuery.BeginDate = dt.AddDays(1 - dt.Day);
            }
            if (orderQuery.EndDate.Year < 2010)
            {
                orderQuery.EndDate = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1);
            }
            if (User.RoleName == "Admin")
                orderQuery.IsAdmin = true;
            else
                orderQuery.UserId = User.UserId;
            ViewBag.Query = orderQuery;
            list = booking.GetBookingStatistics(orderQuery);
            return View(list);
        }    
        public ActionResult Clause(string mode)
        {
            ViewBag.Mode = mode;
            return View();
        }
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult GolfPlace()
        {
            List<GolfPlace> placeList = new List<GolfPlace>();
            placeList = booking.GetGolfPlace();
            return View(placeList);
        }
    }
}