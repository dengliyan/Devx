namespace Devx
{
    using System;
    using System.Web;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtensiones
    {



        #region 截取字符串
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ExSubString(this string s, int num)
        {
            if (s == null || s.Length == 0)
                return "";
            string outstr = "";
            int n = 0;
            foreach (char ch in s)
            {
                n += System.Text.Encoding.Default.GetByteCount(ch.ToString());
                if (n > num)
                    break;
                else
                    outstr += ch;
            }
            return outstr;
        }
        public static string ExSubString(this string s, int num, bool ellipsis)
        {
            if (s == null || s.Length == 0)
                return "";
            string outstr = "";
            int n = 0;
            foreach (char ch in s)
            {
                n += System.Text.Encoding.Default.GetByteCount(ch.ToString());
                if (n > num)
                {
                    outstr += (ellipsis ? "..." : "");
                    break;
                }
                else
                    outstr += ch;
            }
            return outstr;
        }
        #endregion

        #region 颜色处理
        public static string ExColor(this string s, double init, double compare, bool flag = false)
        {
            return ExColor(s, init, compare, "red", "green", flag);
        }

        public static string ExColor(this string s, double init, double compare, string red, string green, bool flag)
        {
            string result = "<span class=\"{0}\">{1}{2}</span>";

            if (init > compare)
            {
                return string.Format(result, red, s, flag ? "↑" : "");
            }
            else if (init < compare)
            {
                return string.Format(result, green, s, flag ? "↓" : "");
            }
            else
            {
                return string.Format("{0}", s);
            }
        }


        public static string ExColor(this string s, double? init, double compare, string red, string green, bool flag)
        {
            string result = "<span class=\"{0}\">{1}{2}</span>";

            if (init.HasValue)
            {
                if (init.Value > compare)
                {
                    return string.Format(result, red, s, flag ? "↑" : "");
                }
                else if (init.Value < compare)
                {
                    return string.Format(result, green, s, flag ? "↓" : "");
                }
            }
            return string.Format("{0}", flag ? s + "&nbsp;" : s);
        }
        public static string ExColor(this string s, double? init, double compare)
        {
            return ExColor(s, init, compare, "red", "green", false);
        }
        public static string ExColor(this string s, double? init, double compare, bool flag)
        {
            return ExColor(s, init, compare, "red", "green", flag);
        }
        #endregion

        #region 字符型格式
        public static string ExString2Json(this string s)
        {
            if (string.IsNullOrEmpty(s)) return "\"\"";
            var sb = new StringBuilder();
            sb.Append('\"');
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            sb.Append('\"');
            return sb.ToString();
        }
        #endregion

        #region 字符转换成16进制
        /// <summary>
        /// 16进制编码转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ExString2Hex(this string s)
        {
            StringBuilder sb = new StringBuilder();//UTF8
            string s1;
            string s2;
            for (int i = 0; i < s.Length; i++)
            {
                byte[] bt = System.Text.Encoding.Unicode.GetBytes(s.Substring(i, 1));
                if (bt.Length > 1)//判断是否汉字
                {
                    s1 = Convert.ToString((short)(bt[1] - '\0'), 16);//转化为16进制字符串
                    s2 = Convert.ToString((short)(bt[0] - '\0'), 16);//转化为16进制字符串
                    s1 = (s1.Length == 1 ? "0" : "") + s1;//不足位补0
                    s2 = (s2.Length == 1 ? "0" : "") + s2;//不足位补0
                    sb.Append("\\u" + s1 + s2);
                }
            }

            return sb.ToString();
        }

        public static string ExString2Hex(this  string s, bool js)
        {
            StringBuilder sb = new StringBuilder();//UTF8
            string s1;
            string s2;
            for (int i = 0; i < s.Length; i++)
            {
                byte[] bt = System.Text.Encoding.Unicode.GetBytes(s.Substring(i, 1));
                if (bt.Length > 1)//判断是否汉字
                {
                    s1 = Convert.ToString((short)(bt[1] - '\0'), 16);//转化为16进制字符串
                    s2 = Convert.ToString((short)(bt[0] - '\0'), 16);//转化为16进制字符串
                    s1 = (s1.Length == 1 ? "0" : "") + s1;//不足位补0
                    s2 = (s2.Length == 1 ? "0" : "") + s2;//不足位补0
                    if (js)
                    {
                        sb.Append("\\u");
                    }
                    sb.Append(s1 + s2);
                }
            }

            return sb.ToString();
        }
        #endregion

        #region 取字符的长度
        /// <summary>
        /// 取字符的长度，汉字当成两个长度
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ExCharactersLength(this string s)
        {
            int len = 0;
            for (int i = 0; i < s.Length; i++)
            {
                var c = s[i].ToString();
              len += System.Text.Encoding.Default.GetBytes(c).Length;
            }
            return len;
        }
        #endregion

        public static string ExDeleteUnVisibleChar(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                int Unicode = (int)s[i];
                if (Unicode >= 16)
                {                   
                    sBuilder.Append(s[i].ToString());
                }
            }
            return sBuilder.ToString();
        }

    }
}
