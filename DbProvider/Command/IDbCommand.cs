using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Devx.DbProvider
{
    public interface IDbCommand : IDisposable
    {
        DbCommandData Data { get; }
        
        /// <summary>
        /// 参数
        /// </summary>
        /// <param name="name">参数名，不需要带@等符号，自动添加</param>
        /// <param name="value">值</param>
        /// <param name="parameterType">类型处理，如果不写，将根据value进行生成</param>
        /// <param name="direction">方向</param>
        /// <param name="size">长度</param>
        /// <returns>当前对象</returns>
        IDbCommand Parameter(string name, object value, System.Data.DbType parameterType = System.Data.DbType.Object, ParameterDirection direction = ParameterDirection.Input, int size = 0);

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <returns>影响的行数</returns>
        int Execute();

        T ExecuteScalar<T>();

        /// <summary>
        /// 返回最新的编号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identityColumnName"></param>
        /// <returns></returns>
        T ExecuteReturnLastId<T>();

        bool QueryPaging(ref int currentPage, int pageSize, out int totalPages, out int totalRecords);

        List<dynamic> Query();

        dynamic QuerySingle();

        TEntity Query<TEntity>(Func<System.Data.IDataReader, TEntity> customMapper);

        TList Query<TEntity, TList>(Func<System.Data.IDataReader, TEntity> customMapper, Func<TEntity, bool> predicate=null) where TList : IList<TEntity>;

        TEntity QuerySingle<TEntity>(Func<IDataReader, TEntity> customMapper);

        DataTable QueryDataTable();

        IDbCommand Sql(string sql);

        /// <summary>
        /// 设置当前的执行方式
        /// </summary>
        /// <param name="dbCommandType"></param>
        /// <returns></returns>
        IDbCommand CommandType(System.Data.CommandType dbCommandType);

    }
}
