using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace Devx.DbProvider
{
    internal class DbTypeMapper
    {
        private static Dictionary<Type, DbType> _types;

        private static readonly object _locker = new object();

        public DbType GetDbTypeForClrType(Type clrType)
        {
            if (_types == null)
            {
                lock (_locker)
                {
                    if (_types == null)
                    {
                        _types = new Dictionary<Type, DbType>();
                        _types.Add(typeof(Int16), DbType.Int16);
                        _types.Add(typeof(Int32), DbType.Int32);
                        _types.Add(typeof(Int64), DbType.Int64);
                        _types.Add(typeof(string), DbType.String);
                        _types.Add(typeof(DateTime), DbType.DateTime);
                        _types.Add(typeof(XDocument), DbType.Xml);
                        _types.Add(typeof(decimal), DbType.Decimal);
                        _types.Add(typeof(Guid), DbType.Guid);
                        _types.Add(typeof(Boolean), DbType.Boolean);
                        _types.Add(typeof(char), DbType.String);
                        _types.Add(typeof(DBNull), DbType.String);
                        _types.Add(typeof(float), DbType.Single);
                        _types.Add(typeof(double), DbType.Double);
                    }
                }
            }

            if (!_types.ContainsKey(clrType))
                return DbType.Object;

            var dbType = _types[clrType];
            return dbType;
        }
    }
}
