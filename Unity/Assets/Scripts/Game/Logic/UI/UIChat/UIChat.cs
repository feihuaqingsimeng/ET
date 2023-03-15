using FairyGUI;
using ET;
using GameEvent;
using System.Collections.Generic;
using Frame;

public class UIChat : UIBase
{
    public GList itemList;
    public GTextInput input;
    public Controller ctrlType;

    public ChatType[] typeMapping = new ChatType[2] { ChatType.World, ChatType.Current };
    public List<ChatInfo> infoList = new List<ChatInfo>();
}
class UIChatAwakeSystem : AwakeSystem<UIChat, GComponent>
{
    protected override void Awake(UIChat self, GComponent a)
	{
		self.Awake(a);
        self.itemList = a.GetList("ChatList");
        self.input = a.GetTextInput("input");
        self.ctrlType = a.GetController("type");
        var btnClose = a.GetButton("btnBack");
        var btnSend = a.GetButton("btnSend");

        btnClose.onClick.Add(self.CloseSelf);
        btnSend.onClick.Add(self.ClickSend);
        self.ctrlType.onChanged.Add(self.PageChange);

        self.itemList.SetItemRender(self.ChatItemRender);
        self.Refresh();
	}
}
public static class UIChatSystem
{
	public static void Refresh(this UIChat self)
	{

	}
    public static void InitChatList(this UIChat self)
    {
        self.infoList.Clear();
        var list = DataSystem.Ins.ChatModel.GetChatList(self.typeMapping[self.ctrlType.selectedIndex]);
        foreach(var v in list)
        {
            self.infoList.Add(v);
        }
        self.itemList.numItems = self.infoList.Count;
    }
    public static void RefreshChatList(this UIChat self)
    {
        self.itemList.RefreshVirtualList();
    }
    public static void ChatItemRender(this UIChat self,int index,GObject go)
    {

    }
    public static void ClickSend(this UIChat self)
    {

    }
    public static void PageChange(this UIChat self)
    {

    }
}