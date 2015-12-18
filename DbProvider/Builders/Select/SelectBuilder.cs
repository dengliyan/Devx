using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Devx.DbProvider
{
    internal class SelectBuilder<TEntity> : ISelectBuilder<TEntity>
    {
        protected BuilderData Data { get; set; }
        protected ActionsHandler Actions { get; set; }

        private IDbCommand Command
        {
            get
            {
                if (Data.PagingItemsPerPage > 0
                    && string.IsNullOrEmpty(Data.OrderBy))
                    throw new DBClientException("Order by must defined when using Paging.");

                Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForSelectBuilder(Data));
                return Data.Command;
            }
        }

        public SelectBuilder(IDbCommand command)
        {
            Data = new BuilderData(command, "");
            Actions = new ActionsHandler(Data);
        }

        public ISelectBuilder<TEntity> Select(string sql)
        {
            Data.Select += sql;
            return this;
        }

        public ISelectBuilder<TEntity> From(string sql)
        {
            Data.From += sql;
            return this;
        }

        public ISelectBuilder<TEntity> Where(string sql)
        {
            Data.WhereSql += sql;
            return this;
        }

        public ISelectBuilder<TEntity> WhereAnd(string sql)
        {
            if (Data.WhereSql.Length > 0)
                Where(" and ");
            Where(sql);
            return this;
        }

        public ISelectBuilder<TEntity> WhereOr(string sql)
        {
            if (Data.WhereSql.Length > 0)
                Where(" or ");
            Where(sql);
            return this;
        }

        public ISelectBuilder<TEntity> OrderBy(string sql)
        {
            Data.OrderBy += sql;
            return this;
        }

        public ISelectBuilder<TEntity> GroupBy(string sql)
        {
            Data.GroupBy += sql;
            return this;
        }

        public ISelectBuilder<TEntity> Having(string sql)
        {
            Data.Having += sql;
            return this;
        }

        public ISelectBuilder<TEntity> Paging(int currentPage, int itemsPerPage)
        {
            Data.PagingCurrentPage = currentPage;
            Data.PagingItemsPerPage = itemsPerPage;
            return this;
        }

        public ISelectBuilder<TEntity> Parameter(string name, object value)
        {
            Data.Command.Parameter(name, value);
            return this;
        }

        public TList Query<TList>(Func<IDataReader, TEntity> customMapper, Func<TEntity, bool> predicate = null) where TList : IList<TEntity>
        {
            //当分页时，必须进行排序
            if (Data.PagingItemsPerPage > 0 && string.IsNullOrEmpty(Data.OrderBy))
            {
                throw new DBClientException("Order by must defined when using Paging.");
            }

            //执行
            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForSelectBuilder(Data)).Query<TEntity, TList>(customMapper);
        }

        public TEntity QuerySingle(Func<IDataReader, TEntity> customMapper)
        {
            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForSelectBuilder(Data)).QuerySingle<TEntity>(customMapper);
        }

        public TEntity Query<TEntity>(Func<System.Data.IDataReader, TEntity> customMapper)
        {
            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForSelectBuilder(Data)).Query<TEntity>(customMapper);
        }

        //public int QueryPaging(ref int currentPage, int itemsPerPage, out int totalPages, out int totalRecords)
        //{
        //    totalPages = totalRecords = 0;
        //    return 0;
        //}
    }
}
