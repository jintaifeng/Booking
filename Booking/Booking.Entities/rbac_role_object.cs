using System;
using System.Collections.Generic;

namespace Booking.Data.Models
{
    public partial class rbac_role_object
    {
        public int rro_id { get; set; }
        public int roles_id { get; set; }
        public string object_id { get; set; }
        public string description { get; set; }
        public System.DateTime tstamp { get; set; }
        public virtual rbac_objects rbac_objects { get; set; }
        public virtual rbac_roles rbac_roles { get; set; }
    }
}
