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
    public class BookingController : CustomController
    {
        BookingDac booking = new BookingDac();
        // GET: Booking
        public ActionResult Write(string mode, int orderid = 0)
        {
            BookingOrder order = new BookingOrder();
            ViewBag.Place = booking.GetGolfPlace();
            ViewBag.Mode = mode;
            if (mode == "write")
            {
            }
            else if (mode == "update")
            {
                order = booking.GetBookingInfo(orderid);
                if (order.BaseResult.Code != 0)
                {
                    return Content(string.Format("<script>alert('{0}');</script>", order.BaseResult.Message));
                }
                else {
                    if (order.BookingStatus == "close")
                    {
                        return Content("<script>alert('취소한 예약은 수정할수 없습니다.');</script>");
                    }
                    else if (order.UserId != User.UserId)
                    {
                        return Content("<script>alert('당신이 신청한 예약이 아닙니다. 수정할수 없습니다.');</script>");
                    }
                }
            }
            else
            {
                return Content("<script>alert('파라미터를 정확히 보내주세요.');</script>");
            }
            return View(order);
        }
        [HttpPost]
        public ActionResult BookingWrite(BookingOrder order, string mode, FormCollection fc)
        {
            ViewBag.Mode = mode;
            order.Deposit = Convert.ToInt32(fc["Deposit"] == "" ? "0" : fc["Deposit"].Replace(",", ""));
            order.PayBalance = Convert.ToInt32(fc["PayBalance"] == "" ? "0" : fc["PayBalance"].Replace(",", ""));
            order.SubTotal = Convert.ToInt32(fc["SubTotal"] == "" ? "0" : fc["SubTotal"].Replace(",", ""));
            order.Total = Convert.ToInt32(fc["Total"] == "" ? "0" : fc["Total"].Replace(",", ""));
            order.Commission = Convert.ToInt32(fc["Commission"] == "" ? "0" : fc["Commission"].Replace(",", ""));
            order.TotalCommission = Convert.ToInt32(fc["TotalCommission"] == "" ? "0" : fc["TotalCommission"].Replace(",", ""));

            if (order.Deposit + order.PayBalance != order.SubTotal
                || order.SubTotal * order.MemberNumber != order.Total
                || order.Commission * order.MemberNumber != order.TotalCommission)
            {
                return Content("<script>alert('예약금액이 잘못되었습니다.관리자를 연락해주세요.');</script>");
            }

            if (mode == "write")
            {
                order.BookingStatus = "success";
                order.SettleStatus = "waiting";
                var result = booking.BookingInsert(order);
                if (result.Code == 0)
                {
                    return Content("<script>alert('예약신청 성공하였습니다.');window.location.href='/Home/ViewList';</script>");
                }
                else {
                    return Content("<script>alert('예약신청 실패하였습니다.');window.location.href='/Booking/Write?mode=write';</script>");
                }
            }
            else if (mode == "update")
            {
                var result = booking.BookingUpdate(order);
                if (result.Code == 0)
                {
                    return Content("<script>alert('예약수정 성공하였습니다.');window.location.reload();</script>");
                }
                else {
                    return Content("<script>alert('예약수정 실패하였습니다.');window.location.href='/Booking/Write?mode=update&orderid=" + order.OrderId + "';</script>");
                }
            }
            else {
                return Content("<script>alert('파라미터를 정확히 보내주세요.');</script>");
            }
        }
        public ActionResult MultiWrite()
        {
            ViewBag.Place = booking.GetGolfPlace();
            return View();
        }

        [HttpPost]
        public ActionResult MultiWrite(IEnumerable<BookingOrder> orders, IEnumerable<FormCollection> fc)
        {
            List<BookingOrder> orderlist = new List<BookingOrder>();
            int i = 0;
            foreach (var item in fc)
            {
                string philsu_key = string.Format("[{0}].philsu", i.ToString());
                if (item[philsu_key] == "OK")
                {
                    BookingOrder order = new BookingOrder();
                    order = orders.ElementAt(i);
                    order.UserId = item["UserId"];
                    order.AppointmentDate = Convert.ToDateTime(item["AppointmentDate"]);
                    string bookingDate = item[string.Format("[{0}].booking_date", i.ToString())];
                    string bookingHour = item[string.Format("[{0}].booking_hour", i.ToString())];
                    string bookingBun = item[string.Format("[{0}].booking_bun", i.ToString())];
                    if (!string.IsNullOrEmpty(bookingDate)
                        && !string.IsNullOrEmpty(bookingHour)
                        && !string.IsNullOrEmpty(bookingBun))
                    {
                        order.BookingDate = Convert.ToDateTime(bookingDate + " " + bookingHour + ":" + bookingBun);
                    }
                    string deposit = item[string.Format("[{0}].Deposit", i.ToString())];
                    string payBalance = item[string.Format("[{0}].PayBalance", i.ToString())];
                    string subtotal = item[string.Format("[{0}].SubTotal", i.ToString())];
                    string commission = item[string.Format("[{0}].Commission", i.ToString())];
                    order.Deposit = Convert.ToInt32(deposit == "" ? "0" : deposit.Replace(",", ""));
                    order.PayBalance = Convert.ToInt32(payBalance == "" ? "0" : payBalance.Replace(",", ""));
                    order.SubTotal = Convert.ToInt32(subtotal == "" ? "0" : subtotal.Replace(",", ""));
                    order.Total = (order.Deposit + order.PayBalance) * order.MemberNumber;
                    order.Commission = Convert.ToInt32(commission == "" ? "0" : commission.Replace(",", ""));
                    order.TotalCommission = order.Commission * order.MemberNumber;
                    order.BookingStatus = "success";
                    order.SettleStatus = "waiting";

                    //예약금액 검증
                    if (order.Deposit + order.PayBalance != order.SubTotal
                || order.SubTotal * order.MemberNumber != order.Total
                || order.Commission * order.MemberNumber != order.TotalCommission)
                    {
                        return Content("<script>alert('예약금액이 잘못되었습니다.관리자를 연락해주세요.');</script>");
                    }
                    orderlist.Add(order);
                }
                i++;
            }
            var result = booking.BookingMultiInsert(orderlist);
            if (result.Code == 0)
            {
                return Content("<script>alert('예약신청 성공하였습니다.');window.location.href='/Home/ViewList';</script>");
            }
            else {
                return Content("<script>alert('예약신청 실패하였습니다.');window.location.href='/Booking/MultiWrite';</script>");
            }

        }

        public ActionResult GetCommission(string courseid)
        {
            var result = booking.GetCommission(courseid);
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BookingHandle(string orderid, string mode)
        {
            BookingOrder order = new BookingOrder();
            if (User.RoleName == "Custom")
            {
                order.UserId = User.UserId;
            }
            if (mode == "bookingdelete")
                order.BookingStatus = "close";
            else if (mode == "settlesuccess" && User.RoleName == "Admin")
                order.SettleStatus = "success";
            else
                return Content("<script>alert('파라미터를 정확히 보내주세요.');</script>");

            var result = booking.BookingHandle(orderid, order);
            if (result.Code == 0)
                return Json(new { isSuccess = true }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { isSuccess = false }, JsonRequestBehavior.AllowGet);
        }
    }
}