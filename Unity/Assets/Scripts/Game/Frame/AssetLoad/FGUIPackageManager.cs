
using FairyGUI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frame
{
    public class FGUIPackageManager : MonoBehaviour
    {
        class PackageInfo
        {
            public string packageName;
            public int count;
            public int requestId;
            public HashSet<string> depedences;
            public Dictionary<string, int> subRequestList;
        }
        public bool enableLog = false;
        private Dictionary<string, PackageInfo> loadedPackageDic = new Dictionary<string, PackageInfo>();

        public static FGUIPackageManager Ins { get; private set; }

        void Awake()
        {
            Ins = this;
            NTexture.CustomDestroyMethod += NTexture_CustomDestroyMethod;
            NAudioClip.CustomDestroyMethod += NAudioClip_CustomDestroyMethod;
        }
        void OnDestroy()
        {
            NTexture.CustomDestroyMethod -= NTexture_CustomDestroyMethod;
            NAudioClip.CustomDestroyMethod -= NAudioClip_CustomDestroyMethod;
        }
        public void LoadFUI(string packageName, string uiName, Action<GComponent> callback)
        {
            var p = LoadPackage(packageName);
            if(p == null)
            {
                LogError($"[LoadFUI packageName is nil ]{ packageName} {uiName}");
                return;
            }
            var obj = p.CreateObject(uiName);
            callback?.Invoke(obj.asCom);
        }
        public void UnLoadFUI(string packageName)
        {
            RemovePackage(packageName);
        }
        public void RemovePackage(string packageName)
        {
            var info = GetLoadedPackage(packageName);
            if (info == null) return;
            if (info.count == 0)
            {
                LogError("[RemovePackage]" + packageName + "非法多次调用");
                return;
            }
            info.count--;
            if (info.count == 0)
            {
                if (info.requestId >= 0)
                    ResourceManager.Ins.ReleaseAsset(info.requestId);
                else
                    UIPackage.RemovePackage(packageName);
                info.requestId = -1;
                foreach (var d in info.subRequestList)
                {
                    ResourceManager.Ins.ReleaseAsset(d.Value);
                }
                info.subRequestList.Clear();
                Log(packageName + "包已卸载");
                foreach (var d in info.depedences)
                {
                    RemovePackage(d);
                }
            }
        }
        public void AddPackageAsync(string packageName)
        {
            AddPackageAsync(packageName, null);
        }
        public void AddPackageAsync(string packageName, Action<UIPackage> pkgLoadCallback)
        {
            PackageInfo info = AddLoadedPackage(packageName);
            var package = UIPackage.GetByName(packageName);
            if (package != null)
            {
                pkgLoadCallback?.Invoke(package);
                return;
            }
            info.requestId = ResourceManager.Ins.LoadAssetAsync<TextAsset>("UI/" + packageName + "_fui.bytes", (asset, param) =>
            {
                info.requestId = -1;
                var p = UIPackage.AddPackage(asset.bytes, packageName, (name, extension, type, item) =>
                {
                    string resKey = "UI/" + name + extension;
                    int subId = ResourceManager.Ins.LoadAssetAsync<UnityEngine.Object>(resKey, (res, param2) =>
                    {
                        Log("item 设置成功 " + res.ToString());
                        info.subRequestList.Remove((string)param2);
                        item.owner.SetItemAsset(item, res, DestroyMethod.Custom);
                    }, resKey);
                    info.subRequestList.Add(resKey, subId);
                });
                ResourceManager.Ins.ReleaseAsset(asset);
                if (p == null || p.dependencies == null)
                {
                    pkgLoadCallback?.Invoke(p);
                    return;
                }
                int count = p.dependencies.Length;
                int curCount = 0;
                foreach (var depend in p.dependencies)
                {
                    var dname = depend["name"];
                    info.depedences.Add(dname);
                    var dp = UIPackage.GetByName(dname);
                    if (dp == null)
                    {
                        AddPackageAsync(dname, (ddp) =>
                        {
                            curCount++;
                            if (curCount == count)
                                pkgLoadCallback?.Invoke(p);
                        });
                    }
                    else
                    {
                        curCount++;
                        var dInfo = GetLoadedPackage(dname);
                        dInfo.count++;
                    }
                }
                if (curCount == count)
                    pkgLoadCallback?.Invoke(p);

            });
        }
        public UIPackage LoadPackage(string packageName)
        {
            PackageInfo info = AddLoadedPackage(packageName);
            var package = UIPackage.GetByName(packageName);
            if (package != null)
            {
                return package;
            }
            var asset = ResourceManager.Ins.LoadAsset<TextAsset>("UI/" + packageName + "_fui.bytes");
            var p = UIPackage.AddPackage(asset.bytes, packageName, (name, extension, type, item) =>
            {
                string resKey = "UI/" + name + extension;
                var res = ResourceManager.Ins.LoadAsset<UnityEngine.Object>(resKey);
                item.owner.SetItemAsset(item, res, DestroyMethod.Custom);
            });
            ResourceManager.Ins.ReleaseAsset(asset);
            if (p == null || p.dependencies == null)
            {
                return p;
            }
            foreach (var depend in p.dependencies)
            {
                var dname = depend["name"];
                info.depedences.Add(dname);
                var dp = UIPackage.GetByName(dname);
                if (dp == null)
                {
                    LoadPackage(dname);
                }
                else
                {
                    var dInfo = GetLoadedPackage(dname);
                    dInfo.count++;
                }
            }
            return p;

            
        }
        private UIPackage AddPackage(string packageName)
        {
            var info = AddLoadedPackage(packageName);
            var package = UIPackage.GetByName(packageName);
            if (package != null) return package;
            package = UIPackage.AddPackage(packageName);
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
                    LogError(string.Format("{0}和{1} 包存在互相引用,无法卸载", packageName, dname));
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
        private void NTexture_CustomDestroyMethod(Texture obj)
        {
            ResourceManager.Ins.ReleaseAsset(obj);
        }
        private void NAudioClip_CustomDestroyMethod(AudioClip obj)
        {
            ResourceManager.Ins.ReleaseAsset(obj);
        }

        private void Log(object str)
        {
            if (enableLog)
                Debug.Log(str);
        }
        private void LogError(object str)
        {
            if (enableLog)
                Debug.LogError(str);
        }
    }
}
