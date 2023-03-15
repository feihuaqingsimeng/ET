using ET;
using FairyGUI;
using Frame;
using System;

public class IconEquipCom : UIBase
{

	public GTextField textName;
    public GTextField textType;
	public Controller selectCtrl;
    public GImage imgFrame;
    public Controller isEmptyCtrl;
    public Controller isEquipedCtrl;

	public int index;
	public EquipInfo info;
    public Action<IconEquipCom> clickCallback;
}

public class IconEquipComAwakeSystem : AwakeSystem<IconEquipCom, GComponent>
{
	protected override void Awake(IconEquipCom self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("title");
        self.textType = a.GetText("type");
		self.selectCtrl = a.GetController("isSelect");
        self.isEmptyCtrl = a.GetController("isEmpty");
        self.isEquipedCtrl = a.GetController("isEquiped");
        self.imgFrame = a.GetImage("frame");

    }
}
public static class IconEquipComSystem
{
    public static void Set(this IconEquipCom self, EquipInfo info, int index = -1)
    {
        self.isEmptyCtrl.selectedIndex = 0;
        self.index = index;
        self.info = info;
        self.textName.text = info.name;
        var color = ItemUtil.GetItemQualityColorByQ(info.quality);
        self.textName.color = color;
        self.imgFrame.color = color;
        self.isEquipedCtrl.selectedIndex = info.IsEquiped ? 1 : 0;
        self.textType.text = RewardInfoFactory.GetTypeStr(info.rewardType, info.modelId);
        self.gCom.onClick.Add(self.OnClick);
    }
    
	public static void SetSelect(this IconEquipCom self,bool isSelect)
	{
		self.selectCtrl.selectedIndex = isSelect ? 1 : 0;
	}
    public static void SetEmpty(this IconEquipCom self)
    {
        self.isEmptyCtrl.selectedIndex = 1;
    }
    public static void AddClick(this IconEquipCom self,Action<IconEquipCom> callback)
    {
        self.clickCallback = callback;
    }
    private static void OnClick(this IconEquipCom self)
    {
        if (self.isEmptyCtrl.selectedIndex == 1) return;
        if(self.clickCallback != null)
        {
            self.clickCallback.Invoke(self);
        }
        else
        {
            UISystem.Ins.Show(UIType.UIEquipDetail, new UIEquipDetailParam() { info = self.info });
        }
    }
}
