using ET;
using FairyGUI;
using GameEvent;
using UnityEngine;
using Frame;

public class UIFaBaoPeiZi: UIBase
{
	public GList itemList;
	public GComponent detail;
	public Controller showDetailCtrl;
	public GTextField detailText;

	public ListComponent<FaBaoInfo> dataList;
	public int selectIndex;
}
public class UIFaBaoPeiZiAwakeSystem : AwakeSystem<UIFaBaoPeiZi, GComponent>
{
	protected override void Awake(UIFaBaoPeiZi self, GComponent a)
	{
		self.Awake(a);
		self.itemList = a.GetList("list");
		self.showDetailCtrl = a.GetController("showDetail");
		self.detail = a.GetCom("detail");
		self.detailText = self.detail.GetText("textPeiZi");

		var btnSure = self.detail.GetButton("btnSure");
		var btnClose = a.GetChild("_bgfull");

		var pool = self.AddComponent<ListPoolComponent>();
		self.dataList = pool.Create<FaBaoInfo>();

		btnClose.onClick.Add(self.ClickClose);
		btnSure.onClick.Add(self.ClickSure);

		self.itemList.itemRenderer = self.ItemRenderer;
		self.itemList.onClickItem.Add(self.ClickItem);
		self.itemList.SetVirtual();

		self.Refresh();

        self.RegisterEvent<FaBaoChangeEvent>(self.Event_FaBaoChange);
	}
}
public static class UIFaBaoPeiZiSystem
{
	public static void Refresh(this UIFaBaoPeiZi self)
	{
		var dic = DataSystem.Ins.ItemModel.GetFabaoList();
		self.dataList.Clear();
		if (dic != null)
		{
			foreach (var v in dic)
			{
				if (v.Value.stage == 0)
					self.dataList.Add(v.Value);
			}
		}
		
		self.itemList.numItems = self.dataList.Count;
	}
	public static void ShowDetail(this UIFaBaoPeiZi self,GComponent go,FaBaoInfo info)
	{
		self.showDetailCtrl.selectedIndex = 1;
		var pos = go.LocalToGlobal(new Vector2(go.width / 2, go.height));
		self.detail.xy = self.gCom.GlobalToLocal( pos);

		var cfg = ConfigSystem.Ins.GetFBAuxiliaryConfig(info.quality);
		self.detail.GetText("textPeiZi").text = string.Format(ConfigSystem.Ins.GetLanguage("fabao_quality_name"), cfg.Get<string>("Name"));
		self.detail.GetText("time").text = string.Format(ConfigSystem.Ins.GetLanguage("fabao_refine_time_tip"),1);

	}
	public static void ItemRenderer(this UIFaBaoPeiZi self,int index,GObject go)
	{
		var com = go.asCom;
		com.name = index.ToString();
		var info = self.dataList[index];
		var cfg = ConfigSystem.Ins.GetFBAuxiliaryConfig(info.quality);
		com.GetText("title").text = cfg.Get<string>("Name");
	}
	public static void ClickItem(this UIFaBaoPeiZi self,EventContext e)
	{
		var com = e.data as GComponent;
		int index = int.Parse(com.name);
		var info = self.dataList[index];
		self.selectIndex = index;
		self.ShowDetail(com,info);
		
	}
	public static void ClickClose(this UIFaBaoPeiZi self)
	{
		if(self.showDetailCtrl.selectedIndex == 1)
		{
			self.showDetailCtrl.selectedIndex = 0;
			return;
		}
		UISystem.Ins.Close(UIType.UIFaBaoPeiZi);
	}
	public static void ClickSure(this UIFaBaoPeiZi self)
	{
		var info = self.dataList[self.selectIndex];
		UISystem.Ins.Show(UIType.UIFaBaoDuanZao, new UIFaBaoDuanZaoParam() { index = (int)info.uid });
	}

	public static void Event_FaBaoChange(this UIFaBaoPeiZi self, FaBaoChangeEvent e)
	{
		self.Refresh();
		self.showDetailCtrl.selectedIndex = 0;
	}
}
