
using ET;
using System;
using System.Collections.Generic;

namespace Frame
{
    [ComponentOf(typeof(Scene))]
	public class UIEventSystem: Entity,IAwake
	{
		[StaticField]
		public static UIEventSystem Ins = null;
		public Dictionary<Type, List<object>> events = new();
	}
    [FriendOf(typeof(UIEventSystem))]
    public static class UIEventSystemSystem
    {
        [ObjectSystem]
        public class AwakeSystem : AwakeSystem<UIEventSystem>
        {
            protected override void Awake(UIEventSystem self)
            {
                UIEventSystem.Ins = self;
            }
        }
        public static void Register<T>(this UIEventSystem self, Action<T> callback) where T : struct
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
        public static void UnRegister(this UIEventSystem self, Type type, object callback)
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
        public static void UnRegister<T>(this UIEventSystem self, Action<T> callback) where T : struct
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
        public static void Publish<T>(this UIEventSystem self, T a) where T : struct
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
