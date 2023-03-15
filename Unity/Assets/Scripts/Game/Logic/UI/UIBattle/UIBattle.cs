using FairyGUI;
using ET;
using TBS;
using System.Collections.Generic;
using UnityEngine;
using module.fabao.packet.impl;
using module.battle.domain;
using System;
using module.yw.message.vo;
using Frame;

public class UIBattle : UIBase
{
	public GTextField textTurn;
	public TeamItemComponent[][] teamItemList = new TeamItemComponent[2][];
	public List<SkillItemComponent> skillItemList = new List<SkillItemComponent>();
    public GButton btnBattle;
    public GComponent sceneRoot;

    public List<Vector2> bluePosList = new List<Vector2>();
    public List<Vector2> redPosList = new List<Vector2>();

    public BattleRecord record;
    public Action<bool> battleFinishCallback;

}
public class UIBattleAwakeSystem : AwakeSystem<UIBattle,GComponent>
{
    protected override void Awake(UIBattle self, GComponent a)
    {
        self.OnAwake(a);
    }
}

public static class UIBattleSystem
{
	public static void RegBattleEvent(this UIBattle self)
	{
		var com = self.GetComponent<BattleEventAutoDestroyComponent>();
        if (com != null) return;
        com = self.AddComponent<BattleEventAutoDestroyComponent>();
        com.Register<BattleCmdParseFinished>(self.Event_BattleCmdParseFinished);
        com.Register<BattleStartEvent>(self.Event_BattleStartEvent);
        com.Register<BattleFinishedEvent>(self.Event_BattleFinishedEvent);

        com.Register<BuffChangeEvent>(self.Event_BuffChangeEvent);
        com.Register<BuffDestroyEvent>(self.Event_BuffDestroyEvent);

        com.Register<SkillActionFrontMoveEvent>(self.Event_SkillActionFrontMoveEvent);
        com.Register<SkillActionAttackPlayEvent>(self.Event_SkillActionAttackPlayEvent);
        com.Register<SkillActionHitPlayEvent>(self.Event_SkillActionHitPlayEvent);
        com.Register<SkillActionBackMoveEvent>(self.Event_SkillActionBackMoveEvent);

        com.Register<PlayerHpChangedEvent>(self.Event_PlayerHpChangedEvent);
        com.Register<DamageShowEvent>(self.Event_DamageShowEvent);
        com.Register<BattleTurnStartEvent>(self.Event_BattleTurnStartEvent);
        com.Register<ActorDeadEvent>(self.Event_ActorDeadEvent);



    }

    public static void OnAwake(this UIBattle self, GComponent a)
    {
        self.Awake(a);
        self.textTurn = a.GetText("turn");
        var enemy = a.GetCom("enemy");
        var our = a.GetCom("our");
        var skill = a.GetCom("skill");
       

        self.sceneRoot = a.GetCom("effectShow");

        self.teamItemList[0] = new TeamItemComponent[6];
        self.teamItemList[1] = new TeamItemComponent[6];
        for (int i = 0; i < 6; i++)
        {
            self.teamItemList[0][i] = self.AddChild<TeamItemComponent, GComponent>(our.GetCom($"item{i}"));
            self.teamItemList[1][i] = self.AddChild<TeamItemComponent, GComponent>(enemy.GetCom($"item{i}"));
            self.skillItemList.Add(self.AddChild<SkillItemComponent, GComponent>(skill.GetCom($"skill{i}")));
            self.bluePosList.Add(self.GlobalToLocal(self.teamItemList[0][i].GetPos()));
            self.redPosList.Add(self.GlobalToLocal(self.teamItemList[1][i].GetPos()));

        }
        self.AddComponent<UIBattleGMItemComp, GComponent>(a.GetCom("GM"));
        self.AddComponent<EffectShowItemComponent, GComponent>(a.GetCom("effectShow"));

        self.ResetTurn();
    }
    public static void Init(this UIBattle self,Action<bool> battleFinishCallback)
    {
        self.battleFinishCallback = battleFinishCallback;
    }
    public static void InitOurDefaultTeam(this UIBattle self)
    {
        //我方
        var teamList = self.teamItemList[0];
        foreach(var v in DataSystem.Ins.PetModel.battlePets)
        {
            int slot = v.Key;
            var info = DataSystem.Ins.PetModel.GetPet(v.Value);
            teamList[slot-1].Set(new YwPetVo() { petModelId = info.modelId, site = slot });
        }
        teamList[ConstValue.PLAYER_SITE].Set(new YwPlayerSimpleInfo() { name = DataSystem.Ins.PlayerModel.name });
        self.InitFabaoSkill();
    }
    public static void RequestReport(this UIBattle self,BattleRecord record)
	{
        self.record = record;
        self.RequestReport(record.reportPath);
	}
    public static async void RequestReport(this UIBattle self, string reportPath)
    {
        var token = self.GetToken(1);
        var request = self.AddComponent<UnityWebRequestAsync>();
        var b = await request.DownloadAsync(ConstValue.GetBattleReportUrl(reportPath), token);
        if (!b)
        {
            request.Dispose();
            return;
        } 
        if (request.Request.downloadHandler == null) return;
        var text = request.Request.downloadHandler.text;
        Log.Info(text);
        request.Dispose();
        var bat = ClientBattle.Get();
        if (bat != null)
            bat.Destroy();
        bat = ClientBattle.Create(self);
        bat.GetComponent<BattleSceneComponent>().Init(self.sceneRoot, self.bluePosList, self.redPosList);
        self.RemoveComponent<BattleEventAutoDestroyComponent>();
        self.RegBattleEvent();
        ClientBattle.Get().Start(text);
    }
    
	private static void InitCmdTeam(this UIBattle self)
	{
		var actorCom = ClientBattle.Get().GetComponent<BattleActorsComponent>();
		var list = actorCom.GetActorList();
        self.ResetTeamItem();

        foreach (var v in list)
		{
			self.GetTeamItem(v).Set(v);
		}
		var data = DataSystem.Ins.ItemModel;
		var fabaoList = data.GetFabaoList(FaBaoPackType.EQUIP);
        if(fabaoList == null)
        {
            for (int i = 0; i < self.skillItemList.Count; i++)
            {
                self.skillItemList[i].Empty();
            }
        }
        else
        {
            for (int i = 0; i < self.skillItemList.Count; i++)
            {
                fabaoList.TryGetValue(i + 1, out var info);
                self.skillItemList[i].Set(info);
            }
            self.Event_BattleTurnStartEvent(new BattleTurnStartEvent() { turn = 1 });
        }
	}
    public static void ResetTeamItem(this UIBattle self)
    {
        for (int i = 0; i < self.teamItemList.Length; i++)
        {
            for (int j = 0; j < self.teamItemList[i].Length; j++)
            {
                self.teamItemList[i][j].Reset();
            }
        }
    }
    public static void ResetTurn(this UIBattle self)
    {
        self.textTurn.text = string.Format(ConfigSystem.Ins.GetLanguage("battle_turn_tip"),1);
    }
    public static void InitFabaoSkill(this UIBattle self)
    {
        var data = DataSystem.Ins.ItemModel;
        var fabaoList = data.GetFabaoList(FaBaoPackType.EQUIP);
        if (fabaoList == null)
        {
            for (int i = 0; i < self.skillItemList.Count; i++)
            {
                self.skillItemList[i].Empty();
            }
        }
        else
        {
            for (int i = 0; i < self.skillItemList.Count; i++)
            {
                fabaoList.TryGetValue(i + 1, out var info);
                self.skillItemList[i].Set(info);
            }
        }
    }
	public static void AddGComponent(this UIBattle self, GComponent com)
	{
        int index = self.gCom.GetChildIndex(self.GetComponent<EffectShowItemComponent>().gCom);
		self.gCom.AddChildAt(com,index);
	}
	public static Vector2 GlobalToLocal(this UIBattle self, Vector2 globalPos)
	{
		return self.gCom.GlobalToLocal(globalPos);
	}
    
	public static TeamItemComponent GetTeamItem(this UIBattle self,Actor actor)
	{
		return self.teamItemList[(int)actor.camp][actor.posIndex];
	}
    public static bool IsDeadAnimFinish(this UIBattle self)
    {
        
        for (int i = 0; i < self.teamItemList.Length; i++)
        {
            for (int j = 0; j < self.teamItemList[i].Length; j++)
            {
                if (self.teamItemList[i][j].isDeadAnim)
                    return false;
            }
        }
        return true;
    }
    public static void Event_BattleCmdParseFinished(this UIBattle self, BattleCmdParseFinished e)
	{
		self.InitCmdTeam();
	}
	public static void Event_BattleStartEvent(this UIBattle self,BattleStartEvent e)
	{
		
	}
    
    public static void Event_BattleFinishedEvent(this UIBattle self,BattleFinishedEvent e)
    {
        self.battleFinishCallback?.Invoke(e.isWin);
    }

    public static void Event_BuffChangeEvent(this UIBattle self , BuffChangeEvent e)
	{
		var actor = e.buff.target;
		self.GetTeamItem(actor).RefreshBuff();
	}
	public static void Event_BuffDestroyEvent(this UIBattle self, BuffDestroyEvent e)
	{
		var actor = e.buff.target;
		self.GetTeamItem(actor).RefreshBuff();
	}
	public static void Event_SkillActionFrontMoveEvent(this UIBattle self, SkillActionFrontMoveEvent e)
	{
		var item = self.GetTeamItem(e.skill.caster);
		item.PlayFrontAction(e.skill, e.target, e.time);
	}
	public static void Event_SkillActionAttackPlayEvent(this UIBattle self, SkillActionAttackPlayEvent e)
	{
		var item = self.GetTeamItem(e.skill.caster);
		item.PlayAttackAction(e.skill);
	}
	public static void Event_SkillActionBackMoveEvent(this UIBattle self, SkillActionBackMoveEvent e)
	{
		var item = self.GetTeamItem(e.skill.caster);
		item.PlayBackAction(e.skill,e.time);
	}
	public static void Event_SkillActionHitPlayEvent(this UIBattle self, SkillActionHitPlayEvent e)
	{
		var item = self.GetTeamItem(e.target);
		item.PlayHitAction();
	}
	public static void Event_PlayerHpChangedEvent(this UIBattle self, PlayerHpChangedEvent e)
	{
		self.GetTeamItem(e.actor).RefreshHp();
	}
	public static void Event_DamageShowEvent(this UIBattle self, DamageShowEvent e)
	{
		self.GetComponent<EffectShowItemComponent>().ShowDamage(e.damage, e.damageType, e.isCrit, e.actor.GetDamageShowPos());
	}
	public static void Event_BattleTurnStartEvent(this UIBattle self,BattleTurnStartEvent e)
	{
		self.textTurn.text = string.Format(ConfigSystem.Ins.GetLanguage("battle_turn_tip"), e.turn);
    }
	public static void Event_ActorDeadEvent(this UIBattle self, ActorDeadEvent e)
	{
		self.GetTeamItem(e.actor).PlayDeadAction();
	}


}