using Booking.DacLayer;
using Booking.Models;
using Booking.Utilities;
using Booking.Utilities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Booking.Controllers
{
    public class AccountController : CustomController
    {
        public JavaScriptSerializer serializer = new JavaScriptSerializer();
        public AccountDac account = new AccountDac();
        public ActionResult Login()

        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                //发放一个Expires为30分钟的cookie
                HttpCookie cookie1 = new HttpCookie("IsContinue", "");
                cookie1.Expires = DateTime.Now.AddMinutes(30);
                Response.Cookies.Add(cookie1);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(string loginName, string password)
        {
            if (Request.IsAjaxRequest())
            {
                string pwd = MD5Hash.CreateMD5Hash(password); 
                User u = account.CheckLogin(loginName, pwd);
                if (u.BaseResult.Code == 0)
                {
                    CreateAuthenticationTicket(u);
                    string urlAction = Url.Action("Index", "Home");
                    return Json(new
                    {
                        isSuccess = true,
                        urlAction = urlAction
                    }, JsonRequestBehavior.AllowGet);
                }
                else {
                    return Json(new
                    {
                        isSuccess = false,
                        errorCode = u.BaseResult.Code,
                        message = u.BaseResult.Message,
                        urlAction = Url.Action("Login", "Account")
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 给用户建立Cookie
        /// </summary>
        /// <param name="user"></param>
        public void CreateAuthenticationTicket(User user)
        {
            string userData = serializer.Serialize(user);
            HttpCookie cookie1 = new HttpCookie("IsContinue", "");
            cookie1.Expires = DateTime.Now.AddMinutes(30);
            Response.Cookies.Add(cookie1);

            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                1, user.UserId.ToString(), DateTime.Now, DateTime.Now.AddDays(7), true, userData);
            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            Response.Cookies.Add(faCookie);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(User user)
        {
            BaseResult result = new BaseResult();
            user.Password = MD5Hash.CreateMD5Hash(user.Password);
            user.Status = "pending";
            user.RoleId =1;
            result = account.InsertUser(user);
            return Content("<script>alert('가입이 완료되습니다.\\n로그인을 위해서는 관리자의 승인이 필요합니다.\\n승인이 완료될때까지 대기하기 바랍니다');window.location.href='/Account/Login';</script>");
        }
        [HttpPost]
        public JsonResult CheckLoginName(string loginname)
        {
            bool isSuccess=account.CheckLoginName(loginname);
            return Json(new { isSuccess = isSuccess }, JsonRequestBehavior.AllowGet);   
        }
        [HttpPost]
        public JsonResult CheckEmail(string email)
        {
            bool isSuccess = account.CheckEmail(email);
            return Json(new { isSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
    }
}