using ET;
using System.Collections.Generic;

namespace TBS
{
	public class ActorAttrComponent:Entity,IAwake
	{
		public Dictionary<AttributeType, int> attrs = new Dictionary<AttributeType, int>();
	}

	public static class ActorAttrComponentSystem
	{
		public static void SetAttr(this ActorAttrComponent self, AttributeType id,int value)
		{
			self.attrs[id] = value;
		}
		public static void AddAttr(this ActorAttrComponent self, AttributeType id,int add)
		{
			int value = add;
			if(self.attrs.TryGetValue(id,out var v))
			{
				value += v;
			}
			self.attrs[id] = value;
		}
		public static int GetAttr(this ActorAttrComponent self, AttributeType id)
		{
			self.attrs.TryGetValue(id, out var v);
			return v;
		}
		public static void SetHp(this ActorAttrComponent self,int hp)
		{
			self.SetAttr(AttributeType.Hp, hp);
		}
		public static void SetHpMax(this ActorAttrComponent self, int max)
		{
			self.SetAttr(AttributeType.Hp_Max, max);
		}
		public static int GetHp(this ActorAttrComponent self)
		{
			return self.GetAttr(AttributeType.Hp);
		}
		public static int GetHpMax(this ActorAttrComponent self)
		{
			return self.GetAttr(AttributeType.Hp_Max);
		}
	}
}
