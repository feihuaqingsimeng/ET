using FairyGUI;
using ET;
using GameEvent;
using System;
using module.common.model;
using module.mail.model;
using Frame;

public class YuWaiBagItemComponent : UIBase
{
	public GTextField textName;
	public GTextField textNum;
	public GTextField textType;
	public Controller slotStateCtrl;

	public Action<YuWaiBagItemComponent> callback;
	public int index;
}
class YuWaiBagItemComponentAwakeSystem : AwakeSystem<YuWaiBagItemComponent, GComponent>
{
	protected override void Awake(YuWaiBagItemComponent self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("name");
		self.textNum = a.GetText("num");
		self.textType = a.GetText("type");
		self.slotStateCtrl = a.GetController("slotState");

		a.onClick.Add(self.ClickItem);
	}
}

public static class YuWaiBagItemComponentSystem
{
	public static void SetClick(this YuWaiBagItemComponent self, Action<YuWaiBagItemComponent> callback)
	{
		self.callback = callback;
	}
	public static void SetIndex(this YuWaiBagItemComponent self, int index)
	{
		self.index = index;
	}
	public static void SetLock(this YuWaiBagItemComponent self)
	{
		self.slotStateCtrl.selectedIndex = 1;
	}
	public static void SetEmpty(this YuWaiBagItemComponent self)
	{
		self.slotStateCtrl.selectedIndex = 2;
	}
	public static void SetItem(this YuWaiBagItemComponent self, CommItemInfo info)
	{
		self.slotStateCtrl.selectedIndex = 0;
		if (info.type == (int)MailAttachType.ITEM)
		{
			self.textName.text = ItemUtil.GetItemName(info.modelId);
			self.textNum.text = info.amount.ToString();
			self.textType.text = ItemUtil.GetItemTypeDes(info.modelId);
		}
		else
		{
			self.textName.text = info.modelId.ToString();
			self.textNum.text = info.amount.ToString();
			self.textType.text = ((MailAttachType)info.type).ToString();
		}
		
	}
	public static void ClickItem(this YuWaiBagItemComponent self)
	{
		self.callback?.Invoke(self);
	}
	public static bool IsEmpty(this YuWaiBagItemComponent self)
	{
		return self.slotStateCtrl.selectedIndex == 2;
	}
	public static bool IsLock(this YuWaiBagItemComponent self)
	{
		return self.slotStateCtrl.selectedIndex == 1;
	}
}