﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpRequest
    {
        public HttpRequest()
        {
            Method = "GET";

            Timeout = 10000;

            ReadWriteTimeout = 30000;

            Cookies = new System.Net.CookieContainer();//创建一个默认

            HttpErrorIsSuccess = false;
        }

        /// <summary>
        /// 方法
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        /// 请求超时
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 发送超时
        /// </summary>
        public int ReadWriteTimeout { get; set; }

        /// <summary>
        /// 数据内容
        /// </summary>
        public string Body { get; set; }

        public byte[] WriteStream { get; set; }

        public System.Net.CookieContainer Cookies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 参数特殊处理
        /// </summary>
        public Action<System.Net.HttpWebRequest> BeforeSend { get; set; }



        /// <summary>
        /// 异常处理
        /// </summary>
        public Action<HttpRequest, Exception> Error { get; set; }

        /// <summary>
        /// 接收二进制数据
        /// </summary>
        public Action<HttpRequest, byte[]> Data { get; set; }

        /// <summary>
        /// 接收字符
        /// </summary>
        public Action<HttpRequest, string> Html { get; set; }


        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(this.Body))
            {
                return this.Method + " " + this.Url;
            }
            return this.Method + " " + this.Url + "" + (this.Method.ToUpper() == "GET" ? "" : " " + this.Body);
        }

        public string GetString(byte[] buffer, Encoding e = null)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return null;
            }
            var encoding = e != null ? e : this.Encoding != null ? this.Encoding : System.Text.Encoding.UTF8;
            return encoding.GetString(buffer, 0, buffer.Length);
        }

        public System.Net.HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Http异常为成功，需要根据HttpStatusCode自行判断
        /// </summary>
        public bool HttpErrorIsSuccess { get; set; }

    }


}
