using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace Devx.Cache
{    
    public class Caches
    {
        private Caches() { }

        private static readonly System.Web.Caching.Cache CacheContainer;

        static Caches()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                CacheContainer = context.Cache;
            }
            else
            {
                CacheContainer = HttpRuntime.Cache;
            }
        }


        /// <summary>
        /// 清空所有数据
        /// </summary>
        public static void Clear()
        {
            IDictionaryEnumerator cacheEnum = CacheContainer.GetEnumerator();
            ArrayList arrayList = new ArrayList();
            while (cacheEnum.MoveNext())
            {
                arrayList.Add(cacheEnum.Key);
            }

            foreach (string key in arrayList)
            {
                CacheContainer.Remove(key);
            }

        }

        /// <summary>
        /// 根据规则替换
        /// </summary>
        /// <param name="pattern">正则规则</param>
        public static void RemoveByPattern(string pattern)
        {
            IDictionaryEnumerator cacheEnum = CacheContainer.GetEnumerator();
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            while (cacheEnum.MoveNext())
            {
                if (regex.IsMatch(cacheEnum.Key.ToString()))
                    CacheContainer.Remove(cacheEnum.Key.ToString());
            }
        }

        /// <summary>
        /// 删除指定的缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public static void Remove(string key)
        {
            CacheContainer.Remove(key);
        }

        public static void Remove(IKeyPrefix prefix, string key)
        {
            CacheContainer.Remove(prefix.Prefix + ":" + key);
        }

        /// <summary>
        /// 添加默认为三分钟的缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="obj">缓存数据</param>
        public static void Insert(string key, object obj)
        {
            Insert(key, obj, null, 60 * 3);
        }

        /// <summary>
        /// 添加默认为三分钟的依赖缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="obj">缓存数据</param>
        /// <param name="dep">依赖项</param>
        public static void Insert(string key, object obj, CacheDependency dep)
        {
            Insert(key, obj, dep, 60 * 3);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="obj">缓存数据</param>
        /// <param name="seconds">有效时间（秒）</param>
        public static void Insert(string key, object obj, int seconds)
        {
            Insert(key, obj, null, seconds);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="obj">缓存数据</param>
        /// <param name="seconds">有效时间（秒）</param>
        /// <param name="priority">优先级</param>
        public static void Insert(string key, object obj, int seconds, CacheItemPriority priority)
        {
            Insert(key, obj, null, seconds, priority);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="obj">缓存数据</param>
        /// <param name="dep">缓存依赖</param>
        /// <param name="seconds">有效时间（秒）</param>
        public static void Insert(string key, object obj, CacheDependency dep, int seconds)
        {
            Insert(key, obj, dep, seconds, CacheItemPriority.Normal);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="obj">缓存数据</param>
        /// <param name="dep">缓存依赖</param>
        /// <param name="seconds">有效时间（秒）</param>
        /// <param name="priority">优先级</param>
        public static void Insert(string key, object obj, CacheDependency dep, int seconds, CacheItemPriority priority)
        {
            if (obj != null)
            {
                CacheContainer.Insert(key, obj, dep, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero, priority, null);
            }

        }

        public static void Insert(IKeyPrefix prefix, string key, object obj, CacheDependency dep = null, CacheItemPriority priority = CacheItemPriority.Normal)
        {
            if (obj != null)
            {
                string realKey = prefix.Prefix + ":" + key;
                CacheContainer.Insert(realKey, obj, dep, prefix.ExpireSeconds <= 0 ? DateTime.MaxValue : DateTime.Now.AddSeconds(prefix.ExpireSeconds), TimeSpan.Zero, priority, null);
            }
        }

        /// <summary>
        /// 添加永不过期缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="obj">缓存数据</param>
        public static void Max(string key, object obj)
        {
            Max(key, obj, null);
        }
        /// <summary>
        /// 添加永不过期缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="obj">缓存数据</param>
        /// <param name="dep">依赖过期</param>
        public static void Max(string key, object obj, CacheDependency dep)
        {
            if (obj != null)
            {
                CacheContainer.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.AboveNormal, null);
            }
        }
        /// <summary>
        /// 取缓存中的数据
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存中的数据</returns>
        public static object Get(string key)
        {
            Object cacheObject = CacheContainer[key];
            if (cacheObject != null)
            {
                string oType = cacheObject.GetType().Name.ToString();
                if (oType.IndexOf("[]") > 0)
                {
                    if (((Array)cacheObject).Length == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return cacheObject;
                    }
                }
            }
            return cacheObject;
        }

        public static object Get(IKeyPrefix prefix, string key)
        {
            string realKey = prefix.Prefix + ":" + key;
            Object cacheObject = CacheContainer[realKey];
            if (cacheObject != null)
            {
                string oType = cacheObject.GetType().Name.ToString();
                if (oType.IndexOf("[]") > 0)
                {
                    if (((Array)cacheObject).Length == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return cacheObject;
                    }
                }
            }
            return cacheObject;
        }

        /// <summary>
        /// 取缓存中的数据
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="func">回调数据</param>
        /// <param name="seconds">过期时间（秒）</param>
        /// <returns>缓存中的数据</returns>
        public static T Get<T>(string key, Func<T> func, int seconds) where T : class
        {
            var o = Caches.Get(key) as T;
            if (o == null || o == default(T))
            {
                try
                {
                    o = func.Invoke();
                }
                catch
                {
                }
                if (o != null)
                {
                    Caches.Insert(key, o, seconds);
                }
            }
            return o;
        }

        /// <summary>
        /// 取缓存中的数据
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="func">回调数据</param>
        /// <param name="seconds">过期时间（秒）</param>
        /// <param name="dep">过期依赖</param>
        /// <returns>缓存中的数据</returns>
        public static T Get<T>(string key, Func<T> func, int seconds, CacheDependency dep) where T : class
        {
            var o = Caches.Get(key) as T;

            if (o == null || o == default(T))
            {
                try
                {
                    o = func.Invoke();
                }
                catch
                {
                }
                if (o != null)
                {
                    Caches.Insert(key, o, dep, seconds);
                }
            }
            return o;
        }
        
        /// <summary>
        /// 取缓存中的数据
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="func">回调数据</param>
        /// <param name="seconds">过期时间（秒）</param>
        /// <param name="dep">过期依赖文件</param>
        /// <returns>缓存中的数据</returns>
        public static T Get<T>(string key, Func<T> func, int seconds, string dependency) where T : class
        {
            var o = Caches.Get(key) as T;

            if (o == null || o == default(T))
            {
                try
                {
                    o = func.Invoke();
                }
                catch
                {
                }

                if (o != null)
                {
                    Caches.Insert(key, o, new CacheDependency(dependency), seconds);
                }
            }
            func = null;
            return o;
        }
    }
}
