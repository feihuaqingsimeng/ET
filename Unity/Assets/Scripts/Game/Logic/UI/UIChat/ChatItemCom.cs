using FairyGUI;
using ET;
using GameEvent;
using Frame;

public class ChatItemCom : UIBase
{
    public GTextField textName;
    public GTextField textContent;
    public Controller ctrlIsMe;

    public ChatInfo info;
}
class ChatItemComAwakeSystem : AwakeSystem<ChatItemCom, GComponent>
{
    protected override void Awake(ChatItemCom self, GComponent a)
	{
		self.Awake(a);
        self.textName = a.GetText("Name");
        self.textContent = a.GetText("Content");
        self.ctrlIsMe = a.GetController("isMe");
	}
}

public static class ChatItemComSystem
{
	public static void Refresh(this ChatItemCom self,ChatInfo info)
	{
        self.info = info;
        self.textName.text = $"【{info.serverId}】";
        self.textContent.text = info.content;
	}
}