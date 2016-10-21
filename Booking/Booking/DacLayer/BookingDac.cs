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
    public class BookingDac
    {
        DBHelper dh = new DBHelper();
        public BookingOrder GetBookingInfo(int orderid)
        {
            BookingOrder order = new BookingOrder();
            try
            {
                string sql = @"SELECT [order_id]
						  ,[user_id]
						  ,[course_id]
						  ,[booking_username]
						  ,[phone]
						  ,[bookling_status]
						  ,[settle_status]
						  ,[member_number]
						  ,[appointment_date]
						  ,[booking_date]
						  ,[deposit]
						  ,[pay_balance]
                          ,[sub_total]
						  ,[total]
						  ,[commission]
						  ,[total_commission]
						  ,[description]
						  ,[tstamp]
						  ,[update_time]
					  FROM [dbo].[booking_order]
                      WHERE order_id={0}";
                DbCommand cmd = dh.GetSqlStringCommond(string.Format(sql, orderid));
                DataTable dt = dh.ExecuteDataTable(cmd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    order.OrderId = Convert.ToInt32(dt.Rows[0]["order_id"].ToString());
                    order.UserId = dt.Rows[0]["user_id"].ToString();
                    order.Phone = dt.Rows[0]["phone"].ToString();
                    order.CourseId = dt.Rows[0]["course_id"].ToString();
                    order.BookingUserName = dt.Rows[0]["booking_username"].ToString();
                    order.BookingStatus = dt.Rows[0]["bookling_status"].ToString();
                    order.SettleStatus = dt.Rows[0]["settle_status"].ToString();
                    order.BookingDate = Convert.ToDateTime(dt.Rows[0]["booking_date"]);
                    order.AppointmentDate = Convert.ToDateTime(dt.Rows[0]["appointment_date"]);
                    order.MemberNumber = Convert.ToInt32(dt.Rows[0]["member_number"].ToString());
                    order.Deposit = Convert.ToInt32(dt.Rows[0]["deposit"].ToString());
                    order.PayBalance = Convert.ToInt32(dt.Rows[0]["pay_balance"].ToString());
                    order.SubTotal = Convert.ToInt32(dt.Rows[0]["sub_total"].ToString());
                    order.Total = Convert.ToInt32(dt.Rows[0]["total"].ToString());
                    order.Commission = Convert.ToInt32(dt.Rows[0]["commission"].ToString());
                    order.TotalCommission = Convert.ToInt32(dt.Rows[0]["total_commission"].ToString());
                    order.Description = dt.Rows[0]["description"].ToString();
                    order.SettleStatus = dt.Rows[0]["settle_status"].ToString();
                    order.CreateDate = Convert.ToDateTime(dt.Rows[0]["tstamp"]);
                }
                else {
                    order.BaseResult.Code = 401;
                    order.BaseResult.Message = "Not Data";
                }
            }
            catch (Exception ex)
            {
                order.BaseResult.Code = 300;
                order.BaseResult.Message = ex.StackTrace + ex.Message;
            }
            return order;
        }

        public BaseResult BookingInsert(BookingOrder order)
        {
            BaseResult baseResult = new BaseResult();
            try
            {
                string sql = @"INSERT INTO [dbo].[booking_order]
						   ([user_id],[course_id]
						   ,[booking_username]
						   ,[phone]
						   ,[bookling_status]
						   ,[settle_status]
						   ,[member_number]
						   ,[appointment_date]
						   ,[booking_date]
						   ,[deposit]
						   ,[pay_balance]
						   ,[sub_total]
                           ,[total]
						   ,[commission]
						   ,[total_commission]
						   ,[description]
						   ,[tstamp]
						   ,[update_time])
					 VALUES
						   (@user_id
						   ,@course_id
						   ,@booking_username
						   ,@phone
						   ,@bookling_status
						   ,@settle_status
						   ,@member_number
						   ,@appointment_date
						   ,@booking_date
						   ,@deposit
						   ,@pay_balance
                           ,@sub_total
						   ,@total
						   ,@commission
						   ,@total_commission
						   ,@description
						   ,GETDATE()
						   ,GETDATE())";
                DbCommand cmd = dh.GetSqlStringCommond(sql);
                SqlParameter[] parameters = {
                                            new SqlParameter("@user_id",order.UserId),
                                            new SqlParameter("@course_id",order.CourseId),
                                            new SqlParameter("@booking_username",order.BookingUserName),
                                            new SqlParameter("@bookling_status",order.BookingStatus),
                                            new SqlParameter("@settle_status",order.SettleStatus),
                                            new SqlParameter("@phone",order.Phone),
                                             new SqlParameter("@member_number",order.MemberNumber),
                                              new SqlParameter("@appointment_date",order.AppointmentDate),
                                               new SqlParameter("@booking_date",order.BookingDate),
                                                new SqlParameter("@deposit",order.Deposit),
                                                 new SqlParameter("@pay_balance",order.PayBalance),
                                                  new SqlParameter("@sub_total",order.SubTotal),
                                                  new SqlParameter("@total",order.Total),
                                                   new SqlParameter("@commission",order.Commission),
                                                    new SqlParameter("@total_commission",order.TotalCommission),
                                                     new SqlParameter("@description",order.Description)
                                        };
                int result = dh.ExecuteNonQuery(cmd, parameters);

                if (result == 0)
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

        public BaseResult BookingMultiInsert(List<BookingOrder> orderlist)
        {
            BaseResult baseResult = new BaseResult();
            try
            {
                string insertData = "";
                foreach (BookingOrder order in orderlist)
                {
                    insertData += string.Format("( '{0}',{1},'{2}','{3}','{4}','{5}',{6},'{7}','{8}',{9},{10},{11},{12},{13},{14},'{15}',GETDATE(),GETDATE()),"
                                   , order.UserId, order.CourseId, order.BookingUserName, order.Phone, order.BookingStatus
                                   , order.SettleStatus, order.MemberNumber, order.AppointmentDate, order.BookingDate, order.Deposit
                                   , order.PayBalance, order.SubTotal, order.Total, order.Commission, order.TotalCommission, order.Description);
                }
                string sql = @"INSERT INTO [dbo].[booking_order]
						   ([user_id],[course_id]
						   ,[booking_username]
						   ,[phone]
						   ,[bookling_status]
						   ,[settle_status]
						   ,[member_number]
						   ,[appointment_date]
						   ,[booking_date]
						   ,[deposit]
						   ,[pay_balance]
                           ,[sub_total]
						   ,[total]
						   ,[commission]
						   ,[total_commission]
						   ,[description]
						   ,[tstamp]
						   ,[update_time])
					 VALUES
						   {0}";
                DbCommand cmd = dh.GetSqlStringCommond(string.Format(sql,insertData.Substring(0, insertData.Length-1)));
                int result = dh.ExecuteNonQuery(cmd);

                if (result == 0)
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

        public BaseResult BookingUpdate(BookingOrder order)
        {
            BaseResult baseResult = new BaseResult();
            try
            {
                string sql = @"UPDATE [dbo].[booking_order] SET
						    [course_id]=@course_id
						   ,[booking_username]= @booking_username
						   ,[phone] = @phone
						   ,[member_number]=@member_number
						   ,[appointment_date]=@appointment_date
						   ,[booking_date]=@booking_date
						   ,[deposit]=@deposit
						   ,[pay_balance]=@pay_balance
                           ,[sub_total]=@sub_total
						   ,[total]=@total
						   ,[commission]=@commission
						   ,[total_commission]=@total_commission
						   ,[description]=@description
						   ,[update_time]=GETDATE() 
					  WHERE order_id=@order_id and [user_id]=@user_id";
                DbCommand cmd = dh.GetSqlStringCommond(sql);
                SqlParameter[] parameters = {
                                            new SqlParameter("@order_id",order.OrderId),
                                            new SqlParameter("@user_id",order.UserId),
                                            new SqlParameter("@course_id",order.CourseId),
                                            new SqlParameter("@booking_username",order.BookingUserName),
                                            new SqlParameter("@phone",order.Phone),
                                             new SqlParameter("@member_number",order.MemberNumber),
                                              new SqlParameter("@appointment_date",order.AppointmentDate),
                                               new SqlParameter("@booking_date",order.BookingDate),
                                                new SqlParameter("@deposit",order.Deposit),
                                                 new SqlParameter("@pay_balance",order.PayBalance),
                                                  new SqlParameter("@sub_total",order.SubTotal),
                                                  new SqlParameter("@total",order.Total),
                                                   new SqlParameter("@commission",order.Commission),
                                                    new SqlParameter("@total_commission",order.TotalCommission),
                                                     new SqlParameter("@description",order.Description)
                                        };
                int result = dh.ExecuteNonQuery(cmd, parameters);

                if (result == 0)
                {
                    baseResult.Code = 301;
                    baseResult.Message = "Update Fail";
                }
            }
            catch (Exception ex)
            {
                baseResult.Code = 300;
                baseResult.Message = ex.StackTrace + ex.Message;
            }
            return baseResult;
        }

        public BaseResult BookingHandle(BookingOrder order)
        {
            BaseResult baseResult = new BaseResult();
            try
            {
                string sql = @"UPDATE[dbo].[booking_order] SET 
                            [update_time])=GETDATE() {0}
					  WHERE [user_id]=@user_id";
                string handle = "";
                if (!string.IsNullOrEmpty(order.BookingStatus))
                    handle += string.Format(sql, " ,[bookling_status]=@bookling_status ");
                if (!string.IsNullOrEmpty(order.SettleStatus))
                    handle += string.Format(sql, " ,[settle_status]=@settle_status ");

                DbCommand cmd = dh.GetSqlStringCommond(string.Format(sql, handle));
                SqlParameter[] parameters = {
                                            new SqlParameter("@user_id",order.UserId),
                                            new SqlParameter("@settle_status",order.SettleStatus),
                                            new SqlParameter("@bookling_status",order.BookingStatus)
                                        };
                int result = dh.ExecuteNonQuery(cmd, parameters);

                if (result == 0)
                {
                    baseResult.Code = 301;
                    baseResult.Message = "Update Fail";
                }
            }
            catch (Exception ex)
            {
                baseResult.Code = 300;
                baseResult.Message = ex.StackTrace + ex.Message;
            }
            return baseResult;
        }

        public List<GolfPlace> GetGolfPlace() {
                List<GolfPlace> placelist = new List<GolfPlace>();
                string sql = @"SELECT [course_id] ,[course_name],[commission],[status],[description]
					  FROM [dbo].[golf_course]
                      WHERE status='open'";
                DbCommand cmd = dh.GetSqlStringCommond(sql);
                DataTable dt = dh.ExecuteDataTable(cmd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        GolfPlace place = new GolfPlace();
                        place.CourseId = dt.Rows[i]["course_id"].ToString();
                        place.CourseName = dt.Rows[i]["course_name"].ToString();
                        place.Commission = Convert.ToInt32(dt.Rows[i]["course_name"]);
                        place.Status = dt.Rows[i]["status"].ToString();
                        place.Description = dt.Rows[i]["description"].ToString();
                        placelist.Add(place);
                    }
                }
            return placelist;
        }

        public BookingOrderList GetBookingOrderList(BookingOrderListQuery orderQuery)
        {
            BookingOrderList orderlist = new BookingOrderList();
            try {
                StringBuilder sb = new StringBuilder();
                sb.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(orderQuery.CourseId))
                {
                    sb.AppendFormat(" and course_id={0} ", orderQuery.CourseId);
                }
                if (!string.IsNullOrEmpty(orderQuery.SettleStatus))
                {
                    sb.AppendFormat(" and settle_status='{0}' ", orderQuery.SettleStatus);
                }
                if (!string.IsNullOrEmpty(orderQuery.BookingStatus))
                {
                    sb.AppendFormat(" and bookling_status='{0}' ", orderQuery.BookingStatus);
                }
                if (!string.IsNullOrEmpty(orderQuery.DateType))
                {
                    sb.AppendFormat(" and {0} between '{0}' and '{1}' ", orderQuery.DateType,orderQuery.BeginDate,orderQuery.EndDate);
                }
                if (string.IsNullOrEmpty(orderQuery.SearchFiled))
                {
                    if (orderQuery.SearchType == "%")
                        sb.AppendFormat(" and {0} LIKE '%{1}%'", orderQuery.SearchFiled, orderQuery.SearchValue);
                    else if (orderQuery.SearchType == "!%")
                        sb.AppendFormat(" and {0} NOT LIKE '%{1}%'", orderQuery.SearchFiled, orderQuery.SearchValue);
                    else if(orderQuery.SearchType == "%~")
                        sb.AppendFormat(" and {0} LIKE '%{1}'", orderQuery.SearchFiled, orderQuery.SearchValue);
                    else if (orderQuery.SearchType == "~%")
                        sb.AppendFormat(" and {0} LIKE '{1}%'", orderQuery.SearchFiled, orderQuery.SearchValue);
                    else
                    sb.AppendFormat(" and {0}{1} '{2}' ", orderQuery.SearchFiled, orderQuery.SearchType, orderQuery.SearchValue);
                }
                if (string.IsNullOrEmpty(orderQuery.SortFiled))
                {
                    sb.AppendFormat(" order by {0} {1} ", orderQuery.SortFiled, orderQuery.SortType);
                }
                string sql = @"SELECT [order_id]
						  ,[user_id]
						  ,[course_id]
						  ,[booking_username]
						  ,[phone]
						  ,[bookling_status]
						  ,[settle_status]
						  ,[member_number]
						  ,[appointment_date]
						  ,[booking_date]
						  ,[deposit]
						  ,[pay_balance]
						  ,[total]
                          ,[sub_total]
						  ,[commission]
						  ,[total_commission]
						  ,[description]
						  ,[tstamp]
						  ,[update_time]
					  FROM [dbo].[booking_order] {0}";
                DbCommand cmd = dh.GetSqlStringCommond(string.Format(sql,sb.ToString()));
                DataTable dt = dh.ExecuteDataTable(cmd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        BookingOrder order = new BookingOrder();
                        order.OrderId = Convert.ToInt32(dt.Rows[i]["order_id"].ToString());
                        order.UserId = dt.Rows[i]["user_id"].ToString();
                        order.Phone = dt.Rows[i]["phone"].ToString();
                        order.CourseId = dt.Rows[i]["course_id"].ToString();
                        order.BookingUserName = dt.Rows[i]["booking_username"].ToString();
                        order.BookingStatus = dt.Rows[i]["bookling_status"].ToString();
                        order.SettleStatus = dt.Rows[i]["settle_status"].ToString();
                        order.BookingDate = Convert.ToDateTime(dt.Rows[i]["booking_date"]);
                        order.AppointmentDate = Convert.ToDateTime(dt.Rows[i]["appointment_date"]);
                        order.MemberNumber = Convert.ToInt32(dt.Rows[i]["member_number"].ToString());
                        order.Deposit = Convert.ToInt32(dt.Rows[i]["deposit"].ToString());
                        order.PayBalance = Convert.ToInt32(dt.Rows[i]["pay_balance"].ToString());
                        order.Total = Convert.ToInt32(dt.Rows[i]["total"].ToString());
                        order.SubTotal = Convert.ToInt32(dt.Rows[i]["sub_total"].ToString());
                        order.Commission = Convert.ToInt32(dt.Rows[i]["commission"].ToString());
                        order.TotalCommission = Convert.ToInt32(dt.Rows[i]["total_commission"].ToString());
                        order.Description = dt.Rows[i]["description"].ToString();
                        order.SettleStatus = dt.Rows[i]["settle_status"].ToString();
                        order.CreateDate = Convert.ToDateTime(dt.Rows[i]["tstamp"]);
                        orderlist.bookingOrderList.Add(order);
                    }
                }
                else {
                    orderlist.BaseResult.Code = 401;
                    orderlist.BaseResult.Message = "Not Data";
                }
            }
            catch (Exception ex)
            {
                orderlist.BaseResult.Code = 300;
                orderlist.BaseResult.Message = ex.StackTrace + ex.Message;
            }
            return orderlist;
        }
    }
}