using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Devx.DbProvider
{
    internal class ConnectionFactory
    {
        /// <summary>
        /// 创建一个连接
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IDbConnection CreateConnection(string providerName, string connectionString)
        {
            var factory = DbProviderFactories.GetFactory(providerName);
            var connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }
    }
}
