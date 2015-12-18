//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;

//namespace Devx.DbProvider
//{
//    internal class UpdateBuilderAnonymous : IUpdateBuilderAnonymous
//    {
//        protected BuilderData Data { get; set; }
//        protected ActionsHandler Actions { get; set; }

//        internal UpdateBuilderAnonymous(IDbCommand command, string name, object item)
//        {
//            if (item != null && !ReflectionHelper.IsAnonymousType(item.GetType()))//如果类别不正确
//            {
//                throw new DBClientException("data is not a anonymous type.");
//            }
//            this.Data = new BuilderData(command, name);
//            this.Data.Item = item;
//            this.Actions = new ActionsHandler(Data);
//        }


//        public int Execute()
//        {
//            if (Data.Columns.Count == 0 || Data.Where.Count == 0)
//                throw new DBClientException("Columns or where filter have not yet been added.");

//            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForUpdateBuilder(Data)).Execute();
//        }

//        public IUpdateBuilderAnonymous Column(string columnName, object value, DbType parameterType = DbType.Object, int size = 0)
//        {
//            Actions.Column(columnName, value, parameterType, size);
//            return this;
//        }

//        public IUpdateBuilderAnonymous Column(string columnName, object value, bool isFunction)
//        {
//            Actions.Column(columnName, value, System.Data.DbType.Object, 0, isFunction);

//            return this;
//        }

//        public IUpdateBuilderAnonymous Where(string columnName, object value, DbType parameterType = DbType.Object, int size = 0)
//        {
//            Actions.Where(columnName, value, parameterType, size);
//            return this;
//        }

//        public IUpdateBuilderAnonymous AutoMap(params string[] ignoreProperties)
//        {
//            if (this.Data.Item != null)
//            {
//                Actions.AutoMapAnonymousTypeColumns(ignoreProperties);
//            }
//            return this;
//        }
//    }
//}
