using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.DbProvider
{
    /// <summary>
    /// 自定义异常
    /// </summary>
    public class DBClientException : Exception
    {
        public DBClientException(string message)
            : base(message)
        {
        }
        public string CmdText { get;set; }
        public DBClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
