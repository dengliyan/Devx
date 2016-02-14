using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using System.Net;

namespace Devx
{
    public class WebHelper
    {
        //浏览器列表
        private static readonly string[] _browserlist = new string[] { "ie", "chrome", "mozilla", "netscape", "firefox", "opera", "konqueror" };
        //搜索引擎列表
        private static readonly string[] _searchenginelist = new string[] { "baidu", "google", "360", "sogou", "bing", "msn", "sohu", "soso", "sina", "163", "yahoo", "jikeu" };
        //meta正则表达式
        private static readonly Regex _metaregex = new Regex("<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        #region Cookie

        /// <summary>
        /// 删除指定名称的Cookie
        /// </summary>
        /// <param name="name">Cookie名称</param>
        public static void DeleteCookie(string name)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 获得指定名称的Cookie值
        /// </summary>
        /// <param name="name">Cookie名称</param>
        /// <returns></returns>
        public static string GetCookie(string name)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
                return cookie.Value;

            return string.Empty;
        }

        /// <summary>
        /// 获得指定名称的Cookie中特定键的值
        /// </summary>
        /// <param name="name">Cookie名称</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string GetCookie(string name, string key)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null && cookie.HasKeys)
            {
                string v = cookie[key];
                if (v != null)
                    return v;
            }

            return string.Empty;
        }

        /// <summary>
        /// 设置指定名称的Cookie的值
        /// </summary>
        /// <param name="name">Cookie名称</param>
        /// <param name="value">值</param>
        public static void SetCookie(string name, string value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
                cookie.Value = value;
            else
                cookie = new HttpCookie(name, value);

            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 设置指定名称的Cookie的值
        /// </summary>
        /// <param name="name">Cookie名称</param>
        /// <param name="value">值</param>
        /// <param name="expires">过期时间</param>
        public static void SetCookie(string name, string value, double expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie == null)
                cookie = new HttpCookie(name);

            cookie.Value = value;
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 设置指定名称的Cookie特定键的值
        /// </summary>
        /// <param name="name">Cookie名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetCookie(string name, string key, string value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie == null)
                cookie = new HttpCookie(name);

            cookie[key] = value;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 设置指定名称的Cookie特定键的值
        /// </summary>
        /// <param name="name">Cookie名称</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expires">过期时间</param>
        public static void SetCookie(string name, string key, string value, double expires)
        {

            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie == null)
                cookie = new HttpCookie(name);

            cookie[key] = value;
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        #endregion

        #region 客户端信息

        public static string Query(string name, bool encode = true)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.QueryString != null)
            {
                if (encode)
                {
                    return System.Web.HttpUtility.HtmlEncode(HttpContext.Current.Request.QueryString[name] ?? string.Empty);
                }
                return HttpContext.Current.Request.QueryString[name] ?? string.Empty;
            }
            return string.Empty;
        }
        public static string Form(string name, bool encode = true)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.Form != null)
            {
                if (encode)
                {
                    return System.Web.HttpUtility.HtmlEncode(HttpContext.Current.Request.Form[name] ?? string.Empty);
                }
                return HttpContext.Current.Request.Form[name] ?? string.Empty;
            }
            return string.Empty;
        }


        /// <summary>
        /// 是否是get请求
        /// </summary>
        /// <returns></returns>
        public static bool IsGet()
        {
            return HttpContext.Current.Request.HttpMethod == "GET";
        }

        /// <summary>
        /// 是否是post请求
        /// </summary>
        /// <returns></returns>
        public static bool IsPost()
        {
            return HttpContext.Current.Request.HttpMethod == "POST";
        }

        /// <summary>
        /// 是否是Ajax请求
        /// </summary>
        /// <returns></returns>
        public static bool IsAjaxRequest()
        {
            return HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        /// <summary>
        /// 获得上次请求的url
        /// </summary>
        /// <returns></returns>
        public static string GetUrlReferrer()
        {
            Uri uri = HttpContext.Current.Request.UrlReferrer;
            if (uri == null)
                return string.Empty;

            return uri.ToString();
        }

        /// <summary>
        /// 获得请求的主机部分
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }

        /// <summary>
        /// 获得请求的url
        /// </summary>
        /// <returns></returns>
        public static string GetUrl()
        {
            return HttpContext.Current.Request.Url.ToString();
        }

        /// <summary>
        /// 获得请求的原始url
        /// </summary>
        /// <returns></returns>
        public static string GetRawUrl()
        {
            return HttpContext.Current.Request.RawUrl;
        }

        /// <summary>
        /// 获得请求的ip
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string ip = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            else
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();

            if (string.IsNullOrEmpty(ip) || !ValidateHelper.IsIP(ip))
                ip = "127.0.0.1";
            return ip;
        }

        /// <summary>
        /// 获得请求的浏览器类型
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserType()
        {
            string type = HttpContext.Current.Request.Browser.Type;
            if (string.IsNullOrEmpty(type))
                return "未知";

            return type.ToLower();
        }

        /// <summary>
        /// 获得请求的浏览器名称
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserName()
        {
            string name = HttpContext.Current.Request.Browser.Browser;
            if (string.IsNullOrEmpty(name))
                return "未知";

            return name.ToLower();
        }

        /// <summary>
        /// 获得请求的浏览器版本
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserVersion()
        {
            string version = HttpContext.Current.Request.Browser.Version;
            if (string.IsNullOrEmpty(version))
                return "未知";

            return version;
        }

        /// <summary>
        /// 获得请求客户端的操作系统类型
        /// </summary>
        /// <returns></returns>
        public static string GetOSType()
        {
            string type = "未知";
            string userAgent = HttpContext.Current.Request.UserAgent;

            if (userAgent.Contains("NT 6.1"))
            {
                type = "Windows 7";
            }
            else if (userAgent.Contains("NT 5.1"))
            {
                type = "Windows XP";
            }
            else if (userAgent.Contains("NT 6.2"))
            {
                type = "Windows 8";
            }
            else if (userAgent.Contains("android"))
            {
                type = "Android";
            }
            else if (userAgent.Contains("iphone"))
            {
                type = "IPhone";
            }
            else if (userAgent.Contains("Mac"))
            {
                type = "Mac";
            }
            else if (userAgent.Contains("NT 6.0"))
            {
                type = "Windows Vista";
            }
            else if (userAgent.Contains("NT 5.2"))
            {
                type = "Windows 2003";
            }
            else if (userAgent.Contains("NT 5.0"))
            {
                type = "Windows 2000";
            }
            else if (userAgent.Contains("98"))
            {
                type = "Windows 98";
            }
            else if (userAgent.Contains("95"))
            {
                type = "Windows 95";
            }
            else if (userAgent.Contains("Me"))
            {
                type = "Windows Me";
            }
            else if (userAgent.Contains("NT 4"))
            {
                type = "Windows NT4";
            }
            else if (userAgent.Contains("Unix"))
            {
                type = "UNIX";
            }
            else if (userAgent.Contains("Linux"))
            {
                type = "Linux";
            }
            else if (userAgent.Contains("SunOS"))
            {
                type = "SunOS";
            }
            return type;
        }

        /// <summary>
        /// 获得请求客户端的操作系统名称
        /// </summary>
        /// <returns></returns>
        public static string GetOSName()
        {
            string name = HttpContext.Current.Request.Browser.Platform;
            if (string.IsNullOrEmpty(name))
                return "未知";

            return name;
        }

        /// <summary>
        /// 判断是否是浏览器请求
        /// </summary>
        /// <returns></returns>
        public static bool IsBrowser()
        {
            string name = GetBrowserName();
            foreach (string item in _browserlist)
            {
                if (name.Contains(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 是否是移动设备请求
        /// </summary>
        /// <returns></returns>
        public static bool IsMobile()
        {
            if (HttpContext.Current.Request.Browser.IsMobileDevice)
                return true;

            bool isTablet = false;
            if (bool.TryParse(HttpContext.Current.Request.Browser["IsTablet"], out isTablet) && isTablet)
                return true;

            return false;
        }

        public static bool IsMobileByUserAgent()
        {
            var userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent != null)
            {
                string[] keywords = { "Android", "iPhone", "iPod", "iPad", "Windows Phone", "MQQBrowser" };
                if (!userAgent.Contains("Windows NT") || (userAgent.Contains("Windows NT") && userAgent.Contains("compatible; MSIE 9.0;")))
                {
                    if (!userAgent.Contains("Windows NT") && !userAgent.Contains("Macintosh"))
                    {
                        foreach (var keyword in keywords)
                        {
                            if (userAgent.Contains(keyword))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否是搜索引擎爬虫请求
        /// </summary>
        /// <returns></returns>
        public static bool IsCrawler()
        {
            bool result = HttpContext.Current.Request.Browser.Crawler;
            if (!result)
            {
                string referrer = GetUrlReferrer();
                if (referrer.Length > 0)
                {
                    foreach (string item in _searchenginelist)
                    {
                        if (referrer.Contains(item))
                            return true;
                    }
                }
            }
            return result;
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 文件上传到其他地方
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeOut"></param>
        /// <param name="fileKeyName"></param>
        /// <param name="filePath"></param>
        /// <param name="stringDict"></param>
        public static void PostData(
            string url
            , int timeOut
            , byte[] bytes
            , System.Collections.Specialized.NameValueCollection values)
        {
            string responseContent;
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            using (var memStream = new MemoryStream())
            {
                // 边界符  
                var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                // 边界符  
                var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
                // 最后的结束符  
                var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

                // 设置属性  
                webRequest.Method = "POST";
                webRequest.Timeout = timeOut;
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

                // 写入文件  
                string filePartHeader =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                     "Content-Type: application/octet-stream\r\n\r\n";
                var header = string.Format(filePartHeader, "x.temp", "");
                var headerbytes = Encoding.UTF8.GetBytes(header);

                memStream.Write(beginBoundary, 0, beginBoundary.Length);//写入分界
                memStream.Write(headerbytes, 0, headerbytes.Length);//写入头

                memStream.Write(bytes, 0, bytes.Length);//写入内容

                // 写入字符串的Key  
                var stringKeyHeader = "\r\n--" + boundary +
                                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                       "\r\n\r\n{1}\r\n";

                foreach (string key in values)
                {
                    header = string.Format(filePartHeader, key, System.Web.HttpUtility.UrlEncode(values[key], Encoding.UTF8).Replace("+", "%20"));
                    bytes = Encoding.UTF8.GetBytes(header);
                    memStream.Write(bytes, 0, bytes.Length);//写入内容
                }

                // 写入最后的结束边界符  
                memStream.Write(endBoundary, 0, endBoundary.Length);

                webRequest.ContentLength = memStream.Length;

                using (var requestStream = webRequest.GetRequestStream())
                {
                    memStream.Position = 0;
                    var tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                    using (var httpWebResponse = (HttpWebResponse)webRequest.GetResponse())
                    using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(),
                                                                    Encoding.GetEncoding("utf-8")))
                    {
                        responseContent = httpStreamReader.ReadToEnd();
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取远程路径数据
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>返回二进制流</returns>
        public static byte[] Get(string url, int timeout)
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = timeout * 1000;
            request.Method = "GET";
            request.Headers.Set("Pragma", "no-cache");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream streamReceive = response.GetResponseStream())
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buff = new byte[512];
                int c = 0;
                while ((c = streamReceive.Read(buff, 0, buff.Length)) > 0)
                {
                    ms.Write(buff, 0, c);
                }
                return ms.ToArray();
            }
        }

        public static byte[] Post(string url, int timeout, string param)
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            byte[] bs = Encoding.ASCII.GetBytes(param);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = timeout * 1000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bs.Length;
            request.Headers.Set("Pragma", "no-cache");
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bs, 0, bs.Length);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream streamReceive = response.GetResponseStream())
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[512];
                int c = 0;
                while ((c = streamReceive.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, c);
                }
                return ms.ToArray();
            }
        }
        public static bool IsUrlValid(string url)
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 30000;
            request.Method = "GET";
            request.Headers.Set("Pragma", "no-cache");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode status = response.StatusCode;

                return status == HttpStatusCode.Accepted || status == HttpStatusCode.NotModified;//304
            }
        }



    }
}
