using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Frame
{
    public class BundleLoader : AssetLoader
    {
        public class LoadingInfo : ILoadingInfo
        {
            public string name;
            public AssetBundleCreateRequest op;

            public void Destroy()
            {
                
            }
        }
        public class ABInfo
        {
            public string name;
            public AssetBundle bundle;
            public bool isDone;
            public int refCount;
            public void Destroy(bool unload = true)
            {
                if (bundle != null)
                    bundle.Unload(unload);
            }
        }
        private const string PREFIX_ASSET_NAME = "assets/bundles/";
        private Dictionary<string, ABInfo> loadedBundleDic = new Dictionary<string, ABInfo>();
        private AssetBundleManifest AssetBundleManifestObject { get; set; }
        private Dictionary<string, string[]> dependenciesCache = new Dictionary<string, string[]>();
        private Dictionary<int, string> assetMappingBundleDic = new Dictionary<int, string>();

        public string[] GetDependencies(string bundleName)
        {
            if (bundleName == "StreamingAssets") return Array.Empty<string>();

            if(dependenciesCache.TryGetValue(bundleName,out var dependencies))
            {
                return dependencies;
            }
            if (AssetBundleManifestObject == null)
            {
                var bundle = LoadBundle("StreamingAssets","");
                AssetBundleManifestObject = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            dependencies = AssetBundleManifestObject.GetAllDependencies(bundleName);
            if (dependencies == null)
                dependencies = Array.Empty<string>();
            dependenciesCache.Add(bundleName, dependencies);
            return dependencies;
        }
        public bool IsDependenciesLoaded(string bundleName)
        {
            var ds = GetDependencies(bundleName);
            foreach(var v in ds)
            {
                if(!loadedBundleDic.TryGetValue(v,out var info))
                {
                    return false;
                }
                if (!info.isDone) return false;
            }
            return true;
        }
        public string GetBundleName(string assetName, string suffix = ".bytes")
        {
            if(!assetName.EndsWith(suffix))
                assetName += suffix;
            return assetName;
        }
        
        public AssetBundle LoadBundle(string assetName,string suffix = ".bytes")
        {
           
            string bundleName = GetBundleName(assetName,suffix);
            Log($"bundle加载  {bundleName}");
            if (loadedBundleDic.TryGetValue(bundleName, out var info))
            {
                info.refCount++;
                return info.bundle;
            }
            string p = Path.Combine(PathHelper.AppHotfixResPath, bundleName);
            AssetBundle bundle;
            if (File.Exists(p))
            {
                bundle = AssetBundle.LoadFromFile(p);
            }
            else
            {
                p = Path.Combine(PathHelper.AppResPath, bundleName);
                bundle = AssetBundle.LoadFromFile(p);
            }
            info = new ABInfo()
            {
                name = bundleName,
                refCount = 1,
                bundle = bundle,
                isDone = true
            };
            loadedBundleDic.Add(bundleName, info);
            if(bundle == null)
            {
                LogError($"bundle 找不到：{bundleName}");
            }
            var ds = GetDependencies(bundleName);
            foreach (var v in ds)
            {
                LoadBundle(v);
            }
            return bundle;
        }
        public int LoadBundleAsync(string assetName,Action<AssetBundle> callback, string suffix = ".bytes")
        {
            string bundleName = GetBundleName(assetName, suffix);
            Log($"bundle加载Async  {bundleName}");
            if (loadedBundleDic.TryGetValue(bundleName, out var info))
            {
                info.refCount++;
                if (info.isDone)
                {
                    callback?.Invoke(info.bundle);
                    return -1;
                }
            }
            foreach(var v in loadingAssetDic)
            {
                var loadingInfo = v.Value as LoadingInfo;
                if(loadingInfo.name == bundleName)
                {
                    loadingInfo.op.completed += op =>
                    {
                        if (loadedBundleDic.ContainsKey(bundleName))
                        {
                            callback?.Invoke(loadingInfo.op.assetBundle);
                        }
                    };
                    return -1;
                }
            }

            int requestId = GetUniqueId();
            string p = Path.Combine(PathHelper.AppHotfixResPath, bundleName);
            AssetBundleCreateRequest request;
            if (File.Exists(p))
            {
                request = AssetBundle.LoadFromFileAsync(p);
            }
            else
            {
                p = Path.Combine(PathHelper.AppResPath, bundleName);
                request = AssetBundle.LoadFromFileAsync(p);
            }
            info = new ABInfo()
            {
                name = bundleName,
                refCount = 1,
            };
            loadedBundleDic.Add(bundleName, info);
            request.completed += op =>
            {
                RemoveLoadingAsset(requestId);
                info.bundle = request.assetBundle;
                info.isDone = true;
                if (info.refCount == 0)
                {
                    info.Destroy();
                    loadedBundleDic.Remove(bundleName);
                    return;
                }
                if(IsDependenciesLoaded(bundleName))
                    callback?.Invoke(info.bundle);
            };
            LoadingInfo loadInfo = new LoadingInfo() { name = bundleName, op = request };
            AddLoadingAsset(requestId, loadInfo);

            var ds = GetDependencies(bundleName);
            foreach (var v in ds)
            {
                Log($"{bundleName} 加载依赖 {v}");
                LoadBundleAsync(v,(ab)=> {
                    if (IsDependenciesLoaded(bundleName))
                        callback?.Invoke(info.bundle);
                });
            }

            return requestId;
        }
        public override int InstantiateAsync(string assetname, Action<GameObject> callback)
        {
            var requestId = LoadAssetAsync<GameObject>(assetname, (go, param) =>
            {
                if (go == null)
                {
                    LogError("找不到资源:" + assetname);
                    callback?.Invoke(null);
                    return;
                }
                var inst = UnityEngine.Object.Instantiate(go);
                callback?.Invoke(inst);
            });
            return requestId;
        }
        public override T LoadAsset<T>(string assetname)
        {
            assetname = assetname.ToLower();
            Log($"资源加载:{assetname}");
            
            var assetBundle = LoadBundle(assetname);
            if (assetBundle == null)
            {
                return default;
            }
            var obj = assetBundle.LoadAsset<T>($"{PREFIX_ASSET_NAME}{assetname}");
            if (obj == null)
                LogError($"找不到资源:{assetname}");
            else
                assetMappingBundleDic[obj.GetInstanceID()] = GetBundleName(assetname);
            return obj;
        }

        public override int LoadAssetAsync<T>(string assetname, Action<T, object> callback, object customParam = null)
        {
            assetname = assetname.ToLower();
            Log($"资源加载Async:{assetname}");
            int requestId = LoadBundleAsync(assetname, (bundle) => {
                if(bundle == null)
                {
                    LogError($"assets bundle not found: {assetname}");
                    return;
                }
                var obj = bundle.LoadAsset<T>($"{PREFIX_ASSET_NAME}{assetname}");
                if (obj == null)
                    LogError($"找不到资源:{assetname}");
                else
                    assetMappingBundleDic[obj.GetInstanceID()] = GetBundleName(assetname);
                callback?.Invoke(obj,customParam);
            });
            return requestId;

        }

        public override void ReleaseAsset(int requestId, UnityEngine.Object obj = null)
        {
            if (requestId > 0 && GetLoadingInfo(requestId, out LoadingInfo info))
            {
                info.Destroy();
                RemoveLoadingAsset(requestId);
                Log($" release requestId {requestId}");
            }
            if (obj != null)
            {
                int id = obj.GetInstanceID();
                if (assetMappingBundleDic.TryGetValue(id,out var bundleName))
                {
                    if (ReleaseBundle(bundleName))
                        assetMappingBundleDic.Remove(id);
                    Log($" release obj {obj.GetType()}");
                }
               
            }
        }
        private bool ReleaseBundle(string bundleName)
        {
            bool delete = false;
            if(loadedBundleDic.TryGetValue(bundleName,out var info))
            {
                info.refCount--;
                if(info.refCount <= 0)
                {
                    delete = true;
                    if (info.bundle!= null)
                    {
                        info.Destroy();
                        loadedBundleDic.Remove(bundleName);
                    }
                }
                foreach(var v in GetDependencies(bundleName))
                {
                    ReleaseBundle(v);
                }
            }
            return delete;
        }
        public override void ReleaseInstantiate(int requestId, GameObject go = null)
        {
            if (requestId > 0 && GetLoadingInfo(requestId, out LoadingInfo info))
            {
                info.Destroy();
                RemoveLoadingAsset(requestId);
                Log($" release requestId {requestId}");
            }
            if (go != null)
            {
                int id = go.GetInstanceID();
                if (assetMappingBundleDic.TryGetValue(id, out var bundleName))
                {
                    if (ReleaseBundle(bundleName))
                        assetMappingBundleDic.Remove(id);

                    UnityEngine.Object.Destroy(go);
                    Log($" release obj {go.GetType()}");
                }

            }
        }
        public override void ShowBundleDebug(List<string> bundleNames, List<int> refCounts)
        {
            foreach(var v in loadedBundleDic)
            {
                bundleNames.Add(v.Value.name);
                refCounts.Add(v.Value.refCount);
            }
        }
    }
}
