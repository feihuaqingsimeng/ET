using FairyGUI;
using ET;
using GameEvent;
using Frame;

public class XianTu_BattleItem : UIBase
{
    public GTextField textChapterProgress;
}
class XianTu_BattleItemAwakeSystem : AwakeSystem<XianTu_BattleItem, GComponent>
{
    protected override void Awake(XianTu_BattleItem self, GComponent a)
	{
		self.Awake(a);
        self.textChapterProgress = a.GetText("chapterProgress");
        var btnBG = a.GetButton("btnBG");
        var btnReward = a.GetButton("btnReward");
        btnReward.visible = false;

        btnBG.onClick.Add(self.ClickItem);

        self.RegisterEvent<ChapterLevelChangeEvent>(self.Event_ChapterLevelChangeEvent);

        self.RefreshCurChapter();
    }
}

public static class XianTu_BattleItemSystem
{
	public static void RefreshCurChapter(this XianTu_BattleItem self)
	{
        var model = DataSystem.Ins.ChapterModel;
        if (model.chapter == 0)
            self.textChapterProgress.text = "";
        else
            self.textChapterProgress.text = $"{model.chapter}-{model.level}";
    }
    public static void ClickItem(this XianTu_BattleItem self)
    {
        UISystem.Ins.Show(UIType.UIChapter);
    }
    public static void Event_ChapterLevelChangeEvent(this XianTu_BattleItem self,ChapterLevelChangeEvent e)
    {
        self.RefreshCurChapter();
    }
}