//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Devx.DbProvider
//{
//    /// <summary>
//    /// 匿名对象更新
//    /// </summary>
//    public interface IUpdateBuilderAnonymous
//    {
//        int Execute();

//        IUpdateBuilderAnonymous Column(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0);

//        IUpdateBuilderAnonymous Column(string columnName, object value, bool isFunction);

//        IUpdateBuilderAnonymous Where(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0);

//        IUpdateBuilderAnonymous AutoMap(params string[] ignoreProperties);
//    }
//}
