namespace Devx
{
    using System;
    using System.Xml;

    public static class ConvertToStringExtensiones
    {        
        public static string ExToString(this int value, string format, sbyte ratio)
        {
            
            double pow = Math.Pow(10, ratio);
            return (value / pow).ToString(format);
        }
        public static string ExToString(this int? value, string format)
        {
            return value.ExToString(format, 0, string.Empty);
        }
        public static string ExToString(this int? value, string format, string defaultValue)
        {
            return value.ExToString(format, 0, defaultValue);
        }
        public static string ExToString(this int? value, string format, sbyte ratio, string defaultValue)
        {
            return value.HasValue ? value.Value.ExToString(format, ratio) : defaultValue;
        }

        public static string ExToString(this long value, string format, sbyte ratio)
        {
            double pow = Math.Pow(10, ratio);
            return (value / pow).ToString(format);
        }
        public static string ExToString(this long? value, string format)
        {
            return value.ExToString(format, 0, string.Empty);
        }
        public static string ExToString(this long? value, string format, string defaultValue)
        {
            return value.ExToString(format, 0, defaultValue);
        }
        public static string ExToString(this long? value, string format, sbyte ratio, string defaultValue)
        {
            return value.HasValue ? value.Value.ExToString(format, ratio) : defaultValue;
        }

        public static string ExToString(this float value, string format, sbyte ratio)
        {
            float pow = (float)Math.Pow(10, ratio);
            return (value / pow).ToString(format);
        }
        public static string ExToString(this float? value, string format)
        {
            return value.ExToString(format, 0, string.Empty);
        }
        public static string ExToString(this float? value, string format, string defaultValue)
        {
            return value.ExToString(format, 0, defaultValue);
        }
        public static string ExToString(this float? value, string format, sbyte ratio, string defaultValue)
        {
            return value.HasValue ? value.Value.ExToString(format, ratio) : defaultValue;
        }

        public static string ExToString(this double value, string format, sbyte ratio)
        {
            double pow = Math.Pow(10, ratio);
            return (value / pow).ToString(format);
        }
        public static string ExToString(this double? value, string format)
        {
            return value.ExToString(format, 0, string.Empty);
        }
        public static string ExToString(this double? value, string format, string defaultValue)
        {
            return value.ExToString(format, 0, defaultValue);
        }
        public static string ExToString(this double? value, string format, sbyte ratio, string defaultValue)
        {
            return value.HasValue ? value.Value.ExToString(format, ratio) : defaultValue;
        }


        public static string ExToUnitString(this double value)
        {
            if (value < 10000)//小于万
            {
                return (value / 10000).ToString("F2") + "万";
            }
            if (value < 100000000)//小于万
            {
                return (value / 10000).ToString("F2") + "万";
            }

            return (value / 100000000).ToString("F2") + "亿";
        }
        
        public static string ExToString(this decimal value, string format, sbyte ratio)
        {
            decimal pow = (decimal)Math.Pow(10, ratio);
            return (value / pow).ToString(format);
        }
        public static string ExToString(this decimal? value, string format)
        {
            return value.ExToString(format, 0, string.Empty);
        }
        public static string ExToString(this decimal? value, string format, string defaultValue)
        {
            return value.ExToString(format, 0, defaultValue);
        }
        public static string ExToString(this decimal? value, string format, sbyte ratio, string defaultValue)
        {
            return value.HasValue ? value.Value.ExToString(format, ratio) : defaultValue;
        }


        public static string ExToString(this DateTime value)
        {
            return value.ExToString("yyyy-MM-dd");
        }
        public static string ExToString(this DateTime value, string format)
        {
            return value.ToString(format);
        }
        public static string ExToString(this DateTime? value)
        {
            return value.ExToString("yyyy-MM-dd", string.Empty);
        }
        public static string ExToString(this DateTime? value, string format)
        {
            return value.ExToString(format, string.Empty);
        }
        public static string ExToString(this DateTime? value, string format, string defaultValue)
        {
            if (value.HasValue)
            {
                return value.Value.ToString(format);
            }
            else
            {
                return defaultValue;
            }
        }

        public static string ExToDateAndWeek(this DateTime value)
        {
            return value.ExToDateAndWeek("yyyy-MM-dd");
        }
        public static string ExToDateAndWeek(this DateTime value, string format)
        {
            var week = new string[] { "日", "一", "二", "三", "四", "五", "六" };
            var result = string.Empty;
            result = value.ExToString(format);//转换成指定的格式
            result += "(周" + week[(int)value.DayOfWeek] + ")";
            return result;
        }
        public static string ExToDateAndWeek(this DateTime? value)
        {
            return value.ExToDateAndWeek("yyyy-MM-dd", string.Empty);
        }
        public static string ExToDateAndWeek(this DateTime? value, string format)
        {
            return value.ExToDateAndWeek(format, string.Empty);
        }
        public static string ExToDateAndWeek(this DateTime? value, string format, string defaultValue)
        {
            if (value.HasValue)
            {
                var week = new string[] { "日", "一", "二", "三", "四", "五", "六" };
                var result = string.Empty;
                result = value.Value.ExToString(format);//转换成指定的格式
                result += "(周" + week[(int)value.Value.DayOfWeek] + ")";
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        public static string ExToDateTimeToW3C(this DateTime value)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:sszzzz");
        }

        
        public const string W3CDATETIME="yyyy-MM-ddTHH:mm:sszzzz";


        /// <summary>

        /// 时间戳转为C#格式时间

        /// </summary>

        /// <param name=”timeStamp”></param>

        /// <returns></returns>

        private static DateTime GetTime(string timeStamp)
        {

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            long lTime = long.Parse(timeStamp + "0000000");

            TimeSpan toNow = new TimeSpan(lTime); 
            
            return dtStart.Add(toNow);

        }



        /// <summary>

        /// DateTime时间格式转换为Unix时间戳格式

        /// </summary>

        /// <param name=”time”></param>

        /// <returns></returns>

        private static int ConvertDateTimeInt(System.DateTime time)
        {

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            return (int)(time - startTime).TotalSeconds;

        }
        
    }

}
