using FairyGUI;
using ET;
using GameEvent;
using module.mail.message;
using Frame;

public class UIEmailDetail : UIBase
{
    public GTextField textTitle;
    public GTextField textContent;
    public GList itemList;
    public Controller ctrlState;
    public Controller ctrlEmpty;
    public GTextField textTime;

    public EmailInfo info;
    
}
class UIEmailDetailAwakeSystem : AwakeSystem<UIEmailDetail, GComponent>
{
    protected override void Awake(UIEmailDetail self, GComponent a)
	{
		self.Awake(a);
        self.textTitle = a.GetText("title");
        self.textContent = a.GetText("content");
        self.itemList = a.GetList("list");
        self.ctrlState = a.GetController("state");
        self.ctrlEmpty = a.GetController("isEmpty");
        self.textTime = a.GetText("time");

        self.itemList.itemRenderer = self.ItemRenderer;

        var bg = a.GetChild("_bgfull");
        var btnSure = a.GetButton("btnSure");
        bg.onClick.Add(self.CloseSelf);
        btnSure.onClick.Add(self.ClickSure);
	}
}
class UIEmailDetailChangeSystem : ChangeSystem<UIEmailDetail, IUIDataParam>
{
	protected override void Change(UIEmailDetail self, IUIDataParam a)
	{
        var param = (UIEmailDetailParam)a;
        self.Refresh(param.info);
	}
}
public static class UIEmailDetailSystem
{
	public static void Refresh(this UIEmailDetail self,EmailInfo info)
	{
        self.info = info;
        self.textTitle.text = info.title;
        self.textContent.text = info.content;
        self.itemList.numItems = info.rewards.Count;
        self.ctrlState.selectedIndex = info.rewards.Count == 0 ? 0 : (info.status == MailStatus.RECV ? 0 : 1);
        self.ctrlEmpty.selectedIndex = info.rewards.Count == 0 ? 0 : 1;
        self.textTime.text = Util.GetTimeYMDStr(info.createTime);


    }
    public static void ItemRenderer(this UIEmailDetail self,int index,GObject go)
    {
        var data = self.info.rewards[index];
        var com = go.asCom;
        var instId = com.displayObject.gameObject.GetInstanceID();
        var child = self.GetChild<IconItemCom>(instId);
        if (child == null)
        {
            child = self.AddChildWithId<IconItemCom, GComponent>(instId, com);
        }
        child.Set(data);
    }

    public static async void ClickSure(this UIEmailDetail self)
    {
        if(self.info.rewards.Count == 0 || self.info.status == MailStatus.RECV)
        {
            self.CloseSelf();
        }
        else
        {
            var token = self.GetToken(1);
            var req = new MailReceiveReq();
            
            req.reciveList.Add(self.info.id);
            var resp =(MailResp) await NetSystem.Call(req, typeof(MailResp), token);
            if (resp == null) return;
            UISystem.Ins.ShowGainReward(self.info.rewards);
            self.CloseSelf();
        }
    }
}