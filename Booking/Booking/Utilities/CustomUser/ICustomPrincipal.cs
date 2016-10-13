using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Booking.Utilities.CustomUser
{
    interface ICustomPrincipal : IPrincipal
    {
        string UserId { get; set; }
        string Password { get; set; }
    }
}