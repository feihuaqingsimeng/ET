using ET;
using FairyGUI;
using Frame;
using module.mail.message;

public class EmailItemCom : UIBase
{
    public GTextField textTitle;
    public GTextField textContent;
    public Controller ctrlState;

    public EmailInfo info;
}
public class EmailItemComAwakeSystem : AwakeSystem<EmailItemCom,GComponent>
{
    protected override void Awake(EmailItemCom self, GComponent a)
    {
        self.Awake(a);
        self.textTitle = a.GetText("title");
        self.textContent = a.GetText("content");
        self.ctrlState = a.GetController("state");
        a.onClick.Add(self.ClickItem);
    }
}
public static class EmailItemComSystem
{
    public static void Refresh(this EmailItemCom self,EmailInfo info)
    {
        self.info = info;
        self.textTitle.text = info.title;
        self.textContent.text = info.content;
        if (info.status == MailStatus.RECV)
            self.ctrlState.selectedIndex = 2;
        else if (info.status == MailStatus.READ)
            self.ctrlState.selectedIndex = 1;
        else
            self.ctrlState.selectedIndex = 0;
    }
    public static void ClickItem(this EmailItemCom self)
    {
        if (self.info.status == MailStatus.CREATE)
        {
            var req = new MailReadReq();
            req.readList.Add(self.info.id);
            NetSystem.Send(req);
        }
        UISystem.Ins.Show(UIType.UIEmailDetail,new UIEmailDetailParam() { info = self.info });
    }
}
