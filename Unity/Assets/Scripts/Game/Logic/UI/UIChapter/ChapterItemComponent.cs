using FairyGUI;
using ET;
using GameEvent;
using System;
using Frame;

public class ChapterItemComponent : UIBase
{
    public GTextField textName;
    public Controller ctrlState;
    public Controller ctrlSelect;
    public Action<ChapterItemComponent> callback;
    public int num;
}
class ChapterItemComponentAwakeSystem : AwakeSystem<ChapterItemComponent, GComponent>
{
    protected override void Awake(ChapterItemComponent self, GComponent a)
	{
		self.Awake(a);
        self.textName = a.GetText("title");
        self.ctrlState = a.GetController("state");
        self.ctrlSelect = a.GetController("isSelect");
        a.onClick.Add(self.ClickItem);
    }
}

public static class ChapterItemComponentSystem
{
	public static void Set(this ChapterItemComponent self,int chapter,int level,bool isShowLevel)
    {
        DataSystem.Ins.ChapterModel.GetCurNext(out int nextChapter, out int nextLevel);
        
        if (isShowLevel)
        {
            if (chapter < nextChapter)
                self.ctrlState.selectedIndex = 1;
            else if (chapter == nextChapter)
                self.ctrlState.selectedIndex = level < nextLevel ? 1 : level == nextLevel ? 2 : 0;
            else
                self.ctrlState.selectedIndex = 0;
            self.textName.text = $"第{level}关";
            self.num = level;
        }
        else
        {
            self.textName.text = $"第{chapter}章";
            self.ctrlState.selectedIndex = chapter < nextChapter ? 1 : chapter == nextChapter ? 2 : 0;
            self.num = chapter;
        }
    }
    public static void SetSelect(this ChapterItemComponent self,bool isSelect)
    {
        self.ctrlSelect.selectedIndex = isSelect ? 1 : 0;
    }
    public static int GetState(this ChapterItemComponent self)
    {
        return self.ctrlState.selectedIndex;
    }
    public static void SetClick(this ChapterItemComponent self, Action<ChapterItemComponent> callback)
    {
        self.callback = callback;
    }
    public static void ClickItem(this ChapterItemComponent self)
    {
        self.callback?.Invoke(self);
    }
    

}