using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Devx
{
    public class HTTP
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void Request(HttpRequest http)
        {
            try
            {
                HttpWebRequest request = HTTP.CreateRequest(http);
                using (HttpWebResponse response = HTTP.BeginRequest(http, request))//开始处理数据
                using (Stream responseStream = response.GetResponseStream())//接收数据
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    #region 提取返回的字符集
                    if (http.Encoding == null)
                    {
                        var contentType = new System.Net.Mime.ContentType(response.Headers[HttpResponseHeader.ContentType]);
                        Encoding encoding = null;
                        if (!string.IsNullOrEmpty(contentType.CharSet))
                        {
                            if (contentType.CharSet.Equals("utf8", StringComparison.OrdinalIgnoreCase))
                            {
                                contentType.CharSet = "utf-8";
                            }
                            try
                            {
                                encoding = Encoding.GetEncoding(contentType.CharSet);
                            }
                            catch
                            {
                                encoding = null;
                            }
                        }
                        http.Encoding = encoding ?? Encoding.UTF8;
                    }
                    #endregion

                    //没有任何响应值
                    if (responseStream == null)
                    {
                        throw new Exception("Response Stream is null");
                    }

                    byte[] buffer = new byte[1024];//获取长度
                    int c = 0;
                    while ((c = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, c);
                    }
                    buffer = null;
                    buffer = memoryStream.ToArray();
                    if (buffer == null || buffer.Length == 0)
                    {
                        throw new Exception("Response Stream Length = 0");
                    }
                    if (http.Data != null)//执行数据处理
                    {
                        http.Data(http, buffer);
                    }
                    if (http.Html != null)//执行HTML代码
                    {
                        http.Html(http, http.Encoding.GetString(buffer));
                    }
                }
            }
            catch (Exception ex)
            {
                if (http.Error!=null)
                    http.Error(http, ex);
            }
        }

        private static HttpWebRequest CreateRequest(HttpRequest http)
        {
            bool isPost = "POST".Equals(http.Method, StringComparison.OrdinalIgnoreCase);
            Uri uri = new Uri(http.Url);
            HttpWebRequest req = HttpWebRequest.Create(uri) as HttpWebRequest;
            req.Method = isPost ? "POST" : "GET";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //RefererHTTP
            if (!string.IsNullOrEmpty(http.Referer))
            {
                req.Referer = http.Referer;
            }
            req.Headers[HttpRequestHeader.AcceptLanguage] = "zh-CN";
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.117 Safari/537.36";
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            req.KeepAlive = true;
            req.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Reload);
            //CookieContainer
            if (http.Cookies != null)
            {
                req.CookieContainer = http.Cookies;
            }
            else
            {
                http.Cookies = new CookieContainer();
                req.CookieContainer = http.Cookies;//创建一个空的请求
            }

            if (http.Timeout > 0)
                req.Timeout = http.Timeout;
            if (http.ReadWriteTimeout > 0)
                req.ReadWriteTimeout = http.ReadWriteTimeout;

            //https支持
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
            {
                return true;
            };
            //其他数据处理
            if (http.BeforeSend != null)
            {
                http.BeforeSend(req);
            }
            return req;
        }

        /// <summary>
        /// 开始请求
        /// </summary>
        private static HttpWebResponse BeginRequest(HttpRequest http, HttpWebRequest request)
        {
            bool isPost = "POST".Equals(http.Method, StringComparison.OrdinalIgnoreCase);

            #region 创建Post请求
            if (isPost && !string.IsNullOrEmpty(http.Body))
            {
                //设置请求格式
                request.ContentType = "application/x-www-form-urlencoded";
                #region POST Write
                //发送数据
                using (Stream requestStream = request.GetRequestStream())
                {

                    if (requestStream != null)
                    {
                        var postData = Encoding.UTF8.GetBytes(http.Body);
                        requestStream.Write(postData, 0, postData.Length);
                        requestStream.Flush();
                        requestStream.Close();
                    }
                }
                #endregion
            }
            else if (isPost)
            {
                request.ContentLength = 0;
            }
            #endregion

            //获取响应的结果
            return (HttpWebResponse)request.GetResponse();
        }


        /// <summary>
        /// 转换参数
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string ToString(System.Collections.Specialized.NameValueCollection values)
        {
            StringBuilder bodyBuilder = new StringBuilder();
            foreach (string key in values)
            {
                bodyBuilder.AppendFormat("&{0}={1}", key, System.Web.HttpUtility.UrlEncode(values[key], Encoding.UTF8).Replace("+", "%20"));
            }
            if (values.Count > 0)
            {
                bodyBuilder.Remove(0, 1);
            }
            return bodyBuilder.ToString();
        }
    }
}
