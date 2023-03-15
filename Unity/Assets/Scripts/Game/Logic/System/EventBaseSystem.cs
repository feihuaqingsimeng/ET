using ET;
using System;
using System.Collections.Generic;
namespace Frame
{
    [ComponentOf(typeof(Scene))]
    public class EventBaseSystem : Entity, IAwake
    {
        public Dictionary<Type, List<object>> events = new Dictionary<Type, List<object>>();
    }
    [FriendOf(typeof(EventBaseSystem))]
    public static class EventBaseSystemSystem
    {
        public static void Register<T>(this EventBaseSystem self, Action<T> callback) where T : struct
        {
            var t = typeof(T);
            if (self.events.TryGetValue(t, out List<object> list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var v = list[i] as Action<T>;
                    if (v == callback)
                    {
                        Log.Error($"重复注册事件 {t}");
                        return;
                    }
                }
                list.Add(callback);
                return;
            }
            self.events.Add(t, new List<object>() { callback });
        }
        public static void UnRegister(this EventBaseSystem self, Type type, object callback)
        {
            if (self.events.TryGetValue(type, out List<object> list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == callback)
                    {
                        list.RemoveAt(i);
                        return;
                    }
                }
            }
        }
        public static void UnRegister<T>(this EventBaseSystem self, Action<T> callback) where T : struct
        {
            var t = typeof(T);
            if (self.events.TryGetValue(t, out List<object> list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var v = list[i] as Action<T>;
                    if (v == callback)
                    {
                        list.RemoveAt(i);
                        return;
                    }
                }
            }
        }
        public static void Publish<T>(this EventBaseSystem self, T a) where T : struct
        {
            var t = typeof(T);
            if (self.events.TryGetValue(t, out List<object> list))
            {
                var cache = ObjectPool.Instance.Fetch(typeof(List<object>)) as List<object>;

                for (int i = 0; i < list.Count; i++)
                {
                    cache.Add(list[i]);
                }
                for (int i = 0; i < cache.Count; i++)
                {
                    var v = cache[i] as Action<T>;
                    v.Invoke(a);
                }
                cache.Clear();
                ObjectPool.Instance.Recycle(cache);
            }
        }
    }
}

