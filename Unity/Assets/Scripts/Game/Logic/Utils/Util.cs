using ET;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static void SetObjectLayer(GameObject go,int layer,bool includeChild = true)
    {
        if (go == null) return;
        go.layer = layer;
        if (includeChild)
        {
            var tran = go.transform;
            for (int i = 0; i < tran.childCount; i++)
            {
                SetObjectLayer(tran.GetChild(i).gameObject,layer,includeChild);
            }
        }
    }
	public static string GetColorText(string str,string color)
	{
		return $"[color={color}]{str}[/color]";
	}
	public static string GetTimeHMSStr(long millSecond)
	{
		if (millSecond <= 0)
			return "00:00:00";
		millSecond /= 1000;
		int hour = (int)(millSecond / 3600);
		int minute = (int)((millSecond - hour * 3600) / 60);
		int second = (int)(millSecond % 60);
		return $"{hour:00}:{minute:00}:{second:00}";
	}
	public static string GetTimeMSStr(long millSecond)
	{
		if (millSecond <= 0)
			return "00:00";
		millSecond /= 1000;
		int minute = (int)(millSecond / 60);
		int second = (int)(millSecond % 60);
		return $"{minute:00}:{second:00}";
	}
    public static string GetTimeYMDStr(long timestamp)
    {
        DateTime dt = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        dt = dt.AddTicks(timestamp * 10000);
        return dt.ToString("yyyy/MM/dd");
    }
}

