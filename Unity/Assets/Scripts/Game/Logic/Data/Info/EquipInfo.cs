
using System.Collections.Generic;
using SEquipInfo = module.equip.message.vo.EquipInfo;

public enum EquipType
{
    Weapon = 1, // 武器
    Cloth = 2,  //衣服
    Cloak = 3,  // 披风
    Belt = 4,   // 腰带
    Shoes = 5,  // 鞋子
    Hat = 6,    //帽子
    Ring = 7,   //戒指
    Necklace = 8,//项链

}
//背包类型
public enum EquipPackType
{
    NONE = 0,
    //仓库
    STORE = 1,
    //装备背包
    PACK = 2,
    //装备栏
    EQUIP = 3,
}
public class EquipInfo:RewardInfo
{
    public override RewardType rewardType => RewardType.EQUIP;
    //public int uid;//唯一性id
    //public int modelId;//配表id
    public int level;
    public EquipPackType packType;
    public bool IsEquiped { get { return packType == EquipPackType.EQUIP; } }

    public Dictionary<int, long> attri = new Dictionary<int, long>();

    public override void InitModelId()
    {
        var cfg = ConfigSystem.Ins.GetEquipConfig(modelId);
        if (cfg == null) return;
        type = cfg.Get("EquipType");
        quality = cfg.Get("Grade");
        name = cfg.Get<string>("Name");
        des = cfg.Get<string>("Introduce");
        level = cfg.Get("UseMinimumLv");
    }
    public void UpdateByDB(SEquipInfo db, EquipPackType type)
    {
        uid = db.index;
        modelId = db.equipId;
        packType = type;
        attri.Clear();
        if(db.attri != null)
        {
            foreach(var v in db.attri)
            {
                attri.Add(v.Key, v.Value);
            }
        }
    }
}

