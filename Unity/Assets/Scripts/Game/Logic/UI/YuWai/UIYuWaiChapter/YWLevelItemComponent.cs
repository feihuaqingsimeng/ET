using ET;
using FairyGUI;
using GameEvent;
using System;
using Frame;

public class YWLevelItemComponent: UIBase
{
	public GTextField textTitle;
	public Controller isInCtrl;
	public Controller isSelectCtrl;

	public Action<YWLevelItemComponent> callback;
	public int index;
	public ConfigData cfg;
}
public class YWLevelItemComponentAwakeSystem : AwakeSystem<YWLevelItemComponent, GComponent>
{
	protected override void Awake(YWLevelItemComponent self, GComponent a)
	{
		self.Awake(a);
		self.textTitle = a.GetText("title");
		self.isInCtrl = a.GetController("isIn");
		self.isSelectCtrl = a.GetController("select");

		a.onClick.Add(self.ClickItem);
	}
}
public static class YWLevelItemComponentSystem
{
	public static void Set(this YWLevelItemComponent self,int index,int id)
	{
		self.index = index;
		var sys = ConfigSystem.Ins;
		self.cfg = sys.GetYWLowerMap(id);
		self.textTitle.text = self.cfg.Get<string>("MapName");
	}
	public static void SetSelect(this YWLevelItemComponent self,bool isSelect)
	{
		self.isSelectCtrl.selectedIndex = isSelect ? 1 : 0;
	}
	public static void SetIsIn(this YWLevelItemComponent self, bool isIn)
	{
		self.isInCtrl.selectedIndex = isIn ? 1 : 0;
	}
	public static void SetClick(this YWLevelItemComponent self, Action<YWLevelItemComponent> callback)
	{
		self.callback = callback;
	}
	public static void ClickItem(this YWLevelItemComponent self)
	{
		self.callback?.Invoke(self);
	}
}
