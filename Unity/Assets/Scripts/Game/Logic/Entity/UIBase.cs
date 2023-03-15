using ET;
using FairyGUI;

namespace Frame
{
    public interface IUIDataParam
    {
    }

    [ChildOf]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ETAllProjectAnalyzers", "ET0003:实体类限制多层继承", Justification = "<挂起>")]
    public abstract class UIBase : ManagedEntity, IAwake<GComponent>, IChange<IUIDataParam>
    {
        public GComponent gCom;
        public string uiType;
       
        public UIBase() { }
        public override void Dispose()
        {
            base.Dispose();
            if (gCom != null)
                gCom.Dispose();
            gCom = null;
        }
        public void SetVisible(bool visible)
        {
            gCom.visible = visible;
        }
        public void SetUIType(string uiType)
        {
            this.uiType = uiType;
        }
        public void SetSortingOrder(int order)
        {
            gCom.sortingOrder = order;
        }
    }
    [FriendOf(typeof(UIBase))]
    public static class UIBaseSystem
    {
        public static void Awake(this UIBase self,GComponent go)
        {
            self.gCom = go;
        }
        public static void CloseSelf(this UIBase self)
        {
            UISystem.Ins.Close(self.uiType);
        }
        public static T GetChildWithGComponent<T>(this UIBase self,GComponent go,bool isAutoAdd = true, ManagedEntity parent = null) where T : Entity, IAwake<GComponent>
        {
            if (go == null) return default;
            if (parent == null)
                parent = self;
            int instId = go.displayObject.gameObject.GetInstanceID();
            var child = parent.GetChild<T>(instId);
            if(child == null && isAutoAdd)
            {
                child = parent.AddChildWithId<T, GComponent>(instId, go);
            }
            return child;
        }
    }
}
