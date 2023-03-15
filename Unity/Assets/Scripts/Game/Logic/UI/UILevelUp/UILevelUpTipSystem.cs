using ET;
using GameEvent;
using FairyGUI;
using module.item.message;
using module.level.message;
using module.pack.message;
using Frame;
public class UILevelUpTipAwakeSystem : AwakeSystem<UILevelUpTip, GComponent>
{
	protected override void Awake(UILevelUpTip self, GComponent a)
	{
		self.Awake(a);
		self.textRate = a.GetText("textRate");
		self.textContent = a.GetText("content");
		self.textMat = a.GetText("textMat");
		self.btnLevelUp = a.GetButton("btnSure");
		self.btnUse = a.GetButton("btnUse");
		self.btnCancel = a.GetButton("btnCancel");
		var btnClose = a.GetCom("btnClose");

		self.btnLevelUp.onClick.Add(self.ClickLevelUp);
		self.btnUse.onClick.Add(self.ClickUse);
		btnClose.onClick.Add(self.ClickCancel);
		self.btnCancel.onClick.Add(self.ClickCancel);

		self.Refresh();

        self.RegisterEvent<ItemChangeEvent>(self.Event_ItemChange);
	}
}
public static class UILevelUpTipSystem
{
	public static void Refresh(this UILevelUpTip self)
	{
		var data = DataSystem.Ins;
        var playerModel = data.PlayerModel;
		var config = ConfigSystem.Ins.GetLvConfig(playerModel.level);
		self.useItemId = config.Get("SuccessRateDrug");

		int rate = config.Get("SuccessRate");
		if (playerModel.failCount > 0)
			rate += playerModel.failCount * ConfigSystem.Ins.GetGlobal("FailSuccessRate");
		rate += playerModel.probAdd;

		self.sucRate = rate;
		self.textRate.text = $"{rate/100}%";
		self.textContent.text = "";
		self.textMat.text = "";
		if (self.sucRate < 10000)
		{
			var tip = ConfigSystem.Ins.GetLanguage("uilevelup_tip");
			var tip2 = ConfigSystem.Ins.GetGlobal("FailLossExp") / 100;
			self.textContent.text = string.Format(tip, tip2);
		}

		self.btnLevelUp.grayed = !playerModel.CheckExpEnough();
		//refresh mat
		self.textMat.text = $"{ItemUtil.GetItemName(self.useItemId)}*{data.ItemModel.GetItemCount(self.useItemId)}";
	}
	public static bool CheckExpEnough(this UILevelUpTip self)
	{
		return DataSystem.Ins.PlayerModel.CheckExpEnough();
	}
	public static void ClickLevelUp(this UILevelUpTip self)
	{
		if (!self.CheckExpEnough())
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("exp_not_enough") });
			return;
		}
		self.SendLevelUpAsync().Coroutine();
	}
	public static async ETTask SendLevelUpAsync(this UILevelUpTip self)
	{
		int level = DataSystem.Ins.PlayerModel.level;
		var resp = (LevelUpResp)await NetSystem.Call(new LevelUpReq(), typeof(LevelUpResp), self.GetToken(1));
		if (resp == null) {
			return;
		}
		
		string tip;
		if (resp.newLv == 0)
			tip = ConfigSystem.Ins.GetLanguage("uilevelup_fail");
		else
			tip = ConfigSystem.Ins.GetLanguage("uilevelup_success");
		UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = tip });
		self.Refresh();
	}
	
	public static void ClickUse(this UILevelUpTip self)
	{
		if (self.sucRate >= 10000)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("uilevelup_rate_max") });
			return;
		}
		var data = DataSystem.Ins.ItemModel;
		long count = data.GetItemCount(self.useItemId);
		if (count == 0)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("uilevelup_mat_not_enough")});
			return;
		}
		self.SendUseAsync(data.GetItem(self.useItemId)).Coroutine();
	}

	public static async ETTask SendUseAsync(this UILevelUpTip self,ItemInfo item)
	{
		var req = new ItemUseReq() { itemIndex = (int)item.uid, useNum = 1 };
		var resp = (ItemChangeResp)await NetSystem.Call(req, typeof(ItemChangeResp), self.GetToken(2));
		if (resp == null)
			return;
		if (resp.tip)
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("uilevelup_use_mat")});
		self.Refresh();
	}
	public static void ClickCancel(this UILevelUpTip self)
	{
		UISystem.Ins.Close(UIType.UILevelUpTip);
	}

	public static void Event_ItemChange(this UILevelUpTip self,ItemChangeEvent e)
	{
		self.Refresh();
	}
}
