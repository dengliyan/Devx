using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Devx
{
    /// <summary>
    /// 分页算法
    /// </summary>
    public class Pagination
    {
        private int _start=0;
        private int _end=0;
        private int _count=0;
        private int _index=0;

        public int[] Range { get { return new int[] { _start, _end }; } }


        public Pagination(int index, int count, int max = 5)
        {
            int start = Math.Max(2, index - (int)(max / 2));
            int end = Math.Min(count - 1, start + max - 1);
            start = Math.Max(2, end - max + 1);

            this._start = start;
            this._end = end;
            this._count = count;
            this._index = index;
        }

        public string Format { get; set; }

        public string Home { get; set; }      
  
        
        public override string ToString()
        {
            System.Text.StringBuilder sbTemp = new System.Text.StringBuilder();

            if (string.IsNullOrEmpty(this.Home))
            {
                this.Home = string.Format(this.Format, 1);
            }

            if (this._count <= 1) return string.Empty;

            string format = this.Format;
            string first = this.Home;

            #region 生成分页
            
            if (this._index > 1)
            {
                string go = ((this._index - 1) == 1 ? first : string.Format(format, this._index - 1));
                sbTemp.Append("<li><a href=\""+go+"\">&laquo;</a></li>");
                sbTemp.Append("<li><a href=\"" + first + "\">1" + ((this._start > 2) ? "..." : "") + "</a></li> ");
            }
            else
            {
                sbTemp.Append("<li class=\"disabled\"><a href=\"#\">&laquo;</a></li>");
                sbTemp.Append("<li class=\"active\"><a href=\"#\">1</a></li> ");
            }
            for (int i = this._start; i <= this._end; i++)
            {
                string go = (i == 1 ? first : string.Format(format, i));

                if (i == this._index)
                {
                    sbTemp.Append("<li class=\"active\"><a href=\"" + go + "\">" + i + "</a></li> ");
                }
                else
                {
                    sbTemp.Append("<li><a href=\"" + go + "\">"+i+"</a></li> ");
                }
            }

            if (this._index == this._count)
            {
                string go = string.Format(format, this._index + 1);

                sbTemp.Append("<li class=\"active\"><a href=\"#\">" + this._count + "</a></li> ");

                sbTemp.Append("<li class=\"disabled\"><a href=\"#\">&raquo;</a></li> ");
            }
            else
            {
                string go = string.Format(format, this._index + 1);

                sbTemp.Append("<li><a href=\"" + string.Format(format, this._count) + "\">" + ((this._end < this._count - 1) ? "..." : "") + "" + this._count + "</a></li> ");

                sbTemp.Append("<li><a href=\"" + go + "\">&raquo;</a></li> ");
            }



            #endregion

            return "<ul>" + sbTemp.ToString() + "</ul>";
        }
    }

    
}