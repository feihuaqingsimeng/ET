using ET;
using Frame;

namespace TBS
{
	class CommandHit:CommandBase
	{
		public int fighterId;
		public int skillId;
		public int skillUniqueId;

		public Actor actor;
		public Skill skill;
		public override bool Execute()
		{
			var flag = base.Execute();
			if (!flag) return false;
			var bat = ClientBattle.Get();
            actor = bat.GetComponent<BattleActorsComponent>().GetActor(fighterId);
			if (actor == null) return false;
			if(skillUniqueId > 0)
			{
				skill = actor.GetComponent<ActorSkillsComponent>().GetSkill(skillUniqueId);
				if(skill != null)
				{
					actor.GetComponent<ActorEventComponent>().Register<SkillHitAfterEvent>(Event_SkillHitAfter);
				}
			}
			Active();
			CmdEnd();
			return true;
		}
		private void Event_SkillHitAfter(SkillHitAfterEvent e)
		{
			if (e.data.attaker != actor) return;
			if (e.data.sourceSkill != skill) return;
			Active();
			CmdEnd();
		}
		public override string ToString()
		{
			var str = base.ToString();
			return $"[    受击]  {str},fighter:{fighterId},技能id:{skillId},技能唯一id:{skillUniqueId}";
		}
	}
}
