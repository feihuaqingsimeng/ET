using ET;
using Frame;
using System;
using System.Collections.Generic;
using TBS;

public class BattleEventAutoDestroyComponent:Entity,IAwake,IDestroy
{
	public Dictionary<Type, object> events = new Dictionary<Type, object>();
}
[ObjectSystem]
public class BattleEventAutoDestroyComponentDestroySystem : DestroySystem<BattleEventAutoDestroyComponent>
{
	protected override void Destroy(BattleEventAutoDestroyComponent self)
	{
        var bat = ClientBattle.Get();
        if (bat == null) return;
        var com = bat.GetComponent<BattleEventComponent>();
        if (com == null) return;
		foreach (var v in self.events)
		{
            com.UnRegister(v.Key, v.Value);
		}
		self.events.Clear();
	}
}

public static class BattleEventAutoDestroyComponentSystem
{
	public static void Register<T>(this BattleEventAutoDestroyComponent self,Action<T> callback) where T : struct
	{
        var bat = ClientBattle.Get();
        if (bat == null) return;
        var com = bat.GetComponent<BattleEventComponent>();
        self.events.Add(typeof(T), callback);
		com.Register(callback);
	}
	public static void UnRegister<T>(this BattleEventAutoDestroyComponent self,Action<T> callback) where T : struct
	{
        var bat = ClientBattle.Get();
        if (bat == null) return;
        var com = bat.GetComponent<BattleEventComponent>();
        self.events.Remove(typeof(T));
		com.UnRegister(callback);
	}
}