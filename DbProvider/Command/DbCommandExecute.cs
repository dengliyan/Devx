using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    internal class DbCommandExecute
    {
        private readonly DbCommand _command;

        private bool _queryAlreadyExecuted = false;

        public DbCommandExecute(DbCommand command)
        {
            _command = command;
        }

        internal void ExecuteQuery(bool useReader, Action action)
        {
            try
            {
                #region 初始化一个查询
                if (this._queryAlreadyExecuted)
                {
                    throw new DBClientException("A query has already been executed on this command object. Please create a new command object.");
                }

                //如果当前连接需要设置执行时间
                if (this._command.Data.Context.Data.CommandTimeout != Int32.MinValue && this._command.Data.Context.Data.CommandTimeout > 0)
                    this._command.Data.InnerCommand.CommandTimeout = this._command.Data.Context.Data.CommandTimeout;

                //如果当前连接未打开，则开启
                if (this._command.Data.InnerCommand.Connection.State != System.Data.ConnectionState.Open)
                {
                    this._command.Data.InnerCommand.Connection.Open();
                }

                if (this._command.Data.Context.Data.UseTransaction)
                {
                    //创建事务
                    if (this._command.Data.Context.Data.Transaction == null)
                        this._command.Data.Context.Data.Transaction = this._command.Data.Context.Data.Connection.BeginTransaction(this._command.Data.Context.Data.IsolationLevel);

                    //初始化事务到连接对象
                    this._command.Data.InnerCommand.Transaction = this._command.Data.Context.Data.Transaction;
                }
                #endregion

                if (useReader)
                {
                    this._command.Data.Reader = this._command.Data.InnerCommand.ExecuteReader();
                }

                if (action == null) throw new DBClientException("未指定任何查询操作！！");

                action();

                this._queryAlreadyExecuted = true;
            }
            catch (Exception ex)
            {
                throw new DBClientException(ex.ToString()) { CmdText = this._command.Data.InnerCommand.CommandText };
            }
            finally
            {
                this._command.CloseCommand();
            }
        }
    }
}
