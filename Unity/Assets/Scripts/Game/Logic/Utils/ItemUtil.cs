using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemUtil
{
	private static Dictionary<int, Color> itemQualityColorDic = new Dictionary<int, Color>();
    public static Color GetItemQualityColorById(int itemId)
    {
        var cfg = ConfigSystem.Ins.GetItemConfig(itemId);
        if (cfg != null)
            return GetItemQualityColorByQ(cfg.Get("Grade"));
        return GetItemQualityColorByQ(1);
    }
    public static Color GetItemQualityColorByQ(int quality)
	{
		if (itemQualityColorDic.TryGetValue(quality, out var color))
		{
			return color;
		}
		var str = ConfigSystem.Ins.GetItemQualityColorStr(quality);
		if (str == "") return Color.white;
		var sub1 = str.Substring(0, 2);
		var sub2 = str.Substring(2, 2);
		var sub3 = str.Substring(4, 2);
		Color c = new Color(Convert.ToInt32(sub1, 16) / 255.0f, Convert.ToInt32(sub2, 16) / 255.0f, Convert.ToInt32(sub3, 16) / 255.0f);
		itemQualityColorDic.Add(quality, c);
		return c;
	}
	public static string GetItemName(int itemId)
	{
		var cfg = ConfigSystem.Ins.GetItemConfig(itemId);
		if (cfg == null) return "";
		return cfg.Get<string>("Name");
	}
	public static string GetItemDes(int itemId)
	{
		var cfg = ConfigSystem.Ins.GetItemConfig(itemId);
		if (cfg == null) return "";
		return cfg.Get<string>("PropDescribe");
	}
	public static string GetItemTypeDes(int itemId)
	{
		var cfg = ConfigSystem.Ins.GetItemConfig(itemId);
		if (cfg == null) return "";
		return cfg.Get<string>("PropTypeDescribe");
	}
}
