using ET;
using module.item.message.vo;
using System.Collections.Generic;

public  class DrugsUseInfo : Object
{
    public int itemId;
    // 生效效果次数
    public int effectCount;
    // 效果总值
    public int effectValue;
    // 效果属性
    public AttributeType attrType;
    // 最低药效,万分比的int值,1000表示10%
    public int minEffect;

}

public class HandBookModel:Entity,IAwake
{
    // 丹药总属性
    public Dictionary<int, long> totalAttr = new Dictionary<int, long>();
    // 丹药图鉴记录
    public List<DrugsUseInfo> drugsRecords = new List<DrugsUseInfo>();
    //已经学会的配方id,就是丹药id
    public HashSet<int> formulaDic = new HashSet<int>();
}

public static class HandBookSystem
{
    public static bool IsUnlock(this HandBookModel self,int itemId)
    {
        return self.formulaDic.Contains(itemId);
    }
    public static long GetAttr(this HandBookModel self,AttributeType type)
    {
        self.totalAttr.TryGetValue((int)type, out long value);
        return value;
    }
    public static void InitTotalAttr(this HandBookModel self, Dictionary<int, long> attrs)
    {
        if (attrs == null) return;
        self.totalAttr.Clear();
        foreach (var v in attrs)
        {
            self.totalAttr[v.Key] = v.Value;
        }
    }
    public static void InitFormula(this HandBookModel self,List<int> dbList)
    {
        if (dbList == null) return;
        self.formulaDic.Clear();
        for(int i = 0; i < dbList.Count; i++)
        {
            self.formulaDic.Add(dbList[i]);
        }
    }
    public static void InitDrugsRecords(this HandBookModel self,List<DrugsTuJianVo> records)
    {
        if (records == null) return;
        self.drugsRecords.Clear();
        foreach(var v in records)
        {
            var info = new DrugsUseInfo();
            self.UpdateDrugsUseInfo(info, v);
            self.drugsRecords.Add( info);
        }
    }
    private static void UpdateDrugsUseInfo(this HandBookModel self, DrugsUseInfo info,DrugsTuJianVo db)
    {
        info.itemId = db.itemId;
        info.effectCount = db.effectCount;
        info.effectValue = db.effectValue;
        info.attrType = (AttributeType)db.attr;
        info.minEffect = db.minEffect;
}
}