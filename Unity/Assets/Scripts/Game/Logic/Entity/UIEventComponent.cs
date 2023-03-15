using ET;
using System;
using System.Collections.Generic;
namespace Frame
{
    [ComponentOf]
    public class EventComponent : Entity, IAwake, IDestroy
    {
        public Dictionary<Type, object> events = new();
    }

    [FriendOf(typeof(EventComponent))]
    public static class UIEventComponentSystem
    {
        public class DestroySystem : DestroySystem<EventComponent>
        {
            protected override void Destroy(EventComponent self)
            {
                foreach (var v in self.events)
                {
                    UIEventSystem.Ins.UnRegister(v.Key, v.Value);
                }
                self.events.Clear();
            }
        }
        public static void Register<T>(this EventComponent self, Action<T> callback) where T : struct
        {
            self.events.Add(typeof(T), callback);
            UIEventSystem.Ins.Register(callback);
        }
        public static void UnRegister<T>(this EventComponent self, Action<T> callback) where T : struct
        {
            self.events.Remove(typeof(T));
            UIEventSystem.Ins.UnRegister(callback);
        }
    }
}