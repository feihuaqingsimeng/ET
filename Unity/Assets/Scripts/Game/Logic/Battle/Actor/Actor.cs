using ET;
using Frame;
using System.Collections.Generic;
using UnityEngine;

namespace TBS
{
	public class Actor : Entity, IAwake, IDestroy
	{
		public long fighterId;
		public ActorType actorType;
		public ActorCampType camp;
		public string name;
		public int level;
		public Vector2 pos;
		public int height = 130;//身高
		public bool isDead;
		public bool isDestroyed;
		public int posIndex;	//站位

		public int modelId;
		public int picFrame;
		public int groupId;
		public int groupIndex;
		public long playerId;
		public List<Actor> targetList = new List<Actor>();

		public ActorEventComponent eventCom = new ActorEventComponent();

	}
	public class ActorAwakeSystem : AwakeSystem<Actor>
	{
		protected override void Awake(Actor self)
		{
			//todo add skillmgr
			self.eventCom = self.AddComponent<ActorEventComponent>();
			self.AddComponent<ActorSkillsComponent>();
			self.AddComponent<ActorBuffsComponent>();
			self.AddComponent<ActorAttrComponent>();

			var globalEvt = ClientBattle.Get().GetComponent<BattleEventComponent>();
			globalEvt.Register<BattleTurnStartEvent>(self.Event_BattleTurnStartEvent);
		}
	}
	public class ActorDestroySystem : DestroySystem<Actor>
	{
		protected override void Destroy(Actor self)
		{
			self.OnDestroy();
			var globalEvt = ClientBattle.Get().GetComponent<BattleEventComponent>();
			globalEvt.UnRegister<BattleTurnStartEvent>(self.Event_BattleTurnStartEvent);
		}
	}
	public static class ActorSystem
	{
		
		public static void TurnStart(this Actor self)
		{
			throw new System.NotImplementedException();
			//self.skillMgr:TurnStart()
			//TBS.BufferMgr.TurnStart(self.id)
		}
		public static void Hurt(this Actor self,DamageData data)
		{
			if (self.isDead) return;
			if (data.damageType == DamageType.none) return;
			var attrCom = self.GetComponent<ActorAttrComponent>();
			var hp = attrCom.GetHp();
			var hpMax = attrCom.GetHpMax();
			int damage = data.damage;
			if (data.damageType == DamageType.heal)
				damage = -Mathf.Abs(damage);
			hp = Mathf.Max(0, hp - damage);
			hp = Mathf.Min(hp, hpMax);
			attrCom.SetHp(hp);

			var com = self.GetComponent<ActorEventComponent>();
			com.Broadcast(new PlayerHpChangedEvent() { actor = self });
			var evt = new DamageShowEvent()
			{
				actor = self,
				damage = damage,
				damageType = data.damageType,
			};
			com.Broadcast(evt);
		}
		public static void Dead(this Actor self)
		{
			if (self.isDead) return;
			self.isDead = true;
			self.GetComponent<ActorAttrComponent>().SetHp(0);
			self.GetComponent<ActorEventComponent>().Broadcast(new ActorDeadEvent() { actor = self });

		}
		public static void OnDestroy(this Actor self)
		{
			self.isDestroyed = true;
			self.isDead = true;
		}
        public static Vector2 GetHitPos(this Actor self)
        {
            Vector2 p = self.pos;
            if (self.camp == ActorCampType.Red)
                p.y += self.height;
            else
                p.y -= self.height;
            return p;
        }
        public static Vector2 GetDamageShowPos(this Actor self)
        {
            Vector2 p = self.pos;
            p.y -= self.height/2;
            return p;
        }
        public static void SetPos(this Actor self, float x, float y)
		{
			self.pos.Set(x, y);
		}
        public static void SetPos(this Actor self, Vector2 p)
        {
            self.pos = p;
        }
        public static void Event_BattleTurnStartEvent(this Actor self, BattleTurnStartEvent e)
		{
			if(e.turn > 1)
			{
				self.GetComponent<ActorSkillsComponent>().TurnStart();
				self.GetComponent<ActorBuffsComponent>().TurnStart();
			}
		}
	}
}
