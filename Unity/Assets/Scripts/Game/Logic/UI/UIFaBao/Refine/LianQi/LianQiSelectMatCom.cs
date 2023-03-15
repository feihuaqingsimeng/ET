using FairyGUI;
using ET;
using GameEvent;
using Frame;
using System;

public class LianQiSelectMatCom : UIBase
{
    public GTextField textName;
    public GTextField textNum;
    public Controller ctrlIsSelect;
    public Controller ctrlIsEmpty;

    public MaterialInfo info;
    public int index;
}
class LianQiSelectMatComAwakeSystem : AwakeSystem<LianQiSelectMatCom, GComponent>
{
    protected override void Awake(LianQiSelectMatCom self, GComponent a)
	{
		self.Awake(a);
        self.textName = a.GetText("name");
        self.textNum = a.GetText("num");
        self.ctrlIsEmpty = a.GetController("isEmpty");
        self.ctrlIsSelect = a.GetController("isSelect");
	}
}
public static class LianQiSelectMatComSystem
{
    public static void SetIndex(this LianQiSelectMatCom self, int index)
    {
        self.index = index;
    }
	public static void Set(this LianQiSelectMatCom self, MaterialInfo info)
	{
        self.info = info;
        self.textName.text = info.info.ColorName;
        self.textNum.text = info.count.ToString();
        self.ctrlIsEmpty.selectedIndex = 0;
	}
    public static void Empty(this LianQiSelectMatCom self)
    {
        self.info = null;
        self.ctrlIsEmpty.selectedIndex = 1;
    }
    public static void SetSelect(this LianQiSelectMatCom self,bool isSelect)
    {
        self.ctrlIsSelect.selectedIndex = isSelect ? 1 : 0;
    }
    public static void SetOnClick(this LianQiSelectMatCom self,Action<LianQiSelectMatCom> callback)
    {
        self.gCom.onClick.Add(()=> {
            callback?.Invoke(self);
        });
    }
}