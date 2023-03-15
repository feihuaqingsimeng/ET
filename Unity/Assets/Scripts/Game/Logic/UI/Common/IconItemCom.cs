using ET;
using FairyGUI;
using Frame;
using System;

public class IconItemCom : UIBase
{

	public GTextField textNum;
	public GTextField textName;
    public GTextField textType;
	public Controller selectCtrl;
    public GImage imgFrame;
    public Controller IsEmptyCtrl;

	public int index;
	public RewardInfo info;
    public Action<IconItemCom> clickCallback;
    public UIItemDetail.ItemDetailShowType showType = UIItemDetail.ItemDetailShowType.None;
}

public class PropItemComponentAwakeSystem : AwakeSystem<IconItemCom, GComponent>
{
	protected override void Awake(IconItemCom self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("title");
		self.textNum = a.GetText("num");
        self.textType = a.GetText("type");
		self.selectCtrl = a.GetController("isSelect");
        self.IsEmptyCtrl = a.GetController("isEmpty");
        self.imgFrame = a.GetImage("frame");

    }
}
public static class PropItemComponentSystem
{
    public static void Set(this IconItemCom self,RewardInfo info, int index = -1)
    {
        self.IsEmptyCtrl.selectedIndex = 0;
        self.index = index;
        self.info = info;
        self.textName.text = info.name;
        var color = ItemUtil.GetItemQualityColorByQ(info.quality);
        self.textName.color = color;
        self.imgFrame.color = color;
        self.textNum.text = $"x{info.num.ToString()}";
        self.textType.text = RewardInfoFactory.GetTypeStr(info.rewardType, info.modelId);
    }
    
	public static void SetSelect(this IconItemCom self,bool isSelect)
	{
		self.selectCtrl.selectedIndex = isSelect ? 1 : 0;
	}
    public static void SetEmpty(this IconItemCom self)
    {
        self.IsEmptyCtrl.selectedIndex = 1;
    }
    public static void ShowNum(this IconItemCom self,bool isShow)
    {
        self.textNum.visible = isShow;
    }
    public static void AddClick(this IconItemCom self,Action<IconItemCom> callback)
    {
        self.clickCallback = callback;
        self.gCom.onClick.Add(self.OnClick);
    }
    private static void OnClick(this IconItemCom self)
    {
        self.clickCallback?.Invoke(self);
    }
}
