using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    internal class DeleteBuilder : IDeleteBuilder
    {
        protected BuilderData Data { get; set; }
		protected ActionsHandler Actions { get; set; }

		private IDbCommand Command
		{
			get
			{
				Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForDeleteBuilder(Data));
				return Data.Command;
			}
		}

        public DeleteBuilder(IDbCommand command, string name)
		{
			Data =  new BuilderData(command, name);
			Actions = new ActionsHandler(Data);
		}

		public int Execute()
		{
            if (this.Data.Where.Count == 0)
            {
                throw new DBClientException("where filter have not yet been added.");
            }
			return Command.Execute();
		}

        public IDeleteBuilder Where(string columnName, object value, System.Data.DbType parameterType, int size)
		{
			Actions.Where(columnName, value, parameterType, size);
			return this;
		}
    }
}
