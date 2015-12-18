using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    public interface IDbProvider
    {
        string ProviderName { get; }
        

        bool SupportsStoredProcedures { get; }
                
        /// <summary>
        /// 创建一个连接
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        System.Data.IDbConnection CreateConnection(string connectionString);

        /// <summary>
        /// 获取参数名称，每个数据库参数化表示不致，MSSQL表示为@
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        string GetParameterName(string parameterName);

        string GetSelectBuilderAlias(string name, string alias);
        string GetSqlForSelectBuilder(BuilderData data);
        string GetSqlForInsertBuilder(BuilderData data);
        string GetSqlForUpdateBuilder(BuilderData data);
        string GetSqlForDeleteBuilder(BuilderData data);       

        /// <summary>
        /// 根据值类别自动返回数据库类型
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        System.Data.DbType GetDbTypeForClrType(Type clrType);

        
        T ExecuteReturnLastId<T>(IDbCommand command);
        
        /// <summary>
        /// 一些字段需要特殊处理
        /// MSSQL表示为[name] 
        /// MYSQL表示为`name`
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string EscapeColumnName(string name);
    }
}
