using ET;
using FairyGUI;
using GameEvent;
using System.Collections.Generic;
using UnityEngine;
using module.fabao.message;
using module.fabao.packet.impl;
using Frame;

public struct StageItem
{
	public GGraph bg;
	public GTextField text;
}
public class UIFaBaoDuanZao:UIBase
{
	public GTextField textScore;
	public GTextField textCountDown;
	public GTextField textScoreTime;
	public Controller cdCtrl;

	public List<StageItem> stageItemList = new List<StageItem>();
	public FaBaoCursorComponent cursorCom;
	public int fabaoBagIndex;
	public int stage;
	public int totalScore;
	public bool isWait;
	public int[] scoreList = new int[6];

	public const int TOKEN_SCORE_TIME = 1;
	public const int TOKEN_COMMIT_REQ = 2;
	public const int TOKEN_CD_TIME = 3;

	public string resultStr;//服务器乱码
	public int resultIndex;//服务器乱码插入

}
public class UIFaBaoDuanZaoAwakeSystem : AwakeSystem<UIFaBaoDuanZao, GComponent>
{
	protected override void Awake(UIFaBaoDuanZao self, GComponent a)
	{
		self.Awake(a);
		self.textScore = a.GetText("textScore");
		self.textCountDown = a.GetText("countdown");
		self.textScoreTime = a.GetText("scoreTime");
		self.cdCtrl = a.GetController("countDown");
		var btnSure = a.GetButton("btnSure");
		var btnClose = a.GetCom("btnClose");

		for(int i = 0; i < 6; i++)
		{
			var g = a.GetCom($"stage{i}");
			var bg = g.GetGraph("bg");
			var text = g.GetText("score");
			self.stageItemList.Add(new StageItem() { bg = bg, text = text });
		}
		self.cursorCom = self.AddChild<FaBaoCursorComponent, GComponent>(a.GetCom("cursor"));
		btnSure.onClick.Add(self.ClickSure);
		btnClose.onClick.Add(self.ClickClose);
	}
}
public class UIFaBaoDuanZaoChangeSystem : ChangeSystem<UIFaBaoDuanZao, IUIDataParam>
{
	protected override void Change(UIFaBaoDuanZao self, IUIDataParam a)
	{
		self.fabaoBagIndex = ((UIFaBaoDuanZaoParam)a).index;
		self.cdCtrl.selectedIndex = 1;
		var info = DataSystem.Ins.ItemModel.GetFabao(self.fabaoBagIndex);
		self.InitStageItems(info.gotScores);
		self.StageStart(self.stage);
	}
}
public static class UIFaBaoDuanZaoSystem
{
	
	public static void InitStageItems(this UIFaBaoDuanZao self,List<int> scoreList)
	{
		if (scoreList != null)
		{
			for (int i = 0; i < scoreList.Count; i++)
			{
				self.scoreList[i] = scoreList[i];
			}
			self.stage = scoreList.Count;
		}
		self.totalScore = 0;
		for (int i = 0; i < self.stageItemList.Count; i++)
		{
			int score = self.scoreList[i];
			self.totalScore += score;
			self.stageItemList[i].bg.color = self.cursorCom.GetColor(score);
			self.stageItemList[i].text.text = score == 0 ? "" :  string.Format( ConfigSystem.Ins.GetLanguage("ui_fabao_score"),score);
		}
		self.textScore.text = self.totalScore.ToString();
	}
	public static void StageStart(this UIFaBaoDuanZao self,int stage)
	{
		self.stage = stage;
		self.StartCountDown().Coroutine();
	}
	public static async ETTask StartCountDown(this UIFaBaoDuanZao self)
	{
		var token = self.GetToken(UIFaBaoDuanZao.TOKEN_CD_TIME);
		self.cdCtrl.selectedIndex = 1;
		self.textCountDown.text = "3";
		var flag = await TimerComponent.Instance.WaitAsync(1000, token);
		if (!flag) return;
		self.textCountDown.text = "2";
		flag = await TimerComponent.Instance.WaitAsync(1000, token);
		if (!flag) return;
		self.textCountDown.text = "1";
		flag = await TimerComponent.Instance.WaitAsync(1000, token);
		if (!flag) return;
		await self.SendStartReq();
	}
	public static async ETTask StartScoreTime(this UIFaBaoDuanZao self)
	{
		self.cdCtrl.selectedIndex = 0;
		var info = DataSystem.Ins.ItemModel.GetFabao(self.fabaoBagIndex);
		var weapon = ConfigSystem.Ins.GetFBMagicWeaponConfig(info.modelId);
		var cfg = ConfigSystem.Ins.GetFBAuxiliaryConfig(weapon.Get("FBGrade"));
		int time = cfg.Get("Time");
		int speed = cfg.Get("Speed");
		var timeStr = ConfigSystem.Ins.GetLanguage("time_second");
		self.textScoreTime.text = string.Format(timeStr, time / 1000);
		self.cursorCom.RandomPos();
		self.cursorCom.StartCursor(speed);
		while (time > 0)
		{
			bool execute = await TimerComponent.Instance.WaitAsync(1000,self.GetToken(UIFaBaoDuanZao.TOKEN_SCORE_TIME));
			if (!execute) return;
			time -= 1000;
			time = time <= 0 ? 0 : time;
			self.textScoreTime.text = string.Format(timeStr, time / 1000);
			if (time == 0)
				self.ClickSure();
			else
				self.cursorCom.StartCursor(speed);
		}
	}
	public static void ClickSure(this UIFaBaoDuanZao self)
	{
		if (self.stage >= self.stageItemList.Count)
			return;
		if (self.isWait)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("ui_fabao_refine_tip")});
			return;
		}
		self.isWait = true;

		self.GetToken(UIFaBaoDuanZao.TOKEN_SCORE_TIME,false).Cancel();
		self.cursorCom.StopCursor();

		self.SendCommitReq(self.cursorCom.GetScore()).Coroutine();
	}
	public static void ClickClose(this UIFaBaoDuanZao self)
	{
		UISystem.Ins.Close(UIType.UIFaBaoDuanZao);
	}
	public static async ETTask SendStartReq(this UIFaBaoDuanZao self)
	{
		var resp = (FaBaoScoreStartResp)await NetSystem.Call(new FaBaoScoreStartReq() { packetIndex = self.fabaoBagIndex }, typeof(FaBaoScoreStartResp));
		if (resp == null) return;
		if (self.fabaoBagIndex != resp.packetIndex) return;
		self.resultStr = resp.resultStore;
		self.resultIndex = resp.storeIndex;
		self.InitStageItems(resp.gotScores);
		await self.StartScoreTime();
	}
	
	public static async ETTask SendCommitReq(this UIFaBaoDuanZao self,int score)
	{
		var token = self.GetToken(UIFaBaoDuanZao.TOKEN_COMMIT_REQ);
		int stage = self.stage;
		Log.Error(score.ToString());
		var str = self.resultStr.Substring(0,self.resultIndex+1) + score.ToString()+self.resultStr.Substring(self.resultIndex+1);
		var req = new FaBaoScoreCommitReq() { packetIndex = self.fabaoBagIndex ,results = str, step = stage};
		var resp = (FaBaoScoreResp)await NetSystem.Call(req, typeof(FaBaoScoreResp), token);
		if (resp == null) return;
		if (self.fabaoBagIndex != resp.index) return;
		self.InitStageItems(resp.gotScores);
		
		var flag = await TimerComponent.Instance.WaitAsync(500,token);
		if (!flag) return;
		self.isWait = false;
		if (self.stage == self.stageItemList.Count)
		{
			self.ClickClose();
            var info = DataSystem.Ins.ItemModel.GetFabao(self.fabaoBagIndex);
			var param = new UIFabaoDetailParam() { info = info };
			UISystem.Ins.Show(UIType.UIFabaoDetail,param);
			return;
		}
		self.StageStart(self.stage);
	}
}

