using SItemInfo = module.item.message.ItemInfo;

//背包类型
public enum PackType
{
    NONE = 0,
    //仓库
    STORE = 1,
    //装备背包
    PACK = 2,
    //装备栏
    EQUIP = 3,
}
//道具信息
public class ItemInfo : RewardInfo
{
    public override RewardType rewardType => RewardType.ITEM;
    // 道具过期时间戳
    public long timeLimit;
    // 道具可以使用的时间
    public long canUseTime;
    // 药效
    public int drugsEffect;
    public PackType packType;
    public override void InitModelId()
    {
        var cfg = ConfigSystem.Ins.GetItemConfig(modelId);
        if (cfg == null)
        {
            name = $"error{modelId}";
            return;
        }
        type = cfg.Get("PropType");
        quality = cfg.Get("Grade");
        name = cfg.Get<string>("Name");
        des = cfg.Get<string>("PropDescribe");
    }
}

public static class ItemInfoSystem
{
    public static ItemInfo Clone(this ItemInfo self)
    {
        var info = new ItemInfo
        {
            uid = self.uid,
            modelId = self.modelId,
            num = self.num,
            timeLimit = self.timeLimit,
            canUseTime = self.canUseTime,
            drugsEffect = self.drugsEffect,
            type = self.type,
            quality = self.quality,
            packType = self.packType,
        };
        return info;
    }
    public static void UpdateByDB(this ItemInfo self,SItemInfo db,PackType packType = PackType.NONE)
    {
        self.uid = db.index;
        self.modelId = db.itemId;
        self.num = db.amount;
        self.timeLimit = db.timeLimit;
        self.canUseTime = db.canUseTime;
        self.drugsEffect = db.drugsEffect;
        self.packType = packType;
    }

    public static void UpdateById(this ItemInfo self,int itemId,int num)
    {
        self.uid = -1;
        self.modelId = itemId;
        self.num = num;
    }
}
