using ET;
using FairyGUI;
using GameEvent;
using module.yw.message;
using Frame;

public class UIYuWaiChapter:UIBase
{
	public GList chapterItemList;
	public GList levelItemList;
	public GTextField textRemainNum;
	public GTextField textTokenNum;

	public ListComponent<ConfigData> chapterList;
	public ListComponent<ConfigData> levelList;
	public int selectChapterIndex;
	public int selectLevelIndex;
	public YWChapterItemComponent selectChapterItem;
	public YWLevelItemComponent selectLevelItem;
	public int inChapterId;
}
public class UIYuWaiChapterAwakeSystem : AwakeSystem<UIYuWaiChapter, GComponent>
{
	protected override void Awake(UIYuWaiChapter self, GComponent a)
	{
		self.Awake(a);
		self.chapterItemList = a.GetList("chapterList");
		self.levelItemList = a.GetList("levelList");
		self.textRemainNum = a.GetText("textRemainNum");
		self.textTokenNum = a.GetText("tokenNum");
		var btnClose = a.GetButton("btnClose");
		var btnTransfer = a.GetButton("btnTransfer");

		self.chapterItemList.itemRenderer = self.ChapterItemRender;
		self.levelItemList.itemRenderer = self.LevelItemRender;

		var pool = self.AddComponent<ListPoolComponent>();
		self.chapterList = pool.Create<ConfigData>();
		self.levelList = pool.Create<ConfigData>();


		btnClose.onClick.Add(self.ClickClose);
		btnTransfer.onClick.Add(self.ClickTransfer);

		self.Refresh();
	}
}
public static class UIYuWaiChapterSystem
{
	public static void Refresh(this UIYuWaiChapter self)
	{
		var data = DataSystem.Ins;
		var max = ConfigSystem.Ins.GetGlobal("YWDelivery");
		var use = data.YuWaiModel.GetYWFreeUseCount();
        self.textRemainNum.text = string.Format(ConfigSystem.Ins.GetLanguage("yuwai_remainfreetime"), max - use, max);
		self.textTokenNum.text = data.ItemModel.GetItemCount(ItemIdType.YuWaiToken).ToString();

		var level = data.PlayerModel.level;
		var dic = ConfigSystem.Ins.GetYWTotalMap();
		self.inChapterId = data.YuWaiModel.GetYWChapterId();
		foreach (var v in dic)
		{
			self.chapterList.Add(v.Value);
		}
		self.chapterList.Sort((a, b) =>
		{
			return a.Get("ID") - b.Get("ID");
		});
		for(int i = 0; i < self.chapterList.Count; i++)
		{
			var v = self.chapterList[i];
			var limit = v.Get<int[]>("LvLimit");
			if (self.inChapterId != 0)
			{
				if (v.Get("ID") == self.inChapterId)
				{
					self.selectChapterIndex = i;
					break;
				}
			}
			else if (level >= limit[0] && level <= limit[1])
			{
				self.selectChapterIndex = i;
				break;
			}
		}
		self.RefreshChapter();
		self.RefreshLevel();
	}
	public static void RefreshChapter(this UIYuWaiChapter self)
	{
		self.chapterItemList.numItems = self.chapterList.Count;
	}
	public static void RefreshLevel(this UIYuWaiChapter self)
	{
		var data = DataSystem.Ins.YuWaiModel;
		var chapterCfg = self.chapterList[self.selectChapterIndex];
		var levels = chapterCfg.Get<int[]>("LowerMap");
		self.levelList.Clear();
		var sys = ConfigSystem.Ins;
		self.selectLevelIndex = 0;
		for (int i = 0; i < levels.Length; i++)
		{
			var cfg = sys.GetYWLowerMap(levels[i]);
			if (cfg != null)
			{
				self.levelList.Add(cfg);
				if(cfg.Get("ID") == data.ywMapId)
					self.selectLevelIndex = i;
			}
		}
		
		self.levelItemList.numItems = self.levelList.Count;
	}
	public static void ChapterItemRender(this UIYuWaiChapter self, int index, GObject go)
	{
		var data = self.chapterList[index];
		var chapter = data.Get("ID");
		var instId = go.displayObject.gameObject.GetInstanceID();
		var item = self.GetChild<YWChapterItemComponent>(instId);
		if (item == null)
		{
			item = self.AddChildWithId<YWChapterItemComponent, GComponent>(instId, go.asCom);
		}
		item.Set(index, chapter);
		item.SetIsIn(chapter == self.inChapterId);
		item.SetSelect(index == self.selectChapterIndex);
		if (index == self.selectChapterIndex)
			self.selectChapterItem = item;
		item.SetClick(self.ClickChapter);
	}
	public static void LevelItemRender(this UIYuWaiChapter self, int index, GObject go)
	{
		var data = self.levelList[index];
		var level = data.Get("ID");
		var instId = go.displayObject.gameObject.GetInstanceID();
		var item = self.GetChild<YWLevelItemComponent>(instId);
		if (item == null)
		{
			item = self.AddChildWithId<YWLevelItemComponent, GComponent>(instId, go.asCom);
		}
		item.Set(index, level);
		item.SetSelect(index == self.selectLevelIndex);
		item.SetIsIn(level == DataSystem.Ins.YuWaiModel.ywMapId);
		if (index == self.selectLevelIndex)
			self.selectLevelItem = item;
		item.SetClick(self.ClickLevel);
	}
	public static void ClickChapter(this UIYuWaiChapter self, YWChapterItemComponent item)
	{
		var level = DataSystem.Ins.PlayerModel.level;
		if (level < item.limitStart)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("yuwai_levelnotenough") });
			return;
		}
		if (self.selectChapterItem != null) self.selectChapterItem.SetSelect(false);
		item.SetSelect(true);
		self.selectChapterItem = item;
		self.selectChapterIndex = item.index;
		self.RefreshLevel();
	}
	public static void ClickLevel(this UIYuWaiChapter self, YWLevelItemComponent item)
	{
		if (self.selectLevelItem != null) self.selectLevelItem.SetSelect(false);
		item.SetSelect(true);
		self.selectLevelItem = item;
		self.selectLevelIndex = item.index;
	}
	public static void ClickClose(this UIYuWaiChapter self)
	{
		UISystem.Ins.Close(UIType.UIYuWaiChapter);
	}
	public static void ClickTransfer(this UIYuWaiChapter self)
	{
        int id = DataSystem.Ins.YuWaiModel.ywMapId;
        if (id > 0)
        {
            if(self.selectLevelItem.cfg.Get("ID") == id)
            {
                UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("yuwai_alreadyinarea") });
                return;
            }
        }
        self.SendYWEntryReq();
	}
	
	public static async void SendYWEntryReq(this UIYuWaiChapter self)
	{

		var cfg = self.levelList[self.selectLevelIndex];
		int mapId = cfg.Get("ID");
		var resp = await NetSystem.Call(new YwEntryReq() { lowerMapId = mapId },typeof(YwEntryResp),self.GetToken(1));
		if (resp == null) return;
		self.ClickClose();
		UISystem.Ins.Show(UIType.UIYuWaiMap);
	}
}