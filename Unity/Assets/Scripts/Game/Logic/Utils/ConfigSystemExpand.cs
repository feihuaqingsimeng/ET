using ET;
using System.Collections.Generic;

public static class ConfigSystemExpand
{
	public static int GetGlobal(this ConfigSystem self,string key)
	{
		var data = self.Get("parameter", key);
		if (data == null) return default;
		if (data.HasIntValue(key))
			return data.Get(key);
		string s = data.Get<string>("parameter");
		int.TryParse(s, out int value);
		data.Set(key, value);
		return value;
	}
	public static T GetGlobal<T>(this ConfigSystem self,string key)
	{
		var data = self.Get("parameter", key);
		if (data == null) return default;
		return data.Get<T>("parameter");
	}
	
	public static string GetLanguage(this ConfigSystem self, string key)
	{
		var data = self.Get("language", key);
		if (data == null) return default;
		var str = data.Get<string>("Describe");
		if (str == "") str = key;
		return str.Replace("\\n", "\n");
	}
	public static Dictionary<int, ConfigData> GetItemConfig(this ConfigSystem self)
	{
		return self.Get("prop");
	}
	public static ConfigData GetItemConfig(this ConfigSystem self, int id)
	{
		return self.Get("prop", id);
	}
	public static string GetItemQualityColorStr(this ConfigSystem self, int id)
	{
        if (id == 0) return "ffffff";
		var cfg = self.Get("propqualitycolor", id);
		if (cfg == null) return "ffffff";
		return cfg.Get<string>("Content");
	}
	public static Dictionary<int, ConfigData> GetLvConfig(this ConfigSystem self)
	{
		return self.Get("lv");
	}
	public static ConfigData GetLvConfig(this ConfigSystem self, int id)
	{
		return self.Get("lv", id);
	}
	public static Dictionary<int, ConfigData> GetAttrConfig(this ConfigSystem self)
	{
		return self.Get("attribute");
	}
	public static ConfigData GetAttrConfig(this ConfigSystem self,int id)
	{
		return self.Get("attribute",id);
	}
	public static Dictionary<int, ConfigData> GetFBAuxiliaryConfig(this ConfigSystem self)
	{
		return self.Get("fbauxiliary");
	}
	public static ConfigData GetFBAuxiliaryConfig(this ConfigSystem self,int id)
	{
		return self.Get("fbauxiliary",id);
	}
	public static ConfigData GetFBAuxiliaryConfigByFabaoId(this ConfigSystem self, int id)
	{
		var cfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(id);
		if (cfg == null) return null;
		return self.Get("fbauxiliary", cfg.Get("FBGrade"));
	}
	public static ConfigData GetFBMagicWeaponConfig(this ConfigSystem self, int id)
	{
		return self.Get("fbmagicweapon", id);
	}
	public static ConfigData GetSkillConfig(this ConfigSystem self, int id)
	{
		return self.Get("skill", id);
	}
	public static ConfigData GetSkillShow(this ConfigSystem self, int id)
	{
		return self.Get("skillshow", id);
	}
	public static string GetErrorCode(this ConfigSystem self, int id)
	{
		var cfg = self.Get("tips", id);
		if (cfg == null)
			return id.ToString();
		return cfg.Get<string>("Content");
	}
	public static Dictionary<int, ConfigData> GetYWTotalMap(this ConfigSystem self)
	{
		return self.Get("ywtotalmap");
	}
	public static ConfigData GetYWTotalMap(this ConfigSystem self,int id)
	{
		return self.Get("ywtotalmap", id);
	}
	public static ConfigData GetYWLowerMap(this ConfigSystem self, int id)
	{
		return self.Get("ywlowermap", id);
	}
	public static ConfigData GetYWEvent(this ConfigSystem self, int id)
	{
		return self.Get("ywevent", id);
	}
	public static ConfigData GetBuff(this ConfigSystem self, int id)
	{
		return self.Get("buff", id);
	}
	public static ConfigData GetBuffShow(this ConfigSystem self, int id)
	{
		return self.Get("buffshow", id);
	}
	public static ConfigData GetYWInformation(this ConfigSystem self,int id)
	{
		return self.Get("ywinformation", id);
	}

    public static ConfigData GetChapterUnlock(this ConfigSystem self, int id)
    {
        return self.Get("chapter", id);
    }
    public static Dictionary<int,ConfigData> GetChapter(this ConfigSystem self)
    {
        return self.Get("checkpoints");
    }
    public static ConfigData GetChapter(this ConfigSystem self, int id)
    {
        return self.Get("checkpoints", id);
    }
    public static ConfigData GetMonster(this ConfigSystem self, int id)
    {
        return self.Get("monster", id);
    }
    public static ConfigData GetLCArrestProbability(this ConfigSystem self, int id)
    {
        return self.Get("lcarrestprobability", id);
    }
    public static ConfigData GetLCPetAttribute(this ConfigSystem self,int id)
    {
        return self.Get("lcattribute", id);
    }
    public static ConfigData GetLCPetLv(this ConfigSystem self, int id)
    {
        return self.Get("lclv", id);
    }
    public static ConfigData GetLCArrestdescribe(this ConfigSystem self, int id)
    {
        return self.Get("lcarrestdescribe", id);
    }
    public static ConfigData GetDrugsRefining(this ConfigSystem self,int id)
    {
        return self.Get("drugsrefining", id);
    }
    public static Dictionary<int, ConfigData> GetDrugsRefining(this ConfigSystem self)
    {
        return self.Get("drugsrefining");
    }
    public static ConfigData GetTaskConfig(this ConfigSystem self,int id)
    {
        return self.Get("task", id);
    }
    public static Dictionary<int, ConfigData> GetTaskConfig(this ConfigSystem self)
    {
        return self.Get("task");
    }
    public static ConfigData GetEquipConfig(this ConfigSystem self,int id)
    {
        return self.Get("equipment",id);
    }
    public static Dictionary<int, ConfigData> GetEquipConfig(this ConfigSystem self)
    {
        return self.Get("equipment");
    }
}

