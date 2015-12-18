//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.Drawing.Imaging;

//namespace Devx
//{
//    /// <summary>
//    /// 文件上传
//    /// </summary>
//    public class FileUploader
//    {
//        public bool Success { get; set; }

//        /// <summary>
//        /// 状态
//        /// </summary>
//        public string Message { get; set; }

//        /// <summary>
//        /// 上传后的路径
//        /// </summary>
//        public string Locator { get; set; }

//        /// <summary>
//        /// 文件类型
//        /// </summary>
//        public string FileType { get; set; }

//        /// <summary>
//        /// 原始路径 
//        /// </summary>
//        public string Original { get; set; }

//        public static FileUploader SaveAs(System.Web.HttpFileCollection files, string pathbase, HashSet<string> filetype, int size)
//        {
//            if (files == null || files.Count == 0)
//            {
//                return new FileUploader() { Success = false, Message = "无上传文件为空" };
//            }

//            try
//            {
//                System.Web.HttpPostedFile uploadFile = files[0];//获取当前文件
//                string originalName = uploadFile.FileName;//获取当前的文件名
//                //当前存储位置，固定

//                string locator = "/" + pathbase + "/" + DateTime.Now.ToString("yyyyMMdd") + "/";

//                string currentType = FileUploader.GetFileExt(originalName);//获取文件扩展名

               

//                if (!filetype.Contains(currentType))//检查当前文件格式是否符合要求
//                {
//                    return new FileUploader() { Success = false, Message = "文件类型不符合要求" };
//                }
//                if (uploadFile.ContentLength == 0 || uploadFile.ContentLength >= (size * 1024))//检查文件大小
//                {
//                    return new FileUploader() { Success = false, Message = "文件大小不符合要求" };
//                }

//                string filename = System.Guid.NewGuid().ToString().ToLower().Replace("-", "") + currentType;

//                byte[] bytes = new byte[uploadFile.ContentLength];

//                using (System.IO.Stream fs = (System.IO.Stream)uploadFile.InputStream)
//                {
//                    fs.Read(bytes, 0, uploadFile.ContentLength);
//                    fs.Close();
//                }

//                if (pathbase == "images")
//                {
//                    //判断是否为图片
//                    try
//                    {
//                        using (var ms = new System.IO.MemoryStream(bytes, 0, bytes.Length))//二进制
//                        using (var sImage = System.Drawing.Image.FromStream(ms))
//                        {
//                            ImageFormat format = sImage.RawFormat;//  
                                               
//                            if (format.Equals(ImageFormat.Jpeg))
//                            {
//                                currentType = ".jpg";
//                            }
//                            if (format.Equals(ImageFormat.Png))
//                            {
//                                currentType = ".png";
//                            }
//                            if (format.Equals(ImageFormat.Bmp))
//                            {
//                                currentType = ".bmp";
//                            }
//                            if (format.Equals(ImageFormat.Gif))
//                            {
//                                currentType = ".gif";
//                            }
//                        }

//                        if (!filetype.Contains(currentType))//检查当前文件格式是否符合要求
//                        {
//                            return new FileUploader() { Success = false, Message = "文件类型不符合要求" };
//                        }
//                    }
//                    catch
//                    {
//                        return new FileUploader() { Success = false, Message = "文件类型不符合要求" };
//                    }
//                }

//                filename = (locator + filename);
//                return new FileUploader() { Success = SaveAs(filename, bytes), Message = "SUCCESS", Locator = "/upload/" + filename.TrimStart('/'), Original = originalName, FileType = currentType };
//            }
//            catch(Exception ex)
//            {
//                return new FileUploader() { Success = false, Message = ex.Message.ToString() };
//            }
//        }

//        private static string GetFileExt(string filename)
//        {
//            string[] temp = filename.Split('.');

//            return "." + temp[temp.Length - 1].ToLower();
//        }

//        public static bool SaveAs(string filename, byte[] bytes)
//        {
//            try
//            {                               
//                //
//                //远程存储一份
//                //
//                Services.FileService fs = new Services.FileService();
//                fs.Url = System.Configuration.ConfigurationManager.AppSettings["request.uri"].TrimEnd('/') + "/Handler/FileService.asmx";
//                fs.Write(filename, bytes);

//                //本机存储一份，防止后台某一些功能无法使用。
//                filename = System.Web.Hosting.HostingEnvironment.MapPath("/upload/" + filename.TrimStart('/'));

//                //防止出现重复写
//                if (!System.IO.File.Exists(filename))
//                {
//                    string dir = System.IO.Path.GetDirectoryName(filename);

//                    if (!System.IO.Directory.Exists(dir))
//                    {
//                        System.IO.Directory.CreateDirectory(dir);
//                    }

//                    System.IO.File.WriteAllBytes(filename, bytes);
//                }

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//    }
//}
