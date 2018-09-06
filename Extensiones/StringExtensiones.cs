namespace Devx
{
    using System;
    using System.Web;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
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
            {
                return "";
            }
            string outstr = "";
            int n = 0;
            if (ellipsis)
            {
                if (ExCharactersLength(s) <= num)
                {
                    return s;
                }
            }
            foreach (char ch in s)
            {
                n += System.Text.Encoding.Default.GetByteCount(ch.ToString());
                if (n > (ellipsis ? num - 4 : num))
                {
                    outstr += (ellipsis ? " ..." : "");
                    break;
                }
                else
                {
                    outstr += ch;
                }
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

        /// <summary>
        /// 删除不可见的字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 删除所有的标签
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ExRemoveHtmlTag(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            return System.Text.RegularExpressions.Regex.Replace(s, "<[^>]*>", "", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 删除HTML标签，但保留指定的标签
        /// </summary>
        /// <param name="s"></param>
        /// <param name="keeps"></param>
        /// <returns></returns>
        public static string ExRemoveHtmlTag(this string s, IEnumerable<string> keeps)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var hash = new HashSet<string>();
            if (keeps != null)
            {
                foreach (var item in keeps)
                {
                    hash.Add(item.ToLower());
                    hash.Add("/" + item.ToLower());
                }
            }
            return System.Text.RegularExpressions.Regex.Replace(s, @"<([/]?[\w]+)[^>]*>", m =>
            {
                if (m.Success && m.Groups[1] != null)
                {
                    var val = (m.Groups[1].Value ?? "").ToLower();
                    if (!string.IsNullOrWhiteSpace(val) && hash.Contains(val))
                    {
                        return "<" + val + ">";
                    }
                }
                return "";
            }, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        public static string ExHtmlEncode(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            return System.Web.HttpUtility.HtmlEncode(s);
        }
        public static string ExHtmlDecode(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            return System.Web.HttpUtility.HtmlDecode(s);
        }

        /// <summary>
        /// 提取摘要
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ExParserDigest(this string s, int min = 220)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                try
                {
                    string digest = "";
                    MatchCollection ms = Regex.Matches(s, @"((?is)<p[^>]*>(?><p[^>]*>(?<o>)|</p>(?<-o>)|(?:(?!</?p\b).)*)*(?(o)(?!))</p>)", RegexOptions.IgnoreCase);
                    foreach (Match mt in ms)
                    {
                        if (mt != null && mt.Value != null && mt.Value.Split(new string[] { "。" }, StringSplitOptions.None).Length > 1)
                        {
                            digest = Regex.Replace(mt.Value, @"</*[^>]*>", "").Replace("&nbsp;", " ").Trim();//移除所有标签
                            break;
                        }
                        if (!string.IsNullOrEmpty(digest.Trim()))//提取到描述
                        {
                            if (System.Text.Encoding.UTF8.GetBytes(digest).Length > min)//长度超过110个汉字，则取一句作为描述
                            {
                                int bytes = 0;

                                string[] spits = digest.Split(new string[] { "。" }, StringSplitOptions.None);

                                digest = string.Empty;

                                foreach (var c in spits)
                                {
                                    digest += c + "。";

                                    bytes += System.Text.Encoding.UTF8.GetBytes(s).Length;//取当前的字数

                                    if (bytes >= min) break;//超过指定则退出
                                }
                            }
                            return digest;
                        }
                    }
                }
                catch
                {
                }
                return s.ExRemoveHtmlTag().ExSubString(min);
            }
            return string.Empty;
        }


        /// <summary>
        /// 编辑距离（Levenshtein Distance）
        /// </summary>
        /// <param name="source">源串</param>
        /// <param name="target">目标串</param>
        /// <param name="similarity">输出：相似度，值在0～１</param>
        /// <param name="isCaseSensitive">是否大小写敏感</param>
        /// <returns>源串和目标串之间的编辑距离</returns>
        public static Int32 LevenshteinDistance(String source, String target, out Double similarity)
        {
            //为空判断
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target))
                {
                    similarity = 1;
                    return 0;
                }
                else
                {
                    similarity = 0;
                    return target.Length;
                }
            }
            else if (String.IsNullOrEmpty(target))
            {
                similarity = 0;
                return source.Length;
            }

            String from, to;//大小写无关

            from = source.ToLower();

            to = target.ToLower();

            // 初始化
            Int32 m = from.Length;
            Int32 n = to.Length;
            Int32[,] h = new Int32[m + 1, n + 1];
            for (Int32 i = 0; i <= m; i++) h[i, 0] = i;  // 注意：初始化[0,0]
            for (Int32 j = 1; j <= n; j++) h[0, j] = j;

            // 迭代
            for (Int32 i = 1; i <= m; i++)
            {
                Char si = from[i - 1];
                for (Int32 j = 1; j <= n; j++)
                {   // 删除（deletion） 插入（insertion） 替换（substitution）
                    if (si == to[j - 1])
                        h[i, j] = h[i - 1, j - 1];
                    else
                        h[i, j] = Math.Min(h[i - 1, j - 1], Math.Min(h[i - 1, j], h[i, j - 1])) + 1;
                }
            }

            // 计算相似度
            Int32 maxLength = Math.Max(m, n);   // 两字符串的最大长度

            similarity = ((Double)(maxLength - h[m, n])) / maxLength;

            return h[m, n];    // 编辑距离
        }

        /// <summary>
        /// 替换成SSL，
        /// </summary>
        /// <param name="html"></param>
        /// <param name="replaceLink"></param>
        /// <param name="removeDomains">http://www.stockstar.com</param>
        /// <returns></returns>
        public static string SSL(this string html, bool replaceLink = true, string[] removeDomains = null)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return html;
            }
            if (replaceLink)
            {
                html = System.Text.RegularExpressions.Regex.Replace(html, @"href[\s]*=[\s]*([""|']?)(http://[^""|^']*)([""|']?)([\s]*)", new MatchEvaluator(m =>
                {
                    var val = m.Groups[2].Value;
                    var space = m.Groups[4].Value;
                    if (removeDomains != null)
                    {
                        foreach (var item in removeDomains)
                        {
                            if (val.ToLower().StartsWith(item.ToLower()))
                            {
                                return "href=\"" + val + "\"" + space;
                            }
                        }
                    }
                    val = Regex.Replace(val, @"http://", "https://", RegexOptions.IgnoreCase);
                    return "href=\"" + val + "\"" + space;
                }), RegexOptions.IgnoreCase);
            }

            html = System.Text.RegularExpressions.Regex.Replace(html, @"src[\s]*=[\s]*([""|']?)[\s]*http://", "src=$1//", RegexOptions.IgnoreCase);

            return html;
        }
    }
}
