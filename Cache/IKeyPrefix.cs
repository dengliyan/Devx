using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.Cache
{
    public interface IKeyPrefix
    {
        int ExpireSeconds { get;  }

        string Prefix { get;  }
    }

    public abstract class DefaultKeyPrefix : IKeyPrefix
    {
        public DefaultKeyPrefix(string prefix, int seconds)
        {
            this._expireSeconds = seconds;
            this._prefix = prefix;
        }
        public DefaultKeyPrefix(string prefix)
        {
            this._expireSeconds = 0;
            this._prefix = prefix;
        }

        private int _expireSeconds;
        public int ExpireSeconds
        {
            get
            {
                return this._expireSeconds;
            }
        }

        private string _prefix;
        public string Prefix
        {
            get
            {
                String className = this.GetType().Name;
                return className + ":" + this._prefix;
            }
        }
    }
}
