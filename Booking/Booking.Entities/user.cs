using System;
using System.Collections.Generic;

namespace Booking.Data.Models
{
    public partial class user
    {
        public user()
        {
            this.orders = new List<order>();
        }

        public System.Guid user_id { get; set; }
        public string login_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public int role_id { get; set; }
        public string phone { get; set; }
        public string status { get; set; }
        public System.DateTime update_time { get; set; }
        public System.DateTime tstamp { get; set; }
        public virtual ICollection<order> orders { get; set; }
        public virtual rbac_roles rbac_roles { get; set; }
    }
}
