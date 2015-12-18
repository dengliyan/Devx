//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Dynamic;

//namespace Devx.DbProvider
//{
//    internal class InsertBuilderAnonymous : IInsertBuilderAnonymous
//    {
//        protected BuilderData Data { get; set; }
//        protected ActionsHandler Actions { get; set; }

//        internal InsertBuilderAnonymous(IDbCommand command, string name, object item)
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
//            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForInsertBuilder(Data)).Execute();
//        }

//        public V ExecuteReturnLastId<V>()
//        {
//            return Data.Command.Sql(Data.Command.Data.Context.Data.Provider.GetSqlForInsertBuilder(Data)).ExecuteReturnLastId<V>();
//        }
        
//        public IInsertBuilderAnonymous Column(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0)
//        {
//            Actions.Column(columnName, value, parameterType, size);
//            return this;
//        }

//        public IInsertBuilderAnonymous Column(string columnName, object value, bool isFunction)
//        {
//            Actions.Column(columnName, value, System.Data.DbType.Object, 0, isFunction);

//            return this;
//        }

//        public IInsertBuilderAnonymous AutoMap(params string[] ignoreProperties)
//        {
//            if (this.Data.Item != null)
//            {
//                Actions.AutoMapAnonymousTypeColumns(ignoreProperties);
//            }
//            return this;
//        }

//    }
//}
