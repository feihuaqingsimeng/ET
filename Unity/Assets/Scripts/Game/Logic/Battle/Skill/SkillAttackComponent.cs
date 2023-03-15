using ET;
using Frame;
namespace TBS
{
	public class SkillAttackComponent : SkillNodeBase
	{
		public float delay;
		public float curTime;
		public bool isExecute;
		public bool isFirst;
		public override void OnSkillStart() {
			curTime = 0;
			isFirst = true;
			isExecute = false;
			this.DoSkill();
		}
	}
	public class SkillAttackComponentAwakeSystem : AwakeSystem<SkillAttackComponent>
	{
		protected override void Awake(SkillAttackComponent self)
		{
			
		}
	}
	public static class SkillAttackComponentSystem
	{
		public static async void HitImmidiate(this SkillAttackComponent self)
        {
            var token = self.GetToken(1);
            var timer = TimerComponent.Instance;
            var hitTime = (long)(self.skill.hitTimeList[0] * 1000);
            var f = await timer.WaitAsync(hitTime, token);
            if (!f) return;
            var targetList = self.skill.targetList;
            int count = targetList.Count;
            if (count == 0)
            {
                self.skill.HitEmpty();
            }
            else
            {
                foreach (var v in targetList)
                {
                    self.skill.Hit(v);
                }
            }
            self.skill.PlayReleaseEffect();
            f = await timer.WaitAsync((long)(self.skill.during*1000) - hitTime, token);
            if (!f) return;
            self.skill.OnSkillEnd();
        }
		public static async void DoSkill(this SkillAttackComponent self)
		{
            var token = self.GetToken(1);
            var timer = TimerComponent.Instance;
            var f = await timer.WaitFrameAsync(token);
			if (!f) return;
			
			var evtCom = self.skill.caster.GetComponent<ActorEventComponent>();
			var targetList = self.skill.targetList;
			int count = targetList.Count;
            if(!self.skill.hasAction || count == 0)
            {
                self.HitImmidiate();
                return;
            }
			//远程一次性攻击
			if(self.skill.releaseType == (int)SkillReleaseType.Remote)
			{
				evtCom.Broadcast(new SkillActionAttackPlayEvent() { skill = self.skill });
				float waitTime = ConstValue.SkillAttackTime * 500;
                //action time
                f = await timer.WaitAsync((long)waitTime, token);
				if (!f) return;
                self.skill.PlayReleaseEffect();

                self.HitAsync();
                f = await timer.WaitAsync((long)(self.skill.during*1000), token);
                if (!f) return;
                self.skill.OnSkillEnd();
				return;
			}
			int index = 0;
			//近战逐个攻击
			while (index < targetList.Count)
			{
				var target = targetList[index];
				//相同目标持续攻击
				var isFront = index == 0 || target != targetList[index - 1];
				//var isBack = (index == targetList.Count - 1) || target != targetList[index + 1];
                //Front time
                if (isFront)
				{
                    self.skill.caster.SetPos(target.GetHitPos());
                    var moveTime = ConstValue.SkillMeleeMoveTime;
					evtCom.Broadcast(new SkillActionFrontMoveEvent() { skill = self.skill,target = target,time = moveTime });
					f = await timer.WaitAsync((long)(moveTime*1000), token);
					if (!f) return;
				}
                //action time
                evtCom.Broadcast(new SkillActionAttackPlayEvent() { skill = self.skill});
				float attackTime = ConstValue.SkillAttackTime * 500;
				f = await timer.WaitAsync((long)attackTime,token);
				if (!f) return;
                
                self.skill.PlayReleaseEffect();
                //hit delay
                f = await timer.WaitAsync((long)(self.skill.hitTimeList[0] * 1000), token);
                if (!f) return;

                self.skill.Hit(target);
                //action back time
                f = await timer.WaitAsync((long)attackTime + 100, token);
                if (!f) return;
                index++;
			}
            //back time
            self.skill.caster.SetPos(ClientBattle.Get().GetComponent<BattleSceneComponent>().GetPos(self.skill.caster.camp, self.skill.caster.posIndex));
            var backTime = ConstValue.SkillMeleeMoveTime;
            evtCom.Broadcast(new SkillActionBackMoveEvent() { skill = self.skill, time = backTime });
            f = await timer.WaitAsync((long)(backTime * 1000), token);
            if (!f) return;

            self.skill.OnSkillEnd();
		}
        public static async void HitAsync(this SkillAttackComponent self)
        {
            //hit delay
            var timer = TimerComponent.Instance;
            var targetList = self.skill.targetList;
            var f = await timer.WaitAsync((long)(self.skill.hitTimeList[0] * 1000), self.GetToken(1));
            if (!f) return;
            foreach (var v in targetList)
            {
                self.skill.Hit(v);
            }
        }


    }
}
