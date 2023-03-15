using FairyGUI;
using ET;
using System;
using UnityEngine;
using module.item.message;
using Frame;

public class PetCaptureFoodItemCom : UIBase
{
    public GTextField textName;
    public GTextField textItem;

    public int itemId;
    public Action<int> callback;
    public bool isEnough;
    public bool useItem;
}
class PetCaptureFoodItemComAwakeSystem : AwakeSystem<PetCaptureFoodItemCom, GComponent>
{
    protected override void Awake(PetCaptureFoodItemCom self, GComponent a)
	{
		self.Awake(a);
        self.textName = a.GetText("name");
        self.textItem = a.GetText("item");

        a.onClick.Add(self.ClickItem);

    }
}

public static class PetCaptureFoodItemComSystem
{
	public static void Set(this PetCaptureFoodItemCom self,int itemId,long remainCount,Action<int> clickCallback)
	{
        self.isEnough = true;
        self.useItem = false;
        self.itemId = itemId;
        self.callback = clickCallback;
        self.textName.text = ItemUtil.GetItemName(self.itemId);
        var cfg = ConfigSystem.Ins.GetItemConfig(self.itemId);
        var price = cfg.Get<int[]>("Price");
        if (price == null || price[1] == 0)
        {
            self.textItem.text = ConfigSystem.Ins.GetLanguage("uipet_btnfree");
            self.textItem.color = Color.green;
        }
        else
        {
            if (remainCount > 0)
            {
                self.useItem = true;
                self.textItem.text = string.Format(ConfigSystem.Ins.GetLanguage("uipet_remaintip"),remainCount);
            }
            else
            {
                self.isEnough = DataSystem.Ins.ItemModel.GetItemCount(price[0]) >= price[1];
                self.textItem.text = $"{ItemUtil.GetItemName(price[0])}*{price[1]}";
            }
            self.textItem.color = self.isEnough ? Color.white : Color.red;
        }
    }
    public static long GetRemainCount(this PetCaptureFoodItemCom self)
    {
        var count = DataSystem.Ins.ItemModel.GetItemCount(self.itemId);
        return count;
    }
    public static void ClickItem(this PetCaptureFoodItemCom self)
    {
        var cfg = ConfigSystem.Ins.GetItemConfig(self.itemId);
        var price = cfg.Get<int[]>("Price");
        if (!self.isEnough)
        {
            UISystem.Ins.ShowFlowTipView(string.Format(ConfigSystem.Ins.GetLanguage("common_not_enough_tip"), ItemUtil.GetItemName(price[0])));
            return;
        }
        if ( self.useItem || price[1] == 0)
        {
            self.callback?.Invoke(self.itemId);
            return;
        }
        var name = ItemUtil.GetItemName(price[0]);
        var str = string.Format(ConfigSystem.Ins.GetLanguage("uipet_buyitemtip"), price[1], name, 1, ItemUtil.GetItemName(self.itemId));
        UISystem.Ins.ShowCommonTipView(str,()=> {
            self.SendBuy();
        });
    }
    public static async void SendBuy(this PetCaptureFoodItemCom self)
    {
        var req = new QuickBuyItemReq() { itemId = self.itemId, count = 1 };
        var token = self.GetToken(1);
        var resp = (QuickBuyItemResp) await NetSystem.Call(req, typeof(QuickBuyItemResp), token);
        if (resp == null) return;
        self.callback?.Invoke(self.itemId);
    }
}