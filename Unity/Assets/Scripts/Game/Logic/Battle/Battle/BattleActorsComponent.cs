using ET;
using System.Collections.Generic;

namespace TBS
{
	public class BattleActorsComponent:Entity,IAwake
	{
		public List<Actor> actorList = new List<Actor>();
	}

	public static class BattleActorsComponentSystem
	{
		public static Actor CreateActor(this BattleActorsComponent self,long id,int modelId,ActorType type,ActorCampType camp)
		{
			var actor = self.AddChildWithId<Actor>(id);
			actor.modelId = modelId;
			actor.actorType = type;
			actor.camp = camp;
			return actor;
		}
		public static void DestroyActor(this BattleActorsComponent self,Actor actor)
		{
			actor.Dispose();
		}
		public static Actor GetActor(this BattleActorsComponent self, long id)
		{
			
			return self.GetChild<Actor>(id);
		}
		public static List<Actor> GetActorList(this BattleActorsComponent self)
		{
			self.actorList.Clear();
			foreach (var v in self.Children)
			{
				self.actorList.Add(v.Value as Actor);
			}
			return self.actorList;
		}
	}
}
