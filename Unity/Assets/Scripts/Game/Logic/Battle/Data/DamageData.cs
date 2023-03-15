
using ET;
using System.Collections.Generic;

namespace TBS
{
	public class DamageData
	{
		public Actor attaker;
		public Actor target;
		public int damage;
		public DamageType damageType;
		public Buffer sourceBuff;
		public Skill sourceSkill;
		public bool isHandled;

		public void Set(Actor attaker, Actor target, int damage, DamageType damageType)
		{
			this.attaker = attaker;
			this.target = target;
			this.damage = damage;
			this.damageType = damageType;
			sourceBuff = null;
			sourceSkill = null;
		}
		public void Set(Actor attaker, Actor target, int damage, DamageType damageType, Buffer sourceBuff)
		{
            Set(attaker, target, damage, damageType);
			this.sourceBuff = sourceBuff;
		}
		public void Set(Actor attaker, Actor target, int damage, DamageType damageType, Skill sourceSkill)
		{
			Set(attaker, target, damage, damageType);
			this.sourceSkill = sourceSkill;
		}
	}
	public class DamageDataFactory
	{
		private static List<DamageData> cacheList = new List<DamageData>();
		public static DamageData Get()
		{
			if (cacheList.Count > 0)
			{
                var d = cacheList[cacheList.Count - 1];
                cacheList.RemoveAt(cacheList.Count - 1);
                return d;
			}
			return new DamageData();
		}
        public static void Release(DamageData data)
        {
            if (cacheList.Contains(data)) return;
            cacheList.Add(data);
        }
	}
}
