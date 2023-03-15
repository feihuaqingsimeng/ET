
using System.Collections.Generic;
using SPetInfo = module.pet.message.vo.PetInfo;

public class PetInfo : RewardInfo
{
    public override RewardType rewardType => RewardType.PET;
    // 出战位置,1-6
    public int battleIndex;
    public int level;
    public long exp;
    public List<int> skills = new List<int>();
    public Dictionary<int, long> attrs = new Dictionary<int, long>();
    public int jinhuaExp;
    public int oldPetId;

    public override void InitModelId()
    {
        var cfg = ConfigSystem.Ins.GetLCPetAttribute(modelId);
        if (cfg == null) return;
        quality = cfg.Get("Grade");
        name = cfg.Get<string>("Name");
        des = cfg.Get<string>("Name");
    }
}
public static class PetInfoSystem
{
    public static bool IsMaxQuality(this PetInfo self)
    {
        return self.quality == ConstValue.PET_QUALITY_MAX;
    }
    public static void UpdateByDB(this PetInfo info, SPetInfo db)
    {
        var oldId = info.modelId;
        info.uid = db.uuid;
        info.modelId = db.petId;
        info.battleIndex = db.zhanIndex;
        info.level = db.level;
        info.exp = db.exp;
        info.skills.Clear();
        info.attrs.Clear();
        info.skills.AddRange(db.skills);
        foreach (var v in db.attrs)
            info.attrs.Add(v.Key, v.Value);
        info.jinhuaExp = db.jinhuaExp;
        info.oldPetId = oldId;
    }
}
