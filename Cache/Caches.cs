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
        /// �����������
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
        /// ���ݹ����滻
        /// </summary>
        /// <param name="pattern">�������</param>
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
        /// ɾ��ָ���Ļ���
        /// </summary>
        /// <param name="key">�����</param>
        public static void Remove(string key)
        {
            CacheContainer.Remove(key);
        }

        /// <summary>
        /// ���Ĭ��Ϊ�����ӵĻ���
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="obj">��������</param>
        public static void Insert(string key, object obj)
        {
            Insert(key, obj, null, 60 * 3);
        }

        /// <summary>
        /// ���Ĭ��Ϊ�����ӵ���������
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="obj">��������</param>
        /// <param name="dep">������</param>
        public static void Insert(string key, object obj, CacheDependency dep)
        {
            Insert(key, obj, dep, 60 * 3);
        }

        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="obj">��������</param>
        /// <param name="seconds">��Чʱ�䣨�룩</param>
        public static void Insert(string key, object obj, int seconds)
        {
            Insert(key, obj, null, seconds);
        }
        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="obj">��������</param>
        /// <param name="seconds">��Чʱ�䣨�룩</param>
        /// <param name="priority">���ȼ�</param>
        public static void Insert(string key, object obj, int seconds, CacheItemPriority priority)
        {
            Insert(key, obj, null, seconds, priority);
        }
        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="obj">��������</param>
        /// <param name="dep">��������</param>
        /// <param name="seconds">��Чʱ�䣨�룩</param>
        public static void Insert(string key, object obj, CacheDependency dep, int seconds)
        {
            Insert(key, obj, dep, seconds, CacheItemPriority.Normal);
        }
        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="obj">��������</param>
        /// <param name="dep">��������</param>
        /// <param name="seconds">��Чʱ�䣨�룩</param>
        /// <param name="priority">���ȼ�</param>
        public static void Insert(string key, object obj, CacheDependency dep, int seconds, CacheItemPriority priority)
        {
            if (obj != null)
            {
                CacheContainer.Insert(key, obj, dep, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero, priority, null);
            }

        }

        /// <summary>
        /// ����������ڻ���
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="obj">��������</param>
        public static void Max(string key, object obj)
        {
            Max(key, obj, null);
        }
        /// <summary>
        /// ����������ڻ���
        /// </summary>
        /// <param name="key">�����</param>
        /// <param name="obj">��������</param>
        /// <param name="dep">��������</param>
        public static void Max(string key, object obj, CacheDependency dep)
        {
            if (obj != null)
            {
                CacheContainer.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.AboveNormal, null);
            }
        }
        /// <summary>
        /// ȡ�����е�����
        /// </summary>
        /// <param name="key">�����</param>
        /// <returns>�����е�����</returns>
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

        /// <summary>
        /// ȡ�����е�����
        /// </summary>
        /// <typeparam name="T">������������</typeparam>
        /// <param name="key">�����</param>
        /// <param name="func">�ص�����</param>
        /// <param name="seconds">����ʱ�䣨�룩</param>
        /// <returns>�����е�����</returns>
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
        /// ȡ�����е�����
        /// </summary>
        /// <typeparam name="T">������������</typeparam>
        /// <param name="key">�����</param>
        /// <param name="func">�ص�����</param>
        /// <param name="seconds">����ʱ�䣨�룩</param>
        /// <param name="dep">��������</param>
        /// <returns>�����е�����</returns>
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
        /// ȡ�����е�����
        /// </summary>
        /// <typeparam name="T">������������</typeparam>
        /// <param name="key">�����</param>
        /// <param name="func">�ص�����</param>
        /// <param name="seconds">����ʱ�䣨�룩</param>
        /// <param name="dep">���������ļ�</param>
        /// <returns>�����е�����</returns>
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
