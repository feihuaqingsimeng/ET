using ET;
using FairyGUI;
using GameEvent;
using System.Collections.Generic;
using Frame;

public class YWMsgComponent: UIBase
{
	public GList msgItemList;
	public Controller pageCtrl;

	public List<YWNoticeInfo> dataList = new List<YWNoticeInfo>();
}
public class YWMsgComponentAwakeSystem : AwakeSystem<YWMsgComponent, GComponent>
{
	protected override void Awake(YWMsgComponent self, GComponent a)
	{
		self.Awake(a);
		self.msgItemList = a.GetList("msgList");
		self.pageCtrl = a.GetController("page");

		self.msgItemList.itemRenderer = self.ItemRender;
		self.pageCtrl.onChanged.Add(self.PangeChange);

        self.RegisterEvent<YWNoticeInfoChangeEvent>(self.Event_YWNoticeInfoChangeEvent);
		self.Refresh();

	}
}
public static class YWMsgComponentSystem
{
	public static void Refresh(this YWMsgComponent self)
	{
		var index = self.pageCtrl.selectedIndex;
		self.dataList.Clear();
		DataSystem.Ins.YuWaiModel.GetYWNoticeInfoList((YWNoticeType)index, self.dataList);
		self.msgItemList.numItems = self.dataList.Count;

	}
	public static void ItemRender(this YWMsgComponent self,int index,GObject go)
	{
		var com = go.asCom;
		com.GetText("msg").text = self.dataList[index].des;
	}
	public static void PangeChange(this YWMsgComponent self)
	{
		self.Refresh();
	}
	public static void Event_YWNoticeInfoChangeEvent(this YWMsgComponent self, YWNoticeInfoChangeEvent e)
	{
		self.Refresh();
	}

}