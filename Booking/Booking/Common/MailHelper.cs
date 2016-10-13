using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;

namespace Booking.Common
{
    public class MailHelper
    {
        public static void SendMail(string subject,string body,string emailaddress) {
            MailMessage mail = new MailMessage();
            mail.To.Add(new MailAddress(emailaddress));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            Thread threadSendMails = new Thread(delegate () { smtp.Send(mail); }); //多线程
            threadSendMails.Start();
        }
    }
}