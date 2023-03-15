using FairyGUI;
using ET;
using System;
using Frame;

public class LianDanMatCom : UIBase
{
    public GTextField textNum;
    public Controller ctrlIsEmpty;
    public IconItemCom item;

    public MaterialInfo info;
    public Action<LianDanMatCom> callback;
}



class LianDanMatComAwakeSystem : AwakeSystem<LianDanMatCom, GComponent>
{
    protected override void Awake(LianDanMatCom self, GComponent a)
	{
		self.Awake(a);
        self.textNum = a.GetText("num");
        self.ctrlIsEmpty = a.GetController("isEmpty");
        self.item = self.AddChild<IconItemCom, GComponent>(a.GetCom("item"));
        self.gCom.onClick.Add(self.ClickItem);
    }
}

public static class LianDanMatComSystem
{
	public static void Refresh(this LianDanMatCom self,MaterialInfo info)
	{
        self.info = info;
        if (info.info == null)
        {
            self.Empty();
            return;
        }
        self.item.Set(info.info,0);
        self.item.ShowNum(false);
        self.ctrlIsEmpty.selectedIndex = 0;
        self.textNum.text = $"{info.count}/{info.max}";
    }
    public static void Empty(this LianDanMatCom self)
    {
        self.info.info = null;
        self.info.count = 0;
        self.ctrlIsEmpty.selectedIndex = 1;
        self.textNum.text = $"{self.info.count}/{self.info.max}";
    }
    public static void AddClick(this LianDanMatCom self,Action<LianDanMatCom> callback)
    {
        self.callback = callback;
    }
    public static void ClickItem(this LianDanMatCom self)
    {
        if (self.ctrlIsEmpty.selectedIndex == 1)
            return;
        self.callback?.Invoke(self);
    }
}