using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Booking.Utilities.CustomUser
{
    public class CustomPrincipal: ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string roles)
        {
            string[] role = roles.Split(',');
            if (role.Length == 1 && role[0] == "")
            {
                return true;
            }
            foreach (var item in role)
            {
                if (item == this.RoleName.ToString())
                {
                    return true;
                }
            }
            return false;
        }
        public CustomPrincipal(string userName, string userType)
        {
            this.Identity = new GenericIdentity(userName, userType);
        }
        public string UserId { get; set; }
        public string LoginName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}