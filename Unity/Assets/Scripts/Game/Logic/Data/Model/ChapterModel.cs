using ET;
using module.battle.domain;
using System.Collections.Generic;

public class ChapterModel:Entity,IAwake
{
    public int chapter = 0;
    public int level = 0;
    public Dictionary<int,Dictionary<int,ConfigData>> chapterDic = new Dictionary<int, Dictionary<int, ConfigData>>(32);
    //result
    public BattleRecord record;
    public List<ItemInfo> rewards = new List<ItemInfo>();
}

public static class ChapterModelSystem
{
    public static void InitChapter(this ChapterModel self, int cfgId)
    {
        self.Init();
        self.GetChapterLevel(cfgId, out self.chapter, out self.level);
    }
    private static void Init(this ChapterModel self)
    {
        if (self.chapterDic.Count > 0) return;
        var cfgs = ConfigSystem.Ins.GetChapter();
        foreach (var v in cfgs)
        {
            int chapter = v.Value.Get("Chapter");
            int level = v.Value.Get("SonID");
            if (self.chapterDic.TryGetValue(chapter, out var levels))
            {
                levels[level] = v.Value;
            }
            else
            {
                levels = new Dictionary<int, ConfigData>(32)
                {
                    { level, v.Value }
                };
                self.chapterDic.Add(chapter, levels);
            }
        }
    }
    public static void GetChapterLevel(this ChapterModel self,int cfgId,out int chapter,out int level)
    {
        chapter = 0;
        level = 0;
        if (cfgId == 0) return;
        var cfg = ConfigSystem.Ins.GetChapter(cfgId);
        if (cfg == null) return;
        chapter = cfg.Get("Chapter");
        level = cfg.Get("SonID");
    }
    public static ConfigData GetCfg(this ChapterModel self,int chapter,int level)
    {
        self.chapterDic.TryGetValue(chapter, out var levels);
        if (levels == null) return null;
        levels.TryGetValue(level, out var cfg);
        return cfg;
    }
    public static void GetCurNext(this ChapterModel self,out int nextChapter,out int nextLevel)
    {
        self.GetNext(self.chapter, self.level,out nextChapter,out nextLevel);
    }
    public static void GetNext(this ChapterModel self, int chapter, int level, out int nextChapter, out int nextLevel)
    {
        nextChapter = 1;
        nextLevel = 1;
        var cfg = self.GetCfg(chapter, level);
        if (cfg == null) return;
        self.GetChapterLevel(cfg.Get("Next"),out nextChapter,out nextLevel);
    }
    public static int[][] GetFirstRewardList(this ChapterModel self,int chapter,int level)
    {
        var cfg = self.GetCfg(chapter, level);
        if (cfg == null) return null;
        return cfg.Get<int[][]>("FirstReward");
    }
    public static int[][] GetSweepRewardList(this ChapterModel self, int chapter, int level)
    {
        var cfg = self.GetCfg(chapter, level);
        if (cfg == null) return null;
        return cfg.Get<int[][]>("RepeatReward");
    }
}
