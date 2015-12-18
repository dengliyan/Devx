using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    internal class SqlGenerator
    {
        public static string Insert(IDbProvider provider, BuilderData data)
        {
            var insertSql = "";
            var valuesSql = "";
            foreach (var column in data.Columns)
            {
                if (insertSql.Length > 0)
                {
                    insertSql += ",";
                    valuesSql += ",";
                }

                insertSql += provider.EscapeColumnName(column.ColumnName);

                valuesSql += (!column.IsFunction ? provider.GetParameterName(column.ParameterName) : column.Value);
            }

            var sql = string.Format("insert into {0}({1}) values({2})",
                                        data.ObjectName,
                                        insertSql,
                                        valuesSql);
            return sql;
        }

        public static string Update(IDbProvider provider, BuilderData data)
        {
            var setSql = "";
            foreach (var column in data.Columns)
            {
                if (setSql.Length > 0)
                    setSql += ", ";

                setSql += string.Format("{0} = {1}",
                                    provider.EscapeColumnName(column.ColumnName),
                                    !column.IsFunction ? provider.GetParameterName(column.ParameterName) : column.Value);
            }

            var whereSql = "";
            foreach (var column in data.Where)
            {
                if (whereSql.Length > 0)
                    whereSql += " and ";

                whereSql += string.Format("{0} = {1}",
                                    provider.EscapeColumnName(column.ColumnName),
                                    provider.GetParameterName(column.ParameterName));
            }

            var sql = string.Format("update {0} set {1} where {2}",
                                        data.ObjectName,
                                        setSql,
                                        whereSql);
            return sql;
        }

        public static string Delete(IDbProvider provider, BuilderData data)
        {
            var whereSql = "";
            foreach (var column in data.Where)
            {
                if (whereSql.Length > 0)
                    whereSql += " and ";

                whereSql += string.Format("{0} = {1}",
                                                provider.EscapeColumnName(column.ColumnName),
                                                provider.GetParameterName(column.ParameterName));
            }

            var sql = string.Format("delete from {0} where {1}", data.ObjectName, whereSql);
            return sql;
        }

        public static string Count(IDbProvider provider, BuilderData data)
        {
            var sql = "";
            if (data.GroupBy.Length > 0 || data.Having.Length > 0)
            {
                var child = "select " + data.Select;
                child += " from " + data.From;
                if (data.WhereSql.Length > 0)
                    child += " where " + data.WhereSql;
                if (data.GroupBy.Length > 0)
                    child += " group by " + data.GroupBy;
                if (data.Having.Length > 0)
                    child += " having " + data.Having;

                sql = "select " + provider.GetSelectBuilderAlias("count(0)", "record") + " from " + provider.GetSelectBuilderAlias("(" + child + ")", "records");
            }
            else
            {
                sql = "select " + provider.GetSelectBuilderAlias("count(0)", "record");
                sql += " from " + data.From;
                if (data.WhereSql.Length > 0)
                    sql += " where " + data.WhereSql;
            }
            return sql;
        }
    }
}
