using FairyGUI;
using ET;
using GameEvent;
using Frame;
using UnityEngine;

public class EquipWearSlotCom : UIBase
{
    public GImage imgFrame;
    public GTextField textTitle;
    public Controller isEmptyCtrl;

    public EquipInfo info;
}
class EquipWearSlotComAwakeSystem : AwakeSystem<EquipWearSlotCom, GComponent>
{
    protected override void Awake(EquipWearSlotCom self, GComponent a)
	{
		self.Awake(a);
        self.imgFrame = a.GetImage("frame");
        self.textTitle = a.GetText("title");
        self.isEmptyCtrl = a.GetController("isEmpty");

        self.gCom.onClick.Add(self.ClickItem);
    }
}
public static class EquipWearSlotComSystem
{
	public static void Set(this EquipWearSlotCom self,EquipInfo info)
	{
        self.info = info;
        self.textTitle.text = info.name;
        var color = ItemUtil.GetItemQualityColorByQ(info.quality);
        self.textTitle.color = color;
        self.imgFrame.color = color;
        self.isEmptyCtrl.selectedIndex = 0;
	}
    public static void Empty(this EquipWearSlotCom self,EquipType type)
    {
        self.info = null;
        self.isEmptyCtrl.selectedIndex = 1;
        self.imgFrame.color = Color.white;
        self.textTitle.text = ConfigSystem.Ins.GetLanguage($"equip_type_{(int)type}");
    }
    public static void ClickItem(this EquipWearSlotCom self)
    {
        if (self.info == null) return;
        UISystem.Ins.Show(UIType.UIEquipDetail, new UIEquipDetailParam() { info = self.info });
    }
}