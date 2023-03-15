using ET;

namespace TBS
{
	class CommandDead:CommandBase
	{
		public int fighterId;
		public int killerId;

		public override bool Execute()
		{
			var flag = base.Execute();
			if (!flag) return false;
			var bat = ClientBattle.Get();
            var cmd = bat.GetComponent<BattleReportComponent>().GetPreCmd(this);
			RegisterPreCmdActive(cmd, Dead);
			return true;
		}
		public void Dead()
		{
			var bat = ClientBattle.Get();
            var actorSys = bat.GetComponent<BattleActorsComponent>();
			var dead = actorSys.GetActor(fighterId);
			var killer = actorSys.GetActor(killerId);
			dead.Dead();
			Active();
			CmdEnd();
		}
		public override string ToString()
		{
			var str = base.ToString();
			return $"    [死亡]  {str},死亡:{fighterId},击杀者:{killerId}";
		}
	}
}
