using ET;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TBS
{
	public class Skill:SkillBase
	{
		public int skillType;
		public int releaseType;
        public bool hasAction;//是否有动作
        public string audio;
        public string hitAudio;
        public List<float> hitTimeList = new List<float>();
		public List<Actor> targetList = new List<Actor>();
		public List<SkillNodeBase> nodeList = new List<SkillNodeBase>();

	}

	public static class SkillSystem
	{
		public static void Set(this Skill self,Actor caster, int skillId,int skillUniqueId)
		{
			self.caster = caster;
			self.skillId = skillId;
			self.uniqueId = skillUniqueId;
			self.targetList.Clear();
			self.InitCfg();
			self.InitComponent();
		}
		public static void InitCfg(this Skill self)
		{
			var cfg = ConfigSystem.Ins.GetSkillConfig(self.skillId);
			var showCfg = ConfigSystem.Ins.GetSkillShow(self.skillId);
			if (cfg == null || showCfg == null) return;
			self.skillType = cfg.Get("SkillType");
			self.releaseType = cfg.Get("CastMagic");
			self.during = showCfg.Get("SkillTime")/1000.0f;
            self.hasAction = showCfg.Get("Action") == 1;
            self.audio = showCfg.Get<string>("SoundEffects");
            self.hitAudio = showCfg.Get<string>("SoundHit");
            self.hitTimeList.Clear();
            var param = showCfg.Get<int[]>("HitTime");
            if(param != null)
            {
                for(int i = 0; i < param.Length; i++)
                {
                    self.hitTimeList.Add(param[i]/1000);
                }
            }
            else
            {
                self.hitTimeList.Add(0);
            }
        }
        public static void InitComponent(this Skill self)
		{
			self.AddNode<SkillAttackComponent>();
		}
		public static T AddNode<T>(this Skill self) where T : SkillNodeBase
		{
			var com = self.AddChild<T>();
			com.skill = self;
			self.nodeList.Add(com);
			return com;
		}
		public static bool DoSkill(this Skill self)
		{
			if (self.IsInCD()) return false;
			self.OnSkillStart();
			return true;
		}
		public static void HitEmpty(this Skill self)
		{
            var data = DamageDataFactory.Get();
			data.Set(self.caster, null, 0, DamageType.none,self);
			var com = self.caster.GetComponent<ActorEventComponent>();
			com.Broadcast(new SkillHitBeforeEvent() { data = data });
			com.Broadcast(new SkillHitAfterEvent() { data = data });
            DamageDataFactory.Release(data);
        }
		public static void Hit(this Skill self,Actor target)
		{ 
			var data = DamageDataFactory.Get();
			data.Set(self.caster, target, 0, DamageType.none, self);
			var com = self.caster.GetComponent<ActorEventComponent>();
			com.Broadcast(new SkillHitBeforeEvent() { data = data });
			self.DoDamage(data);
            com.Broadcast(new SkillHitAfterEvent() { data = data });
			com.Broadcast(new SkillActionHitPlayEvent() { target = target });
            DamageDataFactory.Release(data);
            self.PlayHitEffect(target);
		}
		public static void DoDamage(this Skill self,DamageData data)
		{
			if (data.target == null) return;
			data.target.Hurt(data);
		}

        public static void PlayReleaseEffect(this Skill self)
        {
            var cfg = ConfigSystem.Ins.GetSkillShow(self.skillId);
            if (cfg == null) return;
            var res = cfg.Get<string>("ReleaseSpe");
            if (string.IsNullOrEmpty(res)) return;
            var type = cfg.Get("ReleaseSpeType");
            var offset = cfg.Get<int[]>("ReleaseDeviation");
            self.PlayEffect(res, type, offset == null ? Vector2.zero : new Vector2(offset[0], offset[1]),self.caster);
        }
        public static void PlayHitEffect(this Skill self,Actor target)
        {
            var cfg = ConfigSystem.Ins.GetSkillShow(self.skillId);
            if (cfg == null) return;
            var res = cfg.Get<string>("HitSpe");
            if (string.IsNullOrEmpty(res)) return;
            var type = cfg.Get("HitSpeType");
            var offset = cfg.Get<int[]>("HitDeviation");
          
            self.PlayEffect(res, type, offset == null ? Vector2.zero : new Vector2(offset[0], offset[1]),target);
        }
        public static void PlayEffect(this Skill self, string res, int type, Vector2 offset,Actor target)
        {
            var bat = ClientBattle.Get();
            var effectShowCom = bat.GetComponent<BattleEffectShowComponent>();
            if (type == (int)SkillEffectType.Single)
            {
                var pos = target.pos;
                pos += offset;
                effectShowCom.PlayEffect(res, pos);

            }
            else
            {
                var pos = bat.GetComponent<BattleSceneComponent>().GetPos(target.camp, ConstValue.PLAYER_SITE);
                pos += offset;
                effectShowCom.PlayEffect(res, pos);
            }
        }

        public static void OnSkillStart(this Skill self)
		{
			self.targetList.Clear();
			self.SkillStart();
			foreach(var v in self.nodeList)
			{
				v.OnSkillStart();
			}
			self.caster.GetComponent<ActorEventComponent>().Broadcast(new SkillStartEvent() { skill = self });
		}
		public static void OnSkillEnd(this Skill self)
		{
			self.SkillEnd();
			foreach (var v in self.nodeList)
			{
				v.OnSkillEnd();
			}
			self.caster.GetComponent<ActorEventComponent>().Broadcast(new SkillEndEvent() { skill = self });
		}
		public static void TurnStart(this Skill self)
		{
			self.ReduceCD(1);
		}
		public static void AddTarget(this Skill self,Actor player)
		{
			self.targetList.Add(player);
		}
	}
}
