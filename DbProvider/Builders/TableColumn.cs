using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    public class TableColumn
    {
        public string ColumnName { get; set; }
        public string ParameterName { get; set; }
        public object Value { get; set; }
        public bool IsFunction { get; set; }


        public TableColumn(string columnName, object value, string parameterName)
        {
            ColumnName = columnName;
            Value = value;
            ParameterName = parameterName;
            IsFunction = false;
        }

        public TableColumn(string columnName, object value, string parameterName, bool isFunction)
        {
            ColumnName = columnName;
            Value = value;
            ParameterName = parameterName;
            IsFunction = isFunction;
        }
    }
}
