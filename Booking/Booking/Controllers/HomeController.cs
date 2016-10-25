using Booking.Common;
using Booking.DacLayer;
using Booking.Models;
using Booking.Utilities;
using Booking.Utilities.Attributes;
using Booking.Utilities.Base;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Booking.Controllers
{
    [CustomAuthorize]
    public class HomeController : CustomController
    {
        private static NLog.Logger log = NLog.LogManager.GetLogger("NLog");
        BookingDac booking = new BookingDac();
        public ActionResult Index()
        {
            string userid = "";
            if (User.RoleName != "Admin")
                userid = User.UserId;
            List<BookingGroup> list = new List<BookingGroup>();
            list = booking.GetTodayBookingStatistics(userid);
            return View(list);
        }
        public ActionResult ViewList(BookingOrderListQuery orderQuery ,PageData page)
        {
            BookingOrderList list = new BookingOrderList();
            ViewBag.Place = booking.GetGolfPlace();

            if (string.IsNullOrEmpty(orderQuery.DateType)) {
                orderQuery.DateType = "booking_date";
            }
                DateTime dt = DateTime.Now;
            if (orderQuery.BeginDate.Year<2010)
            {
                orderQuery.BeginDate = dt.AddDays(1 - dt.Day).AddMonths(-1);
            }
            if (orderQuery.EndDate.Year < 2010)
            {
                orderQuery.EndDate = dt.AddDays(1 - dt.Day).AddMonths(2).AddDays(-1);
            }
            if (User.RoleName == "Admin")
                orderQuery.IsAdmin = true;
            else
                orderQuery.UserId = User.UserId;
            ViewBag.Query = orderQuery;
            list = booking.GetBookingOrderList(orderQuery, page);
            TempData[User.UserId] = list;
            return View(list);
        }
        public ActionResult ViewGroup(BookingOrderListQuery orderQuery)
        {
            List<BookingStatistics> list = new List<BookingStatistics>();
            ViewBag.Place = booking.GetGolfPlace();

            if (string.IsNullOrEmpty(orderQuery.DateType))
            {
                orderQuery.DateType = "booking_date";
            }
            if (string.IsNullOrEmpty(orderQuery.SortFiled))
            {
                orderQuery.SortFiled = "booking_date";
            }
            if (string.IsNullOrEmpty(orderQuery.SortType))
            {
                orderQuery.SortType = "ASC";
            }
            DateTime dt = DateTime.Now;
            if (orderQuery.BeginDate.Year < 2010)
            {
                orderQuery.BeginDate = dt.AddDays(1 - dt.Day);
            }
            if (orderQuery.EndDate.Year < 2010)
            {
                orderQuery.EndDate = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1);
            }
            if (User.RoleName == "Admin")
                orderQuery.IsAdmin = true;
            else
                orderQuery.UserId = User.UserId;
            ViewBag.Query = orderQuery;
            list = booking.GetBookingStatistics(orderQuery);
            return View(list);
        }    
        public ActionResult Clause(string mode)
        {
            ViewBag.Mode = mode;
            return View();
        }
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult GolfPlace()
        {
            List<GolfPlace> placeList = new List<GolfPlace>();
            placeList = booking.GetGolfPlace();
            return View(placeList);
        }

        [CustomAuthorize]
        public FileResult ExportExcel()
        {
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("부킹관리");
            //获取list数据
            var list = TempData[User.UserId] as BookingOrderList;

            ICellStyle HeadercellStyle = book.CreateCellStyle();
            HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            //字体
            NPOI.SS.UserModel.IFont headerfont = book.CreateFont();
            headerfont.Boldweight = (short)FontBoldWeight.Bold;
            HeadercellStyle.SetFont(headerfont);

            string[] field = new[] { "No", "매니저", "접수일", "라운딩예약일", "요일", "시간", "예약자", "연락처", "골프장", "인원", "현장(인당)",
                "선입(인당)", "합계(인당)", "그린프총금액", "수수료(인당)", "수수료총금액", "비고사항", "예약구분", "취소일","정산구분", "정산일" };
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            for(int i=0;i<field.Length;i++)
            {
                ICell cell = row1.CreateCell(i);
                cell.SetCellValue(field[i]);
                cell.CellStyle = HeadercellStyle;
            }
            string[] weekdays = { "일", "월", "화", "수", "목", "금", "토" };

            //创建格式化 实例对象
            IDataFormat format = book.CreateDataFormat();
            //设置货币格式
            ICellStyle cellStyle2 = book.CreateCellStyle();
            cellStyle2.DataFormat = format.GetFormat("#,##0");

            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.bookingOrderList.Count; i++)
            {
                var data = list.bookingOrderList[i];
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(i+1);
                rowtemp.CreateCell(1).SetCellValue(data.UserName);
                rowtemp.CreateCell(2).SetCellValue(data.AppointmentDate.ToString("yyyy-MM-dd"));
                rowtemp.CreateCell(3).SetCellValue(data.BookingDate.ToString("yyyy-MM-dd"));
                rowtemp.CreateCell(4).SetCellValue(weekdays[Convert.ToInt32(data.BookingDate.DayOfWeek)]);
                rowtemp.CreateCell(5).SetCellValue(TimeHandle(data.BookingDate.Hour.ToString()) + ":" + TimeHandle(data.BookingDate.Minute.ToString()));
                rowtemp.CreateCell(6).SetCellValue(data.BookingUserName);
                rowtemp.CreateCell(7).SetCellValue(data.Phone);
                rowtemp.CreateCell(8).SetCellValue(data.CourseName);
                rowtemp.CreateCell(9).SetCellValue(data.MemberNumber);
                ICell pb_cell1 = rowtemp.CreateCell(10);
                pb_cell1.SetCellValue(data.PayBalance);
                pb_cell1.CellStyle = cellStyle2;
                ICell pb_cell2 = rowtemp.CreateCell(12);
                pb_cell2.SetCellValue(data.Deposit);
                pb_cell2.CellStyle = cellStyle2;
                ICell pb_cell3 = rowtemp.CreateCell(13);
                pb_cell3.SetCellValue(data.SubTotal);
                pb_cell3.CellStyle = cellStyle2;
                ICell pb_cell4 = rowtemp.CreateCell(14);
                pb_cell4.SetCellValue(data.Total);
                pb_cell4.CellStyle = cellStyle2;
                ICell pb_cell5 = rowtemp.CreateCell(15);
                pb_cell5.SetCellValue(data.Commission);
                pb_cell5.CellStyle = cellStyle2;
                ICell pb_cell6 = rowtemp.CreateCell(16);
                pb_cell6.SetCellValue(data.TotalCommission);
                pb_cell6.CellStyle = cellStyle2;
                rowtemp.CreateCell(16).SetCellValue(data.Description);
                rowtemp.CreateCell(17).SetCellValue(data.BookingStatus=="success"?"완료":"취소");
                rowtemp.CreateCell(18).SetCellValue(data.BookingStatus =="close"?data.UpdateDate.ToString("yyyy-MM-dd") : "");
                rowtemp.CreateCell(19).SetCellValue(data.SettleStatus == "success" ? "완료" : "대기");
                rowtemp.CreateCell(20).SetCellValue(data.SettleStatus == "success" ? data.UpdateDate.ToString("yyyy-MM-dd") : "");
                sheet1.AutoSizeColumn(i);
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            string filename = "부킹관리" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            return File(ms, "application/vnd.ms-excel", filename);
        }

        public string TimeHandle(string s)
        {
            if (s.Length == 1)
                s = "0" + s;
            return s;
        }
    }
}