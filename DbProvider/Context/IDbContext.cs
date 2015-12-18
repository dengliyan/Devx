using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    public interface IDbContext : IDisposable
    {
        /// <summary>
        /// 当前对象数据
        /// </summary>
        DbContextData Data { get; }

        /// <summary>
        /// 创建一个带连接实例
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbProviderType"></param>
        /// <returns></returns>
        IDbContext ConnectionString(string connectionString, DbProviderTypes dbProviderType);

        /// <summary>
        /// 创建一个带连接实例
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="dbProviderType"></param>
        /// <returns></returns>
        IDbContext ConnectionString(string connectionString, IDbProvider dbProvider);

       
        /// <summary>
        /// 执行时间
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        IDbContext CommandTimeout(int timeout);

        /// <summary>
        /// 添加SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IDbCommand Sql(string sql, params object[] parameters);


        IInsertBuilder Insert(string tableName);
        
        //IInsertBuilderAnonymous Insert(string tableName, object item);

        IInsertBuilder<T> Insert<T>(string tableName, T item) where T : class;

        
        IUpdateBuilder Update(string tableName);

        //IUpdateBuilderAnonymous Update(string tableName, object item);


        IUpdateBuilder<T> Update<T>(string tableName, T item) where T : class;

        IDeleteBuilder Delete(string tableName);

        ISelectBuilder<TEntity> Select<TEntity>(string sql);


        IDbContext UseSharedConnection(bool useSharedConnection);

        IDbContext UseTransaction(bool useTransaction);

        IDbContext IsolationLevel(System.Data.IsolationLevel isolationLevel);

        IDbContext Commit();

        IDbContext Rollback();
    }
}
