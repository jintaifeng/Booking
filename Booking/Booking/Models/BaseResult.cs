using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booking.Models
{
    public class BaseResult
    {
        public int Code = 0;
        public string Message = string.Empty;

        public BaseResult()
        {
        }
        public BaseResult(int code, Exception ex)
        {
            this.Code = code;
            this.Message = "Message:" + ex.Message + ", StackTrace:" + ex.StackTrace;
        }
    }
}