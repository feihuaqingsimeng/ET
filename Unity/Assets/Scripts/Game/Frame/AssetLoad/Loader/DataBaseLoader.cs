using System;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Frame
{
    public class DataBaseLoader : AssetLoader
    {
#if UNITY_EDITOR
        private const string PREFIX_ASSET_NAME = "assets/bundles/";
        public string GetAssetName(string assetname)
        {
            return $"{PREFIX_ASSET_NAME}{assetname}";
        }
        public override int InstantiateAsync(string assetname, Action<GameObject> callback)
        {
            LoadAssetAsync<GameObject>(assetname, (obj,param) =>
            {
                GameObject go = null ;
                if (obj != null)
                    go = UnityEngine.Object.Instantiate(obj);
                callback?.Invoke(go);
            });
            return -1;
        }
        public override T LoadAsset<T>(string assetname)
        {
            assetname = GetAssetName(assetname);
            Log($"加载Asset {assetname}");
            return AssetDatabase.LoadAssetAtPath<T>(assetname);
        }

        public override int LoadAssetAsync<T>(string assetname, Action<T, object> callback, object customParam = null)
        {
            ResourceManager.Ins.StartCoroutine(_LoadAssetAsync(assetname, callback, customParam));
            return -1;
        }
        private IEnumerator _LoadAssetAsync<T>(string assetname, Action<T, object> callback, object customParam = null) where T : UnityEngine.Object
        {
            yield return null;
            var obj = LoadAsset<T>(assetname);
            callback?.Invoke(obj, customParam);
        }
        public override void ReleaseAsset(int requestId, UnityEngine.Object obj = null)
        {
            
        }

        public override void ReleaseInstantiate(int requestId, GameObject go = null)
        {
            
        }
#else
    public override int InstantiateAsync(string assetname, Action<GameObject> callback)
        {
            throw new NotImplementedException();
        }

        public override T LoadAsset<T>(string assetname)
        {
            throw new NotImplementedException();
        }

        public override int LoadAssetAsync<T>(string assetname, Action<T, object> callback, object customParam = null)
        {
            throw new NotImplementedException();
        }

        public override void ReleaseAsset(int requestId, UnityEngine.Object obj = null)
        {
            throw new NotImplementedException();
        }

        public override void ReleaseInstantiate(int requestId, GameObject go = null)
        {
            throw new NotImplementedException();
        }
#endif
    }

}
