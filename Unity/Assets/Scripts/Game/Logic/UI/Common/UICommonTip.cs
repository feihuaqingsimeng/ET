using ET;
using GameEvent;
using FairyGUI;
using Frame;
using System;

public class UICommonTip: UIBase
{
	public enum ButtonType
	{
		Double = 0,
		Single = 1,
	}
	public GTextField textName;
	public GTextField textDes;
	public Controller btnStateCtrl;

	public string des;
	public ButtonType buttonType;
	public Action callback;
}
public class UICommonTipAwakeSystem : AwakeSystem<UICommonTip, GComponent>
{
	protected override void Awake(UICommonTip self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("name");
		self.textDes = a.GetText("des");
		self.btnStateCtrl = a.GetController("btnState");
		var btnSure = a.GetButton("btnSure");
		var btnClose = a.GetButton("btnClose");

		btnSure.onClick.Add(self.ClickSure);
		btnClose.onClick.Add(self.ClickClose);
	}
}
public class UICommonTipChangeSystem : ChangeSystem<UICommonTip, IUIDataParam>
{
	protected override void Change(UICommonTip self, IUIDataParam a)
	{
		var param = (UICommonTipParam)a;
		self.des = param.des;
		self.buttonType = param.type;
		self.callback = param.sureCallback;
		self.Refresh();
	}
}
public static class UICommonTipSystem
{
	public static void Refresh(this UICommonTip self)
	{
		self.textDes.text = self.des;
		self.textName.text = "";
		self.btnStateCtrl.selectedIndex = self.buttonType == UICommonTip.ButtonType.Single ? 0 : 1;
	}
	public static void ClickSure(this UICommonTip self)
	{
		var callback = self.callback;
		self.callback = null;
		self.ClickClose();
		callback?.Invoke();
	}
	public static void ClickClose(this UICommonTip self)
	{
		UISystem.Ins.Close(UIType.UICommonTip);
	}
}