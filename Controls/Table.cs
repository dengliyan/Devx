//code by ly.deng

//email:liydotnet@gmail.com

using System;
using System.Data;
using System.Web.UI.WebControls;

namespace Devx.Controls
{

    public class Table
    {
        #region 枚举定义
        public enum TableTag
        {
            Table,
            TableRow,
            TableHeaderCell,
            TableCell
        }

        public enum FieldSort : int
        {
            Asc = 0,
            Desc = 1
        }

        public enum AlignMode
        {
            Left,
            Right,
            Center
        }
        #endregion

        #region 策略对象
        private TableStrategy _ts = null;
        #endregion

        #region 构造函数
        public Table()
        {
            this._ts = new DefaultTableStrategy();
        }

        public Table(TableStrategy ts)
        {
            this._ts = ts;
        }

        /// <summary>
        /// 兼容模式
        /// </summary>
        /// <param name="field"></param>
        /// <param name="sort"></param>
        public Table(string field, FieldSort sort)
        {
            this._ts = new DefaultTableStrategy(field, sort) { };
        }
        #endregion

        #region 方法定义
        /// <summary>
        /// 新建一行数据
        /// </summary>
        /// <returns></returns>
        public Table NewRow()
        {
            this._ts.NewRow();

            return this;
        }

        /// <summary>
        /// 新建一行数据
        /// </summary>
        /// <returns></returns>
        public Table NewRow(TableRowSection section)
        {
            this._ts.NewRow(section);

            return this;
        }

        /// <summary>
        /// 结束一行数据
        /// </summary>
        /// <returns></returns>
        public Table EndRow()
        {
            this._ts.EndRow();

            return this;
        }

        /// <summary>
        /// 添加表头单元格
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Table AddTH(string text)
        {
            this._ts.AddTH(text);

            return this;
        }

        /// <summary>
        /// 添加单元格
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Table AddTD(string text)
        {
            this._ts.AddTD(text);

            return this;
        }

        /// <summary>
        /// 添加样式
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public Table Class(string classname)
        {
            this._ts.Class(classname);

            return this;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Table Attribute(string name, string properties)
        {
            this._ts.AddAttribute(name, properties);

            return this;
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public Table Compare(string compare)
        {
            this._ts.Compare(compare);

            return this;
        }

        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="defaultsort"></param>
        /// <returns></returns>
        public Table Compare(string compare, FieldSort defaultsort)
        {
            this._ts.Compare(compare, defaultsort);

            return this;
        }

        /// <summary>
        /// 设置Style样式
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public Table Css(string properties)
        {
            this._ts.AddAttribute("style", properties);

            return this;
        }

        /// <summary>
        /// 对齐属性
        /// </summary>
        /// <param name="align"></param>
        /// <returns></returns>
        public Table Align(Table.AlignMode align)
        {
            this._ts.AddAttribute("align", align.ToString());

            return this;
        }

        /// <summary>
        /// 设置ColumnSpan属性，对TD、TH有效
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Table ColumnSpan(int value)
        {
            if (this._ts.Tag == TableTag.TableCell || this._ts.Tag == TableTag.TableHeaderCell)
            {
                this._ts.AddAttribute("colspan", value.ToString());
            }

            return this;
        }

        /// <summary>
        /// 设置RowSpan属性，对TD、TH有效
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Table RowSpan(int value)
        {
            if (this._ts.Tag == TableTag.TableCell || this._ts.Tag == TableTag.TableHeaderCell)
            {
                this._ts.AddAttribute("rowspan", value.ToString());
            }

            return this;
        }

        /// <summary>
        /// 设置Table的CellPadding属性
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Table CellPadding(int value)
        {
            if (this._ts.Tag == TableTag.Table)
            {
                this._ts.AddAttribute("cellpadding", value.ToString());
            }

            return this;
        }

        /// <summary>
        /// 设置Table的CellSpacing属性
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Table CellSpacing(int value)
        {
            if (this._ts.Tag == TableTag.Table)
            {
                this._ts.AddAttribute("cellspacing", value.ToString());
            }

            return this;
        }

        #endregion

        #region 数据输出
        public override string ToString()
        {
            return this._ts.ProduceTable();
        }
        #endregion

        #region 策略定义

        public abstract class TableStrategy
        {
            protected System.Web.UI.WebControls.Table _table = null;
            protected System.Web.UI.WebControls.TableRow _row = null;
            protected System.Web.UI.WebControls.TableCell _cell = null;
            protected System.Web.UI.WebControls.TableCell _cellHeader = null;

            protected string[] icon = new string[] { "▲", "▼" };
            protected TableTag _tag = TableTag.Table;
            protected int _index = 0;

            public TableTag Tag { get { return this._tag; } }


            public virtual void NewRow()
            {
                this._tag = TableTag.TableRow;
                _row = new TableRow();
                _row.TableSection = TableRowSection.TableBody;
                this._index = 0;
            }

            public virtual void NewRow(TableRowSection section)
            {
                this._tag = TableTag.TableRow;
                _row = new TableRow();
                _row.TableSection = section;
                this._index = 0;
            }

            public virtual void EndRow()
            {
                this._table.Rows.Add(this._row);
            }

            public virtual void Class(string classname)
            {
                switch (this._tag)
                {
                    case TableTag.Table:

                        #region 表格添加样式
                        _table.CssClass = classname;
                        #endregion

                        break;

                    case TableTag.TableCell:

                        #region 单元格样式
                        if (_cell.CssClass != null && _cell.CssClass.Length > 0)
                        {
                            _cell.CssClass = _cell.CssClass + " " + classname;
                        }
                        else
                        {
                            _cell.CssClass = classname;
                        }
                        #endregion

                        this._row.Cells.AddAt(this._index - 1, this._cell);

                        break;

                    case TableTag.TableHeaderCell:

                        #region 表头样式
                        if (_cellHeader.CssClass != null && _cellHeader.CssClass.Length > 0)
                        {
                            _cellHeader.CssClass = _cellHeader.CssClass + " " + classname;
                        }
                        else
                        {
                            _cellHeader.CssClass = classname;
                        }
                        #endregion
                        this._row.Cells.AddAt(this._index - 1, this._cellHeader);
                        break;

                    case TableTag.TableRow:

                        #region 行添加
                        _row.CssClass = classname;
                        #endregion

                        break;
                }
            }

            public virtual void AddAttribute(string name, string properties)
            {
                switch (this._tag)
                {
                    case TableTag.Table:

                        _table.Attributes.Add(name, properties);

                        break;

                    case TableTag.TableCell:

                        this._cell.Attributes.Add(name, properties);

                        this._row.Cells.AddAt(this._index - 1, this._cell);

                        break;

                    case TableTag.TableHeaderCell:

                        this._cellHeader.Attributes.Add(name, properties);
                        this._row.Cells.AddAt(this._index - 1, this._cellHeader);
                        break;

                    case TableTag.TableRow:

                        _row.Attributes.Add(name, properties);

                        break;
                }
            }

            public virtual void RemoveAttribute(string name)
            {
                switch (this._tag)
                {
                    case TableTag.Table:

                        _table.Attributes.Remove(name);

                        break;

                    case TableTag.TableCell:

                        this._cell.Attributes.Remove(name);

                        this._row.Cells.AddAt(this._index - 1, this._cell);

                        break;

                    case TableTag.TableHeaderCell:

                        this._cellHeader.Attributes.Remove(name);
                        this._row.Cells.AddAt(this._index - 1, this._cellHeader);
                        break;

                    case TableTag.TableRow:

                        _row.Attributes.Remove(name);

                        break;
                }
            }



            public virtual void AddTD(string text)
            {
                this._tag = TableTag.TableCell;
                this._cell = new TableCell();
                this._cell.Text = text;
                this._row.Cells.AddAt(this._index, _cell);
                this._index++;
            }

            public virtual void AddTH(string text)
            {
                this._tag = TableTag.TableHeaderCell;
                _cellHeader = new TableHeaderCell();//此处根据实际情况进行调整
                _cellHeader.Text = text;
                _row.Cells.AddAt(this._index, _cellHeader);
                this._index++;
            }

            public abstract void Compare(string compare);

            public abstract void Compare(string compare, FieldSort defaultsort);

            public virtual string ProduceTable()
            {
                System.Text.StringBuilder sbTemp = new System.Text.StringBuilder();

                using (System.Web.UI.HtmlTextWriter htmlWriter = new System.Web.UI.HtmlTextWriter(new System.IO.StringWriter(sbTemp)))
                {
                    this._table.RenderControl(htmlWriter);
                }

                return sbTemp.ToString();
            }
        }
        #endregion

        #region 默认表格样式
        /// <summary>
        /// 默认表格样式
        /// </summary>
        public class DefaultTableStrategy : TableStrategy
        {
            public string OrderField { get; protected set; }
            public FieldSort Sort { get; protected set; }
            public string[] Icon { set { this.icon = value; } }
            public string OrderingEvent { get; set; }
            public FieldSort DefaultSort { get; set; }

            public DefaultTableStrategy()
            {
                this._tag = TableTag.Table;
                this._table = new System.Web.UI.WebControls.Table();
                //this._table.CellPadding = 0;
                //this._table.CellSpacing = 1;
                //this._table.BorderWidth = 0;
                //this._table.Attributes.Add("width", "100%");
                this.icon = new string[] { "▲", "▼" };
            }

           

            public DefaultTableStrategy(string field, FieldSort sort)
            {
                this._tag = TableTag.Table;
                this._table = new System.Web.UI.WebControls.Table();
                //this._table.CellPadding = 0;
                //this._table.CellSpacing = 1;
                //this._table.BorderWidth = 0;
                //this._table.Attributes.Add("width", "100%");
                this.OrderField = field;//排序字段
                this.Sort = sort;//排序方式
                this.DefaultSort = FieldSort.Desc;//默认排序
                this.OrderingEvent = "javascript:pSort('{0}',{1})";//默认排序事件
                this.icon = new string[] { "▲", "▼" };
            }

            public DefaultTableStrategy(string field, FieldSort sort, Action<System.Web.UI.WebControls.Table> builder)
            {
                this._tag = TableTag.Table;
                this._table = new System.Web.UI.WebControls.Table();
                if (builder != null)
                {
                    builder(this._table);
                }
                this.OrderField = field;//排序字段
                this.Sort = sort;//排序方式
                this.DefaultSort = FieldSort.Desc;//默认排序
                this.OrderingEvent = "javascript:pSort('{0}',{1})";//默认排序事件
            }

            public DefaultTableStrategy(Action<System.Web.UI.WebControls.Table> init)
            {
                this._tag = TableTag.Table;
                this._table = new System.Web.UI.WebControls.Table();
                if (init != null)
                {
                    init(this._table);
                }
            }


            public override void Compare(string compare)
            {
                switch (this._tag)
                {
                    case TableTag.TableCell:

                        if (this.OrderField == compare)
                        {
                            if (_cell.CssClass != null && _cell.CssClass.Length > 0)
                            {
                                _cell.CssClass = _cell.CssClass + " s";
                            }
                            else
                            {
                                _cell.CssClass = "s";
                            }

                            this._row.Cells.AddAt(this._index - 1, this._cell);
                        }

                        break;

                    case TableTag.TableHeaderCell:

                        string format = "<a href=\"{0}\" target=\"_self\">{1}</a>";
                        string text = this._cellHeader.Text;
                        if (this.OrderField == compare)
                        {
                            if (this._cellHeader.CssClass != null && this._cellHeader.CssClass.Length > 0)
                            {
                                this._cellHeader.CssClass = this._cellHeader.CssClass + " s";
                            }
                            else
                            {
                                this._cellHeader.CssClass = "s";
                            }

                            text = text + icon[(int)this.Sort];
                            text = string.Format(format, string.Format(this.OrderingEvent, this.OrderField, (((int)this.Sort) + 1) % 2), text);
                        }
                        else
                        {
                            text = string.Format(format, string.Format(this.OrderingEvent, compare, (int)this.DefaultSort), text);
                        }
                        this._cellHeader.Text = text;
                        this._row.Cells.AddAt(this._index - 1, this._cellHeader);
                        break;
                }
            }

            public override void Compare(string compare, FieldSort defaultsort)
            {
                switch (this._tag)
                {

                    case TableTag.TableHeaderCell:

                        string format = "<a href=\"{0}\" target=\"_self\">{1}</a>";
                        string text = this._cellHeader.Text;
                        if (this.OrderField == compare)
                        {
                            if (this._cellHeader.CssClass != null && this._cellHeader.CssClass.Length > 0)
                            {
                                this._cellHeader.CssClass = this._cellHeader.CssClass + " s";
                            }
                            else
                            {
                                this._cellHeader.CssClass = "s";
                            }

                            text = text + icon[(int)this.Sort];
                            text = string.Format(format, string.Format(this.OrderingEvent, this.OrderField, (((int)this.Sort) + 1) % 2), text);
                        }
                        else
                        {
                            text = string.Format(format, string.Format(this.OrderingEvent, compare, (int)defaultsort), text);
                        }
                        this._cellHeader.Text = text;
                        this._row.Cells.AddAt(this._index - 1, this._cellHeader);
                        break;
                }
            }
        }

        #endregion
    }
}
