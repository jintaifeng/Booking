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
        public string RoleName { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
    public class PageData
    {
        public PageData() {
            this.StartNum = 1;
            this.PageNum = 1;
            this.PageSize = 20;
        }
        public int StartNum { get; set; }
        public int EndNum { get; set; }
        public int MaxCount { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
    }
    public class UserListQuery
    {
        public PageData PageData = new PageData();
        public List<User> UserList = new List<User>();
        public BaseResult BaseResult = new BaseResult();
    }
}
