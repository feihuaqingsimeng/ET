using ET;
using Frame;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigData
{
	public string name;
	private Dictionary<string, object> objDic = new Dictionary<string, object>();
	private Dictionary<string, int> intDic = new Dictionary<string, int>();
    public int Get(string key)
	{
		if (intDic.TryGetValue(key, out int v))
			return v;
		else
			Log.Error($"{name}表找不到value为int的key {key}");
		return 0;
	}
	public bool HasIntValue(string key)
	{
		return intDic.ContainsKey(key);
	}
	public void Set(string key,string value)
	{
		objDic[key]= value;
	}
	public void Set(string key,int value)
	{
		intDic[key] = value;
	}
	public T Get<T>(string key)
	{
		if (objDic.TryGetValue(key, out object v))
		{
			if (typeof(T) == v.GetType()) return (T)v;
			var s = (string)v;
			if (string.IsNullOrEmpty(s)) return default;
			var arr = LitJson.JsonMapper.ToObject<T>(s);
			if (arr != null)
				objDic[key] = arr;
			return arr;
		}
		else
		{
			Log.Error($"{name}找不到key {key}");
		}
		return default;
	}
	public override string ToString()
	{
		string str = LitJson.JsonMapper.ToJson(intDic);
		str += LitJson.JsonMapper.ToJson(objDic);
		return str;
	}

}
public class ConfigSystem
{
	private Dictionary<string, Dictionary<int, ConfigData>> dict = new Dictionary<string, Dictionary<int, ConfigData>>();
	private Dictionary<string, Dictionary<string, ConfigData>> dict2 = new Dictionary<string, Dictionary<string, ConfigData>>();

	public static ConfigSystem Ins { get; private set; } = new ConfigSystem();
	public void Load(string name)
	{
        if(Application.platform != RuntimePlatform.WindowsPlayer)
        {
            
            var asset = ResourceManager.Ins.LoadAsset<TextAsset>($"Config/{name}.bytes");
            if (asset != null)
            {
                using (MemoryStream ms = new MemoryStream(asset.bytes))
                {
                    AnalysisSystem.Ins.StartRecordTime($"Load {name} config");
                    Load(name, asset.text);
                    AnalysisSystem.Ins.EndRecordTime($"Load {name} config");
                    ResourceManager.Ins.ReleaseAsset(asset);
                }

               
            }
        }
        else
        {
            var path = Application.streamingAssetsPath;
            Load(name,ResourceManager.Ins.LoadFile($"{path}/Config /{name}.txt"));
        }
    }
	public void Load(string name,string text)
	{
		/*var asset = Resources.Load<TextAsset>("Config/" + name);
		if (asset == null)
		{
			Log.Error($"加载配置表失败 {name}");
			return ;
		}*/
		if (dict.ContainsKey(name) || dict2.ContainsKey(name))
		{
			Log.Error($"配置表已加载 {name}");
			return;
		}
        
        var datas = text.Replace("\r","").Split('\n');
        var keys = datas[0].Split(';');

        var fieldTypes = datas[1].Split(';');
        
        if(keys.Length != fieldTypes.Length)
            Log.Error($"列表长度错误 表名：{name}  row:2  {datas[1]}");

        var filedIntArr = new bool[fieldTypes.Length];
        for (int i = 0; i < fieldTypes.Length; i++)
        {
            filedIntArr[i] = fieldTypes[i] == "int";
        }
        if (filedIntArr[0]) {
            var dic = new Dictionary<int, ConfigData>(datas.Length-2);
            for (int i = 2; i < datas.Length; i++)
            {
                var data = datas[i];
                if (string.IsNullOrEmpty(data)) continue;
                var config = CreateConfigData(name,data, keys, filedIntArr);
                dic.Add(config.Get(keys[0]), config);
            }
            dict.Add(name, dic);
        }
        else
        {
            var dic = new Dictionary<string, ConfigData>();
            for (int i = 2; i < datas.Length; i++)
            {
                var data = datas[i];
                if (string.IsNullOrEmpty(data)) continue;
                var config = CreateConfigData(name,data, keys, filedIntArr);
                dic.Add(config.Get<string>(keys[0]), config);
            }
            dict2.Add(name, dic);
        }
    }
    private ConfigData CreateConfigData(string name,string data, String[] keys,bool[] filedIntArr)
	{
        var config = new ConfigData()
        {
            name = name
        };
        var dataParam = data.Split(';');
        int len = dataParam.Length;
        for (int k = 0; k < keys.Length; k++)
		{
			if (k >= len)
			{
				//Log.Error($"列表长度错误 表名：{name}  row:{i}  {dataParam}");
				break;
			}
			if (filedIntArr[k])
			{
				int.TryParse(dataParam[k], out int a);
				config.Set(keys[k], a);
			}
            else
            {
                config.Set(keys[k], dataParam[k]);
            }
        }
		return config;
		
	}
	public Dictionary<string, ConfigData> Get2(string name)
	{
		Dictionary<string, ConfigData> config;
		if (dict2.TryGetValue(name, out config))
			return config;
		Load(name);
		if (dict2.TryGetValue(name, out config))
			return config;
		Log.Error($"配置找不到，配置表名: {name}");
		return null;
	}
	public Dictionary<int, ConfigData> Get(string name)
	{
		Dictionary<int, ConfigData> config;
		if (dict.TryGetValue(name, out config))
			return config;
		Load(name);
		if (dict.TryGetValue(name, out config))
			return config;
		Log.Error($"配置找不到，配置表名: {name}");
		return null;
	}
	public ConfigData Get(string name, int id)
	{
		var dic = Get(name);
		if (dic == null) return null;
		if (dic.TryGetValue(id, out ConfigData config))
			return config;
		Log.Error(string.Format("配置找不到，配置表名: {0}，配置id: {1}",name,id));
		return null;
	}
	public ConfigData Get(string name, string id)
	{
		var dic = Get2(name);
		if (dic == null) return null;
		if (dic.TryGetValue(id, out ConfigData config))
			return config;
		Log.Error($"配置找不到，配置表名: {name}，配置id: {id}");
		return null;
	}
}
