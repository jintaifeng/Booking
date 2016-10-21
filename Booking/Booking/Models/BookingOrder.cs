using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booking.Models
{
    public class BookingOrder
    {
        public BaseResult BaseResult = new BaseResult();
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string CourseId { get; set; }
        public string BookingUserName { get; set; }
        public string Phone { get; set; }
        public string BookingStatus { get; set; }
        public string SettleStatus { get; set; }
        public int MemberNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int Deposit { get; set; }
        public int PayBalance { get; set; }
        public int SubTotal { get; set; }
        public int Total { get; set; }
        public int Commission { get; set; }
        public int TotalCommission { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string Agency { get; set; }
    }

    public class GolfPlace{

        public BaseResult BaseResult = new BaseResult();
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public int Commission { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
    public class BookingOrderListQuery
    {
        public string CourseId { get; set; }
        public string BookingStatus { get; set; }
        public string SettleStatus { get; set; }
        public string DateType { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SortFiled { get; set; }
        public string SortType { get; set; }
        public string SearchFiled { get; set; }
        public string SearchValue{ get; set; }
        public string SearchType { get; set; }
        public string CloseStatus { get; set; }

        public PageData PageData = new PageData();
    }
    public class BookingOrderList
    {

        public PageData PageData = new PageData();
        public List<BookingOrder> bookingOrderList = new List<BookingOrder>();
        public BaseResult BaseResult = new BaseResult();
    }
}