using ET;
using FairyGUI;
using GameEvent;
using System.Collections.Generic;
using module.fabao.message;
using Frame;

public class FabaoSkillItemData
{
	public FaBaoInfo info;
	public FaBaoPackType type;
}
public class UIFaBaoSkill: UIBase
{
	public GTextField textSkillName;
	public GTextField textSkillCD;
	public GTextField textSkillDes;
	public GTextField textNum;
	public GTextField textCost;
	public GList itemList;
	public Controller selectItemEmptyCtrl;
	public FabaoSkillItem selectItem;
	public ResourceMultiItemComponent multResCom;

	public List<FabaoSkillItemData> dataList = new List<FabaoSkillItemData>();
	public FaBaoInfo selectInfo;
	public FabaoSkillItemData tempData;
	public string costErrorTip;
}
public class UIFaBaoSkillAwakeSystem : AwakeSystem<UIFaBaoSkill, GComponent>
{
	protected override void Awake(UIFaBaoSkill self, GComponent a)
	{
		self.Awake(a);
		self.textSkillName = a.GetText("name");
		self.textSkillCD = a.GetText("cd");
		self.textSkillDes = a.GetText("des");
		self.textNum = a.GetCom("num").GetText("title");
		self.itemList = a.GetList("list");
		var com = a.GetCom("selectItem");
		self.selectItemEmptyCtrl = com.GetController("empty");
		self.selectItem = self.AddChild<FabaoSkillItem, GComponent>(com.GetCom("item"));
		self.textCost = a.GetText("cost");
		var btnClose = a.GetButton("btnClose");
		var btnSure = a.GetButton("btnSure");
		self.multResCom = self.AddChild<ResourceMultiItemComponent, GComponent>(a.GetCom("resBar"));
		self.multResCom.Add(ResourceType.BaoLing);
		self.multResCom.Add(ResourceType.LingShi);

		btnClose.onClick.Add(self.ClickClose);
		btnSure.onClick.Add(self.ClickSure);

		self.itemList.itemRenderer = self.ItemRender;
		self.itemList.SetVirtual();

		self.RegisterEvent<FaBaoFuLingSelectEvent>(self.Event_FaBaoFuLingSelect);
        self.RegisterEvent<ResourceAllChangedEvent>(self.Event_ResourceAllChanged);
        self.RegisterEvent<FaBaoCountChangeEvent>(self.Event_FaBaoCountChange);

		self.Refresh();
	}
}
public static class UIFaBaoSkillSystem 
{
	public static void Refresh(this UIFaBaoSkill self)
	{
		self.RefreshList();
		self.RefreshSelectItem();
		self.RefreshSkill();
		self.RefreshCost();
	}
	public static void RefreshList(this UIFaBaoSkill self)
	{
		int count = self.dataList.Count;
		int index = 0;
        var data = DataSystem.Ins.ItemModel;
        var dic = data.GetFabaoList(FaBaoPackType.EQUIP);
		if (dic != null)
		{
			foreach (var v in dic)
				self.AddDataList(index++, FaBaoPackType.EQUIP, v.Value);
		}
		dic = data.GetFabaoList();
		if (dic != null)
		{
			foreach (var v in dic)
			{
				if (v.Value.stage > 0)
					self.AddDataList(index++, FaBaoPackType.PACK, v.Value);
			}
		}

		if (count > 0 && index < count)
		{
			for (int i = index; i < count; i++)
				self.dataList.RemoveAt(self.dataList.Count - 1);
		}
		self.dataList.Sort(SortQuality);
		self.itemList.numItems = self.dataList.Count;
		self.textNum.text = string.Format(ConfigSystem.Ins.GetLanguage("common_num_tip"), self.dataList.Count.ToString());
	}
	public static void RefreshSkill(this UIFaBaoSkill self)
	{
		if (self.selectInfo == null || self.selectInfo.skillId == 0)
		{
			self.textSkillName.text = ConfigSystem.Ins.GetLanguage("ui_fabao_skill_default");
			self.textSkillCD.text = "";
			self.textSkillDes.text = "";
			return;
		}
		int skillId = self.selectInfo.skillId;
		var sys = ConfigSystem.Ins;
		var skill = sys.GetSkillConfig(skillId);
		if (skill == null)
		{
			Log.Error($"skill表找不到技能：{skillId} ");
			return;
		}
		self.textSkillName.text = string.Format(sys.GetLanguage("ui_fabao_skill_name"),skill.Get<string>("SkillName"));
		int cd = skill.Get("CD");
		if (cd > 0)
			self.textSkillCD.text = string.Format(sys.GetLanguage("ui_fabao_skill_cd"), cd.ToString()) ;
		else
			self.textSkillCD.text = "";
		int type = skill.Get("SkillType");
		string typeName = sys.GetLanguage($"skill_type_{type}");
		self.textSkillDes.text = $"{typeName}：{skill.Get<string>("SkillDescribe")}";
	}
	public static void RefreshCost(this UIFaBaoSkill self)
	{
		self.costErrorTip = "";
		if (self.selectInfo == null)
		{
			self.textCost.text = "";
			return;
		}
		var sys = ConfigSystem.Ins;
		var weaponCfg = sys.GetFBMagicWeaponConfig(self.selectInfo.modelId);
		var auxiliary = sys.GetFBAuxiliaryConfig(weaponCfg.Get("FBGrade"));
		var cost = auxiliary.Get<int[][]>("BlessingConsume");
		if (cost == null) return;

		string str = sys.GetLanguage("ui_fabao_skill_cost_title");
		for(int i = 0; i < cost.Length; i++)
		{
			int costId = cost[i][0];
			int costNum = cost[i][1];
			var cfg = sys.GetItemConfig(costId);
			if (cfg == null) continue;
			var nameStr = cfg.Get<string>("Name");
			long own = DataSystem.Ins.ItemModel.GetResource((ResourceType)costId);
			if(own < costNum)
			{
				str += $"{nameStr}：{Util.GetColorText(costNum.ToString(), "#ff0000")} ";
				self.costErrorTip = string.Format(sys.GetLanguage("common_not_enough_tip"), nameStr);
			}
			else
				str += $"{nameStr}：{costNum} ";

		}
		self.textCost.text = str;
	}
	public static void AddDataList(this UIFaBaoSkill self,int index,FaBaoPackType type,FaBaoInfo info)
	{
		if(index < self.dataList.Count)
		{
			self.dataList[index].type = type;
			self.dataList[index].info = info;
		}
		else
		{
			self.dataList.Add(new FabaoSkillItemData() { info = info, type = type });
		}
	}
	public static void RefreshSelectItem(this UIFaBaoSkill self)
	{
		if (self.selectInfo == null)
		{
			self.selectItemEmptyCtrl.selectedIndex = 1;
			return;
		}
		self.selectItemEmptyCtrl.selectedIndex = 0;
		self.selectItem.Set(self.selectInfo,self.selectInfo.packType);
		self.selectItem.SetEquiped(false);
	}
	public static int SortQuality(FabaoSkillItemData a, FabaoSkillItemData b)
	{
		if (a.type == FaBaoPackType.EQUIP && b.type != FaBaoPackType.EQUIP)
			return -1;
		if (a.type != FaBaoPackType.EQUIP && b.type == FaBaoPackType.EQUIP)
			return 1;

		var aCfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(a.info.modelId);
		var aq = aCfg.Get("FBGrade");
		var bCfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(b.info.modelId);
		var bq = bCfg.Get("FBGrade");
		if (aq != bq) return bq - aq;
		if (b.info.stage != a.info.stage) return b.info.stage - a.info.stage;
		aq = aCfg.Get("FBType");
		bq = bCfg.Get("FBType");
		return aq - bq;
	}
	public static void ItemRender(this UIFaBaoSkill self,int index,GObject go)
	{
		var info = self.dataList[index];
		var instId = go.displayObject.gameObject.GetInstanceID();
		var item = self.GetChild<FabaoSkillItem>(instId);
		if(item == null)
		{
			item = self.AddChildWithId<FabaoSkillItem, GComponent>(instId, go.asCom);
			item.SetOnClick(self.ClickItem);
		}
		item.Set(info);
		item.SetSelect(info.info == self.selectInfo);
	}
	public static void ClickItem(this UIFaBaoSkill self,FabaoSkillItem item)
	{
		self.tempData = item.info;
		var param = new UIFabaoDetailParam() { info = item.info.info, showFuLing = true };
		UISystem.Ins.Show(UIType.UIFabaoDetail, param);
	}
	public static void ClickClose(this UIFaBaoSkill self)
	{
		UISystem.Ins.Close(UIType.UIFaBaoSkill);
	}
	public static void ClickSure(this UIFaBaoSkill self)
	{
		if(self.selectInfo == null)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("ui_fabao_skill_empty_choice")});
			return;
		}
		if(!string.IsNullOrEmpty(self.costErrorTip))
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = self.costErrorTip });
			return;
		}
		self.SendSkillReqAsync().Coroutine();
	}
	public static async ETTask SendSkillReqAsync(this UIFaBaoSkill self)
	{
		var req = new FaBaoBlessingReq() { packType = ( module.fabao.packet.impl.FaBaoPackType) self.selectInfo.packType,packetIndex = (int)self.selectInfo.uid };
		FaBaoBlessingResp resp = (FaBaoBlessingResp)await NetSystem.Call(req, typeof(FaBaoBlessingResp), self.GetToken(1));
		if (resp == null) return;
		var sys = ConfigSystem.Ins;
		if (!resp.success)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = sys.GetLanguage("ui_fabao_skill_fail") });
			return;
		}
		UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = sys.GetLanguage("ui_fabao_skill_suc") });
		self.RefreshSkill();
	}
	public static void Event_FaBaoFuLingSelect(this UIFaBaoSkill self,FaBaoFuLingSelectEvent e)
	{
		self.selectInfo = e.info;
		self.Refresh();
	}
	public static void Event_ResourceAllChanged(this UIFaBaoSkill self,ResourceAllChangedEvent e)
	{
		self.RefreshCost();
	}
	public static void Event_FaBaoCountChange(this UIFaBaoSkill self,FaBaoCountChangeEvent e)
	{
		self.RefreshList();
		if (self.selectInfo != null && self.selectInfo != DataSystem.Ins.ItemModel.GetFabao((int)self.selectInfo.uid, self.selectInfo.packType))
		{
			self.selectInfo = null;
			self.RefreshSelectItem();
			self.RefreshSkill();
		}
	}
}
