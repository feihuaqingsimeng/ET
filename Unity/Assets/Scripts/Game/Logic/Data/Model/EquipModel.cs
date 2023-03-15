using ET;
using System.Collections.Generic;
using SEquipInfo = module.equip.message.vo.EquipInfo;
public class EquipModel:Entity,IAwake
{
    public int size_pack;
    public int size_store;

    public Dictionary<int, EquipInfo> equips = new Dictionary<int, EquipInfo>();
    public MultiDictionary<int,int, EquipInfo> equipDic = new MultiDictionary<int,int, EquipInfo>();
}

public static class EquipModelSystem
{
    public static void AddEquip(this EquipModel self, SEquipInfo db,EquipPackType type)
    {
        if(self.equipDic.TryGetValue((int)type,db.index,out var info))
        {
            info.UpdateByDB(db, type);
            return;
        }
        info = new EquipInfo();
        info.UpdateByDB(db, type);
        self.equipDic.Add((int)type, (int)info.uid, info);
    }
    public static void RemoveEquip(this EquipModel self,int uid,int type)
    {
        self.equipDic.Remove(type, uid);
    }
    public static void GetEquips(this EquipModel self,EquipPackType type, List<EquipInfo> outList)
    {
        if (outList == null) return;
        if(self.equipDic.TryGetDic((int)type,out var dic))
        {
            foreach(var v in dic)
            {
                outList.Add(v.Value);
            }
        }
    }
    public static EquipInfo GetEquiped(this EquipModel self, EquipType type)
    {
        self.equipDic.TryGetValue((int)EquipPackType.EQUIP,(int)type, out var value);
        return value;
    }
    public static int GetLimitCount(this EquipModel self)
    {
        return ConfigSystem.Ins.GetGlobal("KnapsackUpper");
    }
    public static int Sort(this EquipModel self, EquipInfo a,EquipInfo b)
    {
        int aw = a.IsEquiped ? 0 : 1;
        int bw = b.IsEquiped ? 0 : 1;
        if (aw != bw)
            return aw - bw;
        if (a.quality != b.quality)
            return b.quality - a.quality;
        if (a.level != b.level)
            return b.level - a.level;
        return a.type - b.type;
    }
}
