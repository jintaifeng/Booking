using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Booking.Utilities
{
    public class CodeResult : ActionResult
    {
        private string prefix { get; set; }
        public override void ExecuteResult(ControllerContext context)
        {
            //CreateCheckCodeImage(context.HttpContext.Request.QueryString.Get("img"), context.HttpContext.Response);
            CreateCheckCodeImage(GenerateCheckCode(), context.HttpContext.Response);
        }
        public CodeResult(string sid)
        {
            prefix = sid;
        }
        private string GenerateCheckCode()
        {
            int number;
            char code;
            string checkCode = String.Empty;

            System.Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                number = random.Next();

                if (number % 2 == 0)
                    code = (char)('1' + (char)(number % 9));
                else
                    code = (char)('A' + (char)(number % 26));
                checkCode += code.ToString();
            }

            HttpContext.Current.Response.Cookies.Add(new HttpCookie(prefix + "Captcha", checkCode));


            return checkCode;
        }

        private void CreateCheckCodeImage(string checkCode, HttpResponseBase response)
        {
            if (checkCode == null || checkCode.Trim() == String.Empty)
                return;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 16.5)), 35);
            Graphics g = Graphics.FromImage(image);

            try
            {
                //生成随机生成器
                Random random = new Random();

                //清空图片背景色
                g.Clear(Color.White);
                // Pen drawPen = new Pen(Color.Blue);

                // 添加多种颜色 hooyes
                Color[] colors = { Color.Blue, Color.Silver, Color.SlateGray, Color.Turquoise, Color.Violet, Color.Turquoise, Color.Tomato, Color.Thistle, Color.Teal, Color.SteelBlue };




                //画图片的背景噪音线
                for (int i = 0; i < 9; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);

                    Pen drawPen2 = new Pen(colors[i]);
                    g.DrawLine(drawPen2, x1, y1, x2, y2);

                }
                // drawPen.Dispose(); Tahoma
                Font font = new System.Drawing.Font("Arial", 13, (System.Drawing.FontStyle.Bold));
                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Black, Color.Gray, 1.2f, true);
                g.DrawString(checkCode, font, brush, 2, 1);
                // g.DrawString("J", font, brush, 1, 115);
                font.Dispose();
                brush.Dispose();

                //画图片的前景噪音点
                for (int i = 0; i < 20; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);

                    image.SetPixel(x, y, Color.FromArgb(0x8b, 0x8b, 0x8b));
                }

                //画图片的边框线
                Pen borderPen = new Pen(Color.Transparent);
                g.DrawRectangle(borderPen, 0, 0, image.Width - 1, image.Height - 1);
                borderPen.Dispose();

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] buffer = ms.ToArray();
                ms.Dispose();
                response.ClearContent();
                response.ContentType = "image/bmp";
                response.BinaryWrite(buffer);
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
        /// <summary>
        /// 检查回传的验证码是否正确
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="Captcha"></param>
        /// <returns></returns>
        public static bool VaildateCaptcha(string sid, string Captcha)
        {
            try
            {
                if (string.IsNullOrEmpty(Captcha))
                {
                    return false;
                }
                if (HttpContext.Current.Request.Cookies[sid + "Captcha"] == null)
                {
                    return false;
                }
                string oCaptcha = HttpContext.Current.Request.Cookies[sid + "Captcha"].Value;
                return (oCaptcha.ToLower() == Captcha.ToLower());
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 清除验证码
        /// </summary>
        /// <param name="sid"></param>
        public static void ClearCaptcha(string sid)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[sid + "Captcha"] != null)
                {
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie(sid + "Captcha", ""));
                    HttpContext.Current.Response.Cookies.Remove(sid + "Captcha");
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}