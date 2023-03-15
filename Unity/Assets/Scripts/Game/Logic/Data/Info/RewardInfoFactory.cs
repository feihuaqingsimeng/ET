using Frame;
public class RewardInfoFactory
{
    public static RewardInfo Create(RewardType type)
    {
        RewardInfo info = null;
        switch (type)
        {
            case RewardType.ITEM:
                info = new ItemInfo();
                break;
            case RewardType.FABAO:
                info = new FaBaoInfo();
                break;
            case RewardType.PET:
                info = new PetInfo();
                break;
            case RewardType.EQUIP:
                info = new EquipInfo();
                break;
        }
        return info;
    }
    public static string GetRewardName(RewardType type,int modelId)
    {
        if (type == RewardType.ITEM)
        {
            var cfg = ConfigSystem.Ins.GetItemConfig(modelId);
            if (cfg != null)
            {
                var quality = cfg.Get("Grade");
                var str = ConfigSystem.Ins.GetItemQualityColorStr(quality);
                return $"[color=#{str}]{cfg.Get<string>("Name")}[/color]";
            }
        }
        else if (type == RewardType.FABAO)
        {
            var cfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(modelId);
            if (cfg != null)
                return $"{cfg.Get<string>("FBName")}";
        }
        else if (type == RewardType.PET)
        {
            var cfg = ConfigSystem.Ins.GetLCPetAttribute(modelId);
            if (cfg != null)
                return $"{cfg.Get<string>("Name")}";
        }
        return $"can not find modelId {modelId}";
    }
    public static string GetGainRewardTipStr(RewardType type, int modelId, int num)
    {
        var name = GetRewardName(type, modelId);
        if (type == RewardType.ITEM)
        {
            return string.Format(ConfigSystem.Ins.GetLanguage("gain_item_reward_tip"), name, num);
        }
        else if (type == RewardType.FABAO)
        {
            return string.Format(ConfigSystem.Ins.GetLanguage("gain_fabao_reward_tip"), name, num);
        }
        else if (type == RewardType.PET)
        {
            return string.Format(ConfigSystem.Ins.GetLanguage("gain_pet_reward_tip"), name, num);
        }
        return $"can not find modelId {modelId}";
    }
    public static string GetTypeStr(RewardType type,int modelId)
    {
        var sys = ConfigSystem.Ins;
        if(type == RewardType.ITEM)
        {
            var cfg = sys.GetItemConfig(modelId);
            if (cfg != null)
                return cfg.Get<string>("PropTypeDescribe");
        }else if(type == RewardType.FABAO)
        {
            var cfg = sys.GetFBMagicWeaponConfig(modelId);
            if (cfg != null)
                return sys.GetLanguage($"fabao_type_{cfg.Get("FBType")}");
        }else if(type == RewardType.PET)
        {
            var cfg = sys.GetLCPetAttribute(modelId);
            if (cfg != null)
                return sys.GetLanguage($"pet_grade_{cfg.Get("Grade")}");
        }else if(type == RewardType.EQUIP)
        {
            var cfg = sys.GetEquipConfig(modelId);
            if (cfg != null)
                return sys.GetLanguage($"equip_type_{cfg.Get("EquipType")}");
        }
        return "";
    }
}
