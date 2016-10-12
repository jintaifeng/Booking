using System;
using System.Collections.Generic;

namespace Booking.Data.Models
{
    public partial class rbac_objects
    {
        public rbac_objects()
        {
            this.rbac_role_object = new List<rbac_role_object>();
        }

        public string object_id { get; set; }
        public string object_name { get; set; }
        public string object_url { get; set; }
        public string object_type { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public System.DateTime tstamp { get; set; }
        public string icon { get; set; }
        public Nullable<int> sort { get; set; }
        public virtual ICollection<rbac_role_object> rbac_role_object { get; set; }
    }
}
