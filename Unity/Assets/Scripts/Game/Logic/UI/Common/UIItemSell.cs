using ET;
using FairyGUI;
using GameEvent;
using Frame;
using module.pack.message;

public class UIItemSell:UIBase
{
	public GTextField textName;
	public GTextField textSellPrice;
    public GTextField textSellPriceName;
	public GTextField textTotalNum;
    public GTextField textTotalNumName;
    public GSlider slider;

    public int price;
    public int priceId;
    public RewardInfo info;
}
public class UIItemSellAwakeSystem : AwakeSystem<UIItemSell, GComponent>
{
    protected override void Awake(UIItemSell self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("name");
		self.textSellPrice = a.GetText("sellPrice");
		self.textSellPriceName = a.GetText("sellName");
        self.textTotalNumName = a.GetText("totalNumName");
        self.textTotalNum = a.GetText("totalNum");
        self.slider = a.GetSlider("slider");

        var btnClose = a.GetCom("_bgfull");
		var btnSure = a.GetButton("btnSure");
        self.slider.onChanged.Add(self.SliderChanged);

		btnSure.onClick.Add(self.ClickSure);
		btnClose.onClick.Add(self.ClickClose);

	}
}
public class UIItemSellChangeSystem : ChangeSystem<UIItemSell,IUIDataParam>
{
	protected override void Change(UIItemSell self, IUIDataParam a)
	{
		var param = (UIItemSellParam)a ;
		self.info = param.info;
        var config = ConfigSystem.Ins.GetItemConfig(self.info.modelId);
        var price = config.Get<int[]>("Price");
        if(price!= null)
        {
            self.priceId = price[0];
            self.price = price[1];
        }
        self.Refresh();
	}
}
public static class UIItemSellSystem
{
	public static void Refresh(this UIItemSell self)
	{
		var data = DataSystem.Ins.ItemModel;
        self.textName.text = self.info.ColorName;
        var priceName = ItemUtil.GetItemName(self.priceId);
        self.textTotalNumName.text = priceName;
        self.textSellPriceName.text = priceName;
        self.textSellPrice.text = self.price.ToString();
        self.slider.max = self.info.num;
        self.slider.value = self.info.num;
        self.RefreshTotalNum();
    }
    public static void RefreshTotalNum(this UIItemSell self)
    {
        self.textTotalNum.text = (self.price * self.slider.value).ToString();
    }

    public static void SliderChanged(this UIItemSell self)
    {
        self.RefreshTotalNum();
    }

    public static void ClickClose(this UIItemSell self)
	{
		UISystem.Ins.Close(UIType.UIItemSell);
	}
	public static void ClickSure(this UIItemSell self)
	{
        var req = new ItemSellReq();
        if(self.info.rewardType == RewardType.ITEM)
        {
            var info = self.info as ItemInfo;
            req.packetIndex = (int)info.packType;
            req.num = (int)(self.slider.value);
        }
        NetSystem.Send(req);
        self.ClickClose();
	}
}

