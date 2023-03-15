using FairyGUI;
using ET;
using GameEvent;
using System.Collections.Generic;
using module.mail.message;
using Frame;

public class UIEmail : UIBase
{
    public GList itemList;

    public List<EmailInfo> dataList = new List<EmailInfo>();
}
class UIEmailAwakeSystem : AwakeSystem<UIEmail, GComponent>
{
    protected override void Awake(UIEmail self, GComponent a)
	{
		self.Awake(a);

        self.itemList = a.GetList("list");

        self.itemList.SetItemRender(self.ItemRender);

        var btnClose = a.GetButton("btnClose");
        var btnDelete = a.GetButton("btnDelete");

        btnClose.onClick.Add(self.CloseSelf);
        btnDelete.onClick.Add(self.ClickDelete);

        self.RegisterEvent<EmailUpdateEvent>(self.Event_EmailUpdateEvent);

        self.Refresh();
	}
}

public static class UIEmailSystem
{
	public static void Refresh(this UIEmail self)
	{
        self.dataList.Clear();
        foreach(var v in DataSystem.Ins.EmailModel.emailDic)
        {
            self.dataList.Add(v.Value);
        }
        self.dataList.Sort(Sort);
        self.itemList.numItems = self.dataList.Count;
	}
    public static int Sort(EmailInfo a,EmailInfo b)
    {
        if (a.status != b.status)
            return a.status - b.status;
        if (a.createTime != b.createTime)
            return (int)(b.createTime - a.createTime);
        return (int)(b.id - a.id);
    }
    public static void ItemRender(this UIEmail self,int index, GObject go)
    {
        var data = self.dataList[index];
        var com = go.asCom;
        var instId = com.displayObject.gameObject.GetInstanceID();
        var child = self.GetChild<EmailItemCom>(instId);
        if(child == null)
        {
            child = self.AddChildWithId<EmailItemCom, GComponent>(instId, com);
        }
        child.Refresh(data);
    }
    public static void ClickDelete(this UIEmail self)
    {
        var req = new MailDeleteReq();
        foreach (var v in self.dataList)
        {
            if (v.status == MailStatus.RECV)
                req.deleteList.Add(v.id);
            else if (v.status == MailStatus.READ && v.rewards.Count == 0)
                req.deleteList.Add(v.id);
        }
        if(req.deleteList.Count == 0)
        {
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("uiemail_nodelete_tip"));
            return;
        }
        NetSystem.Send(req);
    }
    public static void Event_EmailUpdateEvent(this UIEmail self, EmailUpdateEvent e)
    {
        self.Refresh();
    }
}