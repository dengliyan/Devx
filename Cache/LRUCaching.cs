using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devx.Cache
{
    /// <summary>
    /// 最近最少使用缓存
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class LRUCaching<TKey, TValue> where TValue : class
    {
        #region 内部缓存对象
        /*
        http://www.cnblogs.com/yiway/archive/2011/07/15/High_Performance_Cache.html
        */
        internal class CachingItem<TKey, TValue>
        {
            public TValue Values { get; set; }

            public DateTime Expired { get; set; }

            public LinkedListNode<TKey> Node { get; set; }
        }
        #endregion

        #region LRU队列
        internal class LRU<T>
        {
            private readonly LinkedList<T> __linklist = new LinkedList<T>();
            private readonly int __maxitem;
            private readonly int __removeRate;

            private readonly Action<LinkedListNode<T>> onRemoveNode;

            public LRU(int maxitem, int removeRate, Action<LinkedListNode<T>> onRemoveNode)
            {
                this.__maxitem = maxitem;
                this.__removeRate = removeRate;
                this.onRemoveNode = onRemoveNode;
            }

            public void Remove(LinkedListNode<T> node)
            {
                __linklist.Remove(node);
                onRemoveNode.Invoke(node);
            }

            public void MarkUse(LinkedListNode<T> node)
            {
                if (node != null)
                {
                    if (node.List != null)
                    {
                        __linklist.Remove(node);
                    }
                    __linklist.AddFirst(node);
                }
            }

            public LinkedListNode<T> AddNew(T value)
            {
                var n = __linklist.AddFirst(value);
                AutoKnockOut();
                return n;
            }

            public void AutoKnockOut()
            {
                if (__linklist.Count > __maxitem)
                {
                    RemoveFromEnd((int)(__maxitem * __removeRate / 100));
                }
            }

            protected void RemoveFromEnd(int n)
            {
                if (__linklist.Count >= n)
                {
                    for (; n > 0; n--)
                    {
                        Remove(__linklist.Last);
                    }
                }
            }

            public int Count
            {
                get { return __linklist.Count; }
            }
        }
        #endregion

        private System.Collections.Hashtable _mapping = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
        private LRU<TKey> _linklist = null;
        private int _maxitem = 1000;
        private int _removeRate = 1;//比例
        public LRUCaching(int maxitem, int rate)
        {
            this._maxitem = maxitem;
            this._removeRate = rate;
            this._linklist = new LRU<TKey>(this._maxitem, this._removeRate, new Action<LinkedListNode<TKey>>(s =>
            {
                _mapping.Remove(s.Value);
            }));
        }

        public void Remove(TKey key)
        {
            lock (_mapping.SyncRoot)
            {
                if (_mapping.ContainsKey(key))
                {
                    var item = _mapping[key] as CachingItem<TKey, TValue>;
                    _linklist.Remove(item.Node);
                }
            }
        }

        public void Set(TKey key, TValue value, DateTime expired)
        {
            lock (_mapping.SyncRoot)
            {
                if (_mapping.ContainsKey(key))
                {
                    var item = _mapping[key] as CachingItem<TKey, TValue>;
                    item.Values = value;
                    item.Expired = expired;
                    _linklist.MarkUse(item.Node);
                }
                else
                {
                    var item = new CachingItem<TKey, TValue>() { Values = value, Expired = expired, Node = _linklist.AddNew(key) };
                    _mapping.Add(key, item);
                }

                //Console.WriteLine("LRUCaching:" + _mapping.Keys.Count);
            }
        }

        public TValue Get(TKey key)
        {
            lock (_mapping.SyncRoot)
            {
                if (_mapping.ContainsKey(key))
                {
                    var item = _mapping[key] as CachingItem<TKey, TValue>;
                    if (item.Expired >= DateTime.Now)
                    {
                        _linklist.MarkUse(item.Node);//标志当前数据为已访问
                        return item.Values;
                    }
                    else
                    {
                        _linklist.Remove(item.Node);//执行删除逻辑
                    }
                }
                return default(TValue);
            }
        }        

        public TValue LazyFetch(TKey mcKey, DateTime expired, Func<Object> func)
        {
            var o = this.Get(mcKey);

            if (o == null)
            {
                try
                {
                    o = func.Invoke() as TValue;
                    if (o != null)
                    {
                        this.Set(mcKey, o, expired);
                    }
                }
                catch
                {
                }
            }
            return o;
        }

        /// <summary>
        /// 执行搜索方法
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public TValue[] Search(string s, Action<TValue> a)
        {
            lock (_mapping.SyncRoot)
            {
                return null;
            }
        }
    }
}
