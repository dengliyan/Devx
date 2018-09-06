using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Devx
{
    public partial class HTTP
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
                    //没有任何响应值
                    if (responseStream == null)
                    {
                        throw new Exception("Response Stream is null");
                    }
                    if ((http.Html != null) && http.Encoding == null)
                    {
                        http.Encoding = GetEncoding(response);
                    }
                    byte[] buffer = new byte[1024];//获取长度
                    int c = 0;
                    while ((c = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, c);
                    }
                    http.StatusCode = response.StatusCode;
                    HTTP.Handler(http, memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                if (http.Error != null)
                {
                    http.Error(http, ex);
                }
            }
        }

        private static Encoding GetEncoding(HttpWebResponse response)
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
            return encoding ?? Encoding.UTF8;
        }

        private static void Handler(HttpRequest http, byte[] buffer)
        {
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

        internal static HttpWebRequest CreateRequest(HttpRequest http)
        {
            bool isPost = "POST".Equals(http.Method, StringComparison.OrdinalIgnoreCase);
            if (!isPost && !string.IsNullOrEmpty(http.Body))
            {
                var url = http.Url;
                http.Url = url + (url.IndexOf("?") >= 0 ? "&" : "?") + http.Body;
            }
            Uri uri = new Uri(http.Url);
            HttpWebRequest req = HttpWebRequest.Create(uri) as HttpWebRequest;
            req.Method = isPost ? "POST" : "GET";
            req.Accept = "text/html,application/xhtml+xml,application/xml;application/json;q=0.9,*/*;q=0.8";
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

            if (http.Url.ToLower().StartsWith("https://"))
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>//https支持
                {
                    return true;
                };
            }
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
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
            if (isPost)
            {
                if (string.IsNullOrEmpty(request.ContentType))
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                }
                if ((http.WriteStream == null || http.WriteStream.Length == 0) && !string.IsNullOrWhiteSpace(http.Body))
                {
                    var postData = Encoding.UTF8.GetBytes(http.Body);
                    http.WriteStream = new byte[postData.Length];
                    postData.CopyTo(http.WriteStream, 0);
                }
                http.WriteStream = http.WriteStream ?? new byte[] { };
                if (http.WriteStream.Length == 0)
                {
                    request.ContentLength = 0;
                }
                else
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        if (requestStream != null)
                        {
                            requestStream.Write(http.WriteStream, 0, http.WriteStream.Length);
                            requestStream.Flush();
                            requestStream.Close();
                        }
                    }
                }
            }
            #endregion

            if (http.HttpErrorIsSuccess)
            {
                try
                {
                    return (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    return (HttpWebResponse)ex.Response;
                }
            }
            else
            {
                return (HttpWebResponse)request.GetResponse();
            }
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

        public static string ToString(object data, bool encode = true)
        {
            var properties = ReflectionHelper.GetProperties(data.GetType());
            var propertyNames = new HashSet<string>();
            var param = new List<string>();
            foreach (var property in properties)
            {
                if (!propertyNames.Contains(property.Key))//如果此字段不存在
                {
                    if (ReflectionHelper.IsBasicClrType(property.Value.PropertyType))
                    {
                        propertyNames.Add(property.Key);
                        var o = ReflectionHelper.GetPropertyValue(data, property.Value);
                        if (o == null)
                        {
                            param.Add(property.Key);
                        }
                        else
                        {
                            param.Add(System.Web.HttpUtility.UrlEncode(property.Key) + "=" + (encode ? System.Web.HttpUtility.UrlEncode(o.ToString()) : o.ToString()));
                        }
                    }
                }
            }
            return param.Count == 0 ? "" : string.Join("&", param.ToArray());
        }

               

        public static byte[] CreateMultipartStream(string boundary, Dictionary<string, byte[]> files, Dictionary<string, string> form)
        {
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            // 最后的结束符  
            var endBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            using (var ms = new System.IO.MemoryStream())
            {
                string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                
                // 写入文件  
                foreach (var item in files)
                {

                    var header = string.Format(filePartHeader, "post.file", item.Key);
                    var headerBytes = Encoding.UTF8.GetBytes(header);

                    ms.Write(beginBoundary, 0, beginBoundary.Length);//写入分界
                    ms.Write(headerBytes, 0, headerBytes.Length);//写入头
                    ms.Write(item.Value, 0, item.Value.Length);//写入内容
                }


                // 写入字符串的Key  
                var stringKeyHeader = "\r\n--" + boundary +
                                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                       "\r\n\r\n{1}";

                //
                //--分隔
                //Content-Disposition: form-data; name=""
                //                    //
                //values
                //

                foreach (var e in form)
                {
                    var header = string.Format(stringKeyHeader, e.Key, e.Value);
                    var headerBytes = Encoding.UTF8.GetBytes(header);
                    ms.Write(headerBytes, 0, headerBytes.Length);//写入内容
                }


                // 写入最后的结束边界符  
                ms.Write(endBoundary, 0, endBoundary.Length);
                ms.Position = 0;
                byte[] bytes= ms.ToArray();
                return bytes;
            }
        }

    }
    
   
}
