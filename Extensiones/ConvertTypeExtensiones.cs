using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx
{
    public static class ConvertTypeExtensiones
    {
        public static bool ToBoolean(this string s, bool defaultValue = false)
        {
            if (string.IsNullOrEmpty(s))
            {
                return defaultValue;
            }

            return s.ToLower() == "true";
        }

        public static int ToInt(this string s, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;

            int i = 0;

            return int.TryParse(s.Trim(), out i) ? i : defaultValue;
        }

        public static int? ToIntNullable(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            int i = 0;

            if (int.TryParse(s.Trim(), out i))
            {
                return i;
            }
            return null;
        }

        public static double ToDouble(this string s, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;

            double i = 0d;

            return double.TryParse(s.Trim(), out i) ? i : defaultValue;
        }

        public static double? ToDoubleNullable(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            double i = 0d;

            if (double.TryParse(s.Trim(), out i))
            {
                return i;
            }
            return null;
        }

        public static float ToFloat(this string s, float defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;

            float i = 0f;

            return float.TryParse(s.Trim(), out i) ? i : defaultValue;
        }

        public static float? ToFloatNullable(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            float i = 0f;

            if (float.TryParse(s.Trim(), out i))
            {
                return i;
            }
            return null;
        }


        public static DateTime ToDateTime(this string s, DateTime defaultValue)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return defaultValue;
            }

            DateTime i = defaultValue;

            return DateTime.TryParse(s, out i) ? i : defaultValue;
        }

        public static DateTime ToDateTime(this string s, string format)
        {
            return ToDateTime(s, format, DateTime.MinValue);
        }

        public static DateTime ToDateTime(this string s, string format, DateTime defaultValue)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return defaultValue;
            }

            DateTime i = defaultValue;

            return DateTime.TryParseExact(s, format, null, System.Globalization.DateTimeStyles.None, out i) ? i : defaultValue;
        }
        


        public static DateTime? ToDateTimeNullable(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            DateTime i = DateTime.Now;

            if (DateTime.TryParse(s, out i))
            {
                return i;
            }

            return null;
        }
        public static DateTime? ToDateTimeNullable(this string s, string format)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            DateTime i = DateTime.Now;

            if (DateTime.TryParseExact(s, format, null, System.Globalization.DateTimeStyles.None, out i))
            {
                return i;
            }

            return null;
        }

        public static TimeSpan ToTimeSpan(this string s, TimeSpan defaultValue)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return defaultValue;
            }
            TimeSpan result = defaultValue;
            if (!TimeSpan.TryParse(s, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        public static int[] ToArray(this string s, string separator = ",")
        {
            if (string.IsNullOrWhiteSpace(s)) return new int[] { };

            int i = 0;

            string[] array = s.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            List<int> lists = new List<int>();

            foreach (var item in array)
            {
                if (int.TryParse(item, out i))
                {
                    lists.Add(i);
                }
            }

            return lists.ToArray();
        }

        #region MyRegion
        //public static dynamic ToDynamic(this object value)
        //{
        //    IDictionary<string, object> expando = new System.Dynamic.ExpandoObject();
        //    Type type = value.GetType();
        //    var properties = ReflectionHelper.GetProperties (type);
        //    foreach (var e in properties)
        //    {
        //        var property = e.Value;
        //        var val = property.GetValue(value, null);
        //        if (property.PropertyType.FullName.StartsWith("<>f__AnonymousType"))
        //        {
        //            dynamic dval = val.ToDynamic();
        //            expando.Add(property.Name, dval);
        //        }
        //        else
        //        {
        //            expando.Add(property.Name, val);
        //        }
        //    }
        //    return expando;
        //} 
        #endregion
    }
}
