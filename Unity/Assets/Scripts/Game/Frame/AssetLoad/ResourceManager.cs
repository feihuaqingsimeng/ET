using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Frame
{
    public class ResourceManager : MonoBehaviour
    {
        
        public LoadType localLoad = LoadType.AssetDatabase;
        public LoadType buildLoad = LoadType.Bundle;
        public bool enableLog = false;

        private LoadType curLoadType;
        private AssetLoader loader;

#if UNITY_EDITOR
        public List<string> bundlesDebug = new List<string>();
        public List<int> bundlesRefDebug = new List<int>();
#endif

        public static ResourceManager Ins { get; private set; }

        void Awake()
        {
            Ins = this;
#if UNITY_EDITOR
            curLoadType = localLoad;
#else
            curLoadType = buildLoad;
#endif
            loader = LoaderFactory.Create(curLoadType);
        }
        public T LoadAsset<T>(string assetname) where T : UnityEngine.Object
        {
            if(loader == null)
            {
                LogError("loader is nil");
                return default;
            }
            return loader.LoadAsset<T>(assetname);
        }
        public int LoadAssetAsync<T>(string assetname, Action<T, object> callback, object customParam = null) where T : UnityEngine.Object
        {
            if (loader == null)
            {
                LogError("loader is nil");
                return -1;
            }
            return loader.LoadAssetAsync(assetname, callback, customParam);
        }
        public void ReleaseAsset(UnityEngine.Object obj)
        {
            ReleaseAsset(-1, obj);
        }
        public void ReleaseAsset(int requestId, UnityEngine.Object obj = null)
        {
            if (loader == null)
            {
                LogError("loader is nil");
                return;
            }
            loader.ReleaseAsset(requestId, obj);
        }

        public int InstantiateAsync(string assetname, Action<GameObject> callback)
        {
            if (loader == null)
            {
                LogError("loader is nil");
                return -1;
            }
            return loader.InstantiateAsync(assetname, callback);
        }
        public void ReleaseInstantiate(int requestId, GameObject go = null)
        {
            if (loader == null)
            {
                LogError("loader is nil");
                return ;
            }
            loader.ReleaseInstantiate(requestId, go);
        }
        public void ReleaseInstantiate(GameObject go)
        {
            ReleaseInstantiate(-1, go);
        }
        public string LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                LogError($"找不到文件：{path}");
                return "";
            }
            using (StreamReader sr = new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }
#if UNITY_EDITOR
        public void Update()
        {
            bundlesDebug.Clear();
            bundlesRefDebug.Clear();
            loader.ShowBundleDebug(bundlesDebug, bundlesRefDebug);
        }
#endif
        void OnDestroy()
        {
            loader.Destroy();
        }
        public void Log(object str)
        {
            if (enableLog)
                Debug.Log(str);
        }
        public void LogError(object str)
        {
            if (enableLog)
                Debug.LogError(str);
        }
    }
}
