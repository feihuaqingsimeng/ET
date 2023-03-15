using ET;
using FairyGUI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frame
{
    /// <summary>
    /// 管理Scene上的UI
    /// </summary>
    [ComponentOf(typeof(Scene))]
	public class UISystem: Entity, IAwake
	{
		public Dictionary<string, UIBase> uiList = new();
		public List<ViewStack> viewStackList = new();
        public Dictionary<string, UIConfig> configs = new();
        [StaticField]
		public static UISystem Ins = null;
	}
   
    /// <summary>
    /// 管理Scene上的UI
    /// </summary>
    [FriendOf(typeof(UISystem))]
    public static class UISystemSystem
    {
        [ObjectSystem]
        public class AwakeSystem : AwakeSystem<UISystem>
        {
            protected override void Awake(UISystem self)
            {
                UISystem.Ins = self;
            }
        }
        
        public static void Show(this UISystem self, string name, IUIDataParam data = null)
        {
            try
            {
                AnalysisSystem.Ins.StartRecordTime($"Show {name}");
                var config = self.GetConfig(name);
                if (config == null)
                {
                    Log.Error($"UIConfig 中未配置{name} UI参数");
                    return;
                }
                var stack = self.GetViewStack(name);
                if (stack != null && stack.state == ViewState.cancel)
                    stack.state = ViewState.loading;
                if (stack != null && stack.state == ViewState.loading)
                    return;
                if (stack == null)
                {
                    stack = new ViewStack();
                    stack.name = name;
                    stack.state = ViewState.loading;
                    self.AddViewStack(stack);
                }
                UIBase uiBase = self.GetUI(name);
                if (uiBase != null)
                {
                    stack.state = ViewState.opened;
                    self.AddViewStack(stack);
                    uiBase.SetVisible(true);
                    self.UpdateOrder();
                    ET.EventSystem.Instance.Change(uiBase, data);
                    UIEventSystem.Ins.Publish(new UIShowEvent() { name = name });

                }
                else
                {
                    AnalysisSystem.Ins.StartRecordTime($"LoadFUI {name}");
                    FGUIPackageManager.Ins.LoadFUI(config.package, config.uiName, (com) => {
                        AnalysisSystem.Ins.EndRecordTime($"LoadFUI {name}");
                        if (com == null)
                        {
                            Log.Error($"[加载ui失败]{config.package},{config.uiName}");
                            return;
                        }
                        if (stack.state == ViewState.cancel)
                        {
                            //FGUIPackageManager.Ins.UnLoadFUI(config.package);
                            com.Dispose();
                            self.RemoveViewStack(name);
                            return;
                        }
                        int width = Screen.width;
                        int height = Screen.height;
                        float factor = (float)width / 1080;// ConstValue.ResolusionX;
                        width = Mathf.CeilToInt(width / UIContentScaler.scaleFactor);
                        height = Mathf.CeilToInt(height / UIContentScaler.scaleFactor);
                        float offset = Screen.safeArea.yMax - Screen.safeArea.yMin;
                        com.SetPivot(0.5f, 0, true);
                        com.SetSize(GRoot.inst.width, GRoot.inst.height - Screen.safeArea.y);
                        com.SetXY((GRoot.inst.width) / 2, Screen.safeArea.y);

                        var child = com.GetChild("_bgfull");
                        if (child != null)
                        {
                            child.SetSize(GRoot.inst.width, GRoot.inst.height);
                            child.SetXY(0, -Screen.safeArea.y);
                        }
                        com.name = com.gameObjectName;
                        com.fairyBatching = true;

                        stack.state = ViewState.opened;
                        GRoot.inst.AddChild(com);
                        AnalysisSystem.Ins.StartRecordTime($"{name} component create");
                        UIBase ui = (UIBase)self.AddChild(config.uiComponent, com);
                        AnalysisSystem.Ins.EndRecordTime($"{name} component create");
                        AnalysisSystem.Ins.StartRecordTime($"{name} component Change");
                        ui.SetUIType(name);
                        self.AddUI(name, ui);
                        self.UpdateOrder();
                        ET.EventSystem.Instance.Change(ui, data);
                        UIEventSystem.Ins.Publish(new UIShowEvent() { name = name });
                        AnalysisSystem.Ins.EndRecordTime($"{name} component Change");
                        AnalysisSystem.Ins.EndRecordTime($"Show {name}");
                    });
                    
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message + e.StackTrace);
            }

        }
        public static void Close(this UISystem self, string name, bool isDestroy = true)
        {
            var stack = self.GetViewStack(name);
            if (stack == null) return;
            if (stack.state == ViewState.loading)
            {
                stack.state = ViewState.cancel;
                return;
            }
            if (stack.state == ViewState.closing || stack.state == ViewState.cancel) return;
            stack.state = ViewState.closing;
            //anim
            stack.state = ViewState.close;
            if (isDestroy)
            {
                var config = self.GetConfig(name);
                FGUIPackageManager.Ins.UnLoadFUI(config.package);
                self.RemoveViewStack(name);
                self.RemoveUI(name);
            }
            else
            {
                UIBase uibase = self.GetUI(name);
                uibase.SetVisible(false);

            }
            self.UpdateOrder();
            UIEventSystem.Ins.Publish(new UIHideEvent() { name = name });
            if (isDestroy)
                UIEventSystem.Ins.Publish(new UIDestroyEvent() { name = name });
        }
        private static UIBase GetUI(this UISystem self, string name)
        {
            self.uiList.TryGetValue(name, out UIBase ui);
            return ui;
        }
        private static void UpdateOrder(this UISystem self)
        {
            int index = 0;
            for (int i = 0; i < self.viewStackList.Count; i++)
            {
                var s = self.viewStackList[i];
                if (s.state == ViewState.opened || s.state == ViewState.closing)
                {
                    var ui = self.GetUI(s.name);
                    ui.SetSortingOrder(index * 100 + (int)self.GetConfig(s.name).order * 1000);
                    index++;
                }
            }
        }
        private static void AddUI(this UISystem self, string name, UIBase ui)
        {
            if (self.uiList.ContainsKey(name))
            {
                Log.Error($"UI重复添加 {name}");
                return;
            }
            self.uiList.Add(name, ui);
        }
        private static UIBase RemoveUI(this UISystem self, string name)
        {
            if (self.uiList.TryGetValue(name, out UIBase ui))
            {
                self.uiList.Remove(name);
                ui.Dispose();
                return ui;
            }
            return ui;
        }

        private static ViewStack GetViewStack(this UISystem self, string name)
        {
            foreach (var v in self.viewStackList)
            {
                if (v.name == name)
                    return v;
            }
            return null;
        }
        private static void RemoveViewStack(this UISystem self, string name)
        {
            for (int i = 0; i < self.viewStackList.Count; i++)
            {
                if (self.viewStackList[i].name == name)
                {
                    self.viewStackList.RemoveAt(i);
                    break;
                }
            }
        }
        private static void AddViewStack(this UISystem self, ViewStack stack)
        {
            self.RemoveViewStack(stack.name);
            self.viewStackList.Add(stack);
        }
        public static void AddConfig(this UISystem self, string uiType, UIConfig config)
        {
            self.configs.Add(uiType, config);
        }
        private static UIConfig GetConfig(this UISystem self, string uiType)
        {
            self.configs.TryGetValue(uiType, out UIConfig v);
            return v;
        }
    }
    public enum ViewState
    {
        loading,
        opened,
        closing,
        close,
        cancel,
    }
    public class ViewStack
    {
        public ViewState state;
        public string name;
        public bool isEnable = false;
    }
    public enum UIOrder
    {
        Low,
        Mid,
        Top,
    }
    public class UIConfig
    {
        public string package;
        public string uiName;
        public UIOrder order;
        public Type uiComponent;
        public UIConfig(string package, string uiName, Type uiComponent, UIOrder order = UIOrder.Mid)
        {
            this.package = package;
            this.uiName = uiName;
            this.order = order;
            this.uiComponent = uiComponent;
        }
    }
    public struct UIShowEvent
    {
        public string name;
    }
    public struct UIHideEvent
    {
        public string name;
    }
    public struct UIDestroyEvent
    {
        public string name;
    }
}