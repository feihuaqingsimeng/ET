using ET;
using Frame;

namespace TBS
{
	class CommandDamage:CommandBase
	{
		public int fighterId;
		public int targetId;
		public int skillId;
		public int skillUniqueId;
		public int damage;
		public DamageType damageType;

		public Skill skill;
		public Actor attacker;
		public Actor target;
		public override bool Execute()
		{
			var flag = base.Execute();
			if (!flag) return false;
			var bat = ClientBattle.Get();
            var actorSys = bat.GetComponent<BattleActorsComponent>();
			attacker = actorSys.GetActor(fighterId);
			if(attacker == null)
			{
				Log.Error($"找不到fighter,{ToString()}");
				return false;
			}
			target = actorSys.GetActor(targetId);
			if (target == null)
			{
				Log.Error($"找不到target,{ToString()}");
				return false;
			}
			if(skillUniqueId > 0)
			{
				skill = attacker.GetComponent<ActorSkillsComponent>().GetSkill(skillUniqueId);
				skill.AddTarget(target);
				var evtSys = attacker.GetComponent<ActorEventComponent>();
				evtSys.Register<SkillHitBeforeEvent>(Event_SkillHitBefore);
				evtSys.Register<SkillHitAfterEvent>(Event_SkillHitAfter);
				return true;
			}
			//触发时间同步上个节点
			var cmd = bat.GetComponent<BattleReportComponent>().GetPreCmd(this);
			RegisterPreCmdActive(cmd, DoDamageImmediate);
			return true;
		}
		private void DoDamageImmediate()
		{
			var data = DamageDataFactory.Get();
			data.Set(attacker, target, damage, damageType);
			target.Hurt(data);
            DamageDataFactory.Release(data);
            Active();
		}
		private void Event_SkillHitBefore(SkillHitBeforeEvent e)
		{
			if (e.data.attaker != attacker || e.data.target != target)
				return;
			if (e.data.isHandled)
				return;
			e.data.damage = damage;
			e.data.damageType = damageType;
			var evtSys = attacker.GetComponent<ActorEventComponent>();
			evtSys.UnRegister<SkillHitBeforeEvent>(Event_SkillHitBefore);
		}
		private void Event_SkillHitAfter(SkillHitAfterEvent e)
		{
			Active();
			var evtSys = attacker.GetComponent<ActorEventComponent>();
			evtSys.UnRegister<SkillHitAfterEvent>(Event_SkillHitAfter);
		}
		public override string ToString()
		{
			var str = base.ToString();
			return $"        [伤害]{str},攻击者:{fighterId},目标:{targetId},技能id:{skillId},技能唯一id:{skillUniqueId},伤害:{damage},伤害类型:{damageType}";
		}
	}
}
