using FairyGUI;
using ET;
using GameEvent;
using module.yw.message;
using Frame;

public class XianTu_YWItem : UIBase
{
    public GTextField textZone;
    public GTextField textNum;
}
class XianTu_YWItemAwakeSystem : AwakeSystem<XianTu_YWItem, GComponent>
{
	protected override void Awake(XianTu_YWItem self, GComponent a)
	{
		self.Awake(a);
        var btnBG = a.GetButton("btnBG");
        self.textZone = a.GetText("zone");
        self.textNum = a.GetCom("CiShu").GetText("num");
        btnBG.onClick.Add(self.ClickItem);

        self.RegisterEvent<YWMapChangeEvent>(self.Event_YWMapChangeEvent);


        NetSystem.Send(new YwOpenReq());

        self.Refresh();
    }
}

public static class XianTu_YWItemSystem
{
	public static void Refresh(this XianTu_YWItem self)
	{
        var data = DataSystem.Ins.YuWaiModel;
        int chapterId = data.GetYWChapterId();
        int mapId = data.ywMapId;
        self.textZone.text = "";
        if (chapterId == 0)
        {
            return;
        }
        var chapterCfg = ConfigSystem.Ins.GetYWTotalMap(chapterId);
        var mapCfg = ConfigSystem.Ins.GetYWLowerMap(mapId);
        if (chapterCfg != null && mapCfg != null)
            self.textZone.text = $"{chapterCfg.Get<string>("MapName")}-{mapCfg.Get<string>("MapName")}";
        var max = ConfigSystem.Ins.GetGlobal("YWDelivery");
        var use = data.GetYWFreeUseCount();
        self.textNum.text = $"{max - use }/{max}";
    }

   
    public static async void ClickItem(this XianTu_YWItem self)
    {
        var token = self.GetToken(1);
        var resp = (YwOpenResp)await NetSystem.Call(new YwOpenReq(), typeof(YwOpenResp), token);
        if (resp == null) return;
        if (resp.lowerMapId == 0)
        {
            UISystem.Ins.Show(UIType.UIYuWaiChapter);
        }
        else
        {
            UISystem.Ins.Show(UIType.UIYuWaiMap);
        }
    }
    public static void Event_YWMapChangeEvent(this XianTu_YWItem self,YWMapChangeEvent e)
    {
        self.Refresh();
    }
}