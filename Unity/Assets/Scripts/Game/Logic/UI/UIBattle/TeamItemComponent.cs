using FairyGUI;
using ET;
using TBS;
using System.Collections.Generic;
using UnityEngine;
using module.yw.message.vo;
using Frame;

public class TeamItemComponent : UIBase
{
	public GTextField textName;
	public GProgressBar hpBar;
	public Controller isEmptyCtrl;
	public List<GTextField> textBuffList = new List<GTextField>();
	public Transition attackTran;
	public Transition attackTran2;
	public Transition hitTran;

    public bool isDeadAnim;
	public Vector2 hpBarPos;
	public Actor actor;
}
class TeamItemComponentAwakeSystem : AwakeSystem<TeamItemComponent, GComponent>
{
	protected override void Awake(TeamItemComponent self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("name");
		self.hpBar = a.GetProgressBar("hpBar");
		self.isEmptyCtrl = a.GetController("isEmpty");
		self.attackTran = self.hpBar.GetTransition("attack");
		self.attackTran2 = self.hpBar.GetTransition("attack2");
		self.hitTran = self.hpBar.GetTransition("hit");
		for (int i = 0; i < 4; i++)
		{
			self.textBuffList.Add(a.GetText($"buff{i}"));
		}
		self.hpBarPos = self.hpBar.xy;
	}
}

public static class TeamItemComponentSystem
{
	public static void Set(this TeamItemComponent self,Actor actor)
	{
		self.isEmptyCtrl.selectedIndex = 0;
		self.actor = actor;
		self.textName.text = self.actor.name;
		self.RefreshBuff();
		self.RefreshHp();
	}
    public static void Set(this TeamItemComponent self, YwPlayerSimpleInfo player)
    {
        self.isEmptyCtrl.selectedIndex = 0;
        self.textName.text = player.name;
        self.RefreshBuff();
        self.RefreshHp();
    }
    public static void Set(this TeamItemComponent self, YwPetVo pet)
    {
        self.isEmptyCtrl.selectedIndex = 0;
        var cfg = ConfigSystem.Ins.GetLCPetAttribute(pet.petModelId);
        if (cfg == null) return;
        self.textName.text = cfg.Get<string>("Name");
        self.RefreshBuff();
        self.RefreshHp();
    }
    public static Vector2 GetPos(this TeamItemComponent self)
    {
        Vector2 p = self.hpBarPos;
        p = self.gCom.LocalToGlobal(p);
        return p;
    }
    public static void Reset(this TeamItemComponent self)
    {
        self.CancelToken(1);
        self.Empty();
        GTween.Kill(self.hpBar);
        self.gCom.AddChild(self.hpBar);
        self.hpBar.xy = self.hpBarPos;
        self.hpBar.visible = true;
    }
    
	public static void RefreshBuff(this TeamItemComponent self)
	{
        int index = 0;
        if (self.actor != null)
        {
            var buffList = self.actor.GetComponent<ActorBuffsComponent>().GetBuffList();
            foreach (var v in buffList)
            {
                Buffer b = v.Value as Buffer;
                var cfg = ConfigSystem.Ins.GetBuff(b.buffId);
                if (cfg == null || cfg.Get("BuffDisplay") == 0) continue;
                var name = cfg.Get<string>("BuffName");
                if (b.turnDuring == 0)
                    self.textBuffList[index++].text = $"[{name}]";
                else
                    self.textBuffList[index++].text = $"[{name}:{b.turnDuring}]";
                if (index >= self.textBuffList.Count)
                    break;
            }
        }
		for(int i = index;i< self.textBuffList.Count; i++)
		{
			self.textBuffList[i].text = "";
		}
	}
	
	public static void Empty(this TeamItemComponent self)
	{
		self.actor = null;
		self.isEmptyCtrl.selectedIndex = 1;
		self.hpBar.value = 0;
	}
	public static void ChangeHpBarParent(this TeamItemComponent self,bool isLocal)
	{
		if (isLocal)
		{
			GTween.Kill(self.hpBar);
			if (self.hpBar.parent == self.gCom)
				return;
			self.gCom.AddChild(self.hpBar);
			self.hpBar.xy = self.hpBarPos;
            self.hpBar.InvalidateBatchingState();
        }
		else
		{
			GTween.Kill(self.hpBar);
			var p = self.GetParent();
			if (self.hpBar.parent == p.gCom)
				return;
			p.AddGComponent(self.hpBar);
			self.hpBar.xy = p.GlobalToLocal(self.gCom.LocalToGlobal(self.hpBarPos));
            self.hpBar.InvalidateBatchingState();
        }
			
	}
	public static UIBattle GetParent(this TeamItemComponent self)
	{
		return self.Parent as UIBattle;
	}
	public static void PlayFrontAction(this TeamItemComponent self, Skill skill,Actor target, float time)
	{
		self.ChangeHpBarParent(false);
		var parent = self.GetParent();
		var hitPos = target.GetHitPos();
		self.hpBar.TweenMove(hitPos, ConstValue.SkillMeleeMoveTime - 0.05f);
	}
	
	public static void PlayAttackAction(this TeamItemComponent self, Skill skill)
	{
		GTween.Kill(self.hpBar);
		if (skill.caster.camp == ActorCampType.Blue)
		{
			self.attackTran2.Play();
		}
		else
		{
			self.attackTran.Play();
		}
	}
	public static async void PlayBackAction(this TeamItemComponent self,Skill skill,float time)
	{
		//回撤
		var token = self.GetToken(1);
		var backPos = self.GetParent().GlobalToLocal(self.gCom.LocalToGlobal(self.hpBarPos));
		self.hpBar.TweenMove(backPos, time);
		var flag = await TimerComponent.Instance.WaitAsync((long)(time*1000), token);
		if (!flag) return;
		self.ChangeHpBarParent(true);
	}
	public static void PlayHitAction(this TeamItemComponent self)
	{
		//GTween.Kill(self.hpBar);
		self.hitTran.Play();
	}
	//随机方向飞出场外，飞出场外后碰到屏幕边界会反弹1次再彻底飞出屏幕外
	public static async void PlayDeadAction(this TeamItemComponent self)
	{
        self.isDeadAnim = true;
        var token = self.GetToken(1);
		var flag = await TimerComponent.Instance.WaitAsync(400, token);
		if (!flag) return;
		float r = self.hpBar.width / 2;
		
		GTween.Kill(self.hpBar);
		self.ChangeHpBarParent(false);
		self.GetScreenBoardPos(out var pos,out var normal);
		int speed = 1000;
		var dir = pos - self.hpBar.xy;
		float time = dir.magnitude / speed;
		self.hpBar.TweenMove(pos, time).SetEase(EaseType.Linear);
		flag = await TimerComponent.Instance.WaitAsync((long)(time*1000), token);
		if (!flag) return;
		//反弹
		dir.Normalize();
		var reflect = Mathf.Abs(Vector2.Dot(dir, normal))*2* normal+ dir;
		reflect *= 10000;
		reflect = self.hpBar.xy + reflect;
		reflect.x = Mathf.Max(reflect.x, -r);
		reflect.x = Mathf.Min(reflect.x, ConstValue.ResolusionX);
		reflect.y = Mathf.Max(reflect.y, -2*r);
		reflect.y = Mathf.Min(reflect.y, ConstValue.ResolusionY);
		pos = reflect - self.hpBar.xy;
		time = pos.magnitude / speed;
		self.hpBar.TweenMove(reflect, time).SetEase(EaseType.Linear);
		flag = await TimerComponent.Instance.WaitAsync((long)(time * 1000), token);
		if (!flag) return;
		self.ChangeHpBarParent(true);
		self.hpBar.visible = false;
        self.isDeadAnim = false;
	}
	private static void GetScreenBoardPos(this TeamItemComponent self,out Vector2 boardPos,out Vector2 normal)
	{
		int width = Screen.width;
		int height = Screen.height;
		int x = Random.Range(100, width-100);
		int y = Random.Range(100, height - 100);
		bool top = Random.Range(0, 2) == 1;
		bool left = Random.Range(0, 2) == 1;
		if(top && left)
		{
			boardPos = new Vector2(0, y);//left
			normal = new Vector2(1, 0);
		}
		else if(top && !left)
		{
			boardPos = new Vector2(x, 0);//top
			normal = new Vector2(0, 1);
		}
		else if(!top && left)
		{
			boardPos = new Vector2(width, x);//right
			normal = new Vector2(-1, 0);
		}
		else
		{
			boardPos = new Vector2(x, height);//bottom
			normal = new Vector2(0,-1);
		}

		boardPos = self.GetParent().GlobalToLocal(boardPos);
		float r = self.hpBar.width / 2;
		boardPos.x = Mathf.Max(boardPos.x, r);
		boardPos.x = Mathf.Min(boardPos.x,ConstValue.ResolusionX-r);
		boardPos.y = Mathf.Max(boardPos.y, r);
		boardPos.y = Mathf.Min(boardPos.y, ConstValue.ResolusionY - r);
	}
	public static void RefreshHp(this TeamItemComponent self)
	{
        if(self.actor != null)
        {
            var com = self.actor.GetComponent<ActorAttrComponent>();
            var hp = com.GetHp();
            var max = com.GetHpMax();
            if (max == 0) max = 1;
            self.hpBar.value = (float)hp / max * 100;
            return;
        }
        self.hpBar.value = 100;
	}
}