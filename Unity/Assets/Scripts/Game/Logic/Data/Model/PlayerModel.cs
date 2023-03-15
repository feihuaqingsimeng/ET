using ET;
using GameEvent;
using Frame;
using module.counter.model;
using System.Collections.Generic;

public class PlayerModel:Entity,IAwake
{
    public long playerId;
    public string name;
    public int level;
    public long exp;
    public int createTime;
    public long expUpdateTime;

    public int failCount;//渡劫失败次数
    public int probAdd;//丹药提升的成功率

    public Dictionary<int, long> attributes = new Dictionary<int, long>();

    public bool autoExpLoop = false;
    public ETCancellationToken autoExpCancelToken;
    //计数器信息：key是枚举CounterType　， val　＝　当前的计数
    public Dictionary<int, int> countMap = new Dictionary<int, int>();
}
public static class PlayerModelSystem
{
    public static void SetPlayerId(this PlayerModel self, long playerId, int createTime)
    {
        self.playerId = playerId;
        self.createTime = createTime;
    }
    public static void SetAttrChange(this PlayerModel self, Dictionary<int, long> changes)
    {
        if (changes == null) return;
        foreach (var a in changes)
        {
            self.attributes[a.Key] = a.Value;
        }
    }
    public static long GetAttr(this PlayerModel self, AttributeType type)
    {
        self.attributes.TryGetValue((int)type, out long value);
        return value;
    }

    public static bool CheckExpEnough(this PlayerModel self)
    {
        var nextConfig = ConfigSystem.Ins.GetLvConfig(self.level + 1);
        if (nextConfig == null)
            return false;
        var config = ConfigSystem.Ins.GetLvConfig(self.level);
        int need = config.Get("UpgradeExp");
        if (self.exp < need)
        {
            return false;
        }
        return true;
    }
    
    public static async ETTask StartExpTick(this PlayerModel self)
    {
        if (self.autoExpLoop) return;
        self.expUpdateTime = TimeHelper.ServerNow();
        bool isMax = self.CheckExpLimit();
        Frame.UIEventSystem.Ins.Publish(new ExpAutoAddEvent() { isMax = isMax });
        if (isMax) return;
        self.autoExpLoop = true;
        self.autoExpCancelToken = new ETCancellationToken();
        while (self.autoExpLoop)
        {
            await TimerComponent.Instance.WaitAsync(10000, self.autoExpCancelToken);
            if (!self.autoExpLoop)
                return;
            self.expUpdateTime = TimeHelper.ServerNow();
            var data = DataSystem.Ins.PlayerModel;
            var config = ConfigSystem.Ins.GetLvConfig(data.level);
            var exp = config.Get("HarvestExp");
            self.exp += exp * 10;
            isMax = self.CheckExpLimit(true);
            Frame.UIEventSystem.Ins.Publish(new ExpAutoAddEvent() { isMax = isMax });
            if (isMax)
            {
                self.autoExpLoop = false;
                return;
            }
        }
    }
    public static bool CheckExpLimit(this PlayerModel self, bool setToLimit = false)
    {
        int rate = ConfigSystem.Ins.GetGlobal("ExpUpperLimit");
        var cfg = ConfigSystem.Ins.GetLvConfig(self.level + 1);
        if (cfg == null) return true;
        cfg = ConfigSystem.Ins.GetLvConfig(self.level);
        var limit = cfg.Get("UpgradeExp") * rate;
        if (self.exp >= limit)
        {
            if (setToLimit)
                self.exp = limit;
            return true;
        }
        return false;
    }
    public static void StopExpTick(this PlayerModel self)
    {
        self.autoExpLoop = false;
        self.autoExpCancelToken.Cancel();
    }

    public static void SetCounterMap(this PlayerModel self, Dictionary<int, int> counter)
    {
        if (counter == null) return;
        foreach (var v in counter)
        {
            self.countMap[v.Key] = v.Value;
        }
    }
    public static int GetCounter(this PlayerModel self, CounterType type)
    {
        self.countMap.TryGetValue((int)type, out int v);
        return v;
    }
}
