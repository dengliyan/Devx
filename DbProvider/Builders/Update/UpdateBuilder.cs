using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Devx.DbProvider
{
    internal class UpdateBuilder : IUpdateBuilder
    {
        protected BuilderData Data { get; set; }
		protected ActionsHandler Actions { get; set; }


        public UpdateBuilder(IDbCommand command, string name)
		{
			Data =  new BuilderData(command, name);
			Actions = new ActionsHandler(Data);
		}

		public int Execute()
		{
            if (Data.Columns.Count == 0
                    || Data.Where.Count == 0)
                throw new DBClientException("Columns or where filter have not yet been added.");

            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForUpdateBuilder(Data)).Execute();
		}

        public IUpdateBuilder Column(string columnName, object value, DbType parameterType = DbType.Object, int size = 0)
        {
            Actions.Column(columnName, value, parameterType, size);
            return this;
        }

        public IUpdateBuilder Column(string columnName, object value, bool isFunction)
        {
            Actions.Column(columnName, value, System.Data.DbType.Object, 0, isFunction);

            return this;
        }

        public IUpdateBuilder Where(string columnName, object value, DbType parameterType = DbType.Object, int size = 0)
        {
            Actions.Where(columnName, value, parameterType, size);
			return this;
        }
    }
}
