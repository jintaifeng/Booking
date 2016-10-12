using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class User
    {
        public BaseResult BaseResult = new BaseResult();
        public string UserId { get; set; }
        public string LoginName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int RoleId { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

}
