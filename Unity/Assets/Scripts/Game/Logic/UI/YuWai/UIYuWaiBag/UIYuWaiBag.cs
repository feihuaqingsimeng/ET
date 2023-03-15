using FairyGUI;
using ET;
using GameEvent;
using module.yw.message;
using UnityEngine;
using Frame;

public class UIYuWaiBag : UIBase
{
	public GList itemlist;

	public int bagLimitCount;
}
class UIYuWaiBagAwakeSystem : AwakeSystem<UIYuWaiBag, GComponent>
{
	protected override void Awake(UIYuWaiBag self, GComponent a)
	{
		self.Awake(a);
		self.itemlist = a.GetList("list");

		var btnClose = a.GetButton("btnClose");

		self.itemlist.itemRenderer = self.ItemRender;
		self.itemlist.SetVirtual();

		btnClose.onClick.Add(self.ClickClose);

		self.Refresh();
	}
}

public static class UIYuWaiBagSystem
{
	public static void Refresh(this UIYuWaiBag self)
	{
		var initCount = ConfigSystem.Ins.GetGlobal("YWWarehouse");
		var data = DataSystem.Ins.YuWaiModel;
		var max = ConfigSystem.Ins.GetGlobal("YWWarehouseUP");
		self.bagLimitCount = Mathf.Max(data.ywPacketSize,initCount);

		self.itemlist.numItems = max;
	}
	public static void ItemRender(this UIYuWaiBag self,int index,GObject go)
	{
		var instId = go.displayObject.gameObject.GetInstanceID();
		var item = self.GetChild<YuWaiBagItemComponent>(instId);
		if(item == null)
		{
			item = self.AddChildWithId<YuWaiBagItemComponent,GComponent>(instId,go.asCom);
			item.SetClick(self.ClickItem);
		}
		item.SetIndex(index);
		var data = DataSystem.Ins.YuWaiModel.GetYWReward(index);
		if(data != null)
		{
			item.SetItem(data);
		}
		else if (index < self.bagLimitCount)
		{
			item.SetEmpty();
		}
		else
		{
			item.SetLock();
		}
	}
	public static void ClickItem(this UIYuWaiBag self,YuWaiBagItemComponent item)
	{
		if (item.IsEmpty()) return;
		if (item.IsLock())
		{
			var initCount = ConfigSystem.Ins.GetGlobal("YWWarehouse");
			var buyCount = DataSystem.Ins.YuWaiModel.ywPacketSize - initCount;
			var sys = ConfigSystem.Ins;
			var buy = sys.GetGlobal<int[]>("YWWarehousePurchase");
			var name = ItemUtil.GetItemName(buy[0]);
			
			var cost = buy[1] + (buyCount+1) / buy[2] * buy[3];
            var costDes = string.Format(ConfigSystem.Ins.GetLanguage("yuwai_cost_title"), name, cost);
			var param = new UICostCommonTipParam() { des = sys.GetLanguage("yw_warehouse"), costDes = costDes, sureCallback = self.ClickUnlock };
			UISystem.Ins.Show(UIType.UICostCommonTip,param);
			return;
		}
		var info = DataSystem.Ins.YuWaiModel.GetYWReward(item.index);
	}
	public static async void ClickUnlock(this UIYuWaiBag self)
	{
		var token = self.GetToken(1);
		var sys = ConfigSystem.Ins;
		var buy = sys.GetGlobal<int[]>("YWWarehousePurchase");
		var cost = buy[1] + 1 / buy[2] * buy[3];

		var resp = (YwBuyPacketResp)await NetSystem.Call(new YwBuyPacketReq(), typeof(YwBuyPacketResp), token);
		if (resp == null) return;
		var data = DataSystem.Ins.YuWaiModel;
		data.ywPacketSize = resp.packetSize;
		self.Refresh();

	}
	public static void ClickClose(this UIYuWaiBag self)
	{
		UISystem.Ins.Close(UIType.UIYuWaiBag);
	}
}