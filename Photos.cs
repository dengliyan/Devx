using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Devx
{
    public class Photos
    {
        public static byte[] Watermarks(byte[] bytes, string position, ref string extension)
        {
            try
            {
                var watermarkFile = System.Web.Hosting.HostingEnvironment.MapPath("~/watermark/logo_1.png");

                if (!System.IO.File.Exists(watermarkFile))
                {
                    return bytes;
                }
                using (var ms = new System.IO.MemoryStream(bytes, 0, bytes.Length))//二进制
                using (var sImage = System.Drawing.Image.FromStream(ms))
                using (var wImage = System.Drawing.Image.FromFile(watermarkFile))//水印图片
                using (var sBitmap = new System.Drawing.Bitmap(sImage.Width, sImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                using (var sGraphics = System.Drawing.Graphics.FromImage(sBitmap))//当前图片      
                {
                    if (sImage.Width <= wImage.Width || sImage.Height <= wImage.Height)
                    {
                        return bytes;
                    }
                    sGraphics.DrawImage(sImage, 0, 0, sImage.Width, sImage.Height);
                    sGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    sGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    var point = new System.Drawing.Point(0, 0);
                    #region 位置处理
                    switch (position)
                    {
                        case "top-right":
                        case "right-top":
                            point.X = sImage.Width - wImage.Width - 10;
                            point.Y = 10;
                            break;

                        case "bottom-right":
                        case "right-bottom":
                            point.X = sImage.Width - wImage.Width - 10;
                            point.Y = sImage.Height - wImage.Height - 10;
                            break;
                        case "top-left":
                        case "left-top":
                            point.X = 10;
                            point.Y = 10;
                            break;
                        case "bottom-left":
                        case "left-bottom":
                            point.X = 10;
                            point.Y = sImage.Height - wImage.Height - 10;
                            break;
                        default:
                            point.X = sImage.Width - wImage.Width - 10;
                            point.Y = sImage.Height - wImage.Height - 10;
                            break;
                    }
                    #endregion
                    sGraphics.DrawImage(wImage, point.X, point.Y, wImage.Width, wImage.Height);
                    var format = sImage.RawFormat;//                
                    if (sImage.RawFormat.Equals(ImageFormat.Gif))//gif文件不设置水印
                    {
                        extension = ".gif";
                        return bytes;
                    }
                    #region 后缀名判断
                    else if (format.Equals(ImageFormat.Jpeg))
                    {
                        extension = ".jpg";
                    }
                    else if (format.Equals(ImageFormat.Bmp))
                    {
                        extension = ".bmp";
                    }
                    else if (format.Equals(ImageFormat.Png))
                    {
                        extension = ".png";
                    }
                    else
                    {
                        extension = ".jpg";
                        format = ImageFormat.Jpeg;
                    }
                    #endregion
                    using (var stream = new System.IO.MemoryStream())//二进制
                    {
                        sBitmap.Save(stream, format);
                        bytes = stream.ToArray();
                        return bytes;
                    }
                }
            }
            catch
            {
                return bytes;
            }
        }

        /// <summary>
        /// 缩小图片
        /// </summary>
        /// <param name="bytes">图片数据</param>
        /// <param name="sWidth">原始的宽度</param>
        /// <param name="sHeight">原始高度</param>
        /// <param name="width">缩小后的宽度</param>
        /// <param name="height">缩小的高度</param>
        /// <param name="mode">
        /// 模式：
        ///     0 ：以宽度缩小，如果高度大于缩小后的高度，则进行裁减，如果高度小于，则以高度进行缩小
        ///     1 ：以宽度缩小，且高度不能大于指定高度，如果大于，则进行高度缩小
        ///     2 ：以高度进行缩小，如果宽度大于缩小后的宽度，以中心进行裁减，如果宽度小于当前的宽度，则将宽度当成高度进行缩放
        /// </param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static bool Zoom(ref byte[] bytes, int sWidth, int sHeight, int width, int height, ref string extension)
        {
            int w = width, h = height, x = 0, y = 0, cWidth = sWidth, cHeigth = sHeight;
            bool canZoom = false;
            //如果不进行缩放，直接返回
            if (width == 0 && height == 0)
            {
                return false;
            }
            if (width > 0 && height == 0 && width <= sWidth)
            {
                #region 只按宽度进行缩放
                w = width;
                h = (int)(width * sHeight / sWidth);
                canZoom = true;
                #endregion
            }
            else if (width == 0 && height > 0 && height <= sHeight)
            {
                #region 只按高度进行缩放
                w = (int)(height * sWidth / sHeight);
                h = height;
                canZoom = true;
                #endregion
            }
            else if (width > 0 && height > 0)
            {
                var w_w = width;
                var h_w = (int)(width * sHeight / sWidth);//计算当前比例下的高度

                var w_h = (int)(height * sWidth / sHeight);//计算当前比例下的宽度
                var h_h = height;

                // 以宽度缩小，如果高度大于缩小后的高度，则进行裁减，如果高度小于，则以高度进行缩小
                if (height <= h_w && width <= sWidth)
                {
                    #region 当前缩小后的高度大于实际的高度
                    w = width;
                    h = height;//设置当前图片的区域大小
                    //计算坐标
                    cWidth = sWidth;//裁减区域的高度为原图片的宽度
                    cHeigth = (int)(cWidth * h / w);//裁减区域的高度，为计算出来的高度，w/h=cWidth/cHeight
                    x = 0;
                    y = (int)(sHeight - cHeigth) / 2;//裁减坐标
                    canZoom = true;
                    #endregion
                }
                else if (width <= w_h && height <= sHeight)
                {
                    #region 当前缩小后的高度小于实际的高度
                    w = width;
                    h = height;
                    cWidth = (int)(sHeight * w / h);
                    cHeigth = sHeight;
                    x = (int)(sWidth - cWidth) / 2;
                    y = 0;
                    canZoom = true;
                    #endregion
                }
            }

            //如果不能缩放，直接返回数据
            if (!canZoom)
            {
                return canZoom;
            }


            using (var ms = new System.IO.MemoryStream(bytes, 0, bytes.Length))//二进制
            using (var sImage = System.Drawing.Image.FromStream(ms))
            using (var outBitmap = new System.Drawing.Bitmap(w, h))//输出的图片
            using (var outGraphics = System.Drawing.Graphics.FromImage(outBitmap))//输出的图片   
            {
                outGraphics.Clear(Color.WhiteSmoke);
                outGraphics.CompositingQuality = CompositingQuality.HighQuality;
                outGraphics.SmoothingMode = SmoothingMode.HighQuality;
                outGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //outGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                outGraphics.DrawImage(sImage, new Rectangle(0, 0, w, h), x, y, cWidth, cHeigth, GraphicsUnit.Pixel);
                var format = sImage.RawFormat;//                
                if (sImage.RawFormat.Equals(ImageFormat.Gif))//当前文件为gif格式，则转换成jpg格式
                {
                    return false;
                }

                #region 后缀名判断
                if (format.Equals(ImageFormat.Gif))
                {
                    extension = ".gif";
                }
                else if (format.Equals(ImageFormat.Jpeg))
                {
                    extension = ".jpg";
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    extension = ".bmp";
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    extension = ".png";
                }
                else
                {
                    extension = ".jpg";
                    format = ImageFormat.Jpeg;
                }
                #endregion

                using (var stream = new System.IO.MemoryStream())//二进制
                {
                    outBitmap.Save(stream, format);
                    bytes = stream.ToArray();
                }
            }
            return canZoom;
        }
        

    }
}
