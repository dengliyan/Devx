using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    /// <summary>
    /// 上下文数据
    /// </summary>
    public class DbContextData
    {
        /// <summary>
        /// 是否使用共享连接，设置共享连接，在一个作用域内，不再创建一个新连接
        /// </summary>
        public bool UseSharedConnection { get; set; }

        /// <summary>
        /// 是否使用事务
        /// </summary>
        public bool UseTransaction { get; set; }       

        /// <summary>
        /// 连接
        /// </summary>
        public System.Data.IDbConnection Connection { get; set; }

        /// <summary>
        /// 事务级别
        /// </summary>
        public System.Data.IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// 事务
        /// </summary>
        public System.Data.IDbTransaction Transaction { get; set; }

        /// <summary>
        /// 数据操作对象
        /// </summary>
        public IDbProvider Provider { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public DbProviderTypes ProviderType { get; set; }

        /// <summary>
        /// 操作执行时间
        /// </summary>
        public int CommandTimeout { get; set; }

        public DbContextData()
        {
            UseTransaction = false; 
            UseTransaction = false;
            IsolationLevel = System.Data.IsolationLevel.ReadCommitted;
            CommandTimeout = Int32.MinValue;
        }
    }
}
