using ET;
using GameEvent;
using Frame;
using System;
using System.Collections.Generic;

namespace TBS
{
	public class ActorEventComponent:EventBaseSystem
	{
	}
	public static class ActorEventComponentSystem
	{
		public static void Broadcast<T>(this ActorEventComponent self, T a) where T : struct
		{
			self.Publish(a);
			ClientBattle.Get().GetComponent<BattleEventComponent>().Publish(a);
		}
	}
}
