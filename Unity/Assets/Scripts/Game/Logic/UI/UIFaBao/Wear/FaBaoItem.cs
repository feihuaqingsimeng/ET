using ET;
using FairyGUI;
using Frame;
using System;

public class FaBaoItem : UIBase
{
	public GTextField textType;
	public GTextField textQuality;
	public GTextField textName;
	public Controller isNewCtrl;
	public Controller isSelectCtrl;

	public FaBaoInfo info;
	public int index;
	public Action<FaBaoItem> callback;
}
public class FaBaoItemAwakeSystem : AwakeSystem<FaBaoItem, GComponent>
{
	protected override void Awake(FaBaoItem self, GComponent a)
	{
		self.Awake(a);
		self.textType = a.GetText("type");
		self.textQuality = a.GetText("quality");
		self.textName = a.GetText("name");
		self.isNewCtrl = a.GetController("isNew");
		self.isSelectCtrl = a.GetController("isSelect");

		a.onClick.Add(self.ClickItem);
	}
}
public static class FaBaoItemSystem
{
	public static void Set(this FaBaoItem self, FaBaoInfo info, int index = 0)
	{
		self.index = index;
		self.info = info;
		var cfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(info.modelId);
		var auxiliary = ConfigSystem.Ins.GetFBAuxiliaryConfig(cfg.Get("FBGrade"));
		self.textType.text = ConfigSystem.Ins.GetLanguage($"fabao_type_{cfg.Get("FBType")}");
		self.textName.text = string.Format(ConfigSystem.Ins.GetLanguage("fabao_stage"), info.stage) + cfg.Get<string>("FBName");
		self.textQuality.text = auxiliary.Get<string>("Name");
	}
	public static void SetOnClick(this FaBaoItem self, Action<FaBaoItem> callback)
	{
		self.callback = callback;
	}
	public static void ClickItem(this FaBaoItem self)
	{
		self.isNewCtrl.selectedIndex = 0;
		self.callback?.Invoke(self);
	}
	public static void SetSelect(this FaBaoItem self,bool isSelect)
	{
		self.isSelectCtrl.selectedIndex = isSelect ? 1 : 0;
	}
	public static bool IsSelect(this FaBaoItem self)
	{
		return self.isSelectCtrl.selectedIndex == 1;
	}
	public static void SetPos(this FaBaoItem self,float x,float y)
	{
		self.gCom.SetXY(x, y);
	}
}
