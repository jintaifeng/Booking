using System;
using System.Collections.Generic;

namespace Booking.Data.Models
{
    public partial class order
    {
        public int order_id { get; set; }
        public System.Guid user_id { get; set; }
        public int course_id { get; set; }
        public string booking_name { get; set; }
        public string phone { get; set; }
        public string order_status { get; set; }
        public string bookling_status { get; set; }
        public string settle_status { get; set; }
        public int member_number { get; set; }
        public System.DateTime appointment_time { get; set; }
        public Nullable<int> deposit { get; set; }
        public Nullable<int> pay_balance { get; set; }
        public int total { get; set; }
        public Nullable<int> tax { get; set; }
        public System.DateTime tstamp { get; set; }
        public string description { get; set; }
        public string agency { get; set; }
        public virtual golf_course golf_course { get; set; }
        public virtual user user { get; set; }
    }
}
