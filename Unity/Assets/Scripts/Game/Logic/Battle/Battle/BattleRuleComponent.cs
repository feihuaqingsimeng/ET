using ET;
using Frame;
using System.Collections.Generic;
using UnityEngine;

namespace TBS
{
	public class BattleRuleComponent:Entity,IAwake,IUpdate
	{
		public float time;
		public int turnCount;
		public BattleStageType battleStage;
		public bool isReleasing;
		public float delayTime = 0.1f;
		public float curTime;
		public int index;
		public bool isWaitingCmdEnd;
		public Queue<CommandBase> cmdQueue = new Queue<CommandBase>();
		public List<CommandBase> executeCmdList = new List<CommandBase>();
		public BattleReportComponent reportCom;

	}
	public class BattleRuleComponentUpdateSystem : UpdateSystem<BattleRuleComponent>
	{
		protected override void Update(BattleRuleComponent self)
		{
			self.Update();
		}
	}
	public static class BattleRuleComponentSystem
	{
		public static void StartFight(this BattleRuleComponent self)
		{
			self.turnCount = 0;
			self.time = 0;
			self.isReleasing = false;
			self.battleStage = BattleStageType.Start;
			self.cmdQueue.Clear();
			self.executeCmdList.Clear();
			self.reportCom = self.Parent.GetComponent<BattleReportComponent>();
		}
		public static void Update(this BattleRuleComponent self)
		{
			var dt = Time.deltaTime;
			self.time += dt;
			if (self.battleStage == BattleStageType.None) return;
			if(self.battleStage == BattleStageType.Start)
			{
				self.battleStage = BattleStageType.Battling;
				self.turnCount = 1;
				self.curTime = 0;
				self.isReleasing = true;
				self.AddTurnCmd();
				self.ExecuteCmd();
			}else if(self.battleStage == BattleStageType.Win)
			{
				self.BattleFinished(true);
			}
			else if (self.battleStage == BattleStageType.Failed)
			{
				self.BattleFinished(false);
			}
			if (self.isReleasing)
			{
				foreach(var v in self.executeCmdList)
				{
					if ((v.action == BattleActionType.ATTACK || v.action == BattleActionType.BUFF) && !v.isEnd)
						return;
				}
				self.curTime -= dt;
				if(self.curTime <= 0)
				{
					if(self.cmdQueue.Count == 0)
					{
						self.turnCount++;
						self.AddTurnCmd();
					}
					self.ExecuteCmd();
				}
			}
		}
		public static void AddTurnCmd(this BattleRuleComponent self)
		{
			if(self.turnCount > self.reportCom.turnCount)
			{
				self.battleStage = BattleStageType.Win;
				if(self.turnCount > self.reportCom.maxTurn)
				{
					self.turnCount = self.reportCom.maxTurn;
					self.battleStage = BattleStageType.Failed;
					return;
				}
				return;
			}
			var list = self.reportCom.GetCmdList(self.turnCount);
			if (list == null) return;
			self.cmdQueue.Clear();
			for(int i = 0; i < list.Count; i++)
			{
				self.cmdQueue.Enqueue(list[i]);
			}
		}
		public static void ExecuteCmd(this BattleRuleComponent self)
		{
			var evtCom = ClientBattle.Get().GetComponent<BattleEventComponent>();
			self.executeCmdList.Clear();
			CommandRound round = null;
			CommandRound nextRound = null;
			while (true)
			{
				if (self.cmdQueue.Count == 0)
					break;
				var cmd = self.cmdQueue.Peek();
				if(cmd.action == BattleActionType.ROUND_NODE)
				{
					if(round == null)
					{
						round = cmd as CommandRound;
						if (round.IsTurnStart())
						{
							//fire event
							evtCom.Publish(new BattleTurnStartEvent() { turn = round.frame });
						}
					}
					else
					{
						nextRound = cmd as CommandRound;
						if(nextRound.IsTurnEnd())
							self.cmdQueue.Dequeue();
						break;
					}
				}
				self.cmdQueue.Dequeue();
				cmd.Execute();
				self.executeCmdList.Add(cmd);
			}
			if (round == null) return;
			self.curTime = 0;
			self.isReleasing = true;
			self.isWaitingCmdEnd = false;
			self.curTime += self.delayTime;
		}
		public static void BattleFinished(this BattleRuleComponent self, bool isWin)
		{
			self.isReleasing = false;
			self.battleStage = BattleStageType.Finished;
            ClientBattle.Get().GetComponent<BattleEventComponent>().Publish(new BattleFinishedEvent() { isWin = isWin });
		}
	}
}
