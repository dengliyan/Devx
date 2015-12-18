using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Devx.DbProvider
{
    internal class UpdateBuilder<T> : IUpdateBuilder<T> where T : class
    {
        protected BuilderData Data { get; set; }
        protected ActionsHandler Actions { get; set; }

        internal UpdateBuilder(IDbCommand command, string name, T item)
        {
            if (item == null)
            {
                throw new DBClientException("insert data is null.");
            }
            this.Data = new BuilderData(command, name);
            this.Actions = new ActionsHandler(Data);
            this.Data.Item = item;
        }


        public int Execute()
        {
            if (Data.Columns.Count == 0 || Data.Where.Count == 0)
                throw new DBClientException("Columns or where filter have not yet been added.");

            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForUpdateBuilder(Data)).Execute();
        }
        
        public IUpdateBuilder<T> Where(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0)
        {
            Actions.Where(columnName, value, parameterType, size);
            return this;
        }

        public IUpdateBuilder<T> Where(Expression<Func<T, object>> expression, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0)
        {
            Actions.Where<T>(expression, parameterType, size);
            return this;
        }

        public IUpdateBuilder<T> Column(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0)
        {
            Actions.Column(columnName, value, parameterType, size);
            return this;
        }

        public IUpdateBuilder<T> Column(string columnName, object value, bool isFunction)
        {
            Actions.Column(columnName, value, System.Data.DbType.Object, 0, isFunction);

            return this;
        }

        public IUpdateBuilder<T> AutoMap(params Expression<Func<T, object>>[] ignoreProperties)
        {
            this.Actions.AutoMapColumns<T>(ignoreProperties);
            return this;
        }
    }
}
