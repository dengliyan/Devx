using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Net;
using System.IO;

namespace Devx
{
    /// <summary>
    /// 文件上传
    /// </summary>
    public class FileUploader
    {
        public bool Success { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 上传后的路径
        /// </summary>
        public string Locator { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 原始路径
        /// </summary>
        public string Original { get; set; }


        private static bool SaveAs(string filename, byte[] bytes)
        {
            try
            {
                //读取配置文件
                string path = System.Configuration.ConfigurationManager.AppSettings["FileUploader"];

                if (string.IsNullOrEmpty(path))//如果配置为空，则使用当前的根目录
                {
                    path = System.Web.Hosting.HostingEnvironment.MapPath("/");
                }

                filename = System.IO.Path.Combine(path, filename.TrimStart('/'));

                string dir = System.IO.Path.GetDirectoryName(filename);

                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }

                System.IO.File.WriteAllBytes(filename, bytes);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetFileExt(string filename)
        {
            string[] temp = filename.Split('.');

            return "." + temp[temp.Length - 1].ToLower();
        }
        
        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="files"></param>
        /// <param name="pathbase"></param>
        /// <param name="filetype"></param>
        /// <param name="size"></param>
        /// <param name="onProcess"></param>
        /// <returns></returns>
        public static FileUploader Receive()
        {
            //参数初始化
            var filetype = WebHelper.Form("filetype").ToInt(WebHelper.Query("filetype").ToInt(1));//文件上传模式，可以上传的文件类型
            //var filesize = WebHelper.Form("filesize").ToInt(50 * 1024);//最大允许上传的文件大小  20M
            var fileexts = WebHelper.Form("fileexts").ToLower();//可以文件类型
            var imgzoom  = WebHelper.Form("zoom").ToArray(":");//文件裁减//w:h:0
            var files = System.Web.HttpContext.Current.Request.Files;

            //filesize = filesize < 0 ? 50 * 1024 : filesize;
            //filesize = filesize > 50 * 1024 ? 50 * 1024 : filesize;
            var filesize = 50 * 1024;
            #region 文件格式处理
            HashSet<string> allowtypes = new HashSet<string>() { ".rar", ".zip", ".doc", ".docx", ".pdf", ".txt", ".swf", ".xls", ".xlsx" };
            if (filetype == 1)
            {
                allowtypes = new HashSet<string>() { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };
                filesize = 1024 * 10;
            }
            if (!string.IsNullOrEmpty(fileexts) && fileexts != "" && fileexts.StartsWith("."))
            {
                string[] splits = fileexts.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                HashSet<string> types = new HashSet<string>();
                foreach (var s in splits)
                {
                    if (allowtypes.Contains(s))
                    {
                        types.Add(s);
                    }
                }
                allowtypes.Clear();
                allowtypes = types;
            }
            #endregion
            
            #region 文件检查
            if (files == null || files.Count == 0 || files[0].InputStream == null || files[0].InputStream.Length == 0)
            {
                return new FileUploader() { Success = false, Message = "未找到有效的上传文件！" };
            }
            string currentType = FileUploader.GetFileExt(files[0].FileName);//获取文件扩展名

            if (!allowtypes.Contains(currentType))//检查当前文件格式是否符合要求
            {
                return new FileUploader() { Success = false, Message = "文件类型不符合要求。" };
            }

            if (filesize > 0 && files[0].ContentLength >= (filesize * 1024))//检查文件大小
            {
                return new FileUploader() { Success = false, Message = "文件大小不符合要求。" };
            }
            #endregion

            try
            {
                var uploadFile = files[0];//获取当前文件
                string originalName = uploadFile.FileName;//获取当前的文件名
                DateTime now = DateTime.Now.Date;

                //保存的路径 
                string locator = "/" + "upload" + "/" + (filetype == 1 ? "images" : "files") + "/" + now.Year + "/" + now.Month.ToString("00") + "/" + now.Day.ToString("00") + "/";

                byte[] bytes = new byte[uploadFile.ContentLength];

                using (System.IO.Stream fs = (System.IO.Stream)uploadFile.InputStream)
                {
                    fs.Read(bytes, 0, uploadFile.ContentLength);
                    fs.Close();
                }

                #region 图片需要特殊处理
                if (filetype == 1)//判断是否为图片
                {
                    #region 加载成图片，并可图片进行缩放处理
                    try
                    {
                        using (var ms = new System.IO.MemoryStream(bytes, 0, bytes.Length))//二进制
                        using (var sImage = System.Drawing.Image.FromStream(ms))
                        {
                            ImageFormat format = sImage.RawFormat;//  

                            if (format.Equals(ImageFormat.Jpeg))
                            {
                                currentType = ".jpg";
                            }
                            if (format.Equals(ImageFormat.Png))
                            {
                                currentType = ".png";
                            }
                            if (format.Equals(ImageFormat.Bmp))
                            {
                                currentType = ".bmp";
                            }
                            if (format.Equals(ImageFormat.Gif))
                            {
                                currentType = ".gif";
                            }

                            //如果需要进行处理，则进行相应的操作
                            if (imgzoom.Length > 0 && (imgzoom[0] > 0 || imgzoom[1] > 0))
                            {
                                if (!Photos.Zoom(ref bytes, sImage.Width, sImage.Height, imgzoom[0], imgzoom[1],  ref currentType))
                                {
                                    //文件缩放失败
                                }
                            }

                            if (!allowtypes.Contains(currentType))//检查当前文件格式是否符合要求
                            {
                                return new FileUploader() { Success = false, Message = "文件类型不符合要求" };
                            }
                        }
                    }
                    catch
                    {
                        return new FileUploader() { Success = false, Message = "文件类型不符合要求。" };
                    }
                    #endregion
                }
                #endregion

                string filename = System.Guid.NewGuid().ToString().ToLower().Replace("-", "") + currentType;

                filename = (locator + filename);

                return new FileUploader() { Success = SaveAs(filename, bytes), Message = "", Locator = "/" + filename.TrimStart('/'), Original = originalName, FileType = currentType };
            }
            catch (Exception ex)
            {
                return new FileUploader() { Success = false, Message = ex.Message.ToString() };
            }
        }

        public static FileUploader Proxy(string remote)
        {
            //参数初始化
            var filetype = WebHelper.Form("filetype").ToInt(1);//文件上传模式，可以上传的文件类型
            //var filesize = WebHelper.Form("filesize").ToInt(20 * 1024);//最大允许上传的文件大小 20M
            var fileexts = WebHelper.Form("fileexts").ToLower();//可以文件类型
            var imgzoom = WebHelper.Form("zoom").ToArray(":");//文件裁减//w:h:0
            var files = System.Web.HttpContext.Current.Request.Files;
            var filesize = 50 * 1024;
            


            #region 文件格式处理
            HashSet<string> allowtypes = new HashSet<string>() { ".rar", ".zip", ".doc", ".docx", ".pdf", ".txt", ".swf", ".xls", ".xlsx" };
            if (filetype == 1)
            {
                allowtypes = new HashSet<string>() { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };
                filesize = 1024 * 10;
            }
            if (!string.IsNullOrEmpty(fileexts) && fileexts != "")
            {
                string[] splits = fileexts.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                HashSet<string> types = new HashSet<string>();
                foreach (var s in splits)
                {
                    if (allowtypes.Contains(s))
                    {
                        types.Add(s);
                    }
                }
                allowtypes.Clear();
                allowtypes = types;
            }
            #endregion

            #region 文件预处理
            if (files == null)
            {
                return new FileUploader() { Success = false, Message = "未找到有效的上传文件！" };
            }

            var uploadFile = files[0];

            string currentType = FileUploader.GetFileExt(uploadFile.FileName);//获取文件扩展名

            if (!allowtypes.Contains(currentType))//检查当前文件格式是否符合要求
            {
                return new FileUploader() { Success = false, Message = "文件类型不符合要求。" };
            }

            if (filesize > 0 && (uploadFile.ContentLength == 0 || uploadFile.ContentLength >= (filesize * 1024)))//检查文件大小
            {
                return new FileUploader() { Success = false, Message = "文件大小不符合要求。" };
            }
            #endregion

            //提交数据
            #region 通过http的方式将数据转交给另一个接口
            try
            {
                //remote
                var request = (HttpWebRequest)WebRequest.Create(remote);
                using (var ms = new MemoryStream())
                {
                    // 边界符  
                    var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                    // 边界符  
                    var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
                    // 最后的结束符  
                    var endBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                    // 设置属性  
                    request.Method = "POST";
                    request.Timeout = 30*1000;
                    request.ContentType = "multipart/form-data; boundary=" + boundary;

                    // 写入文件  
                    string filePartHeader =
                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                         "Content-Type: application/octet-stream\r\n\r\n";
                    var header = string.Format(filePartHeader, "http.post.file", uploadFile.FileName);
                    var headerbytes = Encoding.UTF8.GetBytes(header);

                    ms.Write(beginBoundary, 0, beginBoundary.Length);//写入分界
                    ms.Write(headerbytes, 0, headerbytes.Length);//写入头

                    byte[] bytes = new byte[uploadFile.ContentLength];
                    using (System.IO.Stream fs = (System.IO.Stream)uploadFile.InputStream)
                    {
                        fs.Read(bytes, 0, uploadFile.ContentLength);
                        fs.Close();
                    }
                    ms.Write(bytes, 0, bytes.Length);//写入内容

                    // 写入字符串的Key  
                    var stringKeyHeader = "\r\n--" + boundary +
                                           "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                           "\r\n\r\n{1}";

                  
                    var values=new Dictionary<string,string>();
                    values["filetype"]=filetype.ToString();
                    values["filesize"]=filesize.ToString();
                    if (string.IsNullOrEmpty(fileexts))
                    {
                        values["fileexts"] = fileexts.ToString();
                    }
                    if (imgzoom != null && imgzoom.Length > 0)
                    {
                        values["zoom"] = string.Join(":", Array.ConvertAll(imgzoom, i => i.ToString()));
                    }
                    foreach (var e in values)
                    {
                        header = string.Format(filePartHeader, e.Key, System.Web.HttpUtility.UrlEncode(e.Value, Encoding.UTF8).Replace("+", "%20"));
                        bytes = Encoding.UTF8.GetBytes(header);
                        ms.Write(bytes, 0, bytes.Length);//写入内容
                    }

                    //提交参数
                    foreach (var e in System.Web.HttpContext.Current.Request.Form)
                    {

                    }

                    // 写入最后的结束边界符  
                    ms.Write(endBoundary, 0, endBoundary.Length);

                    request.ContentLength = ms.Length;
                    ms.Position = 0;

                    using (var requestStream = request.GetRequestStream())
                    {
                        var bs = new byte[ms.Length];
                        ms.Read(bs, 0, bs.Length);
                        ms.Close();
                        ms.Dispose();
                        requestStream.Write(bs, 0, bs.Length);
                    }

                    //
                    //
                    string result = string.Empty;
                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var streamReceive = response.GetResponseStream())
                    using (var streamReader = new StreamReader(streamReceive, System.Text.Encoding.UTF8))
                    {
                        result = streamReader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(result))
                    {
                        //将数据解析成json
                        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                        var o = serializer.DeserializeObject(result) as Dictionary<string, object>;
                        if (o == null)
                        {
                            return new FileUploader() { Success = false, Message = "上传出错！！" };
                        }
                        return new FileUploader()
                        {
                            Success = o.ContainsKey("error") ? (int)o["error"] == 0 : false,
                            Message = o.ContainsKey("message") ? o["message"].ToString() : string.Empty,
                            Locator = o.ContainsKey("url") ? o["url"].ToString() : string.Empty
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new FileUploader() { Success = false, Message = ex.Message.ToString() };
            }
            return new FileUploader() { Success=false, Message="未知错误！！" };
            #endregion
        }

        public static bool Transfer(string remote, byte[] bytes, string filename)
        {
            try
            {
                //remote
                var request = (HttpWebRequest)WebRequest.Create(remote);
                using (var ms = new MemoryStream())
                {
                    // 边界符  
                    var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                    // 边界符  
                    var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
                    // 最后的结束符  
                    var endBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                    // 设置属性  
                    request.Method = "POST";
                    request.Timeout = 30 * 1000;
                    request.ContentType = "multipart/form-data; boundary=" + boundary;


                    // 写入文件  
                    string filePartHeader =
                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                         "Content-Type: application/octet-stream\r\n\r\n";
                    var header = string.Format(filePartHeader, "http.post.file", filename);
                    var headerbytes = Encoding.UTF8.GetBytes(header);

                    ms.Write(beginBoundary, 0, beginBoundary.Length);//写入分界
                    ms.Write(headerbytes, 0, headerbytes.Length);//写入头


                    ms.Write(bytes, 0, bytes.Length);//写入内容

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

                    var values = new Dictionary<string, string>();
                    values["filename"] = filename;
                    values["filetransfer"] = "true";
                    foreach (var e in values)
                    {
                        header = string.Format(stringKeyHeader, e.Key, e.Value);
                        bytes = Encoding.UTF8.GetBytes(header);
                        ms.Write(bytes, 0, bytes.Length);//写入内容
                    }


                    // 写入最后的结束边界符  
                    ms.Write(endBoundary, 0, endBoundary.Length);

                    request.ContentLength = ms.Length;
                    ms.Position = 0;

                    using (var requestStream = request.GetRequestStream())
                    {
                        var bs = new byte[ms.Length];
                        ms.Read(bs, 0, bs.Length);
                        ms.Close();
                        ms.Dispose();
                        requestStream.Write(bs, 0, bs.Length);
                    }

                    //
                    //
                    string result = string.Empty;
                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var streamReceive = response.GetResponseStream())
                    using (var streamReader = new StreamReader(streamReceive, System.Text.Encoding.UTF8))
                    {
                        result = streamReader.ReadToEnd();
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        //将数据解析成json
                        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                        var o = serializer.DeserializeObject(result) as Dictionary<string, object>;
                        if (o == null)
                        {
                            return false;
                        }
                        return o.ContainsKey("error") && (int)o["error"] == 0;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
