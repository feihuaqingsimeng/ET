using FairyGUI;
using ET;
using GameEvent;
using System;
using Frame;

public class UICostCommonTip : UIBase
{
	public GTextField textDes;
	public GTextField textCost;

	public Action sureCallback;
}
class UICostCommonTipAwakeSystem : AwakeSystem<UICostCommonTip, GComponent>
{
	protected override void Awake(UICostCommonTip self, GComponent a)
	{
		self.Awake(a);
		self.textDes = a.GetText("des");
		self.textCost = a.GetText("cost");

		var btnClose = a.GetButton("btnClose");
		var btnSure = a.GetButton("btnSure");
		var btnBg = a.GetChild("_bgfull");

		btnBg.onClick.Add(self.ClickClose);
		btnClose.onClick.Add(self.ClickClose);
		btnSure.onClick.Add(self.ClickSure);
	}
}
class UICostCommonTipChangeSystem : ChangeSystem<UICostCommonTip, IUIDataParam>
{
	protected override void Change(UICostCommonTip self, IUIDataParam a)
	{
		var param = (UICostCommonTipParam)a;
		self.textDes.text = param.des;
		self.textCost.text = param.costDes;
		self.sureCallback = param.sureCallback;
	}
}
public static class UICostCommonTipSystem
{
	public static void ClickClose(this UICostCommonTip self)
	{
		UISystem.Ins.Close(UIType.UICostCommonTip);
	}
	public static void ClickSure(this UICostCommonTip self)
	{
		var cb = self.sureCallback;
		self.sureCallback = null;
		self.ClickClose();
		cb?.Invoke();
	}
}