

using ET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Frame
{

    public abstract class AssetLoader : IAssetLoader
    {
        protected Dictionary<int, ILoadingInfo> loadingAssetDic = new Dictionary<int, ILoadingInfo>();
        [StaticField]
        private static int uniqueId;
        protected static int GetUniqueId()
        {
            uniqueId = (uniqueId) % int.MaxValue + 1;
            return uniqueId;
        }
        protected void AddLoadingAsset(int requestId, ILoadingInfo info)
        {
            if (loadingAssetDic.ContainsKey(requestId))
            {
                LogError($"重复添加requestId {requestId}");
                return;
            }
            loadingAssetDic.Add(requestId, info);
        }
        protected void RemoveLoadingAsset(int requestId)
        {
            loadingAssetDic.Remove(requestId);
        }
        protected bool HasLoadingAsset(int requestId)
        {
            return loadingAssetDic.ContainsKey(requestId);
        }
        
        protected bool GetLoadingInfo<T>(int requestId,out T info) where T: class,ILoadingInfo
        {
            bool flag = loadingAssetDic.TryGetValue(requestId, out var _info);
            info = _info as T;
            return flag;
        }
        protected void Log(object str)
        {
            ResourceManager.Ins.Log(str);
        }
        protected void LogError(object str)
        {
            ResourceManager.Ins.LogError(str);
        }
        public virtual void Destroy()
        {
            foreach (var v in loadingAssetDic)
            {
                v.Value.Destroy();
            }
            loadingAssetDic.Clear();
        }
        public virtual void ShowBundleDebug(List<string> bundleNames,List<int> refCounts) { }

        public abstract int InstantiateAsync(string assetname, Action<GameObject> callback);

        public abstract T LoadAsset<T>(string assetname) where T :UnityEngine.Object;

        public abstract int LoadAssetAsync<T>(string assetname, Action<T, object> callback, object customParam = null) where T : UnityEngine.Object;

        public abstract void ReleaseAsset(int requestId, UnityEngine.Object obj = null);

        public abstract void ReleaseInstantiate(int requestId, GameObject go = null);

    }
}
