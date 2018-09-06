using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.Cache
{
    /// <summary>
    /// 缓存整个表，根据最后更新时间来判断是否有更新
    /// </summary>
    public class DbKey
    {
        private string _key = "";
        private string _value = "";
        private IEnumerable<string> _removeKeys = null;
        private Func<DateTime> _loader = null;
        public int _timeout = 60;

        public DbKey(string baseKey, Func<DateTime> loader, params string[] removes)
        {
            this._key = baseKey;
            this._value = "";
            this._loader = loader;
            this._removeKeys = removes;
            if (_removeKeys == null || _removeKeys.Count() == 0)
            {
                throw new Exception("必须指定需要移除的Keys");
            }
            this._timeout = 60;//延迟最大为60秒
        }
        public DbKey(string baseKey, Func<DateTime> loader, int timeout, params string[] removes)
        {
            this._key = baseKey;
            this._value = "";
            this._loader = loader;
            this._removeKeys = removes;
            if (_removeKeys == null || _removeKeys.Count() == 0)
            {
                throw new Exception("必须指定需要移除的Keys");
            }
            this._timeout = timeout;//延迟最大为60秒
        }

        public string Key
        {
            get
            {
                var cache = Devx.Cache.Caches.Get(_key) as string;
                if (string.IsNullOrWhiteSpace(cache))
                {
                    var time = _loader();
                    if (time != DateTime.MinValue)
                    {
                        cache = time.ToString("yyyyMMddHHmmssfff");//从数据库读取的值
                    }
                    else
                    {
                        cache = DateTime.Now.ToString("yyyyMMddHHmmss.rd");
                    }
                    Devx.Cache.Caches.Insert(this._key, cache, this._timeout);//新数据添加到缓存中
                }
                if (_value != cache)//如果当前的值与缓存中不一致，则代表有更新，则清空缓存
                {
                    if (!string.IsNullOrWhiteSpace(_value))
                    {
                        foreach (var item in this._removeKeys)
                        {
                            Devx.Cache.Caches.Remove(item + _value);
                        }
                    }
                    this._value = cache;
                }
                return this._value;
            }
        }
    }
}
