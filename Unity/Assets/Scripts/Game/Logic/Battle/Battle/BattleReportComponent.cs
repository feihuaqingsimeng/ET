using ET;
using Frame;
using LitJson;
using System.Collections.Generic;

namespace TBS
{
	public class BattleReportComponent:Entity,IAwake
	{
		public Dictionary<int, List<CommandBase>> commandList = new Dictionary<int, List<CommandBase>>();
		public List<CommandBase> initCmdList = new List<CommandBase>();
		public int maxTurn = 30;
		public int turnCount = 0;
		public string newCmdStr;
		public string reportPath;
	}
	public static class BattleReportComponentSystem
	{
		public static string GetJsonMapValueStr(JsonData data,string key)
		{
			if(data.TryGetValue(key, out JsonData d))
			{
				return d.ToString();
			}
			return "";
		}
		public static void ParseReport(this BattleReportComponent self,string report)
		{
			if (string.IsNullOrEmpty(report)) return;
			self.turnCount = 0;
			int cid = 1;
			JsonData datas = JsonMapper.ToObject(report);
			self.commandList.Clear();
			self.initCmdList.Clear();
			int count = datas.Count;
			for(int i= 0; i < count; i++)
			{
				JsonData data = datas[i];
				int.TryParse(data["1"].ToString(),out int frame);
				int.TryParse(data["2"].ToString(), out int action);
				JsonData childMap = data["4"];
				if (self.turnCount < frame) self.turnCount = frame;
				CommandBase cmd = null;
				switch ((BattleActionType)action)
				{
					case BattleActionType.INIT:
						{
							
							int.TryParse(childMap["1"].ToString(), out int fighterId);
							int.TryParse(childMap["4"].ToString(), out int hp);
							int.TryParse(childMap["5"].ToString(), out int objType);
							string name = GetJsonMapValueStr(childMap, "6");
							int.TryParse(childMap["7"].ToString(), out int teamIndex);
							int.TryParse(childMap["8"].ToString(), out int posIndex);
                            int.TryParse(childMap["10"].ToString(), out int modelId);

                            cmd = new CommandInit
							{
								cid = cid++,
								frame = frame,
								action = (BattleActionType)action,
								fighterId = fighterId,
                                modelId = modelId,
                                hp = hp,
								hpMax = hp,
								posIndex = posIndex,
								camp = (ActorCampType)teamIndex,
								actorType = (ActorType)objType,
								playerName = name,
							};
							cmd.Execute();
							self.initCmdList.Add(cmd);
						}
						break;
					case BattleActionType.ATTACK:
						{
							int.TryParse(childMap["1"].ToString(), out int fighterId);
							int.TryParse(childMap["2"].ToString(), out int targetId);
							int.TryParse(childMap["3"].ToString(), out int skillId);
							int.TryParse(childMap["10"].ToString(), out int skillUniqueId);
							int.TryParse(childMap["11"].ToString(), out int sourceSkillUniqueId);
							cmd = new CommandAttack
							{
								cid = cid++,
								frame = frame,
								action = (BattleActionType)action,
								fighterId = fighterId,
								skillId = skillId,
								skillUniqueId = skillUniqueId,
								sourceSkillUniqueId = sourceSkillUniqueId
							};
							self.AddCommand(cmd);
						}
						break;
					case BattleActionType.DAMAGE:
						{
							int.TryParse(childMap["1"].ToString(), out int fighterId);
							int.TryParse(childMap["2"].ToString(), out int targetId);
							int.TryParse(childMap["3"].ToString(), out int damage);
							int.TryParse(childMap["4"].ToString(), out int damageType);
							int.TryParse(childMap["5"].ToString(), out int skillId);
							int.TryParse(childMap["10"].ToString(), out int skillUniqueId);
							cmd = new CommandDamage
							{
								cid = cid++,
								frame = frame,
								action = (BattleActionType)action,
								fighterId = fighterId,
								targetId = targetId,
								damage = damage,
								damageType = (DamageType)damageType,
								skillId = skillId,
								skillUniqueId = skillUniqueId
							};
							self.AddCommand(cmd);
						}
						break;
					case BattleActionType.BUFF:
						{
							int.TryParse(data["3"].ToString(), out int type);
							int.TryParse(childMap["1"].ToString(), out int fighterId);
							int.TryParse(childMap["2"].ToString(), out int targetId);
							int.TryParse(childMap["3"].ToString(), out int buffId);
							int.TryParse(childMap["5"].ToString(), out int overlay);
							int.TryParse(childMap["9"].ToString(), out int sourceSkillId);
							int.TryParse(childMap["10"].ToString(), out int sourceUniqueSkillId);
							cmd = new CommandBuff
							{
								cid = cid++,
								frame = frame,
								action = (BattleActionType)action,
								type = type,
								fighterId = fighterId,
								targetId = targetId,
								buffId = buffId,
								overlay = overlay,
								sourceSkillId = sourceSkillId,
								sourceUniqueSkillId = sourceUniqueSkillId
							};
							self.AddCommand(cmd);
						}
						break;
					case BattleActionType.ROUND_NODE:
						{
							int.TryParse(childMap["1"].ToString(), out int fighterId);
							cmd = new CommandRound
							{
								cid = cid++,
								frame = frame,
								action = (BattleActionType)action,
								fighterId = fighterId,
							};
							self.AddCommand(cmd);
						}
						break;
					case BattleActionType.DIE:
						{
							int.TryParse(childMap["1"].ToString(), out int fighterId);
							int.TryParse(childMap["2"].ToString(), out int killerId);
							cmd = new CommandDead
							{
								cid = cid++,
								frame = frame,
								action = (BattleActionType)action,
								fighterId = fighterId,
								killerId = killerId,
							};
							self.AddCommand(cmd);

						}
						break;
					case BattleActionType.HIT:
						{
							int.TryParse(childMap["1"].ToString(), out int fighterId);
							int.TryParse(childMap["3"].ToString(), out int skillId);
							int.TryParse(childMap["10"].ToString(), out int skillUniqueId);
							cmd = new CommandHit
							{
								cid = cid++,
								frame = frame,
								action = (BattleActionType)action,
								fighterId = fighterId,
								skillId = skillId,
								skillUniqueId = skillUniqueId,
							};
							self.AddCommand(cmd);
						}
						break;
					default:
						Log.Error($"额外未解析的命令：{action}");
						break;
				}
			}
			self.Parent.GetComponent<BattleEventComponent>().Publish(new BattleCmdParseFinished());
		}
		public static void AddCommand(this BattleReportComponent self, CommandBase cmd)
		{
			if(self.commandList.TryGetValue(cmd.frame,out var list))
			{
				list.Add(cmd);
				return;
			}
			list = new List<CommandBase>();
			list.Add(cmd);
			self.commandList.Add(cmd.frame, list);
		}
		public static CommandBase GetPreCmd(this BattleReportComponent self,CommandBase cmd)
		{
			var list = self.GetCmdList(cmd.frame);
			if (list == null) return null;
			for(int i = 0; i < list.Count; i++)
			{
				if(list[i] == cmd)
				{
					return i == 0 ? null : list[i - 1];
				}
			}
			return null;
		}
		public static CommandAttack GetPreSkillCmd(this BattleReportComponent self, CommandBase cmd,int skillUniqueId)
		{
			var list = self.GetCmdList(cmd.frame);
			if (list == null) return null;
			CommandAttack pre = null;
			foreach(var v in list)
			{
				if (v == cmd)
					return pre;
				else if (v.action == BattleActionType.ATTACK)
				{
					var atk = v as CommandAttack;
					if (atk.skillUniqueId == skillUniqueId) pre = atk;
				}
			}
			return null;
		}
		public static List<CommandBase> GetCmdList(this BattleReportComponent self,int frame)
		{
			self.commandList.TryGetValue(frame, out var v);
			return v;
		}
	}
}
