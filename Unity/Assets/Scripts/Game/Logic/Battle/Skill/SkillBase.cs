using ET;

namespace TBS
{
	public class SkillBase:Entity,IAwake
	{
		public int uniqueId;
		public Actor caster;
		public int skillId;
		public float during;
		public float curTime;
		public bool isEnd;
		public bool isStart;
		//cd
		public int initialCD;
		public int maxCD;
		public int curCD;
	}
	
	public static class SkillBaseSystem
	{
		public static void StartCD(this SkillBase self,int cd)
		{
			self.maxCD = cd;
			self.curCD = cd;
		}
		public static void ReduceCD(this SkillBase self,int cd)
		{
			if (self.curCD == 0) return;
			self.curCD -= cd;
			if(self.curCD <= 0)
			{
				self.curCD = 0;
			}
		}
		public static bool IsInCD(this SkillBase self)
		{
			return self.curCD > 0;
		}
		public static void SkillStart(this SkillBase self)
		{
			self.curTime = 0;
			self.isEnd = false;
			self.isStart = true;
		}
		public static void SkillEnd(this SkillBase self)
		{
			self.isStart = false;
			self.isEnd = true;
		}
	}
	
}
