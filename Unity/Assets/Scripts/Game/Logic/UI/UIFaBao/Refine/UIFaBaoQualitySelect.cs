using ET;
using FairyGUI;
using GameEvent;
using System;
using Frame;

public class UIFaBaoQualitySelect:UIBase
{
	public GList itemList;

	public ListComponent<ConfigData> dataList;

	public Action<int> callback;
}
[ObjectSystem]
public class UIFaBaoQualitySelectAwakeSystem : AwakeSystem<UIFaBaoQualitySelect, GComponent>
{
	protected override void Awake(UIFaBaoQualitySelect self, GComponent a)
	{
		self.Awake(a);
		self.itemList = a.GetList("list");
		self.itemList.itemRenderer = self.ItemRender;
		var bg = a.GetChild("_bgfull");

		var poolCom = self.AddComponent<ListPoolComponent>();
		self.dataList = poolCom.Create<ConfigData>();
		bg.onClick.Add(self.ClickClose);

		self.Refresh();
	}
}
[ObjectSystem]
public class UIFaBaoQualitySelectChangeSystem : ChangeSystem<UIFaBaoQualitySelect, IUIDataParam>
{
	protected override void Change(UIFaBaoQualitySelect self, IUIDataParam a)
	{
		self.callback = ((UIFaBaoQualitySelectParam)a).callback;
	}
}

public static class UIFaBaoQualitySelectSystem
{
	public static void Refresh(this UIFaBaoQualitySelect self)
	{
		var configs = ConfigSystem.Ins.GetFBAuxiliaryConfig();
		foreach(var v in configs)
		{
			self.dataList.Add(v.Value);
		}
		self.itemList.numItems = self.dataList.Count;
		self.itemList.onClickItem.Add(self.ClickItem);
	}
	public static void ItemRender(this UIFaBaoQualitySelect self,int index,GObject go)
	{
		var config = self.dataList[index];
		var com = go.asButton;
		com.name = index.ToString();
		com.title = config.Get<string>("Name");
		var data = DataSystem.Ins.PlayerModel;
		var lvCfg = ConfigSystem.Ins.GetLvConfig(data.level);
		com.grayed = lvCfg.Get("State") < config.Get("Unlock");
	}
	public static string GetStageNameByState(int state)
	{
		var sys = ConfigSystem.Ins;
		var configs = sys.GetLvConfig();
		int max = configs.Count;
		int start = 1;
		int end = max;
		int mid = 0;
		while (start < end)
		{
			mid = start + (end-start)/2;
			var s = sys.GetLvConfig(mid).Get("State");
			if (s > state)
				end = mid - 1;
			else if (s < state)
				start = mid + 1;
			else
				break;
		}
		for (int i = start; i <= mid; i++)
		{
			var cfg = sys.GetLvConfig(i);
			if (cfg.Get("State") == state)
				return cfg.Get<string>("Describe");
		}
		return "[]";
	}
	public static void ClickItem(this UIFaBaoQualitySelect self,EventContext e)
	{
		var com = (GButton)e.data;
		int index = int.Parse(com.name);
		var config = self.dataList[index];
		var data = DataSystem.Ins.PlayerModel;
		var lvCfg = ConfigSystem.Ins.GetLvConfig(data.level);
		var state = config.Get("Unlock");
		if (lvCfg.Get("State") < state)
		{
			string str = string.Format(ConfigSystem.Ins.GetLanguage("fabao_quality_unlock_tip"),GetStageNameByState(state));

            UISystem.Ins.Show(UIType.UIFlowTip,new UIFlowTipParam() { content = str });
			return;
		}
		self.callback?.Invoke(self.dataList[index].Get("ID"));
		self.ClickClose();
	}
	public static void ClickClose(this UIFaBaoQualitySelect self)
	{
		UISystem.Ins.Close(UIType.UIFaBaoQualitySelect);
	}
}

