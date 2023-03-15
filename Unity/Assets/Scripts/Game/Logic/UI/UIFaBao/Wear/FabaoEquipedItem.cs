using ET;
using FairyGUI;
using Frame;
using System;

public class FabaoEquipedItem: UIBase
{
	public Controller isEmptyCtrl;

	public FaBaoInfo info;
	public int index;

	public FaBaoItem item;
	public Action<FabaoEquipedItem> clickCallback;

	public Action<FabaoEquipedItem, EventContext> moveCallback;
	public Action<FabaoEquipedItem, EventContext> endCallback;

}

public class FabaoEquipedItemAwakeSystem : AwakeSystem<FabaoEquipedItem, GComponent>
{
	protected override void Awake(FabaoEquipedItem self, GComponent a)
	{
		self.Awake(a);
		self.isEmptyCtrl = a.GetController("empty");
		self.item = self.AddChildWithId<FaBaoItem, GComponent>(0, a.GetCom("item"),true);

		a.onClick.Add(self.ClickItem);
		a.onTouchMove.Add(self.TouchMove);
		a.onTouchEnd.Add(self.TouchEnd);
	}
}
public static class FabaoEquipedItemSystem
{
	public static void Set(this FabaoEquipedItem self,FaBaoInfo info)
	{
		self.info = info;
		self.isEmptyCtrl.selectedIndex = info == null ? 1 : 0;
		if (info == null) 
			return;
		self.item.Set(info);
	}
	public static void SetIndex(this FabaoEquipedItem self,int index)
	{
		self.index = index;
	}
	public static bool IsEmpty(this FabaoEquipedItem self)
	{
		return self.info == null;
	}
	public static void SetOnClick(this FabaoEquipedItem self, Action<FabaoEquipedItem> callback)
	{
		self.clickCallback = callback;
	}
	public static void SetTouchCallback(this FabaoEquipedItem self, Action<FabaoEquipedItem, EventContext> move,Action<FabaoEquipedItem, EventContext> end)
	{
		self.moveCallback = move;
		self.endCallback = end;
	}
	public static void ClickItem(this FabaoEquipedItem self)
	{
		self.clickCallback?.Invoke(self);
	}
	public static void TouchMove(this FabaoEquipedItem self, EventContext e)
	{
		if (self.IsEmpty()) return;
		self.moveCallback?.Invoke(self, e);
	}
	public static void TouchEnd(this FabaoEquipedItem self, EventContext e)
	{
		if (self.IsEmpty()) return;
		self.endCallback?.Invoke(self, e);
	}
}