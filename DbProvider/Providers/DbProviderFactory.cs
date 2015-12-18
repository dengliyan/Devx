using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    internal class DbProviderFactory
    {
        public static IDbProvider GetDbProvider(DbProviderTypes dbProvider)
        {
            IDbProvider provider = null;
            switch (dbProvider)
            {
                case DbProviderTypes.SqlServer:
                //case DbProviderTypes.SqlAzure:
                    provider = new SqlServerProvider();
                    break;
                //case DbProviderTypes.SqlServerCompact40:
                //    provider = new SqlServerCompactProvider();
                //    break;
                //case DbProviderTypes.Oracle:
                //    provider = new OracleProvider();
                //    break;
                case DbProviderTypes.MySql:
                    provider = new MySqlProvider();
                    break;
                //case DbProviderTypes.Access:
                //    provider = new AccessProvider();
                //    break;
                //case DbProviderTypes.Sqlite:
                //    provider = new Sqlite();
                //    break;
                //case DbProviderTypes.PostgreSql:
                //    provider = new PostgreSqlProvider();
                //    break;
                //case DbProviderTypes.DB2:
                //    provider = new DB2Provider();
                //    break;
            }

            if (provider == null)
            {
                throw new DBClientException("DBProvider:" + dbProvider + " Not Supported.");
            }

            return provider;
        }
    }
}
