using ET;
using FairyGUI;
using GameEvent;
using module.fabao.message;
using module.fabao.packet.impl;
using UnityEngine;
using Frame;

public class UIFaBaoWear:UIBase
{
	public GTextField textNum;
	public GList itemList;
	public GComboBox sortBox;
	public GComboBox typeBox;

	public ListComponent<FaBaoInfo> dataList;

	public int sortType;
	public int fabaoType;
	public bool isTouchWait;
	public GComponent dragItemParent;
}
public class UIFaBaoWearAwakeSystem : AwakeSystem<UIFaBaoWear, GComponent>
{
	protected override void Awake(UIFaBaoWear self, GComponent a)
	{
		self.Awake(a);
		self.textNum = a.GetCom("textNum").GetText("title");
		self.itemList = a.GetList("list");
		self.sortBox = a.GetComboBox("sortBox");
		self.typeBox = a.GetComboBox("typeBox");
		var btnClose = a.GetButton("btnClose");
		for(int i = 0; i < 6; i++)
		{
			var com = self.AddChildWithId<FabaoEquipedItem, GComponent>(i+1, a.GetCom($"wear{i}"));
			com.SetIndex(i+1);
			com.SetOnClick(self.ClickEquipedItem);
			com.SetTouchCallback( self.TouchMove, self.TouchEnd);
		}
		var pool = self.AddComponent<ListPoolComponent>();
		self.dataList = pool.Create<FaBaoInfo>();

		self.itemList.itemRenderer = self.ItemRender;
		self.itemList.SetVirtual();

		btnClose.onClick.Add(self.ClickClose);
		self.sortBox.onChanged.Add(self.SortBoxChange);
		self.typeBox.onChanged.Add(self.TypeBoxChange);

        self.RegisterEvent<FaBaoWearSucEvent>(self.Event_FaBaoWearSuc);

		self.Refresh();
	}
}
public static class UIFaBaoWearSystem
{
	public static void Refresh(this UIFaBaoWear self)
	{
		self.SortRefresh();
		self.RefreshEquipedItems();
	}
	public static void RefreshEquipedItems(this UIFaBaoWear self)
	{
        var data = DataSystem.Ins.ItemModel;
        for (int i = 1; i <= ConstValue.FABAO_EQUIP_COUNT; i++)
		{
			var child = self.GetChild<FabaoEquipedItem>(i);
			child.Set(data.GetFabao(i,FaBaoPackType.EQUIP));
		}
	}
	public static void SortRefresh(this UIFaBaoWear self)
	{
		self.dataList.Clear();
		var dic = DataSystem.Ins.ItemModel.GetFabaoList();
		if (dic != null)
		{
			foreach (var v in dic)
			{
				var cfg2 = ConfigSystem.Ins.GetFBMagicWeaponConfig(v.Value.modelId);
				int type2 = cfg2.Get("FBType");
				if (v.Value.stage > 0 && (self.fabaoType == 0 || (self.fabaoType == type2)))
					self.dataList.Add(v.Value);
			}
		}
		if (self.sortType == 0)//境界排序
			self.dataList.Sort(SortQuality);
		else
			self.dataList.Sort(SortStage);
		self.itemList.numItems = self.dataList.Count;
		self.textNum.text = string.Format(ConfigSystem.Ins.GetLanguage("common_num_tip"),self.dataList.Count);
	}
	
	//境界排序
	public static int SortStage(FaBaoInfo a,FaBaoInfo b)
	{
		if (b.stage != a.stage) return b.stage - a.stage;
		var aCfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(a.modelId);
		var aq = aCfg.Get("FBGrade");
		var bCfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(b.modelId);
		var bq = bCfg.Get("FBGrade");
		if (aq != bq) return bq - aq;
		aq = aCfg.Get("FBType");
		bq = bCfg.Get("FBType");
		return aq - bq;
	}
	public static int SortQuality(FaBaoInfo a,FaBaoInfo b)
	{
		var aCfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(a.modelId);
		var aq = aCfg.Get("FBGrade");
		var bCfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(b.modelId);
		var bq = bCfg.Get("FBGrade");
		if (aq != bq) return bq - aq;
		if (b.stage != a.stage) return b.stage - a.stage;
		aq = aCfg.Get("FBType");
		bq = bCfg.Get("FBType");
		return aq - bq;
	}
	public static void ItemRender(this UIFaBaoWear self,int index,GObject go)
	{
		var info = self.dataList[index];
		var insId = go.displayObject.gameObject.GetInstanceID();
		var item = self.GetChild<FaBaoItem>(insId);
		if (item == null)
			item = self.AddChildWithId<FaBaoItem, GComponent>(insId, go.asCom,true);
		item.Set(info,index);
		item.SetOnClick(self.ClickItem);
	}
	
	public static void ClickEquipedItem(this UIFaBaoWear self, FabaoEquipedItem item)
	{
		if (item.IsEmpty()) return;
		var param = new UIFabaoDetailParam() { info = item.info };
		UISystem.Ins.Show(UIType.UIFabaoDetail, param);
	}
	public static void ClickItem(this UIFaBaoWear self, FaBaoItem item)
	{
		var param = new UIFabaoDetailParam() { info = item.info };
		UISystem.Ins.Show(UIType.UIFabaoDetail, param);
	}
	public static void SendUnWearReq(this UIFaBaoWear self, int packetIndex)
	{
		var req = new FaBaoWearReq() { packetIndex = packetIndex,};
		NetSystem.Send(req);
	}
	public static void SendChangeReq(this UIFaBaoWear self, int packetIndex, int wearIndex)
	{
		var req = new FaBaoWearReq() { packetIndex = packetIndex, wearIndex = wearIndex ,actionType = 2};
		NetSystem.Send(req);
	}
	public static void ClickClose(this UIFaBaoWear self)
	{
		UISystem.Ins.Close(UIType.UIFaBaoWear);
	}
	public static void SortBoxChange(this UIFaBaoWear self)
	{
		self.sortType = self.sortBox.selectedIndex;
		self.SortRefresh();
	}
	public static void TypeBoxChange(this UIFaBaoWear self)
	{
		self.fabaoType = self.typeBox.selectedIndex;
		self.SortRefresh();
	}
	public static void TouchMove(this UIFaBaoWear self, FabaoEquipedItem item, EventContext e)
	{
		if (self.dragItemParent == null)
		{
			self.dragItemParent = item.item.gCom.parent;
			self.gCom.AddChild(item.item.gCom);
        }
        var pos = new Vector2(e.inputEvent.x, e.inputEvent.y);
        pos /= UIContentScaler.scaleFactor;
        item.item.SetPos(pos.x - item.item.gCom.width / 2, pos.y - item.item.gCom.height / 2);
        item.item.gCom.InvalidateBatchingState();
    }
	public static void TouchEnd(this UIFaBaoWear self, FabaoEquipedItem item, EventContext e)
	{
		if (self.dragItemParent == null) return;
		self.dragItemParent.AddChild(item.item.gCom);
		self.dragItemParent = null;
		item.item.SetPos(0, 0);
        var pos = new Vector2(e.inputEvent.x, e.inputEvent.y);
        pos /= UIContentScaler.scaleFactor;
		float x = pos.x;
		float y = pos.y;
		for (int i = 1; i <= ConstValue.FABAO_EQUIP_COUNT; i++)
		{
			var child = self.GetChild<FabaoEquipedItem>(i);
			var com = child.gCom;
			if (x >= com.x && x <= com.x + com.width && y >= com.y && y <= com.y + com.height)
			{
				if (child != item)
				{
					self.SendChangeReq((int)item.info.uid, i);
					self.TouchEndWaitAsync().Coroutine();
					var info = item.info;
					item.Set(child.info);
					child.Set(info);
				}
				return;
			}
		}
		self.TouchEndWaitAsync().Coroutine();
		self.SendUnWearReq((int)item.info.uid);
		item.Set(null);
	}
	public static async ETTask TouchEndWaitAsync(this UIFaBaoWear self)
	{
		self.isTouchWait = true;
		var flag = await TimerComponent.Instance.WaitAsync(1000, self.GetToken(1));
		self.isTouchWait = false;
		if (flag)
		{
			self.RefreshEquipedItems();
		}
	}

	public static void Event_FaBaoWearSuc(this UIFaBaoWear self, FaBaoWearSucEvent e)
	{
		self.Refresh();
        self.CancelToken(1);
	}
}
 