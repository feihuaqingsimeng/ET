using ET;
using FairyGUI;
using module.fabao.message;
using UnityEngine;
using Frame;
using System.Collections.Generic;

public class MaterialInfo
{
	public ItemInfo info;
	public int count;
    public int max;
}
public class UIFaBaoLianQi: UIBase
{
	public GButton btnFabao;
	public GList matItemList;
	public GTextField textNeedTip;
	public Controller emptyCtl;

    public EmptyEntity selectRoot;
    public EmptyEntity listRoot;
	public int selectFaBaoQuality;
	public ListComponent<ItemInfo> dataList;
	public MaterialInfo[] selectList = new MaterialInfo[8];

	public int curSelectIndex = -1;//8个格子中的高亮
	public int totalCount;//
	public int needCount;
	public bool isSendMsg = false;

}



[ObjectSystem]
class UIFaBaoLianQiAwakeSystem : AwakeSystem<UIFaBaoLianQi, GComponent>
{
	protected override void Awake(UIFaBaoLianQi self, GComponent a)
	{
		self.Awake(a);
		self.btnFabao = a.GetButton("btnFaBao");
		self.matItemList = a.GetList("list");
		self.textNeedTip = a.GetText("textNeedTip");
		self.emptyCtl = a.GetController("empty");
		var btnZhuZao = a.GetButton("btnZhuZao");
		var btnBack = a.GetButton("btnBack");
		var btnFuLing = a.GetButton("btnFuLing");
		var btnPeiZi = a.GetButton("btnPeiZi");

		self.btnFabao.onClick.Add(self.ClickFabao);
		btnBack.onClick.Add(self.ClickClose);
		btnZhuZao.onClick.Add(self.ClickZhuZao);
		btnFuLing.onClick.Add(self.ClickFuLing);
		btnPeiZi.onClick.Add(self.ClickPeiZi);
		var poolCom = self.AddComponent<ListPoolComponent>();
		self.dataList = poolCom.Create<ItemInfo>();

        self.selectRoot = self.AddChild<EmptyEntity>();
        self.listRoot = self.AddChild<EmptyEntity>();
        for (int i = 0; i < 8; i++)
		{
			var g = a.GetButton($"mat{i}");
            var child = self.selectRoot.AddChildWithId<LianQiSelectMatCom, GComponent>(i, a.GetCom($"mat{i}"));
            child.SetOnClick(self.ClickSelectMat);
            self.selectList[i] = new MaterialInfo();

        }
		
		self.matItemList.itemRenderer = self.ItemRender;
		self.matItemList.onClickItem.Add(self.ClickItem);
		self.matItemList.SetVirtual();
		self.Refresh();
	}

}

public static class UIFaBaoLianQiSystem
{
	public static void Refresh(this UIFaBaoLianQi self)
	{
		if(self.selectFaBaoQuality == 0)
		{
			self.textNeedTip.text = "";
			self.emptyCtl.selectedIndex = 0;
			self.RefreshSelectMat();
			return;
		}
		var cfg = ConfigSystem.Ins.GetFBAuxiliaryConfig(self.selectFaBaoQuality);
		self.btnFabao.title = cfg.Get<string>("Name");
		self.RefreshList();
		self.RefreshSelectMat();
	}
	public static void RefreshSelectMat(this UIFaBaoLianQi self)
	{
		self.totalCount = 0;
		foreach (var v in self.selectRoot.Children)
		{
            var item = v.Value as LianQiSelectMatCom;
			var data = self.selectList[v.Key];
			if (data.info != null)
			{
                item.Set(data);
				self.totalCount += data.count;
			}
			else
			{
                item.Empty();
			}
		}
		if (self.selectFaBaoQuality == 0)
		{
			self.textNeedTip.text = "";
			return;
		}
		var fbcfg = ConfigSystem.Ins.GetFBAuxiliaryConfig(self.selectFaBaoQuality);
		self.needCount = fbcfg.Get("MCQuantity");
		self.textNeedTip.text = string.Format(ConfigSystem.Ins.GetLanguage("mat_need_tip"), self.totalCount, self.needCount);
	}
	public static void RefreshList(this UIFaBaoLianQi self)
	{
		var faBaoCfg = ConfigSystem.Ins.GetFBAuxiliaryConfig(self.selectFaBaoQuality);
		int quality = faBaoCfg.Get("MCQuality");
		var data = DataSystem.Ins.ItemModel;
		var dic = data.GetItems(PackType.PACK);
		self.dataList.Clear();
		if (dic != null)
		{
			var sys = ConfigSystem.Ins;
			foreach (var v in dic)
			{
                ItemInfo info = v.Value;
				if (info.quality <= quality && info.type == (int)ItemType.Refine )
					self.dataList.Add(v.Value);
			}
		}
		self.matItemList.numItems = self.dataList.Count;
		self.emptyCtl.selectedIndex = self.dataList.Count == 0 ? 1 : 2;

	}
	public static void ItemRender(this UIFaBaoLianQi self,int index ,GObject go)
	{
		var info = self.dataList[index];
		int insId = go.displayObject.gameObject.GetInstanceID();
		var com = self.GetChild<IconItemCom>(insId);
		if(com == null)
		{
			com = self.AddChildWithId<IconItemCom, GComponent>(insId, go.asCom, true);
		}
		com.Set(info, index);

		for(int i = 0;i< self.selectList.Length; i++)
		{
			if(self.selectList[i].info == info)
			{
				com.SetSelect(true);
				return;
			}
		}
		com.SetSelect(false);
	}
	public static void ClickItem(this UIFaBaoLianQi self,EventContext e)
	{
		var go = e.data as GComponent;
		var insId = go.displayObject.gameObject.GetInstanceID();
		var com = self.GetChild<IconItemCom>(insId);
		var info = self.dataList[com.index];
		if (self.totalCount == self.needCount)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("mat_full_tip") });
			return;
		}
		int count = 0;
		for(int i = 0; i < self.selectList.Length; i++)
		{
			if (self.selectList[i].info != null && self.selectList[i].info != info)
				count += self.selectList[i].count;
		}
		var limit = Mathf.Min( self.needCount - count, info.num);
		var param = new UIItemSelectParam() { itemInfo = info,isAdd = true,limit = limit, callback = self.AddMatCallback };
		UISystem.Ins.Show(UIType.UIItemSelect, param);
	}
	public static void AddMatCallback(this UIFaBaoLianQi self,ItemInfo info, int count)
	{
		bool find = false;
		int index = -1;
		for(int i = 0;i<self.selectList.Length;i++)
		{
			var mat = self.selectList[i];
			if(mat.info == null&& index == -1)
				index = i;
			if (mat.info == info)
			{
				mat.count = count;
				self.selectList[i] = mat;
				find = true;
				break;
			}
		}
		if (self.curSelectIndex != -1)
			index = self.curSelectIndex;
		if (!find && index != -1)
		{
			self.selectList[index].info = info;
			self.selectList[index].count = count;
		}
		self.RefreshSelectMat();
		self.SetSelectState(-1);
		foreach(var v in self.Children)
		{
			if (!(v.Value is IconItemCom)) continue;
			var prop = v.Value as IconItemCom;
			if(prop.info == info)
			{
				prop.SetSelect(true);
				return;
			}
		}
	}
	public static void ReduceMatCallback(this UIFaBaoLianQi self, ItemInfo info, int count)
	{
		bool isRemove = false;
		for (int i = 0; i < self.selectList.Length; i++)
		{
			var mat = self.selectList[i];
			if (mat.info == info)
			{
				if(count == mat.count)
				{
					isRemove = true;
					self.selectList[i].info = null;
					break;
				}
				mat.count -=  count;
				self.selectList[i] = mat;
				break;
			}
		}
		self.RefreshSelectMat();
		self.SetSelectState(-1);
		if (!isRemove) return;
		foreach (var v in self.Children)
		{
			if (!(v.Value is IconItemCom)) continue;
			var prop = v.Value as IconItemCom;
			if (prop.info == info)
			{
				prop.SetSelect(false);
				return;
			}
		}
		
	}
	//格子选中
	public static void SetSelectState(this UIFaBaoLianQi self,int index)
	{
		if (self.curSelectIndex != -1)
		{
            var old = self.selectRoot.GetChild<LianQiSelectMatCom>(index);
            old.SetSelect(false);
		}
		self.curSelectIndex = index;
		if (index == -1) return;
        self.selectRoot.GetChild<LianQiSelectMatCom>(index).SetSelect(true);
	}
	//点击格子
	public static void ClickSelectMat(this UIFaBaoLianQi self, LianQiSelectMatCom item)
	{
		if(item.info == null)
		{
			self.SetSelectState(item.index);
		}
		else
		{
			var param = new UIItemSelectParam() { itemInfo = item.info.info, isAdd = false, limit = item.info.count, callback = self.ReduceMatCallback };
			UISystem.Ins.Show(UIType.UIItemSelect, param);
		}
	}
	
	public static void ClickFabao(this UIFaBaoLianQi self)
	{
		var param = new UIFaBaoQualitySelectParam() { callback = self.SelectCallback};
		UISystem.Ins.Show(UIType.UIFaBaoQualitySelect,param);
	}
	public static void SelectCallback(this UIFaBaoLianQi self,int id)
	{
		self.selectFaBaoQuality = id;
		self.Refresh();
	}
	public static void ClickClose(this UIFaBaoLianQi self)
	{
		UISystem.Ins.Close(UIType.UIFaBaoLianQi);
	}
	public static void ClickZhuZao(this UIFaBaoLianQi self)
	{
		if (self.isSendMsg)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("ui_fabao_refine_wait_tip")});
			return;
		}
		if(self.selectFaBaoQuality == 0)
		{
			UISystem.Ins.Show(UIType.UIFlowTip,new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("ui_fabao_quality_tip")});
			return;
		}
		var fbcfg = ConfigSystem.Ins.GetFBAuxiliaryConfig(self.selectFaBaoQuality);
		if (self.totalCount < self.needCount)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("fabao_mat_not_enough")});
			return;
		}
		if (self.totalCount > self.needCount)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("fabao_mat_more_tip")});
			return;
		}
		self.SendZhuZaoAsync().Coroutine();
	}
	public static async ETTask SendZhuZaoAsync(this UIFaBaoLianQi self)
	{
		self.WaitAsync().Coroutine();
		var req = new FaBaoZhuZaoReq() { fbAuxiId = self.selectFaBaoQuality };
		for (int i = 0; i < self.selectList.Length; i++)
		{
			var info = self.selectList[i].info;
			if (info != null)
			{
				var f = new module.item.message.ItemInfo() { index = (int)info.uid,itemId = info.modelId, amount = self.selectList[i].count };
				req.cailiao.Add(f);
			}
		}
		self.isSendMsg = true;
		var resp = (FaBaoZhuZaoResp) await NetSystem.Call(req, typeof(FaBaoZhuZaoResp), self.GetToken(1));
        if (resp == null) return;
		//reset
		for (int i = 0; i < self.selectList.Length; i++)
			self.selectList[i].info = null;
		self.Refresh();
		//第二阶段
		UISystem.Ins.Show(UIType.UIFaBaoDuanZao, new UIFaBaoDuanZaoParam() { index = resp.index });
	}
	public static async ETTask WaitAsync(this UIFaBaoLianQi self)
	{
		await TimerComponent.Instance.WaitAsync(1000);
		self.isSendMsg = false;
	}
	public static void ClickFuLing(this UIFaBaoLianQi self)
	{
		UISystem.Ins.Show(UIType.UIFaBaoSkill);
	}
	public static void ClickPeiZi(this UIFaBaoLianQi self)
	{
		UISystem.Ins.Show(UIType.UIFaBaoPeiZi);
	}
}