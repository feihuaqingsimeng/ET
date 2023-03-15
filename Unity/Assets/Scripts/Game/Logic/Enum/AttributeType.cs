

using System.Collections.Generic;

public enum AttributeType
{
	Atk = 1,	//攻击
	Def = 2,	//防御
	Hp = 3,		//生命
	Speed = 4,	//	速度
	Physic_Crit_Rate = 5,  //	物理暴击率（万分比）

	Physical_Resist = 6,	//	物抗（万分比）
	Magic_Resist = 7,		//	法抗（万分比）
	Physical_Penetrate = 8, //	物穿（万分比）
	Magic_Penetrate = 9,	//	法穿（万分比）
	Toughness = 10,			//	韧性（万分比）
	Perception = 11,		//	悟性
	Cultivated = 12,		//	修炼效率（固定）
	Cultivated_Per = 13,	//    修炼效率（万分比）
	Atk_Per = 14,			//	攻击（万分比）
	Def_Per = 15,			//	防御（万分比）
	HP_Per = 16,			//	生命（万分比）
	Crit_Physic = 17,       //	物理爆伤（万分比）
	Magic_Crit_Rate = 18,       //	法术暴击率（万分比）
    Vampire = 19,           //吸血
    BeTreat_Per = 20,       //被治疗加成（万分比）
    Treat_Per = 21,         //主动治疗加成（万分比）
    Magic_Crit_Resist = 22, //法术抗暴（万分比）
    Luck = 23,              //机缘
    HeathQi = 24,           //正气
    EvilQi = 25,            //邪气
    Gold = 30,              //金属性
    Wood = 31,              //木属性
    Water = 32,             //水属性
    Fire = 33,              //火属性
    Soil = 34,              //土属性
    Gold_Resist = 35,              //金抗性
    Wood_Resist = 36,              //木抗性
    Water_Resist = 37,             //水抗性
    Fire_Resist = 38,              //火抗性
    Soil_Resist = 39,              //土抗性

    MAX = 40,
	Hp_Max = 100,
}
public static class AttributeTypeUtil
{
	public static string GetNameStr(AttributeType type)
	{
		var cfg = ConfigSystem.Ins.GetAttrConfig((int)type);
		return cfg.Get<string>("Display");
	}
	public static string GetValueStr(AttributeType type, long value)
	{
		if (IsPercent(type))
		{
			value = value / 100;
			return $"{value}%";
		}
		else
			return value.ToString();
	}
    public static bool IsPercent(AttributeType type)
    {
        var cfg = ConfigSystem.Ins.GetAttrConfig((int)type);
        return cfg.Get("Percentage") == 1;
    }
	public static void GetBaseList(List<AttributeType> list)
	{
		var dic = ConfigSystem.Ins.GetAttrConfig();
		for (int i = 1; i < (int)AttributeType.MAX; i++)
		{
			if (dic.TryGetValue(i, out ConfigData data))
			{
				if (data.Get("Type") == 1)
					list.Add((AttributeType)i);
			}
		}
	}
    public static bool IsBaseAttr(AttributeType type)
    {
        var dic = ConfigSystem.Ins.GetAttrConfig();
        if (dic.TryGetValue((int)type, out ConfigData data))
        {
            return data.Get("Type") == 1;
        }
        return false;
    }
	public static void GetSpecialList(List<AttributeType> list)
	{
		var dic = ConfigSystem.Ins.GetAttrConfig();
		for (int i = 1; i < (int)AttributeType.MAX; i++)
		{
			if (dic.TryGetValue(i, out ConfigData data))
			{
				if (data.Get("Type") == 2)
					list.Add((AttributeType)i);
			}
		}
	}
}
