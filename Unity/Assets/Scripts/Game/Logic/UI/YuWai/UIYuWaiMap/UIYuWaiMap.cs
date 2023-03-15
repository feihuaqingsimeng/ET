using ET;
using FairyGUI;
using GameEvent;
using module.yw.message;
using Frame;

public class UIYuWaiMap: UIBase
{
	public GTextField textLingli;
	public GTextField textTimeRecover;
	public GTextField textTitle;

	public YWMapComponent mapCom;
	public YWMapDesComponent mapDesCom;
	public YWMsgComponent msgCom;
	public bool isStartRecover;
}
public class UIYuWaiMapAwakeSystem : AwakeSystem<UIYuWaiMap, GComponent>
{
	protected override void Awake(UIYuWaiMap self, GComponent a)
	{
		self.Awake(a);
		self.textLingli = a.GetText("textLingli");
		self.textTimeRecover = a.GetText("textTime");
		self.textTitle = a.GetText("title");

		var btnClose = a.GetButton("btnClose");
		var btnLeave = a.GetButton("btnLeave");
		var btnMap = a.GetButton("btnMap");
		var btnAdd = a.GetButton("btnAdd");
		var btnBag = a.GetButton("btnBag");


		self.mapCom = self.AddComponent<YWMapComponent, GComponent>(a.GetCom("map"));
		self.mapDesCom = self.AddComponent<YWMapDesComponent, GComponent>(a.GetCom("mapDes"));
		self.msgCom = self.AddComponent<YWMsgComponent, GComponent>(a.GetCom("msg"));

		

		btnClose.onClick.Add(self.ClickClose);
		btnLeave.onClick.Add(self.ClickLeave);
		btnMap.onClick.Add(self.ClickMap);
		btnAdd.onClick.Add(self.ClickLingLiAdd);
		btnBag.onClick.Add(self.ClickBag);

        self.RegisterEvent<YWLingLiChangeEvent>(self.Event_YWLingLiChangeEvent);

		if (!DataSystem.Ins.YuWaiModel.ywNoticeInfoIsRequest)
			NetSystem.Send(new YwInformationListReq());
	}
}
public class UIYuWaiMapChangeSystem : ChangeSystem<UIYuWaiMap, IUIDataParam>
{
	protected override void Change(UIYuWaiMap self, IUIDataParam a)
	{
		self.Refresh();
	}
}
public static class UIYuWaiMapSystem
{
	public static void Refresh(this UIYuWaiMap self)
	{
		var data = DataSystem.Ins.YuWaiModel;
		int chapterId = data.GetYWChapterId();
		int mapId = data.ywMapId;
		var chapterCfg = ConfigSystem.Ins.GetYWTotalMap(chapterId);
		var mapCfg = ConfigSystem.Ins.GetYWLowerMap(mapId);
		if(chapterCfg!= null && mapCfg!= null)
			self.textTitle.text = $"{chapterCfg.Get<string>("MapName")}-{mapCfg.Get<string>("MapName")}";
		else
			self.textTitle.text = "";
		 
		self.mapCom.Refresh();
		self.mapDesCom.Refresh();
		self.msgCom.Refresh();
		self.RefreshLingli();
	}
	public static void RefreshLingli(this UIYuWaiMap self)
	{
		var data = DataSystem.Ins.YuWaiModel;
		var limit = ConfigSystem.Ins.GetGlobal("YWManaUP");
		self.textLingli.text = $"{data.ywLingli}/{limit}";
		self.UpdateLingliRecoverTime();
	}
	public static void CloseRecoverTime(this UIYuWaiMap self)
	{
		self.CancelToken(1);
	}
	public static async void UpdateLingliRecoverTime(this UIYuWaiMap self)
	{
		var data = DataSystem.Ins.YuWaiModel;
		var recovery = ConfigSystem.Ins.GetGlobal("YWManaRecovery")*60*1000;
		var limit = ConfigSystem.Ins.GetGlobal("YWManaUP");
		if (data.ywLingli >= limit)
		{
			self.textTimeRecover.text = "";
			self.CancelToken(1);
			self.isStartRecover = false;
			return;
		}
		if (self.isStartRecover) return;
		self.isStartRecover = true;
		while (true)
		{
			var time = recovery -(TimeInfo.Instance.ServerNow() - data.ywLingLiLastUpdateTime);
			if(time <= 0)
			{
				data.ywLingLiLastUpdateTime = TimeInfo.Instance.ServerNow();
				data.ywLingli++;
				self.RefreshLingli();
				if (data.ywLingli == limit)
				{
					self.textTimeRecover.text = "";
					self.isStartRecover = false;
					return;
				}
			}
			self.textTimeRecover.text = Util.GetTimeMSStr(time);
			bool flag;
			if (time % 1000 != 0)
				flag = await TimerComponent.Instance.WaitAsync(time % 1000, self.GetToken(1));
			else
				flag = await TimerComponent.Instance.WaitAsync(1000, self.GetToken(1));
			if (!flag)
			{
				self.isStartRecover = false;
				return;
			}
			
		}
	}
	
	public static void MapMoveFinished(this UIYuWaiMap self)
	{
		self.mapDesCom.Refresh();
	}
	public static void ClickClose(this UIYuWaiMap self)
	{
		UISystem.Ins.Close(UIType.UIYuWaiMap);
	}
	public static void ClickLeave(this UIYuWaiMap self)
	{
		self.ExitReq();
	}
	public static async void ExitReq(this UIYuWaiMap self)
	{
		var token = self.GetToken(2);
		var resp = (YwExitResp) await NetSystem.Call(new YwExitReq(), typeof(YwExitResp), token);
		if (resp == null) return;
		self.ClickClose();
	}
	public static void ClickMap(this UIYuWaiMap self)
	{

		UISystem.Ins.Show(UIType.UIYuWaiChapter);
	}
	public static void ClickBag(this UIYuWaiMap self)
	{
		UISystem.Ins.Show(UIType.UIYuWaiBag);
	}
	public static void ClickLingLiAdd(this UIYuWaiMap self)
	{
		var param = ConfigSystem.Ins.GetGlobal<int[]>("YWManaBuy");
		var data = DataSystem.Ins.YuWaiModel;
		var name = ItemUtil.GetItemName(param[0]);
		var price = param[1] + data.GetYWLingLiBuyCount() / param[2] * param[3];
		var num = param[4];
		var des = string.Format(ConfigSystem.Ins.GetLanguage("yuwai_buyitemtip"),price,name,num);
		var uiParam = new UICommonTipParam() { des = des, sureCallback = self.AddLingLiSure };
		UISystem.Ins.Show(UIType.UICommonTip, uiParam);
	}
	public static async void AddLingLiSure(this UIYuWaiMap self)
	{
		var token = self.GetToken(3);
		var resp = (YwLingLiUpdateResp) await NetSystem.Call(new YwBuyLingLiReq(), typeof(YwLingLiUpdateResp), token);
		if (resp == null) return;
	}
	public static void Event_YWLingLiChangeEvent(this UIYuWaiMap self, YWLingLiChangeEvent e)
	{
		self.RefreshLingli();
	}
}