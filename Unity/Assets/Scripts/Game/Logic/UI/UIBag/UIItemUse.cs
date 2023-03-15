using ET;
using FairyGUI;
using GameEvent;
using UnityEngine;
using module.item.message;
using Frame;

public class UIItemUse:UIBase
{
	public GTextField textName;
	public GTextField textOwnNum;
	public GTextField textNum;

	public int index;
	public int selectNum = 1;
	public int limitNum = 0;
	public int[] opArr = new int[] { -10, -1, 1, 10 };
	public ListComponent<GButton> changeBtns;
}
public class UIItemUseAwakeSystem : AwakeSystem<UIItemUse, GComponent>
{
	protected override void Awake(UIItemUse self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("name");
		self.textOwnNum = a.GetText("ownNum");
		self.textNum = a.GetText("textNum");
		var btnClose = a.GetChild("_bgfull");
		var btnSure = a.GetButton("btnSure");


		var poolCom = self.AddComponent<ListPoolComponent>();
		self.changeBtns = poolCom.Create<GButton>();
		for (int i = 0; i <= 3; i++)
		{
			var btn = a.GetButton($"btnChange{i}");
			self.changeBtns.Add(btn);
			btn.onClick.Add(self.ClickChangeNum);
		}
		btnSure.onClick.Add(self.ClickSure);
		btnClose.onClick.Add(self.ClickClose);

		self.AddComponent<CancelTokenComponent>();
	}
}
public class UIItemUseChangeSystem : ChangeSystem<UIItemUse,IUIDataParam>
{
	protected override void Change(UIItemUse self, IUIDataParam a)
	{
		var param = (UIItemUseParam)a ;
		self.index = param.index;
		self.Refresh();
	}
}
public static class UIItemUseSystem
{
	public static void Refresh(this UIItemUse self)
	{
		var data = DataSystem.Ins.ItemModel;
		var info = data.GetItemBySlot(self.index);
		var config = ConfigSystem.Ins.GetItemConfig(info.modelId);
		if (config == null) return;
		self.textName.text = ItemUtil.GetItemName(info.modelId);
		var own = data.GetItemCount(info.modelId);
		self.textOwnNum.text = own.ToString();
		self.limitNum = Mathf.Min((int)own, config.Get("Superposition"));
		self.selectNum = self.limitNum;
		self.RefreshNum();
	}
	public static void RefreshNum(this UIItemUse self)
	{
		self.textNum.text = $"{self.selectNum}/{self.limitNum}";
	}

	public static void ClickChangeNum(this UIItemUse self, EventContext e)
	{
		int i = 0;
		for (; i < self.changeBtns.Count; i++)
		{
			if(self.changeBtns[i] == e.sender)
				break;
		}
		var op = self.opArr[i];
		self.selectNum += op;
		self.selectNum = Mathf.Min(self.selectNum, self.limitNum);
		self.selectNum = Mathf.Max(1, self.selectNum);
		self.RefreshNum();
	}

	public static void ClickClose(this UIItemUse self)
	{
		UISystem.Ins.Close(UIType.UIItemUse);
	}
	public static void ClickSure(this UIItemUse self)
	{
		var req = new ItemUseReq() { itemIndex = self.index, useNum = self.selectNum };
		NetSystem.Send(req);
		self.ClickClose();
	}
}

