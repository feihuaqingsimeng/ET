using ET;
using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageLoader
{
	class PackageInfo
	{
		public string packageName;
		public int count;
		public int requestId;
		public HashSet<string> depedences;
		public Dictionary<string, int> subRequestList;
	}

	private Dictionary<string, PackageInfo> loadedPackageDic = new Dictionary<string, PackageInfo>();
	public static PackageLoader Ins { get; } = new PackageLoader();
	void ReleaseAsset()
	{

	}
	static UIPackage.LoadResource _loadFromResourcesPath = (string name, string extension, System.Type type, out DestroyMethod destroyMethod) =>
	{
		destroyMethod = DestroyMethod.Unload;
		Log.Error($"[_loadFromResourcesPath] name:{name},extension:{extension},type:{type}");
		return Resources.Load(name, type);
	};
	void LoadAsset(string bundleName)
	{
		AssetsBundleHelper.LoadBundle(bundleName);
	}
	public void LoadFUI(string packageName, string uiName, Action<GComponent> callback)
	{
		var package = AddPackage(packageName);
		var obj = package.CreateObject(uiName);
		callback?.Invoke(obj.asCom);
		
	}
	public GComponent LoadFUI(string packageName, string uiName)
	{
		var package = AddPackage(packageName);
		var obj = package.CreateObject(uiName);
		return obj.asCom;

	}
	public void UnLoadFUI(string packageName, string uiName)
	{
		RemovePackage(packageName);
	}
	public void RemovePackage(string packageName)
	{
		var info = GetLoadedPackage(packageName);
		if (info == null) return;
		if (info.count == 0)
		{
			Debug.LogError("[RemovePackage]" + packageName + "非法多次调用");
			return;
		}
		info.count--;
		if (info.count == 0)
		{
			//if (info.requestId >= 0)
				//ReleaseAsset(info.requestId);
			//else
			UIPackage.RemovePackage(packageName);
			//info.requestId = -1;
			//foreach (var d in info.subRequestList)
			//{
			//	ReleaseAsset(d.Value);
			//}
			//info.subRequestList.Clear();
			Debug.LogError(packageName + "包已卸载");
			foreach (var d in info.depedences)
			{
				RemovePackage(d);
			}
		}
	}
	public UIPackage AddPackage(string packageName)
	{
		var info = AddLoadedPackage(packageName);
		var package = UIPackage.GetByName(packageName);
		if (package != null) return package;
		package = UIPackage.AddPackage(packageName, _loadFromResourcesPath);
		foreach (var depend in package.dependencies)
		{
			var dname = depend["name"];
			var p = UIPackage.GetByName(dname);
			info.depedences.Add(dname);
			if (p == null)
			{
				AddPackage(dname);
			}
			else
			{
				var dInfo = GetLoadedPackage(dname);
				dInfo.count++;
			}
			if (GetLoadedPackage(dname).depedences.Contains(packageName))
			{
				Debug.LogError(string.Format("{0}和{1} 包存在互相引用,无法卸载", packageName, dname));
			}
		}
		return package;
	}
	
	private PackageInfo GetLoadedPackage(string name)
	{
		if (loadedPackageDic.TryGetValue(name, out PackageInfo info))
		{
			return info;
		}
		return null;
	}
	private PackageInfo AddLoadedPackage(string name)
	{
		if (loadedPackageDic.TryGetValue(name, out PackageInfo info))
		{
			info.count++;
		}
		else
		{
			info = new PackageInfo();
			info.packageName = name;
			info.count = 1;
			info.requestId = -1;
			info.depedences = new HashSet<string>();
			info.subRequestList = new Dictionary<string, int>();
			loadedPackageDic.Add(name, info);
		}
		return info;
	}
}
