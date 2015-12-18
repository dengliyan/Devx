using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Devx.DbProvider
{
    internal class InsertBuilder<T> : IInsertBuilder<T> where T:class
    {
        protected BuilderData Data { get; set; }

        protected ActionsHandler Actions { get; set; }

        public InsertBuilder(IDbCommand command, string name, T item)
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
            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForInsertBuilder(Data)).Execute();
        }

        public V ExecuteReturnLastId<V>()
        {
            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForInsertBuilder(Data)).ExecuteReturnLastId<V>();
        }

        public IInsertBuilder<T> Column(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0)
        {
            Actions.Column(columnName, value, parameterType, size);
            return this;
        }

        public IInsertBuilder<T> Column(string columnName, object value, bool isFunction)
        {
            Actions.Column(columnName, value, System.Data.DbType.Object, 0, isFunction);

            return this;
        }

        public IInsertBuilder<T> AutoMap(params Expression<Func<T, object>>[] ignoreProperties)
        {            
            this.Actions.AutoMapColumns<T>(ignoreProperties);
            return this;
        }


    }
}
