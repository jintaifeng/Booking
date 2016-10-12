using System;
using System.Collections.Generic;

namespace Booking.Data.Models
{
    public partial class rbac_roles
    {
        public rbac_roles()
        {
            this.rbac_role_object = new List<rbac_role_object>();
            this.users = new List<user>();
        }

        public int role_id { get; set; }
        public string role_name { get; set; }
        public string description { get; set; }
        public System.DateTime tstamp { get; set; }
        public virtual ICollection<rbac_role_object> rbac_role_object { get; set; }
        public virtual ICollection<user> users { get; set; }
    }
}
