//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;


//namespace Devx.DbProvider
//{
//    /// <summary>
//    /// 动态对象
//    /// </summary>
//    public interface IInsertBuilderAnonymous
//    {
//        int Execute();

//        T ExecuteReturnLastId<T>();

//        IInsertBuilderAnonymous Column(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0);
        
//        IInsertBuilderAnonymous Column(string columnName, object value, bool isFunction);

//        IInsertBuilderAnonymous AutoMap(params string[] ignoreProperties);
//    }
//}
