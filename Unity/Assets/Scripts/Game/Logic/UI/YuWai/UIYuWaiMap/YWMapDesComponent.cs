using ET;
using FairyGUI;
using GameEvent;
using module.yw.message;
using module.yw.message.vo;
using Frame;

public class YWMapDesComponent: UIBase
{
	public GTextField textName;
	public GTextField textTime;
	public GTextField textDes;
	public GTextField emptyName;
	public GTextField emptyDes;
	public GTextField textCost;
	public GTextField textExploreTitle;
	public Controller stateCtrl;
	public Controller isExploreCtrl;
	public GList playerItemList;

	public int during;
	public YWPosInfo info;
	public YwPlayerVo pkPlayer;

}
public class YWMapDesComponentAwakeSystem : AwakeSystem<YWMapDesComponent, GComponent>
{
	protected override void Awake(YWMapDesComponent self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("name");
		self.textTime = a.GetText("time");
		self.textDes = a.GetText("des");
		self.emptyName = a.GetText("emptyName");
		self.emptyDes = a.GetText("emptyDes");
		self.stateCtrl = a.GetController("state");
		self.isExploreCtrl = a.GetController("isExplore");
		self.playerItemList = a.GetList("playerList");

		var btnExplore = a.GetButton("btnExplore");
		self.textExploreTitle = btnExplore.GetText("title");
		self.textCost = btnExplore.GetText("cost");

		btnExplore.onClick.Add(self.ClickExplore);


		self.playerItemList.itemRenderer = self.PlayerItemRender;
		self.playerItemList.onClickItem.Add(self.ClickPlayer);

        self.RegisterEvent<YWPosInfoChangeEvent>(self.Event_YWPosInfoChange);
        self.RegisterEvent<YWMiJingChangeEvent>(self.Event_YWMiJingChangeEvent);
	}
}
public static class YWMapDesComponentSystem
{
	public static void Refresh(this YWMapDesComponent self)
	{
		self.RefreshEvent();
		self.RefreshPlayer();
	}
	public static void RefreshEvent(this YWMapDesComponent self)
	{
		self.during = 0;
		self.CloseTime();
		self.textCost.text = string.Format(ConfigSystem.Ins.GetLanguage("yuwai_cost_lingqi"),ConfigSystem.Ins.GetGlobal("YWActionConsume"));
		var data = DataSystem.Ins.YuWaiModel;
		YWPosInfo info = data.GetYWPosInfo(data.ywCurRow, data.ywCurCol);
		self.info = info;
		if (info == null || !info.tansuo)
		{
			self.isExploreCtrl.selectedIndex = 1;
			self.stateCtrl.selectedIndex = 0;
			self.emptyName.text = ConfigSystem.Ins.GetLanguage("yuwai_unexplorearea");
			self.emptyDes.text = ConfigSystem.Ins.GetLanguage("yuwai_exploretip");
			return;
		}
		if ((info.tansuo && info.miJing == null)||info.miJing.hadTanSuo)
		{
			self.isExploreCtrl.selectedIndex = 0;
			self.stateCtrl.selectedIndex = 1;
			self.emptyName.text = ConfigSystem.Ins.GetLanguage("yuwai_normalarea");
			self.emptyDes.text = ConfigSystem.Ins.GetLanguage("yuwai_normalexploretip");
			return;
		}
		self.stateCtrl.selectedIndex = 2;
		var cfg = ConfigSystem.Ins.GetYWEvent(info.miJing.eventId);
		if (cfg == null) return;
		
		self.textName.text = cfg.Get<string>("EventName");
		
		self.textDes.text = cfg.Get<string>("Explain");
		self.during = cfg.Get("Time") * 60*1000;
		self.UpdateTime();
		self.isExploreCtrl.selectedIndex = 1;
	}
	public static void CloseTime(this YWMapDesComponent self)
	{
		self.CancelToken(1);
	}
	public static async void UpdateTime(this YWMapDesComponent self)
	{
		while (true)
		{
			long time = self.during - (TimeInfo.Instance.ServerNow()-self.info.miJing.startTime);
			self.textTime.text = string.Format(ConfigSystem.Ins.GetLanguage("yuwai_diappeartime"),Util.GetTimeHMSStr(time)) ;
			var flag = true;
			if (time % 1000 != 0)
				flag = await TimerComponent.Instance.WaitAsync(time % 1000, self.GetToken(1));
			else
				flag = await TimerComponent.Instance.WaitAsync(1000,self.GetToken(1));
			if (!flag) return;
		}
	}
	public static void RefreshPlayer(this YWMapDesComponent self)
	{
		var data = DataSystem.Ins.YuWaiModel;
		YWPosInfo info = data.GetYWPosInfo(data.ywCurRow, data.ywCurCol);
		if (info != null && info.playerList != null)
		{
			self.playerItemList.numItems = info.playerList.Count;
		}
		else
		{
			self.playerItemList.numItems = 0;
		}
	}
	public static void PlayerItemRender(this YWMapDesComponent self, int index, GObject go)
	{
		var data = DataSystem.Ins.YuWaiModel;
		YWPosInfo info = data.GetYWPosInfo(data.ywCurRow, data.ywCurCol);
		var player = info.playerList[index];
		var com = go.asButton;
		com.title = player.simpleInfo.name;
		com.name = index.ToString();
	}
	public static void ClickPlayer(this YWMapDesComponent self, EventContext e)
	{
		var obj = (GObject)e.data;
		int index = int.Parse(obj.name);
		var data = DataSystem.Ins.YuWaiModel;
		YWPosInfo info = data.GetYWPosInfo(data.ywCurRow, data.ywCurCol);
		var player = info.playerList[index];
		self.pkPlayer = player;
		var sys = ConfigSystem.Ins;
		var des = string.Format(sys.GetLanguage("yw_pk_1"),player.simpleInfo.name)+"\n"+sys.GetLanguage("yw_pk_2");
		var cost = sys.GetGlobal<string>("YWMoveConsume");
        var costDes = string.Format(sys.GetLanguage("yuwai_cost_lingqi"), cost);
		var param = new UICostCommonTipParam() { des = des, costDes = costDes, sureCallback = self.ClickPK };
		UISystem.Ins.Show(UIType.UICostCommonTip, param);
	}
	public static void ClickPK(this YWMapDesComponent self)
	{
        UISystem.Ins.Show(UIType.UIYWBattle, new UIYWBattleParam() { player = self.pkPlayer });
        
	}
	public static void ClickExplore(this YWMapDesComponent self)
	{
		if (self.info == null) return;
		self.SendExploreReq().Coroutine();
	}
	public static async ETTask SendExploreReq(this YWMapDesComponent self)
	{
		var data = DataSystem.Ins.YuWaiModel;
		if (!self.info.tansuo)
		{
			var req = new YwTanSuoReq() { lowerMapId = data.ywMapId, x = data.ywCurCol, y = data.ywCurRow };
			var resp = (YwTanSuoResp)await NetSystem.Call(req, typeof(YwTanSuoResp), self.GetToken(2));
			if (resp == null) return;
			self.Refresh();
		}
		else if(!self.info.miJing.hadTanSuo)
		{
			var req = new YwTanSuoMiJingReq() { lowerMapId = data.ywMapId, x = data.ywCurCol, y = data.ywCurRow };
			var resp = (YwTanSuoResp)await NetSystem.Call(req, typeof(YwTanSuoResp), self.GetToken(2));
			if (resp == null) return;
			self.Refresh();
		}
	}

	public static void Event_YWPosInfoChange(this YWMapDesComponent self, YWPosInfoChangeEvent e)
	{
		var data = DataSystem.Ins.YuWaiModel;
		if(e.info.y != data.ywCurRow||e.info.x != data.ywCurCol)
			return;
		self.Refresh();
	}
	public static void Event_YWMiJingChangeEvent(this YWMapDesComponent self, YWMiJingChangeEvent e)
	{
		var data = DataSystem.Ins.YuWaiModel;
		if (e.y != data.ywCurRow || e.x != data.ywCurCol)
			return;
		self.Refresh();
	}
}