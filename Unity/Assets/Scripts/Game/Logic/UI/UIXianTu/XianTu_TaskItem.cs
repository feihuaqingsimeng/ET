using FairyGUI;
using ET;
using GameEvent;
using Frame;

public class XianTu_TaskItem : UIBase
{
}
class XianTu_TaskItemAwakeSystem : AwakeSystem<XianTu_TaskItem, GComponent>
{
	protected override void Awake(XianTu_TaskItem self, GComponent a)
	{
		self.Awake(a);
        var btnBG = a.GetButton("btnBG");
        btnBG.onClick.Add(self.ClickItem);
    }
}

public static class XianTu_TaskItemSystem
{
	
    public static void ClickItem(this XianTu_TaskItem self)
    {
        UISystem.Ins.Show(UIType.UITask);
    }
   
}