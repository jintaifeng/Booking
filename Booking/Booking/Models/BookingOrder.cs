using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booking.Models
{
    public class BookingOrder
    {
        public BaseResult BaseResult = new BaseResult();
        public string UserId { get; set; }
        public string CourseId { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string SettleStatus { get; set; }
        public int MemberNumber { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string Deposit { get; set; }
        public string PayBalance { get; set; }
        public string Total { get; set; }
        public string Tax { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string Agency { get; set; }
    }
}