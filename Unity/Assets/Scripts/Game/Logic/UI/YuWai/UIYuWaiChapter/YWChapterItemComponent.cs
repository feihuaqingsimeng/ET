using ET;
using FairyGUI;
using GameEvent;
using System;
using Frame;

public class YWChapterItemComponent: UIBase
{
	public GTextField textTitle;
	public GTextField textLimit;
	public GTextField textStage1;
	public GTextField textStage2;
	public Controller isUnlockCtrl;
	public Controller isSelectCtrl;
	public Controller isInCtrl;

	public Action<YWChapterItemComponent> callback;
	public int index;
	public ConfigData cfg;
	public int limitStart;
	public int limitEnd;
}
public class YWChapterItemComponentAwakeSystem : AwakeSystem<YWChapterItemComponent, GComponent>
{
	protected override void Awake(YWChapterItemComponent self, GComponent a)
	{
		self.Awake(a);
		self.textTitle = a.GetText("title");
		self.textLimit = a.GetText("limit");
		self.textStage1 = a.GetText("stage1");
		self.textStage2 = a.GetText("stage2");
		self.isUnlockCtrl = a.GetController("isUnlock");
		self.isSelectCtrl = a.GetController("select");
		self.isInCtrl = a.GetController("isIn");

		a.onClick.Add(self.ClickItem);
	}
}
public static class YWChapterItemComponentSystem
{
	public static void Set(this YWChapterItemComponent self,int index,int id)
	{
		self.index = index;
		var sys = ConfigSystem.Ins;
		self.cfg = sys.GetYWTotalMap(id);
		self.textTitle.text = self.cfg.Get<string>("MapName");
		var limit = self.cfg.Get<int[]>("LvLimit");
		self.limitStart = limit[0];
		self.limitEnd = limit[1];
		var startCfg = sys.GetLvConfig(self.limitStart);
		var endCfg = sys.GetLvConfig(self.limitEnd);
		self.textStage1.text = startCfg.Get<string>("Describe").Substring(0, 2);
		self.textStage2.text = endCfg.Get<string>("Describe").Substring(0, 2);
		var level = DataSystem.Ins.PlayerModel.level;
		if (level >= self.limitEnd || (level >= self.limitStart && level <= self.limitEnd))
			self.isUnlockCtrl.selectedIndex = 1;
		else
			self.isUnlockCtrl.selectedIndex = 0;
	}
	public static void SetSelect(this YWChapterItemComponent self,bool isSelect)
	{
		self.isSelectCtrl.selectedIndex = isSelect ? 1 : 0;
	}
	public static void SetIsIn(this YWChapterItemComponent self, bool isIn)
	{
		self.isInCtrl.selectedIndex = isIn ? 1 : 0;
	}
	public static void SetClick(this YWChapterItemComponent self, Action<YWChapterItemComponent> callback)
	{
		self.callback = callback;
	}
	public static void ClickItem(this YWChapterItemComponent self)
	{
		self.callback?.Invoke(self);
	}
}
