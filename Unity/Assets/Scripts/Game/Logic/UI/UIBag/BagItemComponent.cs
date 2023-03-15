using ET;
using FairyGUI;
using Frame;
using module.pack.model;
using System;

public class BagItemComponent: UIBase
{
    public IconItemCom itemCom;
	public Controller slotStateCtrl;

	public ItemInfo info;
	public PackType type;
	public Action<BagItemComponent> callback;
}
public class BagItemComponentAwakeSystem : AwakeSystem<BagItemComponent, GComponent>
{
	protected override void Awake(BagItemComponent self, GComponent a)
	{
		self.Awake(a);
        self.itemCom = self.AddChild<IconItemCom, GComponent>(a.GetCom("item"));
		self.slotStateCtrl = a.GetController("slotState");
		self.info = null;
        self.itemCom.AddClick(self.ClickItem);

	}
}
public static class BagItemComponentSystem
{
	public static void SetLock(this BagItemComponent self)
	{
		self.slotStateCtrl.selectedIndex = 1;
        self.itemCom.SetEmpty();
	}
	public static void SetEmpty(this BagItemComponent self)
	{
		self.slotStateCtrl.selectedIndex = 0;
        self.itemCom.SetEmpty();
    }
	public static void SetItem(this BagItemComponent self,PackType type, int bagIndex)
	{
        self.slotStateCtrl.selectedIndex = 0;
        self.type = type;
		self.info = null;

		var info = DataSystem.Ins.ItemModel.GetItemBySlot(bagIndex,type);
		if(info == null)
		{
			self.SetEmpty();
			return;
		}
        self.info = info;
        self.itemCom.Set(info);
	}
	public static void SetClick(this BagItemComponent self,Action<BagItemComponent> callback)
	{
		self.callback = callback;
	}
	public static void ClickItem(this BagItemComponent self,IconItemCom e)
	{
		self.callback?.Invoke(self);
	}
}
