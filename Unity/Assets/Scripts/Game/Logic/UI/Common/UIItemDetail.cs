using ET;
using FairyGUI;
using GameEvent;
using module.pack.model;
using module.pack.message;
using module.item.message;
using System;
using Frame;

public class UIItemDetail: UIBase
{
    //
    public enum ItemDetailShowType
    {
        None,
        Normal,
        //仓库
        Store,
        //背包
        Bag,
        //炼丹
        LianDan,
    }
    public GTextField textName;
	public GTextField textType;
	public GTextField textDes;
	public GTextField textElement;
	public Controller canUseCtrl;
	public Controller canSellCtrl;
	public Controller canPutCtrl;

	public ItemDetailShowType packType;
    public ItemInfo info;
    public Action<ItemInfo> putCallback;
}
public class UIItemDetailAwakeSystem : AwakeSystem<UIItemDetail, GComponent>
{
	protected override void Awake(UIItemDetail self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("name");
		self.textType = a.GetText("type");
		self.textDes = a.GetText("des");
		self.textElement = a.GetText("element");
		self.canUseCtrl = a.GetController("canUse");
		self.canSellCtrl = a.GetController("canSell");
		self.canPutCtrl = a.GetController("canPut");

		var btnClose = a.GetChild("_bgfull");
		var btnUse = a.GetButton("btnUse");
		var btnSell = a.GetButton("btnSell");
		var btnGet = a.GetButton("btnGet");
        var btnPut = a.GetButton("btnPut");

		btnClose.onClick.Add(self.CloseSelf);
		btnUse.onClick.Add(self.ClickUse);
		btnSell.onClick.Add(self.ClickSell);
		btnGet.onClick.Add(self.ClickGet);
        btnPut.onClick.Add(self.ClickPut);

	}
}
public class UIItemDetailChangeSystem : ChangeSystem<UIItemDetail, IUIDataParam>
{
	protected override void Change(UIItemDetail self, IUIDataParam a)
	{
		var p = (UIItemDetailParam)a;
		self.packType = p.type;
		self.info = p.info;
        self.putCallback = p.putCallback;

        self.Refresh();
	}
}
public static class UIItemDetailSystem
{
	public static void Refresh(this UIItemDetail self)
	{
        var info = self.info;
		var cfg = ConfigSystem.Ins.GetItemConfig(info.modelId);
		self.textName.text = ItemUtil.GetItemName(info.modelId);
        self.textName.color = ItemUtil.GetItemQualityColorByQ(info.quality);
		self.textType.text = cfg.Get<string>("PropTypeDescribe");
		self.textElement.text = ConfigSystem.Ins.GetLanguage($"element_{cfg.Get("Elements")}");
		self.textDes.text = ItemUtil.GetItemDes(info.modelId);
		
		if (self.packType == UIItemDetail.ItemDetailShowType.Store)
		{
			self.canPutCtrl.selectedIndex = 2;
        }
        else if(self.packType == UIItemDetail.ItemDetailShowType.Bag)
        {
            self.canPutCtrl.selectedIndex = 1;
            self.canUseCtrl.selectedIndex = cfg.Get("ActiveUse") == 1 ? 1 : 0;
            self.canSellCtrl.selectedIndex = cfg.Get<int[]>("Price") == null ? 0 : 1;
        }else if(self.packType == UIItemDetail.ItemDetailShowType.LianDan)
        {
            self.canPutCtrl.selectedIndex = 1;
        }
	}
	
	public static void ClickUse(this UIItemDetail self)
	{
		var cfg = ConfigSystem.Ins.GetItemConfig(self.info.modelId);

		if(cfg.Get("BatchUse") == 1)
		{
			UISystem.Ins.Show(UIType.UIItemUse, new UIItemUseParam() { index = (int)self.info.uid });
		}
		else
		{
			var req = new ItemUseReq() { itemIndex = (int)self.info.uid, useNum = 1 };
			NetSystem.Send(req);
		}
        self.CloseSelf();
    }
	public static void ClickSell(this UIItemDetail self)
	{
		UISystem.Ins.Show(UIType.UIItemSell, new UIItemSellParam() { info = self.info });
        self.CloseSelf();
    }
	public static void ClickGet(this UIItemDetail self)
	{
		var size = ConfigSystem.Ins.GetGlobal("KnapsackUpper");
		var list = DataSystem.Ins.ItemModel.GetItems();
		if (list != null && list.Count >= size)
		{
			//背包已满
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content =ConfigSystem.Ins.GetErrorCode(8) });
			return;
		}
		var req = new PackStoreReq() { reqType = 1 };
		req.indexs.Add((int)self.info.uid);
		NetSystem.Send(req);
        self.CloseSelf();

    }
    public static void ClickPut(this UIItemDetail self)
    {
        self.putCallback?.Invoke(self.info);
        self.CloseSelf();
    }
}