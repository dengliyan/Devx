using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Devx.DbProvider
{
    internal class ActionsHandler
    {
        private readonly BuilderData _data;

        private bool _autoMappedAlreadyCalled = false;

        public ActionsHandler(BuilderData data)
        {
            this._data = data;
        }

        internal void Column(string columnName, object value, DbType parameterType, int size, bool isFunction = false)
        {
            var parameterName = columnName;

            //如果当前为函数，则不添加成参数
            if (!isFunction)
            {
                if (value == null)
                {
                    _data.Command.Parameter(parameterName, DBNull.Value, parameterType, ParameterDirection.Input, size);
                }
                else
                {
                    if (parameterType == DbType.Object)
                        parameterType = _data.Command.Data.Context.Data.Provider.GetDbTypeForClrType(value.GetType());

                    _data.Command.Parameter(parameterName, value, parameterType, ParameterDirection.Input, size);
                }
            }
            _data.Columns.Add(new TableColumn(columnName, value, parameterName, isFunction));
        }

        private void Column(string columnName, object value, DbType parameterType, Type clrType = null)
        {
            var parameterName = columnName;

            if (value == null)
            {
                _data.Command.Parameter(parameterName, DBNull.Value, parameterType, ParameterDirection.Input, 0);
            }
            else
            {
                if (parameterType == DbType.Object)
                {
                    if (clrType == null || clrType == typeof(System.Object))
                    {
                        parameterType = _data.Command.Data.Context.Data.Provider.GetDbTypeForClrType(value.GetType());
                    }
                    else
                    {
                        parameterType = _data.Command.Data.Context.Data.Provider.GetDbTypeForClrType(clrType);
                    }
                }

                _data.Command.Parameter(parameterName, value, parameterType, ParameterDirection.Input, 0);
            }

            _data.Columns.Add(new TableColumn(columnName, value, parameterName, false));
        }

        internal void Where(string columnName, object value, DbType parameterType, int size)
        {
            //防止重复
            var parameterName = "_WHERE_COLUMN_" + columnName;
            if (parameterType == DbType.Object)
                parameterType = _data.Command.Data.Context.Data.Provider.GetDbTypeForClrType(value.GetType());
            _data.Command.Parameter(parameterName, value, parameterType, ParameterDirection.Input, size);
            _data.Where.Add(new TableColumn(columnName, value, parameterName));
            //
            //where字段自动忽略更新
            //
        }

        internal void Where<T>(Expression<Func<T, object>> expression, DbType parameterType, int size)
        {
            var parser = new PropertyExpressionParser<T>(this._data.Item, expression);
            this.Where(parser.Name, parser.Value, parameterType, size);
        }

        #region 自动映射
        internal void AutoMapDynamicTypeColumnsAction(params string[] ignorePropertyExpressions)
        {
            //此代码只能被执行一次
            if (this._autoMappedAlreadyCalled)
                throw new DBClientException("AutoMap cannot be called more than once.");
            this._autoMappedAlreadyCalled = true;

            var properties = (IDictionary<string, object>)_data.Item;
            var ignorePropertyNames = new HashSet<string>();

            //添加要过滤的名单
            if (ignorePropertyExpressions != null && ignorePropertyExpressions.Length > 0)
            {
                foreach (var ignorePropertyExpression in ignorePropertyExpressions)
                {
                    ignorePropertyNames.Add(ignorePropertyExpression.ToLower());
                }
            }
            //添加字段
            foreach (var property in properties)
            {
                if (!ignorePropertyNames.Contains(property.Key.ToLower()))//如果此字段不存在
                {
                    this.Column(property.Key, property.Value, DbType.Object, 0);
                }
            }
        }

        internal void AutoMapAnonymousTypeColumns(params string[] ignorePropertyExpressions)
        {
            //此代码只能被执行一次
            if (this._autoMappedAlreadyCalled)
                throw new DBClientException("AutoMap cannot be called more than once.");
            this._autoMappedAlreadyCalled = true;

            var properties = ReflectionHelper.GetProperties(_data.Item.GetType());
            var ignorePropertyNames = new HashSet<string>();

            //添加要过滤的名单
            if (ignorePropertyExpressions != null && ignorePropertyExpressions.Length > 0)
            {
                foreach (var ignorePropertyExpression in ignorePropertyExpressions)
                {
                    ignorePropertyNames.Add(ignorePropertyExpression.ToLower());
                }
            }
            //添加字段
            foreach (var property in properties)
            {
                if (!ignorePropertyNames.Contains(property.Key.ToLower()))//如果此字段不存在
                {
                    if (ReflectionHelper.IsBasicClrType(property.Value.PropertyType))
                    {
                        this.Column(property.Key, ReflectionHelper.GetPropertyValue(_data.Item, property.Value), DbType.Object, property.Value.PropertyType);
                    }
                }
            }
        }

        internal void AutoMapColumns<T>(params Expression<Func<T, object>>[] ignorePropertyExpressions)
        {
            //此代码只能被执行一次
            if (this._autoMappedAlreadyCalled)
                throw new DBClientException("AutoMap cannot be called more than once.");
            this._autoMappedAlreadyCalled = true;

            var properties = ReflectionHelper.GetProperties(_data.Item.GetType());
            var ignorePropertyNames = new HashSet<string>();

            //添加要过滤的名单
            if (ignorePropertyExpressions != null && ignorePropertyExpressions.Length > 0)
            {
                foreach (var ignorePropertyExpression in ignorePropertyExpressions)
                {
                    var ignorePropertyName = new PropertyExpressionParser<T>(_data.Item, ignorePropertyExpression).Name;
                    ignorePropertyNames.Add(ignorePropertyName);
                }
            }
            //添加字段
            foreach (var property in properties)
            {
                if (!ignorePropertyNames.Contains(property.Key))//如果此字段不存在
                {
                    if (ReflectionHelper.IsBasicClrType(property.Value.PropertyType))
                    {
                        this.Column(property.Key, ReflectionHelper.GetPropertyValue(_data.Item, property.Value), DbType.Object, property.Value.PropertyType);
                    }
                }
            }
        }
        #endregion
    }
}
