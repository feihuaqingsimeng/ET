using ET;

using FairyGUI;
using Frame;
using System;

public class FabaoSkillItem: UIBase
{
	public GTextField textType;
	public GTextField textQuality;
	public GTextField textName;
	public Controller isSelectCtrl;
	public Controller isEquipedCtrl;
	
	public FabaoSkillItemData info;

	public Action<FabaoSkillItem> callback;

}

public class FabaoSkillItemAwakeSystem : AwakeSystem<FabaoSkillItem, GComponent>
{
	protected override void Awake(FabaoSkillItem self, GComponent a)
	{
		self.Awake(a);
		self.textType = a.GetText("type");
		self.textQuality = a.GetText("quality");
		self.textName = a.GetText("name");
		self.isEquipedCtrl = a.GetController("isEquiped");
		self.isSelectCtrl = a.GetController("isSelect");

		a.onClick.Add(self.ClickItem);
	}
}
public static class FabaoSkillItemSystem
{
	public static void Set(this FabaoSkillItem self, FabaoSkillItemData info)
	{
		self.info = info;
		self.Set(info.info, info.type);
	}
	public static void Set(this FabaoSkillItem self,FaBaoInfo info,FaBaoPackType type)
	{
		var cfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(info.modelId);
		var auxiliary = ConfigSystem.Ins.GetFBAuxiliaryConfig(cfg.Get("FBGrade"));
		self.textType.text = ConfigSystem.Ins.GetLanguage($"fabao_type_{cfg.Get("FBType")}");
		self.textName.text = string.Format(ConfigSystem.Ins.GetLanguage("fabao_stage"), info.stage) + cfg.Get<string>("FBName");
		self.textQuality.text = auxiliary.Get<string>("Name");
		self.SetEquiped(type == FaBaoPackType.EQUIP);
	}
	public static void SetEquiped(this FabaoSkillItem self, bool isEquiped)
	{
		self.isEquipedCtrl.selectedIndex = isEquiped ? 1 : 0;
	}
	public static void SetOnClick(this FabaoSkillItem self, Action<FabaoSkillItem> callback)
	{
		self.callback = callback;
	}
	public static void ClickItem(this FabaoSkillItem self)
	{
		self.callback?.Invoke(self);
	}
	public static void SetSelect(this FabaoSkillItem self, bool isSelect)
	{
		self.isSelectCtrl.selectedIndex = isSelect ? 1 : 0;
	}
	public static bool IsSelect(this FabaoSkillItem self)
	{
		return self.isSelectCtrl.selectedIndex == 1;
	}
}