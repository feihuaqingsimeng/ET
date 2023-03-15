using ET;
using Frame;

namespace TBS
{
	public class CommandAttack : CommandBase
	{
		public int fighterId;
		public int skillId;
		public int skillUniqueId;
		public int sourceSkillUniqueId;

		public override bool Execute()
		{
			var flag = base.Execute();
			if (!flag) return false;
			var bat = ClientBattle.Get();
            var sys = bat.GetComponent<BattleActorsComponent>();
			var actor = sys.GetActor(fighterId);
			if (sourceSkillUniqueId <= 0)//触发时机查找上一次damage,attack
			{
				actor.GetComponent<ActorSkillsComponent>().GetOrCreateSkill(skillUniqueId, skillId);
				CommandBase node = this;
				CommandBase preCmd = null;
				var report = bat.GetComponent<BattleReportComponent>();
				while (node != null)
				{
					var cmd = report.GetPreCmd(node);
					if (cmd == null||cmd.action == BattleActionType.ROUND_NODE) break;
					if(cmd.action == BattleActionType.ATTACK && (cmd as CommandAttack).sourceSkillUniqueId != -1)
					{
						preCmd = cmd;
						break;
					}
					if(cmd.action == BattleActionType.DAMAGE)
					{
						var damageCmd = cmd as CommandDamage;
						if(damageCmd.skillUniqueId != -1)
						{
							var skillCmd = report.GetPreSkillCmd(damageCmd, damageCmd.skillUniqueId);
							if(skillCmd != null && skillCmd.sourceSkillUniqueId != -1)
							{
								preCmd = cmd;
								break;
							}
						}
						
					}
					node = cmd;
				}
				RegisterPreCmdActive(preCmd, DoSkill);
			}
			else
			{
				var sourceSkill = actor.GetComponent<ActorSkillsComponent>().GetSkill(sourceSkillUniqueId);
				if(sourceSkill == null)
				{
					Log.Error($"{cid}找不到source skill:{sourceSkillUniqueId}");
					return false;
				}
				Log.Error("未实现技能触发技能");
			}
			return true;
		}
		
		public void DoSkill()
		{
			var bat = ClientBattle.Get();
            var sys = bat.GetComponent<BattleActorsComponent>();
			var actor = sys.GetActor(fighterId);
			var skillCom = actor.GetComponent<ActorSkillsComponent>();
			skillCom.DoSkill(skillUniqueId);
			Active();
			var skill = skillCom.GetSkill(skillUniqueId);
			if(skill == null|| skill.during <= 0)
			{
				CmdEnd();
				return;
			}
			var evtSys = actor.GetComponent<ActorEventComponent>();
			evtSys.Register<SkillEndEvent>(SkillEnd);
		}
		public void SkillEnd(SkillEndEvent e)
		{
			CmdEnd();
		}
		public override string ToString()
		{
			var str = base.ToString();
			return $"    [攻击]  {str},fighterId:{fighterId},技能id:{skillId},技能唯一id:{skillUniqueId},来源技能:{sourceSkillUniqueId}";
		}
	}
}
