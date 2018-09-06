using Devx.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.Configuration
{
    public interface IConfig
    {
        bool IsValid { get; }
    }


    public class ConfigParse
    {
        private static readonly object locker = new object();

        public static T Parse<T>(string path, Func<string, T> mapper, Action<T> conver = null) where T : class,IConfig
        {
            var cacheKey = "cache::" + typeof(T).FullName;
            var instance = Caches.Get(cacheKey) as T;
            if (instance == null || !instance.IsValid)
            {
                lock (locker)
                {
                    if (System.IO.File.Exists(path))
                    {
                        instance = mapper(path);

                        if (instance != null && instance.IsValid)
                        {
                            if (conver != null)
                            {
                                conver(instance);
                            }

                            Caches.Insert(cacheKey, instance, new System.Web.Caching.CacheDependency(path), 3600);

                            return instance;
                        }
                    }
                }
            }
            return instance;
        }
    }
}
