using FairyGUI;
using ET;
using GameEvent;
using module.checkpoints.message;
using System.Collections.Generic;
using UnityEngine;
using module.yw.message.vo;
using Frame;

public class UIChapter : UIBase
{
    public GTextField textChapter;
    public GList listItemChapter;
    public GList listAwardItem;
    public GButton btnBattle;
    public GButton btnChapter;
    public Controller ctrlIsBattle;

    public bool isShowLevel;
    public int selectChapter;
    public int selectLevel;
    public bool isSweep;

    public ChapterItemComponent selectItem;

    public const int STATE_LOCK = 0;
    public const int STATE_PASS = 1;
    public const int STATE_UNLOCK = 2;

    public int[][] rewardShowList;
    public List<int> chapterDataList = new List<int>();

    public UIBattle uiBattle;
    public bool isInBattle;
    public bool isHide;
}
class UIChapterAwakeSystem : AwakeSystem<UIChapter, GComponent>
{
    protected override void Awake(UIChapter self, GComponent a)
	{
		self.Awake(a);

        self.textChapter = a.GetCom("btn_chapter").GetText("Chapter");
        self.listItemChapter = a.GetList("listLevel");
        self.listAwardItem = a.GetCom("award").GetList("listReward");
        self.ctrlIsBattle = a.GetController("isBattle");
        var btnChapter = a.GetButton("btn_chapter");
        var btnClose = a.GetButton("btnClose");
        var btnBattle = a.GetButton("btnBattle");
        self.btnBattle = btnBattle;
        self.btnChapter = btnChapter;
        self.uiBattle = self.AddComponent<UIBattle, GComponent>(a.GetCom("battle"));

        btnChapter.onClick.Add(self.ClickChapter);
        btnBattle.onClick.Add(self.ClickBattle);
        btnClose.onClick.Add(self.ClickClose);

        self.listItemChapter.itemRenderer = self.ChapterItemRender;
        self.listAwardItem.itemRenderer = self.AwardItemRender;

        var chapterModel = DataSystem.Ins.ChapterModel;
        chapterModel.GetCurNext(out self.selectChapter, out self.selectLevel);

        self.Refresh();
	}
}
public class UIChapterChangeSystem : ChangeSystem<UIChapter, IUIDataParam>
{
    protected override void Change(UIChapter self, IUIDataParam a)
    {
        if (self.isHide)
            self.gCom.visible = true;
        self.isHide = false;
    }
}

public static class UIChapterSystem
{
	public static void Refresh(this UIChapter self)
	{
        self.RefreshTitle();
        self.RefreshChapter();
        self.RefreshAward();
        self.RefreshBattleBtn();
        self.uiBattle.Init(self.BattleFinish);
        self.RefreshTeam();
    }
    public static void RefreshTeam(this UIChapter self)
    {
        self.uiBattle.ResetTeamItem();
        var teamList = self.uiBattle.teamItemList[1];
        //敌方
        var cfg = DataSystem.Ins.ChapterModel.GetCfg(self.selectChapter, self.selectLevel);
        var monsters = cfg.Get<int[][]>("Monster");
        for(int i = 0; i < monsters.Length; i++)
        {
            var m = monsters[i];
            int pos = m[0]-1;
            int id = m[1];
            var monsterCfg = ConfigSystem.Ins.GetMonster(id);
            teamList[pos].Set(new YwPlayerSimpleInfo() { name = monsterCfg.Get<string>("Name") });
        }
        self.uiBattle.InitOurDefaultTeam();
        self.uiBattle.ResetTurn();
    }
    public static void RefreshTitle(this UIChapter self)
    {
        self.textChapter.text = $"第{self.selectChapter}章";
    }
    public static void RefreshChapter(this UIChapter self)
    {
        self.isShowLevel = false;
        self.listItemChapter.numItems = DataSystem.Ins.ChapterModel.chapterDic.Count;
    }
    
    public static void RefreshLevel(this UIChapter self)
    {
        self.isShowLevel = true;
        var chapterModel = DataSystem.Ins.ChapterModel;
        chapterModel.chapterDic.TryGetValue(self.selectChapter, out var levels);
        if (self.selectChapter == chapterModel.chapter)
            self.selectLevel = Mathf.Min( chapterModel.level + 1,levels.Count);
        else
            self.selectLevel = 1;
        self.listItemChapter.numItems = levels.Count;
        self.RefreshTitle();
    }
    public static void ChapterItemRender(this UIChapter self,int index,GObject go)
    {

        var com = go.asCom;
        var id = go.displayObject.gameObject.GetInstanceID();
        var item = self.GetChild<ChapterItemComponent>(id);
        if(item == null)
        {
            item = self.AddChildWithId<ChapterItemComponent, GComponent>(id, com);
            item.SetClick(self.ClickChapterItem);
        }
        var chapterModel = DataSystem.Ins.ChapterModel;
        index = index + 1;
        
        if (self.isShowLevel)
        {
            item.Set(self.selectChapter,index, self.isShowLevel);
            item.SetSelect(index == self.selectLevel);
            if (index == self.selectLevel)
            {
                self.selectItem = item;
            }
        }
        else
        {
            item.Set(index, 0, self.isShowLevel);
            item.SetSelect(index == self.selectChapter);
        }
        
    }
    public static void AwardItemRender(this UIChapter self,int index,GObject go)
    {
        var data = self.rewardShowList[index];
        var com = go.asCom;
        com.GetText("award").text = $"{ItemUtil.GetItemName(data[0])}*{data[1]}";
    }
    public static void RefreshAward(this UIChapter self)
    {
        if (self.isShowLevel)
        {
            var model = DataSystem.Ins.ChapterModel;
            if (self.isSweep)
                self.rewardShowList = model.GetSweepRewardList(self.selectChapter, self.selectLevel);
            else
                self.rewardShowList = model.GetFirstRewardList(self.selectChapter, self.selectLevel);

            self.listAwardItem.numItems = self.rewardShowList.Length;
        }
        else
        {
            self.listAwardItem.numItems = 0;
        }
        
    }
    public static void RefreshBattleBtn(this UIChapter self)
    {
        var model = DataSystem.Ins.ChapterModel;
        model.GetCurNext(out int nextChapter, out int nextLevel);
        self.isSweep = self.selectChapter < nextChapter || self.selectChapter == nextChapter && self.selectLevel < nextLevel;
        if (self.isSweep)
            self.btnBattle.title = ConfigSystem.Ins.GetLanguage("uichapter_btnsweep");
        else
            self.btnBattle.title = ConfigSystem.Ins.GetLanguage("uichapter_btnbattle");

    }
    public static void ClickChapterItem(this UIChapter self,ChapterItemComponent item)
    {
        DataSystem.Ins.ChapterModel.GetCurNext(out int nextChapter, out int nextLevel);
        if (self.isShowLevel)
        {
            if (item.GetState() == UIChapter.STATE_LOCK)
            {
                UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("ui_chapter_unlockbyprelevel")});
                return;
            }
            self.selectLevel = item.num;
            if (self.selectItem != null)
                self.selectItem.SetSelect(false);
            self.selectItem = item;
            item.SetSelect(true);
            self.RefreshBattleBtn();
            self.RefreshTeam();
        }
        else
        {
            if (item.GetState() == UIChapter.STATE_LOCK)
            {
                var cfg = ConfigSystem.Ins.GetChapterUnlock(item.num);
                var lv = cfg.Get("UnlockLevel");
                string content = "";
                if (DataSystem.Ins.PlayerModel.level < lv)
                {
                    var lvCfg = ConfigSystem.Ins.GetLvConfig(lv);
                    content = string.Format(ConfigSystem.Ins.GetLanguage("ui_chapter_unlocktip"), lvCfg.Get<string>("Describe"));
                }
                else
                    content = ConfigSystem.Ins.GetLanguage("uichapter_unlockbyprelevel");
                UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = content });
                return;
            }
            self.selectChapter = item.num;
            self.RefreshLevel();
        }
        self.RefreshAward();
    }
    public static void ClickChapter(this UIChapter self)
    {
        self.RefreshChapter();
        self.RefreshAward();
    }
    public static void ClickClose(this UIChapter self)
    {
        if (self.isInBattle)
        {
            self.isHide = true;
            self.gCom.visible = false;
            return;
        }
        UISystem.Ins.Close(UIType.UIChapter);
        
    }
    
    public static async void ClickBattle(this UIChapter self)
    {
        if (!self.isShowLevel)
        {
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("uichapter_choicelevel_tip"));
            return;
        }
        if (self.isSweep)
        {

        }
        var token = self.GetToken(1);
        var resp = (CheckPointsBattleResp) await NetSystem.Call(new CheckPointsBattleReq(), typeof(CheckPointsBattleResp), token);
        if (resp == null) return;
        self.isInBattle = true;
        self.uiBattle.RequestReport(resp.record);
        self.ctrlIsBattle.selectedIndex = 1;
    }
    public static async void BattleFinish(this UIChapter self,bool isWin)
    {
        var token = self.GetToken(1);
        while (true)
        {
            var flag = await TimerComponent.Instance.WaitFrameAsync(token);
            if (!flag) return;
            if (self.uiBattle.IsDeadAnimFinish())
                break;
        }
        
        self.ctrlIsBattle.selectedIndex = 0;
        self.RefreshLevel();
        self.RefreshAward();
        self.RefreshBattleBtn();
        self.RefreshTeam();
        if (isWin)
        {
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("battle_win_tip"));
            UISystem.Ins.ShowFlowTipView(DataSystem.Ins.ChapterModel.rewards);
        }
        else
        {
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("battle_failed_tip"));
        }
            
        self.isInBattle = false;
        if (self.isHide)
            self.ClickClose();
    }

}