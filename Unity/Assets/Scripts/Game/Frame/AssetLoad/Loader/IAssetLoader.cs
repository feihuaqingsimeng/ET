using System;
using UnityEngine;

namespace Frame
{
    public interface IAssetLoader
    {
        T LoadAsset<T>(string assetname) where T : UnityEngine.Object;
        int LoadAssetAsync<T>(string assetname, Action<T, object> callback, object customParam = null) where T : UnityEngine.Object;
        void ReleaseAsset(int requestId, UnityEngine.Object obj = null);
        int InstantiateAsync(string assetname, Action<GameObject> callback);
        void ReleaseInstantiate(int requestId, GameObject go = null);
    }
    public interface ILoadingInfo
    {
        void Destroy();
    }
}