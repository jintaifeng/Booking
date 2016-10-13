using Booking.Models;
using Booking.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Booking.DacLayer
{
    public class AccountDac
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

        public bool CheckLoginName(string login_name)
        {
            bool isExist = false;
            try
            {
                string sql = "select count(1) as count  from dbo.[user] where login_name='{0}'";
                DbCommand cmd = dh.GetSqlStringCommond(string.Format(sql, login_name));
                DataTable dt = dh.ExecuteDataTable(cmd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int count = Convert.ToInt16(dt.Rows[0]["count"]);
                    if (count == 0)
                        isExist = true;

                }
            }
            catch (Exception ex)
            {
                isExist = false;
            }
            return isExist;
        }
        public bool CheckEmail(string email)
        {
            bool isExist = false;
            try
            {
                string sql = "select count(1) as count  from dbo.[user] where email='{0}'";
                DbCommand cmd = dh.GetSqlStringCommond(string.Format(sql, email));
                DataTable dt = dh.ExecuteDataTable(cmd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int count = Convert.ToInt16(dt.Rows[0]["count"]);
                    if (count == 0)
                        isExist = true;

                }
            }
            catch (Exception ex)
            {
                isExist = false;
            }
            return isExist;
        }

        public User CheckField(User user)
        {
            User u = new User();
            StringBuilder sql = new StringBuilder("select [user_id],login_name,email,name,role_id,phone,[status]  from dbo.[user] where 1=1 ");
            if (!string.IsNullOrEmpty(user.Name))
            {
                sql.AppendFormat("and name='{0}'", user.Name);
            }
            if (!string.IsNullOrEmpty(user.Email))
            {
                sql.AppendFormat("and email='{0}'", user.Email);
            }
            if (!string.IsNullOrEmpty(user.LoginName))
            {
                sql.AppendFormat("and login_name='{0}'", user.LoginName);
            }
            try
            {
                DbCommand cmd = dh.GetSqlStringCommond(sql.ToString());
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

        public BaseResult InsertUser(User user)
        {
            BaseResult baseResult = new BaseResult();
            try
            {
                string sql = @"insert into dbo.[user] (login_name,email,name,password,role_id,phone,[status],tstamp, update_time)
                              values (@login_name,@email,@name,@password,@role_id,@phone,@status,GETDATE(), GETDATE()) ";
                DbCommand cmd = dh.GetSqlStringCommond(sql);
                SqlParameter[] parameters = {
                                            new SqlParameter("@login_name",user.LoginName),
                                            new SqlParameter("@email",user.Email),
                                            new SqlParameter("@name",user.Name),
                                            new SqlParameter("@password",user.Password),
                                            new SqlParameter("@role_id",user.RoleId),
                                            new SqlParameter("@phone",user.Phone),
                                            new SqlParameter("@status",user.Status)
                                        };
                int result = dh.ExecuteNonQuery(cmd,parameters);

                if (result!=1)
                {
                    baseResult.Code = 301;
                    baseResult.Message = "Insert Fail";
                }
            }
            catch (Exception ex)
            {
                baseResult.Code = 300;
                baseResult.Message = ex.StackTrace + ex.Message;
            }
            return baseResult;
        }

        public BaseResult GenerateCode(string userid,string code)
        {
            BaseResult baseResult = new BaseResult();
            try
            {
                string sql = @" IF EXISTS (select 1 from dbo.[user_code] where user_id=@user_id) 
                                begin
                                    update dbo.[user_code] set code=@code,tstamp=GETDATE() where user_id=@user_id
                                end
                                else begin
                                    insert into dbo.[user_code] (user_id,code,tstamp)
                                    values (@user_id,@code,GETDATE()) 
                                end";
                DbCommand cmd = dh.GetSqlStringCommond(sql);
                SqlParameter[] parameters = {
                                            new SqlParameter("@user_id",userid),
                                            new SqlParameter("@code",code)
                                        };
                int result = dh.ExecuteNonQuery(cmd, parameters);

                if (result != 1)
                {
                    baseResult.Code = 301;
                    baseResult.Message = "Insert Fail";
                }
            }
            catch (Exception ex)
            {
                baseResult.Code = 300;
                baseResult.Message = ex.StackTrace + ex.Message;
            }
            return baseResult;
        }

        public BaseResult CheckGenerateCode(string userid, string code,string expire_time)
        {
            BaseResult baseResult = new BaseResult();
            try
            {
                string sql = @" select count(1) as count from dbo.[user_code] where user_id=@user_id and code=@code and DATEDIFF(MINUTE,tstamp,GETDATE())<= @expire_time ";                                ;
                DbCommand cmd = dh.GetSqlStringCommond(sql);
                SqlParameter[] parameters = {
                                            new SqlParameter("@user_id",userid),
                                            new SqlParameter("@code",code),
                                            new SqlParameter("@expire_time",expire_time) 
                                        };
                DataTable dt = dh.ExecuteDataTable(cmd, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int count = Convert.ToInt16(dt.Rows[0]["count"]);
                    if (count == 0)
                    {
                        baseResult.Code = 401;
                        baseResult.Message = "No Data";
                    }

                }
            }
            catch (Exception ex)
            {
                baseResult.Code = 300;
                baseResult.Message = ex.StackTrace + ex.Message;
            }
            return baseResult;
        }
    }
}