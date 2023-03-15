using ET;

namespace TBS
{
	public class ActorSkillsComponent:Entity,IAwake
	{

	}
	public static class ActorSkillsComponentSystem
	{
		public static Actor GetActor(this ActorSkillsComponent self)
		{
			return self.Parent as Actor;
		}
		public static Skill GetSkill(this ActorSkillsComponent self,int uniqueId)
		{
			return self.GetChild<Skill>(uniqueId);
		}
		public static Skill GetOrCreateSkill(this ActorSkillsComponent self, int uniqueId, int skillId)
		{
			var s = self.GetSkill(uniqueId);
			if (s != null) return s;
			s = self.AddChildWithId<Skill>(uniqueId);
			s.Set(self.GetActor(), skillId, uniqueId);
			return s;
		}
		public static bool DoSkill(this ActorSkillsComponent self,int uniqueId)
		{
			var s = self.GetSkill(uniqueId);
			if (s == null) return false;
			return s.DoSkill();
		}
		public static void TurnStart(this ActorSkillsComponent self)
		{
			foreach(var v in self.Children)
			{
				var skill = v.Value as Skill;
				skill.TurnStart();
			}
		}
	}
}
