using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    internal class InsertBuilder : IInsertBuilder
    {
        protected BuilderData Data { get; set; }
        protected ActionsHandler Actions { get; set; }

       
        public InsertBuilder(IDbCommand command, string name)
        {
            this.Data = new BuilderData(command, name);
            this.Actions = new ActionsHandler(Data);
        }

        public int Execute()
        {
            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForInsertBuilder(Data)).Execute();
        }

        public T ExecuteReturnLastId<T>()
        {
            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForInsertBuilder(Data)).ExecuteReturnLastId<T>();
        }

        public IInsertBuilder Column(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0)
        {
            Actions.Column(columnName, value, parameterType, size);
            return this;
        }

        public IInsertBuilder Column(string columnName, object value, bool isFunction)
        {
            Actions.Column(columnName, value, System.Data.DbType.Object, 0, isFunction);

            return this;
        }
    }
}
