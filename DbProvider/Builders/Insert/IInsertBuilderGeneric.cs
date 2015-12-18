using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Devx.DbProvider
{
    public interface IInsertBuilder<T> where T : class
    {
        int Execute();

        V ExecuteReturnLastId<V>();

        IInsertBuilder<T> Column(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0);
        
        IInsertBuilder<T> Column(string columnName, object value, bool isFunction);

        IInsertBuilder<T> AutoMap(params Expression<Func<T, object>>[] ignoreProperties);
    }
}
