using ET;
using module.common.model;
using module.counter.model;
using module.yw.domain;
using System.Collections.Generic;
public class YWPosInfo
{
    public int lowerMapId;
    public int x;
    public int y;
    public List<module.yw.message.vo.YwPlayerVo> playerList;
    // 是否已探索
    public bool tansuo;
    // 秘境信息,如果没有秘境,该字段为空
    public module.yw.message.vo.MiJingVo miJing;
}
public class YWNoticeInfo
{
    public int id;
    public int type;
    public string des;

    public YWNoticeInfo(Information info)
    {
        id = info.id;
        var cfg = ConfigSystem.Ins.GetYWInformation(id);
        if (cfg == null) return;
        type = cfg.Get("Type");
        des = string.Format(cfg.Get<string>("Content"), info.paramList.ToArray());
    }
}
public class YuWaiModel:Entity,IAwake
{
    
    public int ywCurRow;
    public int ywCurCol;
    public int ywMapId;
    public int ywLingli;
    public int ywPacketSize;
    public List<CommItemInfo> ywRewards = new List<CommItemInfo>();
    public long ywLingLiLastUpdateTime;
    public Dictionary<int, YWPosInfo> ywPosInfoDic = new Dictionary<int, YWPosInfo>();
    //通知
    public bool ywNoticeInfoIsRequest;
    public List<YWNoticeInfo> ywNoticeInfoList = new List<YWNoticeInfo>();
}

public static class YuWaiModelSystem
{
    public static void ClearYWPosDic(this YuWaiModel self, int mapId)
    {
        if (mapId == 0) return;
        List<int> list = new List<int>();
        foreach (var v in self.ywPosInfoDic)
        {
            if (v.Value.lowerMapId == mapId) list.Add(v.Key);
        }
        foreach (var v in list)
        {
            self.ywPosInfoDic.Remove(v);
        }
    }
    public static YWPosInfo GetYWPosInfo(this YuWaiModel self, int row, int col)
    {
        int index = ConstValue.YWCol * row + col;
        self.ywPosInfoDic.TryGetValue(index, out var value);
        return value;
    }
    public static void AddYWPosInfo(this YuWaiModel self, int row, int col, YWPosInfo info)
    {
        int index = ConstValue.YWCol * row + col;
        self.ywPosInfoDic.Add(index, info);
    }
    public static int GetYWChapterId(this YuWaiModel self)
    {
        if (self.ywMapId == 0) return 0;
        var cfgs = ConfigSystem.Ins.GetYWTotalMap();
        foreach (var v in cfgs)
        {
            var maps = v.Value.Get<int[]>("LowerMap");
            for (int i = 0; i < maps.Length; i++)
            {
                if (self.ywMapId == maps[i])
                    return v.Key;
            }
        }
        return 0;

    }
    public static int GetYWLingLiBuyCount(this YuWaiModel self)
    {
        return DataSystem.Ins.PlayerModel.GetCounter(CounterType.YW_BUY_LING_LI);
    }
    public static int GetYWFreeUseCount(this YuWaiModel self)
    {
        return DataSystem.Ins.PlayerModel.GetCounter(CounterType.YW_FREE);
    }
    public static CommItemInfo GetYWReward(this YuWaiModel self, int index)
    {
        if (index >= self.ywRewards.Count) return null;
        return self.ywRewards[index];
    }
    public static void GetYWNoticeInfoList(this YuWaiModel self, YWNoticeType type, List<YWNoticeInfo> list)
    {
        foreach (var v in self.ywNoticeInfoList)
        {
            var cfg = ConfigSystem.Ins.GetYWInformation(v.id);
            if (cfg == null) continue;
            var t = cfg.Get("Type");
            if (type == YWNoticeType.All)
            {
                list.Add(v);
                continue;
            }
            if (t == 1 && type == YWNoticeType.Persion)//个人
            {
                list.Add(v);
            }
            else if (t != 1 && type != YWNoticeType.Persion)
            {
                list.Add(v);
            }
        }
    }
}
