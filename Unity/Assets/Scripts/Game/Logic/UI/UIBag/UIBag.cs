using ET;
using FairyGUI;
using GameEvent;
using module.pack.message;
using Frame;

public class UIBag: UIBase
{
	public GList bagItemList;
	public GList storageItemList;
	public GTextInput storageNameInput;
	public GTextField textPage;

	public GButton btnBagSort;
	public GButton btnStorageSort;

	public int page;
	public int size_pack;
	public int size_store;
	public int pageMax;

}
public class UIBagAwakeSystem : AwakeSystem<UIBag, GComponent>
{
	protected override void Awake(UIBag self, GComponent a)
	{
		self.Awake(a);
		self.bagItemList = a.GetList("bagList");
		self.storageItemList = a.GetList("storageList");
		self.storageNameInput = a.GetTextInput("storageName");
		self.textPage = a.GetText("page");
		self.btnBagSort = a.GetButton("btnBagSort");
		var btnRight = a.GetButton("btnRight");
		var btnLeft = a.GetButton("btnLeft");
		self.btnStorageSort = a.GetButton("btnStorageSort");
		var btnBuy = a.GetButton("btnBuy");
		var btnBack = a.GetButton("btnBack");
		self.btnBagSort.onClick.Add(self.ClickBagSort);
		self.storageNameInput.onFocusOut.Add(self.ClickStoreNameCommit);
		btnRight.onClick.Add(self.ClickRight);
		btnLeft.onClick.Add(self.ClickLeft);
		self.btnStorageSort.onClick.Add(self.ClickStorageSort);
		btnBuy.onClick.Add(self.ClickBuy);
        btnBack.onClick.Add(self.ClickBack);

		self.bagItemList.SetItemRender(self.BagItemRender);
		self.storageItemList.SetItemRender(self.StorageItemRender);

        self.RegisterEvent<ItemChangeEvent>(self.Event_ItemChange);

		self.size_pack = ConfigSystem.Ins.GetGlobal("KnapsackUpper");
		self.size_store = ConfigSystem.Ins.GetGlobal("Warehouse");
		self.pageMax = ConfigSystem.Ins.GetGlobal("WarehouseUP");
		self.page = 1;
		self.Refresh();
	}
}
public static class UIBagSystem
{
	public static async void Refresh(this UIBag self)
	{
		self.RefreshBag();
        await self.WaitAsync(1);
		self.RefreshStorage();
        await self.WaitAsync(1);
        self.RefreshPage();
	}
	public static void RefreshBag(this UIBag self)
	{
		self.bagItemList.numItems = self.size_pack;
	}
	public static void RefreshStorage(this UIBag self)
	{
		self.storageItemList.numItems = self.size_store;
		string name = DataSystem.Ins.ItemModel.GetStoreName(self.page);
		self.storageNameInput.text = name;
	}
	public static void RefreshPage(this UIBag self)
	{
		self.textPage.text = $"{self.page}/{self.pageMax}";
	}
	public static void BagItemRender(this UIBag self, int index, GObject go)
	{
		int instId = go.displayObject.gameObject.GetInstanceID();
		var item = self.GetChild<BagItemComponent>(instId);
		if (item == null)
		{
			item = self.AddChildWithId<BagItemComponent, GComponent>(instId, go.asCom, true);
		}
		item.SetClick(self.ClickBagItem);
		item.SetItem(PackType.PACK, index + 1);
	}
	public static void StorageItemRender(this UIBag self, int index, GObject go)
	{
		int instId = go.displayObject.gameObject.GetInstanceID();
		var item = self.GetChild<BagItemComponent>(instId);
		if (item == null)
		{
			item = self.AddChildWithId<BagItemComponent, GComponent>(instId, go.asCom, true);
		}
		int grid = (self.page - 1) * self.size_store + index + 1;
		if (grid <= DataSystem.Ins.ItemModel.size_store )
		{
			item.SetItem(PackType.STORE, grid);
			item.SetClick(self.ClickBagItem);
		}
		else
		{
			item.SetLock();
		}
	}
	public static void ClickBagItem(this UIBag self, BagItemComponent item)
	{
		if (item.info == null) return;
		/*if (self.clickStateCtrl.selectedIndex == 1)//快速存取
		{
			var req = new PackStoreReq() { reqType = item.type == PackType.PACK_TYPE ? 0 : 1, page = self.page };
			req.indexs.Add((int)item.info.uid);
			NetSystem.Send(req);
		}
		else*/
		//{
            var type = UIItemDetail.ItemDetailShowType.Bag;
            if (item.type == PackType.STORE)
                type = UIItemDetail.ItemDetailShowType.Store;

            UISystem.Ins.Show(UIType.UIItemDetail, new UIItemDetailParam() {
                type = type,
                info = item.info,
                putCallback = (info)=> {

                    var req = new PackStoreReq() { reqType = item.type == PackType.PACK ? 0 : 1, page = self.page };
                    req.indexs.Add((int)item.info.uid);
                    NetSystem.Send(req);
                }
            });
		//}
	}
	//快捷存取
	public static void ClickBack(this UIBag self)
	{
        self.CloseSelf();
        UISystem.Ins.Show(UIType.UIEquipWear);
	}
	public static void ClickBagSort(this UIBag self)
	{
		self.SendSortAsync(PackType.PACK).Coroutine();
	}

	public static void ClickRight(this UIBag self)
	{
		if (self.page == self.pageMax) return;
		self.page++;
		self.RefreshPage();
		self.RefreshStorage();
	}

	public static void ClickLeft(this UIBag self)
	{
		if (self.page == 1) return;
		self.page--;
		self.RefreshPage();
		self.RefreshStorage();
	}

	public static void ClickStorageSort(this UIBag self)
	{
		self.SendSortAsync(PackType.STORE).Coroutine();
	}
	public static async ETTask SendSortAsync(this UIBag self, PackType type)
	{
		var req = new ClassifyPackReq() { packType = (module.pack.model.PackType) type };
		NetSystem.Send(req);
		int count = 1;
		if (type == PackType.PACK)
			self.btnBagSort.enabled = false;
		else
			self.btnStorageSort.enabled = false;
		var sys = ConfigSystem.Ins;
		while (count > 0)
		{
			if (type == PackType.PACK)
				self.btnBagSort.title = $"{sys.GetLanguage("ui_bag_sort_name")}{count}s";
			else
				self.btnStorageSort.title = $"{sys.GetLanguage("ui_bag_store_sort_name")}{count}s";
			var flag = await TimerComponent.Instance.WaitAsync(1000, self.GetToken(1));
			if (!flag) return;
			count--;
		}
		if (type == PackType.PACK)
		{
			self.btnBagSort.title = sys.GetLanguage("ui_bag_sort_name");
			self.btnBagSort.enabled = true;
		}
		else
		{
			self.btnStorageSort.title = sys.GetLanguage("ui_bag_store_sort_name");
			self.btnStorageSort.enabled = true;
		}

	}
	public static void ClickBuy(this UIBag self)
	{
		var sys = ConfigSystem.Ins;
		var p = sys.GetGlobal<int[]>("WarehousePrice");
		var name = ItemUtil.GetItemName(p[0]);
		string des = string.Format(sys.GetLanguage("ui_bag_store_buy_tip"), p[1], name);
		var param = new UICommonTipParam()
		{
			type = UICommonTip.ButtonType.Double,
			des = des,
			sureCallback = () =>
			{
				var own = DataSystem.Ins.ItemModel.GetResource((ResourceType)p[0]);
				if (own < p[1])
				{
					UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = string.Format(sys.GetLanguage("common_not_enough_tip"), name) });
					return;
				}
				self.SendBuyAsync().Coroutine();
			}
		};

		UISystem.Ins.Show(UIType.UICommonTip, param);
	}
	public static async ETTask SendBuyAsync(this UIBag self)
	{
		var req = new PackExpansionReq() { packType = (module.pack.model.PackType.STORE_TYPE)};
		var resp = (PackSizeChangeResp)await NetSystem.Call(req, typeof(PackSizeChangeResp), self.GetToken(1));
		if (resp == null) return;
		self.RefreshStorage();
	}
	public static void ClickStoreNameCommit(this UIBag self)
	{
		DataSystem.Ins.ItemModel.SetStoreName(self.page, self.storageNameInput.text);
	}
	public static void Event_ItemChange(this UIBag self, ItemChangeEvent e)
	{
		if (e.type == PackType.PACK)
		{
			self.RefreshBag();
		}
		else if (e.type == PackType.STORE)
		{
			self.RefreshStorage();
		}

	}
}