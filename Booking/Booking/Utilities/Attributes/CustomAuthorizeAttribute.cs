using Booking.Models;
using Booking.Utilities.CustomUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Booking.Utilities.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            bool isAuth = false;
            if (httpContext.User.Identity.IsAuthenticated)// && httpContext.Request.Cookies.AllKeys.Contains("UserInfo") == true)
            {
                var authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    User serializeModel = null;
                    CustomPrincipal newUser = null;
                    try
                    {
                        serializeModel = serializer.Deserialize<User>(authTicket.UserData);
                        newUser = new CustomPrincipal(serializeModel.Name, serializeModel.RoleName);
                        newUser.UserId = serializeModel.UserId;
                        newUser.LoginName = serializeModel.LoginName;
                        newUser.Email = serializeModel.Email;
                        newUser.Name = serializeModel.Name;
                        newUser.Phone = serializeModel.Phone;
                        newUser.Status = serializeModel.Status;
                        newUser.RoleId = serializeModel.RoleId;
                        newUser.RoleName = serializeModel.RoleName;
                        newUser.UpdateTime = serializeModel.UpdateTime;
                        newUser.CreateTime = serializeModel.CreateTime;
                        HttpContext.Current.User = newUser;
                        if (this.Roles == "")
                        {
                            isAuth = true;
                        }
                        if (newUser.IsInRole(this.Roles))
                        {
                            isAuth = true;
                        }
                        else {
                            isAuth = false;
                        }
                    }
                    catch (Exception)
                    {
                        isAuth = false;
                    }
                }

            }
            return isAuth;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            //判断用户是否登录
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                //redirect to login page
                filterContext.Result = new HttpUnauthorizedResult();
                return;
            }
            else
            {
                if (!AuthorizeCore(filterContext.HttpContext))
                {
                    ContentResult content = new ContentResult();
                    content.Content = "<script type='text/javascript'>alert('권한이 없습니다.');window.location.href='/Home/Index'</script>";
                    filterContext.Result = content;
                }
                
            }
            //base.OnAuthorization(filterContext);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            ContentResult content = new ContentResult();
            content.Content = "<script type='text/javascript'>alert('로그아웃 되었습니다.');window.location.href='/'</script>";
            filterContext.Result = content;
        }
    }
}