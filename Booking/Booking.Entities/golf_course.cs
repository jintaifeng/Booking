using System;
using System.Collections.Generic;

namespace Booking.Data.Models
{
    public partial class golf_course
    {
        public golf_course()
        {
            this.orders = new List<order>();
        }

        public int course_id { get; set; }
        public string course_name { get; set; }
        public string description { get; set; }
        public virtual ICollection<order> orders { get; set; }
    }
}
