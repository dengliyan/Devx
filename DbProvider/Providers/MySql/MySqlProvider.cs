using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    /// <summary>
    /// MSSQL数据库连接
    /// </summary>
    internal class MySqlProvider : IDbProvider
    {
        public string ProviderName
        {
            get
            {
                return "MySql.Data.MySqlClient";
            }
        }

        public System.Data.IDbConnection CreateConnection(string connectionString)
        {
            return ConnectionFactory.CreateConnection(ProviderName, connectionString);
        }
        
        public string EscapeColumnName(string name)
        {
            return "`" + name + "`";
        }

        public T ExecuteReturnLastId<T>(IDbCommand command)
        {
            //添加
            if (command.Data.InnerCommand.CommandText[command.Data.InnerCommand.CommandText.Length - 1] != ';')
                command.Data.InnerCommand.CommandText += ';';

            command.Data.InnerCommand.CommandText += "select LAST_INSERT_ID() as `lastInsertedId`";

            T result = default(T);

            command.Data.Execute.ExecuteQuery(false, () =>
            {
                object value = command.Data.InnerCommand.ExecuteScalar();

                if (value == null || value == DBNull.Value)
                {
                    result = default(T);
                }
                else
                {
                    if (value.GetType() == typeof(T))
                    {
                        result = (T)value;
                    }
                    else
                    {
                        result = (T)Convert.ChangeType(value, typeof(T));
                    }
                }
            });

            return result;
        }

        public bool SupportsStoredProcedures
        {
            get { return true; }
        }

        public string GetParameterName(string parameterName)
        {
            return "@" + parameterName;
        }

        public System.Data.DbType GetDbTypeForClrType(Type clrType)
        {
            return new DbTypeMapper().GetDbTypeForClrType(clrType);
        }

        public string GetSelectBuilderAlias(string name, string alias)
        {
            return name + " as " + alias;
        }

        public string GetSqlForSelectBuilder(BuilderData data)
        {
            var sql = "";
            sql = "select " + data.Select;
            sql += " from " + data.From;
            if (data.WhereSql.Length > 0)
                sql += " where " + data.WhereSql;
            if (data.GroupBy.Length > 0)
                sql += " group by " + data.GroupBy;
            if (data.Having.Length > 0)
                sql += " having " + data.Having;
            if (data.OrderBy.Length > 0)
                sql += " order by " + data.OrderBy;
            if (data.PagingItemsPerPage > 0
                && data.PagingCurrentPage > 0)
            {
                sql += string.Format(" limit {0}, {1}", data.GetFromItems() - 1, data.PagingItemsPerPage);
            }

            return sql;
        }

        public string GetSqlForInsertBuilder(BuilderData data)
        {
            return SqlGenerator.Insert(this, data);
        }

        public string GetSqlForUpdateBuilder(BuilderData data)
        {
            return SqlGenerator.Update(this, data);
        }

        public string GetSqlForDeleteBuilder(BuilderData data)
        {
            return SqlGenerator.Delete(this, data);
        }
    }
}
