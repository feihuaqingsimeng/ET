using ET;
using FairyGUI;
using GameEvent;
using module.yw.message.vo;
using module.yw.message;
using Frame;

public class UIYWBattle:UIBase
{
    public GButton btnBattle;
    public YwPlayerVo playerVo;

    public UIBattle uiBattle;
}
public class UIYWBattleAwakeSystem : AwakeSystem<UIYWBattle, GComponent>
{
    protected override void Awake(UIYWBattle self, GComponent a)
    {
        self.Awake(a);
        self.uiBattle = self.AddComponent<UIBattle, GComponent>(a.GetCom("battle"));
        var btnClose = a.GetButton("btnClose");
        var btnBattle = a.GetButton("btnBattle");
        self.btnBattle = btnBattle;
        btnBattle.onClick.Add(self.ClickBattle);
        btnClose.onClick.Add(self.ClickClose);

        self.uiBattle.Init(self.BattleFinish);
    }
}
class UIYWBattleChangeSystem : ChangeSystem<UIYWBattle, IUIDataParam>
{
    protected override void Change(UIYWBattle self, IUIDataParam a)
    {
        var param = (UIYWBattleParam)a;
        self.playerVo = param.player;
        self.Init();
    }
}

public static class UIYWBattleSystem
{
    public static void Init(this UIYWBattle self)
    {
        self.uiBattle.ResetTeamItem();
        var teamList = self.uiBattle.teamItemList[1];
        //敌方
        teamList[ConstValue.PLAYER_SITE].Set(self.playerVo.simpleInfo);
        if (self.playerVo.petList != null)
        {
            foreach(var v in self.playerVo.petList)
            {
                teamList[v.site].Set(v);
            }
        }
        self.uiBattle.InitOurDefaultTeam();
    }
    public static async void ClickBattle(this UIYWBattle self)
    {
        var token = self.GetToken(1);
        var data = DataSystem.Ins.YuWaiModel;
        var req = new YwPkReq
        {
            lowerMapId = data.ywMapId,
            x = data.ywCurCol,
            y = data.ywCurRow,
            targetPlayerId = self.playerVo.simpleInfo.playerId,
        };
        self.btnBattle.visible = false;
        var resp = (YwPkResp)await NetSystem.Call(req, typeof(YwPkResp), token);
        if (resp == null) return;
        self.uiBattle.RequestReport(resp.record);
    }
    
    public static void BattleFinish(this UIYWBattle self,bool isWin)
    {

    }
    public static void ClickClose(this UIYWBattle self)
    {
        UISystem.Ins.Close(UIType.UIYWBattle);
        if (self.uiBattle.record != null && self.uiBattle.record.winner != 1)//进攻方失败
        {
            UISystem.Ins.Close(UIType.UIYuWaiMap);
        }
        self.ShowBattleResult();
    }
    public static void ShowBattleResult(this UIYWBattle self)
    {
        if (self.uiBattle.record == null) return;
        string str = "";
        if (self.uiBattle.record.winner == 1)//进攻方胜利
        {
            str = ConfigSystem.Ins.GetLanguage("yuwai_battle_win");
        }
        else
        {
            str = ConfigSystem.Ins.GetLanguage("yuwai_battle_failed");
        }
        UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = str });
    }
}
