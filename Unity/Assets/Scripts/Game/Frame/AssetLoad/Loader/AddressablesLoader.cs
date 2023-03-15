/*using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Frame
{
    public class AddressablesLoader : AssetLoader
    {
        class LoadingInfo:ILoadingInfo
        {
            public string name;
            public AsyncOperationHandle operation;

            public void Destroy()
            {
                Addressables.Release(operation);
            }
        }
        public override int InstantiateAsync(string assetname, Action<GameObject> callback)
        {
            int requestId = GetUniqueId();
            var handle = Addressables.InstantiateAsync(assetname);
            handle.Completed += (operation) =>
            {
                RemoveLoadingAsset(requestId);
                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(operation.Result);
                }
                else
                {
                    LogError("找不到资源:" + assetname);
                    Addressables.Release(operation);
                }
            };
            LoadingInfo info = new LoadingInfo
            {
                name = assetname,
                operation = handle
            };
            AddLoadingAsset(requestId, info);
            return requestId;
        }

        public override T LoadAsset<T>(string assetname)
        {
            Log($"资源加载:{assetname}");
            var op = Addressables.LoadAssetAsync<T>(assetname);
            var obj = op.WaitForCompletion();
            if (obj == null)
                LogError($"找不到资源:{assetname}");
            return obj;
        }

        public override int LoadAssetAsync<T>(string assetname, Action<T, object> callback, object customParam = null)
        {
            Log($"资源加载Async:{assetname}");
            int requestId = GetUniqueId();
            var handle = Addressables.LoadAssetAsync<T>(assetname);
            handle.Completed += (operation) =>
            {
                if (!HasLoadingAsset(requestId))
                {
                    Addressables.Release(operation);
                    return;
                }
                RemoveLoadingAsset(requestId);
                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    Log($"资源加载成功:{assetname},requestId{requestId}");
                    callback?.Invoke(operation.Result, customParam);
                }
                else
                {
                    LogError($"找不到资源:{assetname}");
                    Addressables.Release(operation);
                }
            };

            LoadingInfo info = new LoadingInfo
            {
                name = assetname,
                operation = handle
            };
            AddLoadingAsset(requestId, info);
            return requestId;
        }

        public override void ReleaseAsset(int requestId, UnityEngine.Object obj = null)
        {
            if (requestId > 0 && GetLoadingInfo(requestId, out LoadingInfo info))
            {
                Addressables.Release(info.operation);
                RemoveLoadingAsset(requestId);
                Log($"Addressables release requestId {requestId}");
            }
            if (obj != null)
            {
                Addressables.Release(obj);
                Log($"Addressables release obj {obj.GetType()}");
            }
        }

        public override void ReleaseInstantiate(int requestId, GameObject go = null)
        {
            if (requestId >= 0 && GetLoadingInfo(requestId, out LoadingInfo info))
            {
                Addressables.ReleaseInstance(info.operation);
                RemoveLoadingAsset(requestId);
            }
            if (go != null)
                Addressables.ReleaseInstance(go);
        }
    }
}*/
