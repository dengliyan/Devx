using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Devx.DbProvider
{
    public interface IUpdateBuilder<T> where T : class
    {
        int Execute();

        IUpdateBuilder<T> Column(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0);

        IUpdateBuilder<T> Column(string columnName, object value, bool isFunction);

        IUpdateBuilder<T> Where(string columnName, object value, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0);

        IUpdateBuilder<T> Where(Expression<Func<T, object>> expression, System.Data.DbType parameterType = System.Data.DbType.Object, int size = 0);

        IUpdateBuilder<T> AutoMap(params Expression<Func<T, object>>[] ignoreProperties);
    }
}
