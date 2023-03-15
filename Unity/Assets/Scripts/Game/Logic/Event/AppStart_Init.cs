using ET;
using ET.EventType;
using UnityEngine;
using FairyGUI;
using Frame;

namespace EGame
{
    [Event(SceneType.Process)]
    internal class AppStart_Init : AEvent<AppStart>
    {
        protected override async ETTask Run(Scene scene, AppStart a)
        {
            Root.Instance.Scene.AddComponent<NetThreadComponent>();
            Root.Instance.Scene.AddComponent<OpcodeTypeComponent>();
            Root.Instance.Scene.AddComponent<MessageDispatcherComponent>();
            Root.Instance.Scene.AddComponent<ClientSceneManagerComponent>();

            Root.Instance.Scene.AddComponent<UISystem>();
            Root.Instance.Scene.AddComponent<UIEventSystem>();
            Root.Instance.Scene.AddComponent<AnalysisSystem>();

            Root.Instance.Scene.AddComponent<UIConfigConponent>();
            Root.Instance.Scene.AddComponent<DataSystem>();

            GameObject go = new("ResourceManager");
            go.AddComponent<ResourceManager>();
            go.AddComponent<FGUIPackageManager>();
            go = new("Audio");
            go.AddComponent<AudioListener>();
            go.AddComponent<AudioManager>();

            Application.targetFrameRate = 60;
            var scaler = Stage.inst.gameObject.GetComponent<UIContentScaler>();
            scaler.ignoreOrientation = true;
            GRoot.inst.SetContentScaleFactor(ConstValue.ResolusionX, ConstValue.ResolusionY);
            UISystem.Ins.Show(UIType.UILogin);
            await ETTask.CompletedTask;
        }
    }
}
