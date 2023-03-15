using ET;
using FairyGUI;
using GameEvent;
using UnityEngine;
using Frame;

public class UIItemSelect:UIBase
{
	public GTextField textName;
	public GTextField textElement;
	public GTextField textDes;
	public GTextField textNum;
	public Controller btnStateCtrl;
	public GButton btnSure;

	public UIItemSelectParam param;

	public int selectNum = 1;
	public int[] opArr = new int[] { -100, -10, -1, 1, 10, 100 };
	public ListComponent<GButton> changeBtns;
}
public class UIItemSelectAwakeSystem : AwakeSystem<UIItemSelect, GComponent>
{
	protected override void Awake(UIItemSelect self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("name");
		self.textElement = a.GetText("textElement");
		self.textDes = a.GetText("des");
		self.textNum = a.GetText("textNum");
		self.btnStateCtrl = a.GetController("btnState");
		var btnClose = a.GetCom("_bgfull");

		self.btnSure = a.GetButton("btnSure");


		var poolCom = self.AddComponent<ListPoolComponent>();
		self.changeBtns = poolCom.Create<GButton>();
		for (int i = 0; i <= 5; i++)
		{
			var btn = a.GetButton($"btnChange{i}");
			self.changeBtns.Add(btn);
			btn.onClick.Add(self.ClickChangeNum);
		}
		self.btnSure.onClick.Add(self.ClickSure);
		btnClose.onClick.Add(self.ClickClose);
	}
}
public class UIItemSelectChangeSystem : ChangeSystem<UIItemSelect,IUIDataParam>
{
	protected override void Change(UIItemSelect self, IUIDataParam a)
	{
		self.param = (UIItemSelectParam)a ;
		self.selectNum = self.param.limit;
		self.Refresh();
	}
}
public static class UIItemSelectSystem
{
	public static void Refresh(this UIItemSelect self)
	{
		self.btnStateCtrl.selectedIndex = self.param.isAdd ? 0 : 1;
		var item = self.param.itemInfo;
		var config = ConfigSystem.Ins.GetItemConfig(item.modelId);
		if (config == null) return;
		int element = config.Get("Elements");
		self.textName.text = item.ColorName;
		self.textDes.text = item.des;
		self.textElement.text = string.Format(ConfigSystem.Ins.GetLanguage("element_tip"),
			ConfigSystem.Ins.GetLanguage($"element_{element}"));
		self.RefreshNum();
	}
	public static void RefreshNum(this UIItemSelect self)
	{
		self.textNum.text = $"{self.selectNum}/{self.param.limit}";
	}

	public static void ClickChangeNum(this UIItemSelect self, EventContext e)
	{
		int i = 0;
		for (; i < self.changeBtns.Count; i++)
		{
			if(self.changeBtns[i] == e.sender)
				break;
		}
		var op = self.opArr[i];
		self.selectNum += op;
		self.selectNum = Mathf.Min(self.selectNum, self.param.limit);
		if(self.param.isAdd)
			self.selectNum = Mathf.Max(1, self.selectNum);
		else
			self.selectNum = Mathf.Max(0, self.selectNum);
		self.RefreshNum();
	}

	public static void ClickClose(this UIItemSelect self)
	{
		UISystem.Ins.Close(UIType.UIItemSelect);
	}
	public static void ClickSure(this UIItemSelect self)
	{
		var cb = self.param.callback;
		self.ClickClose();
		cb?.Invoke(self.param.itemInfo, self.selectNum);
	}
}

