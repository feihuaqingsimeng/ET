using ET;
using System.Collections.Generic;

namespace TBS
{
	public class ActorBuffsComponent:Entity,IAwake
	{

	}

	public static class ActorBuffsComponentSystem
	{
		public static Actor GetActor(this ActorBuffsComponent self)
		{
			return self.Parent as Actor;
		}
		public static Dictionary<long,Entity> GetBuffList(this ActorBuffsComponent self)
		{
			return self.Children;
		}
		public static Buffer GetBuff(this ActorBuffsComponent self,int buffId)
		{
			return self.GetChild<Buffer>(buffId);
		}
		public static Buffer AddBuff(this ActorBuffsComponent self, int buffId,int overlay)
		{
			var buff = self.GetBuff(buffId);
			if(buff == null)
			{
				buff = self.AddChildWithId<Buffer>(buffId);
			}
			buff.Set(buffId, overlay, self.GetActor());
			self.Parent.GetComponent<ActorEventComponent>().Broadcast(new BuffChangeEvent() { buff = buff });
			return buff;
		}
		public static void RemoveBuff(this ActorBuffsComponent self, int buffId)
		{
			var buff = self.GetChild<Buffer>(buffId);
			if (buff == null) return;
			self.Parent.GetComponent<ActorEventComponent>().Broadcast(new BuffDestroyEvent() { buff = buff });

			buff.Dispose();
		}
		public static void RemoveAll(this ActorBuffsComponent self)
		{
			foreach (Entity child in self.Children.Values)
			{
				child.Dispose();
			}
		}
		public static void TurnStart(this ActorBuffsComponent self)
		{
			foreach (var v in self.Children)
			{
				var buff = v.Value as Buffer;
				buff.TurnStart();
			}
		}
	}
}
