using FairyGUI;
using ET;
using GameEvent;
using System.Collections.Generic;
using Frame;

public class UITask : UIBase
{
    public Controller ctrlPage;
    public GList itemList;
    public GTextField textTitle;

    public List<TaskInfo> infoList = new List<TaskInfo>();
}
class UITaskAwakeSystem : AwakeSystem<UITask, GComponent>
{
    protected override void Awake(UITask self, GComponent a)
	{
		self.Awake(a);
        self.textTitle = a.GetCom("title").GetCom("title").GetText("title");
        self.itemList = a.GetList("list");
        self.ctrlPage = a.GetController("page");
        var btnClose = a.GetButton("btnClose");

        btnClose.onClick.Add(self.CloseSelf);

        self.itemList.SetItemRender(self.ItemRender);
        self.ctrlPage.onChanged.Add(self.PageChange);
        self.Refresh();

        self.RegisterEvent<TaskUpdateEvent>(self.Event_TaskUpdateEvent);
    }
}

public static class UITaskSystem
{
	public static void Refresh(this UITask self)
	{
        self.InitTaskList();
	}
    public static void InitTaskList(this UITask self)
    {
        self.infoList.Clear();
        var type = (TaskType)(self.ctrlPage.selectedIndex + 1);
        DataSystem.Ins.TaskModel.GetTaskList(type,self.infoList);
        self.itemList.numItems = self.infoList.Count;
    }
    public static void ItemRender(this UITask self, int index, GObject go)
    {
        var com = go.asCom;
        var info = self.infoList[index];
        var insId = com.displayObject.gameObject.GetInstanceID();
        var item = self.GetChild<TaskItemCom>(insId);
        if(item == null)
        {
            item = self.AddChildWithId<TaskItemCom, GComponent>(insId, com);
        }
        item.Refresh(info);
    }
    public static void PageChange(this UITask self)
    {
        self.InitTaskList();
    }
    public static void Event_TaskUpdateEvent(this UITask self, TaskUpdateEvent e)
    {
        self.InitTaskList();
    }
}