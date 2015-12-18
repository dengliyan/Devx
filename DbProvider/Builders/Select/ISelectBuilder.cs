using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Devx.DbProvider
{
    public interface ISelectBuilder<TEntity>
    {
        ISelectBuilder<TEntity> Select(string sql);
        ISelectBuilder<TEntity> From(string sql);
        ISelectBuilder<TEntity> Where(string sql);
        ISelectBuilder<TEntity> WhereAnd(string sql);
        ISelectBuilder<TEntity> WhereOr(string sql);
        ISelectBuilder<TEntity> GroupBy(string sql);
        ISelectBuilder<TEntity> OrderBy(string sql);
        ISelectBuilder<TEntity> Having(string sql);
        ISelectBuilder<TEntity> Paging(int currentPage, int itemsPerPage);

        ISelectBuilder<TEntity> Parameter(string name, object value);

        TList Query<TList>(Func<IDataReader, TEntity> customMapper, Func<TEntity, bool> predicate = null) where TList : IList<TEntity>;

        /// <summary>
        /// 带分页查询
        /// </summary>
        /// <typeparam name="TList"></typeparam>
        /// <param name="customMapper"></param>
        /// <param name="currentPage">当前的页码</param>
        /// <param name="totalPages">总页数</param>
        /// <param name="itemsTotalRecords">总记录数</param>
        /// <returns></returns>
        //TList Query<TList>(Func<IDataReader, TEntity> customMapper, out int currentPage, out int totalPages, out int totalRecords) where TList : IList<TEntity>;

        TEntity QuerySingle(Func<IDataReader, TEntity> customMapper = null);

        TEntity Query<TEntity>(Func<System.Data.IDataReader, TEntity> customMapper);
        
    }
}
