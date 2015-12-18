using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Devx.DbProvider
{
    public class DbContext : IDbContext
    {
        /// <summary>
        /// 上下文操作数据
        /// </summary>
        public DbContextData Data { get; private set; }

        public DbContext()
        {
            Data = new DbContextData();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        internal void CloseConnection()
        {
            if (Data.Connection == null)
                return;

            //如果当前使用了事务，且事务没有退出
            if (Data.UseTransaction && Data.Transaction != null)
            {
                Rollback();
            }

            if (Data.Connection.State != ConnectionState.Closed)
            {
                Data.Connection.Close();
            }
            Data.Connection.Dispose();//释放连接资源
        }

        #region Dispose
        private bool m_AlreadyDispose = false;

        ~DbContext()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (this.m_AlreadyDispose)//资源已经释放
                return;
            if (isDisposing)
            {
                this.CloseConnection();
            }
            this.m_AlreadyDispose = true;//已经进行的处理
        }

        /// <summary>
        /// 手动调用释放
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region 创建一个上下文对象
        public IDbContext ConnectionString(string connectionString, DbProviderTypes dbProviderType)
        {
            this.Data.ConnectionString = connectionString;
            this.Data.ProviderType = dbProviderType;
            this.Data.Provider = DbProviderFactory.GetDbProvider(dbProviderType);
            return this;
        }
        public IDbContext ConnectionStringName(string connectionstringName, DbProviderTypes dbProviderType)
        {
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings[connectionstringName];
            if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
                throw new DBClientException("A connectionstring with the specified name was not found in the .config file");

            return ConnectionString(settings.ConnectionString, dbProviderType);
        }

        public IDbContext ConnectionString(string connectionString, IDbProvider dbProvider)
        {
            this.Data.ConnectionString = connectionString;
            this.Data.ProviderType = DbProviderTypes.Custom;
            this.Data.Provider = dbProvider;
            return this;
        }

        #endregion

        #region 执行SQL
        /// <summary>
        /// 创建一个连接
        /// </summary>
        private DbCommand CreateCommand
        {
            get
            {
                IDbConnection connection = null;

                //使用事务或共享连接
                if (Data.UseTransaction || Data.UseSharedConnection)
                {
                    //如果当前没有连接，则创建一个连接
                    if (Data.Connection == null)
                    {
                        Data.Connection = Data.Provider.CreateConnection(Data.ConnectionString);
                    }
                    connection = Data.Connection;
                }
                else
                {
                    connection = Data.Provider.CreateConnection(Data.ConnectionString);
                }
                //创建一个相当的执行命令
                var cmd = connection.CreateCommand();
                cmd.Connection = connection;
                return new DbCommand(this, cmd);
            }
        }

        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDbCommand Sql(string sql, params object[] parameters)
        {
            var command = CreateCommand.Sql(sql);
            if (parameters != null)
            {
                for (var i = 0; i < parameters.Count(); i++)
                    command.Parameter(i.ToString(), parameters[i]);
            }
            return command;
        }

        #endregion
                
        public IDbContext UseSharedConnection(bool useSharedConnection)
        {
            this.Data.UseSharedConnection = useSharedConnection;
            return this;
        }

        #region 事务相关 / Transactions
        public IDbContext UseTransaction(bool useTransaction)
        {
            this.Data.UseTransaction = useTransaction;
            return this;
        }       

        public IDbContext IsolationLevel(System.Data.IsolationLevel isolationLevel)
        {
            this.Data.IsolationLevel = isolationLevel;
            return this;
        }

        public IDbContext Commit()
        {
            TransactionAction(() => Data.Transaction.Commit());
            return this;
        }

        public IDbContext Rollback()
        {
            TransactionAction(() => Data.Transaction.Rollback());
            return this;
        }

        private void TransactionAction(Action action)
        {
            if (Data.Transaction == null)
                return;
            if (!Data.UseTransaction)
                throw new DBClientException("Transaction support has not been enabled.");
            if (action != null)
                action();
            Data.Transaction.Dispose();
            Data.Transaction = null;
        }
        #endregion

        
        public IDbContext CommandTimeout(int timeout)
        {
            this.Data.CommandTimeout = timeout;

            return this;
        }

        public IInsertBuilder Insert(string tableName)
        {
            return new InsertBuilder(CreateCommand, tableName);
        }
        //public IInsertBuilderAnonymous Insert(string tableName, object item)
        //{
        //    return new InsertBuilderAnonymous(CreateCommand, tableName, item);
        //}

        public IInsertBuilder<T> Insert<T>(string tableName, T item) where T : class
        {
            return new InsertBuilder<T>(CreateCommand, tableName, item);
        }
        

        public IUpdateBuilder Update(string tableName)
        {
            return new UpdateBuilder( CreateCommand, tableName);
        }

        //public IUpdateBuilderAnonymous Update(string tableName, object item)
        //{
        //    return new UpdateBuilderAnonymous(CreateCommand, tableName,item);
        //}

        public IUpdateBuilder<T> Update<T>(string tableName, T item) where T : class
        {
            return new UpdateBuilder<T>(CreateCommand, tableName, item);
        }

        public IDeleteBuilder Delete(string tableName)
        {
            return new DeleteBuilder(CreateCommand, tableName);
        }

        public ISelectBuilder<TEntity> Select<TEntity>(string sql)
        {
            return new SelectBuilder<TEntity>(CreateCommand).Select(sql);
        }

    }


}
