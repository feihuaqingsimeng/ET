using ET;
using FairyGUI;
using GameEvent;
using System;
using Frame;

public class YWMapItemComponent: UIBase
{
	public Controller stateCtrl;

	public int index;
	public Action<YWMapItemComponent> callback;
}
public class YWMapItemComponentAwakeSystem : AwakeSystem<YWMapItemComponent, GComponent>
{
	protected override void Awake(YWMapItemComponent self, GComponent a)
	{
		self.Awake(a);
		self.stateCtrl = a.GetController("state");

		a.onClick.Add(self.ClickItem);
	}
}
public static class YWMapItemComponentSystem
{
	public static void Refresh(this YWMapItemComponent self,int index)
	{
		self.index = index;
	}
	public static void SetState(this YWMapItemComponent self, YWMapState state)
	{
		self.stateCtrl.selectedIndex = (int)state;
	}
	public static void SetClick(this YWMapItemComponent self, Action<YWMapItemComponent> callback)
	{
		self.callback = callback;
	}
	public static void ClickItem(this YWMapItemComponent self)
	{
		self.callback?.Invoke(self);
	}
}