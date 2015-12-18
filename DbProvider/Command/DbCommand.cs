using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    internal class DbCommand : IDbCommand
    {
        public DbCommandData Data { get; private set; }

        public DbCommand(DbContext dbContext, System.Data.IDbCommand innerCommand)
        {
            Data = new DbCommandData(dbContext, innerCommand);
            
            this.Data.Execute = new DbCommandExecute(this);
        }

        public IDbCommand CommandType(System.Data.CommandType dbCommandType)
        {
            this.Data.InnerCommand.CommandType = dbCommandType;
            return this;
        }   
        
        /// <summary>
        /// 参数定义
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="parameterType"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IDbCommand Parameter(string name
            , object value
            , System.Data.DbType parameterType = System.Data.DbType.Object
            , System.Data.ParameterDirection direction = System.Data.ParameterDirection.Input
            , int size = 0)
        {
            if (value == null)
                value = DBNull.Value;

            if (value.GetType().IsEnum)
                value = (int)value;

            var dbParameter = Data.InnerCommand.CreateParameter();
            if (parameterType == System.Data.DbType.Object)
                dbParameter.DbType = (System.Data.DbType)Data.Context.Data.Provider.GetDbTypeForClrType(value.GetType());
            else
                dbParameter.DbType = parameterType;

            dbParameter.ParameterName = Data.Context.Data.Provider.GetParameterName(name);
            dbParameter.Direction = direction;
            if (value == null)
            {
                dbParameter.Value = DBNull.Value;
            }
            else
            {

                dbParameter.Value = value;
            }
            if (size > 0)
                dbParameter.Size = size;

            Data.InnerCommand.Parameters.Add(dbParameter);

            return this;
        }

        public IDbCommand Sql(string sql)
        {
            Data.InnerCommand.CommandText = sql;
            return this;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            int recordsAffected = 0;

            this.Data.Execute.ExecuteQuery(false, () =>
            {
                recordsAffected = this.Data.InnerCommand.ExecuteNonQuery();
            });

            return recordsAffected;
        }

        public T ExecuteScalar<T>()
        {
            T result = default(T);

            this.Data.Execute.ExecuteQuery(false, () =>
            {
                object value = this.Data.InnerCommand.ExecuteScalar();

                if (value == null || value == DBNull.Value)
                {
                    result = default(T);
                }
                else
                {
                    if (value.GetType() == typeof(T))
                    {
                        result = (T)value;
                    }
                    else
                    {
                        result = (T)Convert.ChangeType(value, typeof(T));
                    }
                }
            });

            return result;
        }


        /// <summary>
        /// 返回最新的Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identityColumnName"></param>
        /// <returns></returns>
        public T ExecuteReturnLastId<T>()
        {
            var lastId = Data.Context.Data.Provider.ExecuteReturnLastId<T>(this);

            return lastId;
        }

        public bool QueryPaging(ref int currentPage, int pageSize, out int totalPages, out int totalRecords)
        {
            totalRecords = this.ExecuteScalar<int>();

            totalPages = 0;

            if (totalRecords <= 0) return false;

            totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);

            currentPage = currentPage < 1 ? 1 : currentPage;

            currentPage = currentPage > totalPages ? totalPages : currentPage;

            return true;
        }

        public TEntity Query<TEntity>(Func<System.Data.IDataReader, TEntity> customMapper)
        {
            TEntity o = default(TEntity);
            this.Data.Execute.ExecuteQuery(true, () =>
            {
                if (this.Data.Reader != null)
                {
                    o = customMapper(this.Data.Reader);

                }
            });
            return o;
        }

        /// <summary>
        /// 返回一个集合
        /// </summary>
        /// <typeparam name="List"></typeparam>
        /// <param name="customMapper"></param>
        /// <returns></returns>
        public TList Query<TEntity, TList>(Func<System.Data.IDataReader, TEntity> customMapper, Func<TEntity, bool> predicate = null) where TList : IList<TEntity>
        {
            TList lists = (TList)Activator.CreateInstance(typeof(TList));
            this.Data.Execute.ExecuteQuery(true, () =>
            {
                if (this.Data.Reader != null)
                {
                    while (this.Data.Reader.Read())
                    {
                        var entity = customMapper(this.Data.Reader);

                        if (predicate == null || predicate(entity))
                        {
                            lists.Add(entity);
                        }
                    }
                }
            });

            return lists;
        }

        /// <summary>
        /// 返回单个数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="customMapper"></param>
        /// <returns></returns>
        public TEntity QuerySingle<TEntity>(Func<System.Data.IDataReader, TEntity> customMapper)
        {
            TEntity entity = default(TEntity);

            this.Data.Execute.ExecuteQuery(true, () =>
            {
                if (this.Data.Reader != null && this.Data.Reader.Read())
                {
                    entity = customMapper(this.Data.Reader);//进行数据输出
                }
            });

            return entity;
        }

        /// <summary>
        /// 返回一个DataTable类型
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable QueryDataTable()
        {
            var dataTable = new DataTable();

            this.Data.Execute.ExecuteQuery(true, () =>
            {
                dataTable.Load(Data.Reader, LoadOption.PreserveChanges);
            });

            return dataTable;
        }

        public List<dynamic> Query()
        {
            List<dynamic> items = null;

            this.Data.Execute.ExecuteQuery(true, () =>
            {
                items = new List<dynamic>();

                var autoMapper = new DynamicTypAutoMapper(this.Data);

                if (this.Data.Reader != null)
                {
                    while (this.Data.Reader.Read())
                    {
                        var item = autoMapper.AutoMap();

                        items.Add(item);
                    }
                }
            });

            return items;
        }

        public dynamic QuerySingle()
        {
            dynamic item = null;

            this.Data.Execute.ExecuteQuery(true, () =>
            {
                var autoMapper = new DynamicTypAutoMapper(this.Data);

                if (this.Data.Reader != null && this.Data.Reader.Read())
                {
                    item = autoMapper.AutoMap();
                }
            });

            return item;
        }
               

        /// <summary>
        /// 关闭连接
        /// </summary>
        internal void CloseCommand()
        {
            //如果当前使用了事务，连接不关闭
            //事务还将在其他地方使用
            if (!Data.Context.Data.UseTransaction && !Data.Context.Data.UseSharedConnection)
            {
                Data.InnerCommand.Connection.Close();
            }

            //释放读取器
            if (Data.Reader != null)
            {
                if (!Data.Reader.IsClosed)
                    Data.Reader.Close();
                Data.Reader.Dispose();
            }

            //释放当前的Command资源
            if (Data.InnerCommand != null)
            {
                Data.InnerCommand.Dispose();
            }
        }

        private bool m_AlreadyDispose = false;

        ~DbCommand()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (this.m_AlreadyDispose)//资源已经释放
                return;
            if (isDisposing)
            {
                this.CloseCommand();
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

    }
}
