using FairyGUI;
using ET;
using GameEvent;
using Frame;

class UIHome : UIBase
{
	public GTextField textName;
	public GTextField textPlayerId;
	public GTextField textStage;
	public GTextField textStageLevel;
	public GTextField textNeedExp;
	public GTextField textCurExp;

	public GButton btnLevelUp;

	public ResourceMultiItemComponent multResCom;
	public ExpItemComponent expBarComp;

}
class UIHomeAwakeSystem : AwakeSystem<UIHome, GComponent>
{
	protected override void Awake(UIHome self, GComponent a)
	{
        AnalysisSystem.Ins.StartRecordTime("UIHome awake1");
		self.Awake(a);
		self.textName = a.GetText("playerName");
		self.textPlayerId = a.GetText("uid");
		self.textStage = a.GetText("textStage");
		self.textStageLevel = a.GetText("textStageLevel");
		self.textNeedExp = a.GetText("textNeedExp");
		self.textCurExp = a.GetText("textCurExp");

		self.btnLevelUp = a.GetButton("btnLevelUp");
		var btnAttr = a.GetButton("btnAttr");
		var btnGM = a.GetButton("btnGM");
		var btnLianDan = a.GetButton("btnLianDan");
		var btnLianQi = a.GetButton("btnLianQi");
        var btnEmail = a.GetButton("btnEmail");
       
        self.multResCom = self.AddChild<ResourceMultiItemComponent, GComponent>(a.GetCom("resBar"));
        AnalysisSystem.Ins.EndRecordTime("UIHome awake1");
        AnalysisSystem.Ins.StartRecordTime("UIHome awake2");
        self.multResCom.Add(ResourceType.XianYu);
        self.multResCom.Add(ResourceType.LingShi);
        AnalysisSystem.Ins.EndRecordTime("UIHome awake2");
        AnalysisSystem.Ins.StartRecordTime("UIHome awake3");
        self.expBarComp = self.AddChild<ExpItemComponent, GComponent>(a.GetCom("expBar"));
       
        self.btnLevelUp.onClick.Add(self.ClickLevelUp);
		btnAttr.onClick.Add(self.ClickAttr);
		btnGM.onClick.Add(self.ClickGM);
		btnLianDan.onClick.Add(self.ClickLianDan);
		btnLianQi.onClick.Add(self.ClickLianQi);
        btnEmail.onClick.Add(self.ClickEmail);
        
        self.Refresh();
        self.expBarComp.Refresh();

        self.RegisterEvent<LevelUpEvent>(self.Event_LevelUp);
        self.RegisterEvent<ExpAutoAddEvent>(self.Event_ExpAutoAdd);
        AnalysisSystem.Ins.EndRecordTime("UIHome awake3");
    }
}
static class UIHomeSystem
{
	public static void Refresh(this UIHome self)
	{
		var data = DataSystem.Ins;
		self.textName.text = data.AccountModel.accountName;
		self.textPlayerId.text = "UID：" + data.PlayerModel.playerId.ToString();

		self.UpdateExp();

	}

	public static void UpdateExp(this UIHome self)
	{
		var data = DataSystem.Ins.PlayerModel;
		self.textCurExp.text = data.exp.ToString();
		var nextConfig = ConfigSystem.Ins.GetLvConfig(data.level + 1);
		if (nextConfig == null)
		{
			self.textNeedExp.text = ConfigSystem.Ins.GetLanguage("uihome_max_level_tip");
			return;
		}

		var config = ConfigSystem.Ins.GetLvConfig(data.level);
		if (config == null)
		{
			Log.Error($"当前等级错误：{data.level}");
			return;
		}
		self.textNeedExp.text = config.Get("UpgradeExp").ToString();
		self.textStage.text = config.Get<string>("Describe");
		self.textStageLevel.text = config.Get<string>("Describe2");
		self.btnLevelUp.enabled = data.CheckExpEnough();
	}
	public static void ClickLevelUp(this UIHome self)
	{
		UISystem.Ins.Show(UIType.UILevelUpTip);
	}
	public static void ClickAttr(this UIHome self)
	{
		UISystem.Ins.Show(UIType.UIAttrDetail);
	}
	public static void ClickGM(this UIHome self)
	{
		UISystem.Ins.Show(UIType.UIGM);
	}
	
	public static void ClickLianDan(this UIHome self)
	{
        UISystem.Ins.Show(UIType.UILianDan);
	}
	public static void ClickLianQi(this UIHome self)
	{
		UISystem.Ins.Show(UIType.UIFaBaoLianQi);
	}
	public static void ClickEmail(this UIHome self)
    {
        UISystem.Ins.Show(UIType.UIEmail);
    }


    public static void Event_LevelUp(this UIHome self, LevelUpEvent e)
	{
		self.UpdateExp();
	}
	public static void Event_ExpAutoAdd(this UIHome self, ExpAutoAddEvent e)
	{
		self.UpdateExp();
		self.expBarComp.Refresh();
	}
}