using ET;
using UnityEngine;

namespace TBS
{
	public class Buffer:Entity,IAwake
	{
		public int buffId;
		public int overlay;			//叠加层级
		public Actor target;		//挂载对象
		public int turnDuring;      //持续回合
		public Vector2 pos;     //位置
		public bool isDie;
	}

	public static class BufferSystem
	{
		public static void Set(this Buffer self,int buffId,int overlay,Actor target)
		{
			self.buffId = buffId;
			self.overlay = overlay;
			self.target = target;
			self.InitCfg();
		}
		private static void InitCfg(this Buffer self)
		{
			var cfg = ConfigSystem.Ins.GetBuff(self.buffId);
			var showCfg = ConfigSystem.Ins.GetBuffShow(self.buffId);
			if (cfg == null || showCfg == null)
			{
				self.isDie = true;
				return;
			}
			self.turnDuring = cfg.Get("Continued");
		}
		public static void OnBuffStart(this Buffer self)
		{

		}
		public static void OnBuffRemove(this Buffer self)
		{

		}
		public static void TurnStart(this Buffer self)
		{
			if (self.turnDuring == 0) return;
			self.turnDuring--;
			if (self.turnDuring <= 0)
			{
				self.turnDuring = 0;
			}
		}
	}
}
