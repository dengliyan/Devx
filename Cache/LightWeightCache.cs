using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.CompilerServices;

namespace Devx.Cache
{
    /// <summary>
    /// 缓存类
    /// </summary>
    /// <typeparam name="T">类别，必须为可实例化类对象，不支持结构体</typeparam>
    public class LightWeightCache<T> where T : class
    {
        #region 缓存实体
        /// <summary>
        /// 缓存实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class CacheEntityObject
        {
            /// <summary>
            /// 缓存名
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// 对象
            /// </summary>
            public T Value { get; set; }

            /// <summary>
            /// 过期时间
            /// </summary>
            public TimeSpan Timeout { get; set; }

            /// <summary>
            /// 是否为每日更新
            /// </summary>
            public bool IsDaily { get; set; }

            /// <summary>
            /// 最后更新时间
            /// </summary>
            public DateTime Updatetime { get; set; }

            /// <summary>
            /// 执行函数
            /// </summary>
            public Func<T> Execute { get; set; }

        }
        #endregion

        private static readonly object LockXXXUltimate = new object();
        private static Dictionary<string, CacheEntityObject> CacheTable = new Dictionary<string, CacheEntityObject>();//缓存数据
        private static Queue<string> CacheQueue = new Queue<string>();//数据更新队列
        private static HashSet<string> CacheMutexMapping = new HashSet<string>();//数据更新队列排他字典

        private static string BaseDirectory = null;
        private static Thread WScheduling = null;//调度线程，负责任务的分发
        private static Thread[] WExecuting = null;//更新线程，负责从队列中读数据进行更新
        private static int WCount = 1;
        
        static LightWeightCache()
        {
            if (typeof(T).FullName == typeof(System.String).FullName)
            {
                WCount = 3;
            }

            #region 存储目录处理
            BaseDirectory = System.Web.HttpContext.Current == null ? AppDomain.CurrentDomain.BaseDirectory : System.Web.Hosting.HostingEnvironment.MapPath("~");

            BaseDirectory = Path.Combine(BaseDirectory, "cache");

            try
            {
                if (!System.IO.Directory.Exists(BaseDirectory))
                {
                    System.IO.Directory.CreateDirectory(BaseDirectory);
                }
            }
            catch
            {
            }
            #endregion

            #region 程序启动时线程关闭
            if (WExecuting != null && WExecuting.Length > 0)
            {
                for (int i = 0; i < WExecuting.Length; i++)
                {
                    if (WExecuting[i] != null)
                    {
                        if (WExecuting[i].IsAlive)
                        {
                            try
                            {
                                WExecuting[i].Abort();
                            }
                            catch
                            {
                            }
                        }
                        WExecuting[i] = null;
                    }
                }
            }
            WExecuting = null;

            if (WScheduling != null)
            {
                if (WScheduling.IsAlive)
                {
                    try
                    {
                        WScheduling.Abort();
                    }
                    catch
                    {
                    }
                }
                WScheduling = null;
            }
            #endregion
                        
            #region 程序启动
            WExecuting = new Thread[WCount];
            for (int i = 0; i < WCount; i++)
            {
                WExecuting[i] = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        while (true)
                        {
                            try
                            {
                                Executing();
                            }
                            catch (System.Threading.ThreadAbortException)
                            {
                                //线程被程序强制停止，不记录日志
                            }
                            catch (Exception ex)
                            {
                                Log("[Executing<" + typeof(T).FullName + ">] " + ex.Message.ToString());//记录因执行时产生的异常
                            }
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                    catch (System.Threading.ThreadAbortException)
                    {

                    }
                    catch (Exception ex)
                    {
                        Log("[Executing<" + typeof(T).FullName + ">] " + ex.Message.ToString());//主线程的异常
                    }
                }));
                WExecuting[i].Start();
            }
            

            WScheduling = new Thread(new ThreadStart(() =>
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            Scheduling();
                        }
                        catch (System.Threading.ThreadAbortException)
                        {
                            //线程被程序强制停止，不记录日志
                        }
                        catch (Exception ex)
                        {
                            Log("[Scheduling<" + typeof(T).FullName + ">] " + ex.Message.ToString());//记录因调度时产生的异常
                        }
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch (System.Threading.ThreadAbortException)
                {

                }
                catch (Exception ex)
                {
                    Log("[Scheduling<" + typeof(T).FullName + ">] " + ex.Message.ToString());
                }
            }));
            WScheduling.Start();
            #endregion

            //Log("LightWeightCache<" + typeof(T).FullName + ">.Thread.Starting");
        }

        /// <summary>
        /// 调度
        /// </summary>
        private static void Scheduling()
        {
            string[] keys = null;
            //读取全部数据
            //using (__lockXXXUltimate.ReadOnlyLock())
            lock (LockXXXUltimate)
            {
                keys = new string[CacheTable.Keys.Count];

                CacheTable.Keys.CopyTo(keys, 0);
            }

            foreach (var key in keys)
            {
                CacheEntityObject e = null;
                
                lock (LockXXXUltimate)//调度线程先进入锁状态，此时还在还在更新中，则退出
                {
                    if (!CacheMutexMapping.Contains(key))
                    {
                        //开始遍历每一条记录
                        DateTime tick = DateTime.Now;

                        //根据Key读取数据信息
                        e = CacheTable.TryGetValue(key, out e) && e != null && e.Value != null && e.Value != default(T) ? e : null;

                        //判断当前是否可以运行
                        if (e != null && e.IsDaily && e.Updatetime < DateTime.Now.Date.Add(e.Timeout) && DateTime.Now.Date.Add(e.Timeout) < tick)
                        {
                            //当前文件最后更新时间小于更新点，且当前时间大于更新点
                            // 9:00运行，更新为上一次8点，且当前为9：01，则可以运行
                            CacheMutexMapping.Add(key);

                            CacheQueue.Enqueue(key);
                        }
                        else if (e != null && !e.IsDaily && tick.Subtract(e.Updatetime).TotalSeconds > e.Timeout.TotalSeconds)
                        {                            
                            //根据上一次的运行时间作为更新对比时间
                            CacheMutexMapping.Add(key);

                            CacheQueue.Enqueue(key);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        private static void Executing()
        {
            var key = string.Empty;

            CacheEntityObject e = null;

            var result = default(T);
            //从队列中读取一条记录
            lock (LockXXXUltimate)
            {
                if (CacheQueue.Count > 0)
                {
                    key = CacheQueue.Dequeue();

                    if (!CacheMutexMapping.Contains(key))//队列及缓存锁中都存在才能执行
                    {
                        key = string.Empty;
                    }
                }
            }

            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            //读取数据信息
            //using (__lockXXXUltimate.ReadOnlyLock())//并行只读锁
            lock (LockXXXUltimate)
            {
                e = CacheTable.TryGetValue(key, out e) && e != null && e.Value != null && e.Value != default(T) ? e : null;
            }

            if (e == null)
            {
                lock (LockXXXUltimate)
                {
                    CacheMutexMapping.Remove(key);//在数据执行完成清空数据，等待下一次完成
                }

                Log("[Executing] [" + key + "] is null");

                return;
            }

            try
            {
                //执行
                if ((result = e.Execute.Invoke()) != null && result != default(T))
                {
                    lock (LockXXXUltimate)
                    {
                        e.Value = result;//更新结果
                        e.Updatetime = DateTime.Now;//更新最后更新时间
                        CacheTable[e.Key] = e;
                        CacheMutexMapping.Remove(key);//在数据执行完成清空数据，等待下一次完成
                        W(e.Key, result);//同时更新文件
                    }

                    //Log("[Executing] [" + e.Key + "] [" + e.Updatetime.ToString("yyyy-MM-dd HH:mm:ss") + "] [" + (e.IsDaily ? e.Timeout.ToString() : ((int)e.Timeout.TotalSeconds).ToString()) + "] OK...");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                lock (LockXXXUltimate)
                {
                    CacheMutexMapping.Remove(key);//在数据执行完成清空数据，等待下一次完成
                }
            }
            catch (Exception ex)
            {
                lock (LockXXXUltimate)
                {
                    CacheMutexMapping.Remove(key);//在数据执行完成清空数据，等待下一次完成
                }

                Log("[Executing] " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// 解析数据对象
        /// </summary>
        /// <param name="o"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static bool TryParser(CacheEntityObject o, out T result)
        {
            CacheEntityObject e = null;

            result = default(T);

            #region 从内存中加载数据
            //using (__lockXXXUltimate.ReadOnlyLock())//并行只读锁
            lock (LockXXXUltimate)
            {
                e = CacheTable.TryGetValue(o.Key, out e) && e != null && e.Value != null && e.Value != default(T) ? e : null;
            }
            #endregion

            #region  内存中不存在数据，则从文件中读取
            if (e == null || e.Value == null || e.Value == default(T))//如果内存中不存在
            {
                lock (LockXXXUltimate)
                {
                    e = CacheTable.TryGetValue(o.Key, out e) && e != null && e.Value != null && e.Value != default(T) && e.Updatetime >= DateTime.MinValue ? e : null;

                    if (e == null || e.Value == null || e.Value == default(T))//如果内存中不存在
                    {
                        DateTime w = DateTime.MinValue;
                        result = R(o.Key, out w);//从文件加载 
                        e = new CacheEntityObject();//创建一个新对象，添加到缓存中
                        e.Key = o.Key;
                        e.IsDaily = o.IsDaily;
                        e.Timeout = o.Timeout;
                        e.Updatetime = o.Updatetime;
                        e.Execute = o.Execute;
                        e.Value = result;
                        e.Updatetime = w;//更新当前从缓存中读取的数据                        
                        CacheTable[e.Key] = e;//更新到缓存中
                    }
                }
            }
            #endregion

            #region 在内在或文件中读取到数据
            if (e != null && e.Value != null && e.Value != default(T) && e.Updatetime != DateTime.MinValue)//如果内存中不存在
            {
                result = e.Value;

                return true;
            }
            #endregion

            #region 如果内存和文件中都不存在当前的数据，则直接调用回调，填充到内存及文件中
            try
            {
                if ((result = o.Execute.Invoke()) != null && result != default(T))
                {
                    lock(LockXXXUltimate)
                    {
                        W(o.Key, result);//同时更新文件
                        o.Value = result;
                        o.Updatetime = DateTime.Now;
                        CacheTable[o.Key] = o;
                        CacheMutexMapping.Remove(o.Key);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lock (LockXXXUltimate)
                {
                    CacheMutexMapping.Remove(o.Key);
                }

                Log("[Executing] " + ex.Message.ToString());
            }
            #endregion

            return false;
        }

        /// <summary>
        /// 从文件中加载数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="wtime"></param>
        /// <returns></returns>
        private static T R(string key, out DateTime wtime)
        {
            var filename = Path.Combine(BaseDirectory, key + ".cache");
            byte[] bytes = null;
            var o = default(T);
            wtime = DateTime.MinValue;
            if (System.IO.File.Exists(filename))
            {
                try
                {
                    using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, bytes.Length);
                    }
                    if (bytes != null && bytes.Length > 0)
                    {
                        o = DeSerialize(bytes);
                    }
                    wtime = o != null && o != default(T) ? System.IO.File.GetLastWriteTime(filename) : DateTime.MinValue;
                }
                catch (Exception ex)
                {
                    Log("[R] Exception:" + ex.Message.ToString());
                }
            }
            return o;
        }

        /// <summary>
        /// 更新数据到文件中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        private static bool W(string key, T o)
        {
            var filename = Path.Combine(BaseDirectory, key + ".cache");
            byte[] bytes = null;
            try
            {
                if (o != null && o != default(T) && (bytes = Serialize(o)) != null && bytes.Length > 0)
                {
                    using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log("[W] Exception:" + ex.Message.ToString());
            }
            return false;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] Serialize(T value)
        {
            byte[] bytes;
            if (typeof(T) == typeof(byte[]))
            {
                bytes = value as byte[];
            }
            else if (typeof(T) == typeof(string))
            {
                bytes = Encoding.UTF8.GetBytes(value as string);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(ms, value);
                    bytes = ms.ToArray();
                }
            }
            return bytes;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static T DeSerialize(byte[] bytes)
        {
            var t = typeof(T);
            if (t == typeof(string))
            {
                return Encoding.UTF8.GetString(bytes) as T;
            }
            else if (t == typeof(byte[]))
            {
                return bytes as T;
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    var formatter = new BinaryFormatter();
                    var o = formatter.Deserialize(ms);
                    return o as T;
                }
            }
        }

        /// <summary>
        /// 简易记录日志
        /// </summary>
        /// <param name="o"></param>
        /// <param name="path"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void Log(string s)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(BaseDirectory, "cachings.log"), true))
                {
                    sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + s);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 刷新数据，默认300秒更新一次
        /// </summary>
        /// <param name="key">缓存键，格式需要符合文件命名格式</param>
        /// <param name="fun">委托执行方法</param>
        /// <returns></returns>
        public static T Flush(string key, Func<T> fun)
        {
            return Flush(key, fun, 300);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="key">缓存键，格式需要符合文件命名格式</param>
        /// <param name="fun">委托执行方法</param>
        /// <param name="timeout">过期时间，单位：秒</param>
        /// <returns></returns>
        public static T Flush(string key, Func<T> fun, int timeout)
        {
            //return fun();

            var e = default(T);
            var o = new CacheEntityObject();
            o.Key = key;
            o.Timeout = TimeSpan.FromSeconds(timeout);
            o.IsDaily = false;
            o.Value = e;
            o.Updatetime = DateTime.MinValue;
            o.Execute = fun;
            //o.Initialed = false;
            return TryParser(o, out e) ? e : default(T);
        }

        /// <summary>
        /// 每日更新一次刷新
        /// </summary>
        /// <param name="key">缓存键，格式需要符合文件命名格式</param>
        /// <param name="fun">委托执行方法</param>
        /// <param name="ts">更新时间，格式：00:00:00</param>
        /// <returns></returns>
        public static T FlushDaily(string key, Func<T> fun, TimeSpan ts)
        {
            //return fun();
            var e = default(T);
            var o = new CacheEntityObject();
            o.Key = key;
            o.Timeout = ts;
            o.IsDaily = true;
            o.Value = e;
            o.Updatetime = DateTime.MinValue;
            o.Execute = fun;
            //o.Initialed = false;
            return TryParser(o, out e) ? e : default(T);
        }
       

        /// <summary>
        /// 重新刷新，如果内存中有数据，则直接刷新，否则删除文件及缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static void ReFlush(string key)
        {
            try
            {
                var result = default(T);

                CacheEntityObject e = null;

                lock (LockXXXUltimate)
                {
                    CacheMutexMapping.Remove(key);//清空掉缓存锁中的数据，防止自动更新
                
                    e = CacheTable.TryGetValue(key, out e) && e != null && e.Value != null && e.Value != default(T) ? e : null;
                }
                if (e != null)//如果内存中有数据，则执行相应的操作
                {
                    try
                    {
                        if ((result = e.Execute.Invoke()) != null && result != default(T))
                        {
                            lock (LockXXXUltimate)
                            {
                                e.Value = result;
                                e.Updatetime = DateTime.Now;
                                CacheTable[e.Key] = e;
                                W(e.Key, result);//同时更新文件
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("[ReFlush] " + ex.Message.ToString());
                    }
                }
                else//如果内存中不存在，则删除文件，清空缓存
                {
                    var filename = Path.Combine(BaseDirectory, key + ".cache");

                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);//删除缓存文件
                    }

                    lock (LockXXXUltimate)
                    {
                        if (CacheTable.ContainsKey(key))
                        {
                            CacheTable.Remove(key);//清空缓存中的数据
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log("[ReFlush] Exception:" + ex.Message.ToString());
            }
        }

    }
}
