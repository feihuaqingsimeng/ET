using ET;

namespace TBS
{
	class CommandRound:CommandBase
	{
		public int fighterId;//战斗对象id, -1表示回合开始时, -2表示回合结束时

		public bool IsTurnStart()
		{
			return fighterId == -1;
		}
		public bool IsTurnEnd()
		{
			return fighterId == -2;
		}
		public override bool Execute()
		{
			var flag = base.Execute();
			if (!flag) return false;
			Active();
			CmdEnd();
			return true;
		}
		public override string ToString()
		{
			var str = base.ToString();
			var str2 = "";
			if (IsTurnStart())
				str2 = "[回合开始]";
			else if (IsTurnEnd())
				str2 = "[回合结束]";
			else
				str2 = "[英雄回合]";
			return $"{str2}{str},fighter:{fighterId}";
		}

	}
}
