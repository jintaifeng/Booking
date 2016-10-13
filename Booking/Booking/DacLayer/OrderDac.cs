using Booking.Models;
using Booking.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Booking.DacLayer
{
    public class OrderDac
    {
        DBHelper dh = new DBHelper();
        public User CheckLogin(string userId, string userPassword)
        {
            User u = new User();
            try
            {
                string sql = "select [user_id],login_name,email,name,role_id,phone,[status] from dbo.[user] where status='open' and login_name='{0}' and password='{1}'";
                DbCommand cmd = dh.GetSqlStringCommond(string.Format(sql, userId, userPassword));
                DataTable dt = dh.ExecuteDataTable(cmd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    u.UserId = dt.Rows[0]["user_id"].ToString();
                    u.LoginName = dt.Rows[0]["login_name"].ToString();
                    u.Email = dt.Rows[0]["email"].ToString();
                    u.Name = dt.Rows[0]["name"].ToString();
                    u.RoleId = Convert.ToInt32(dt.Rows[0]["role_id"].ToString());
                    u.Phone = dt.Rows[0]["phone"].ToString();
                    u.Status = dt.Rows[0]["status"].ToString();
                }
                else {
                    u.BaseResult.Code = 401;
                    u.BaseResult.Message = "Not Data";
                }
            }
            catch (Exception ex)
            {
                u.BaseResult.Code = 300;
                u.BaseResult.Message = ex.StackTrace + ex.Message;
            }
            return u;
        }
    }
}