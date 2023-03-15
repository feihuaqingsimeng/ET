using ET;
using Frame;

namespace TBS
{
	class CommandBuff:CommandBase
	{
		public int fighterId;
		public int targetId;
		public int type;
		public int buffId;
		public int overlay;
		public int sourceSkillId;
		public int sourceUniqueSkillId;

		public Skill skill;
		public Actor caster;
		public Actor target;
		public override bool Execute()
		{
			var flag = base.Execute();
			if (!flag) return false;
			var bat = ClientBattle.Get();
            var actorSys = bat.GetComponent<BattleActorsComponent>();
			caster = actorSys.GetActor(fighterId);
			if (caster == null)
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
			if(type == 1)//移除
			{
				var pre = bat.GetComponent<BattleReportComponent>().GetPreCmd(this);
				RegisterPreCmdActive(pre, RemoveBuffImmediate);
			}
			if (sourceUniqueSkillId > 0)
			{
				var cfg = ConfigSystem.Ins.GetSkillConfig(sourceSkillId);
				if (cfg == null) return false;
				skill = caster.GetComponent<ActorSkillsComponent>().GetSkill(sourceUniqueSkillId);
				var enemyBuffs = cfg.Get<int[][]>("EnemyBuff");
				if(enemyBuffs != null)
				{
					for(int i = 0; i < enemyBuffs.Length; i++)
					{
						int type = enemyBuffs[i][0];
						int id = enemyBuffs[i][1];
						if (buffId == id)
						{
							if (type == 2 || type == 3)//伤害前后
							{
								caster.GetComponent<ActorEventComponent>().Register<SkillHitAfterEvent>(Event_SkillHitAfter);
								return true;
							}
							else
							{
								break;
							}
						}
						
					}
				}
				var selfBuffs = cfg.Get<int[][]>("OwnBuff");
				if (selfBuffs != null)
				{
					for (int i = 0; i < selfBuffs.Length; i++)
					{
						int type = selfBuffs[i][0];
						int id = selfBuffs[i][1];
						if (buffId == id)
						{
							if (type == 1)//攻击结束
							{
								caster.GetComponent<ActorEventComponent>().Register<SkillHitAfterEvent>(Event_SkillHitAfter);
								return true;
							}
							else
							{
								break;
							}
						}

					}
				}
			}
			var cmd = bat.GetComponent<BattleReportComponent>().GetPreCmd(this);
			RegisterPreCmdActive(cmd, AddBuffImmediate);
			return true;
		}
		private void Event_SkillHitAfter(SkillHitAfterEvent e)
		{
            if (e.data.attaker != caster) return;
			if (e.data.sourceSkill != skill) return;
			AddBuffImmediate();
		}
		private void AddBuffImmediate()
		{
			target.GetComponent<ActorBuffsComponent>().AddBuff(buffId, overlay);
			Active();
			CmdEnd();
		}
		private void RemoveBuffImmediate()
		{
			target.GetComponent<ActorBuffsComponent>().RemoveBuff(buffId);
			Active();
			CmdEnd();
		}
		public override string ToString()
		{
			var str = base.ToString();
			var str2 = "";
			if (type == 1)
				str2 = ",(移除)";
			return $"    [BUFF]  {str},施法者:{fighterId},目标:{targetId},buffId:{buffId},层数:{overlay},来源技能:{sourceSkillId},来源技能唯一id:{sourceUniqueSkillId}{str2}";
		}
	}
}
