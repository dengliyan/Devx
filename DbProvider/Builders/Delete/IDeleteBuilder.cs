using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Devx.DbProvider
{
    public interface IDeleteBuilder
    {
        int Execute();

        IDeleteBuilder Where(string columnName, object value, DbType parameterType = DbType.Object, int size = 0);
    }
}
