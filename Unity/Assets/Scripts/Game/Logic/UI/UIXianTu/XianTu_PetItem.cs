using FairyGUI;
using ET;
using GameEvent;
using Frame;
using module.counter.model;

public class XianTu_PetItem : UIBase
{
    public GTextField textNum;
    public GButton btnAdd;
}
class XianTu_PetItemAwakeSystem : AwakeSystem<XianTu_PetItem, GComponent>
{
    protected override void Awake(XianTu_PetItem self, GComponent a)
	{
		self.Awake(a);
        self.textNum = a.GetCom("num").GetText("num");
        self.btnAdd = a.GetCom("num").GetButton("add");

        var btnBG = a.GetButton("btnBG");

        self.btnAdd.onClick.Add(self.ClickAdd);
        btnBG.onClick.Add(self.ClickItem);

        self.Refresh();

        self.RegisterEvent<CounterUpdateEvent>(self.Event_CounterUpdateEvent);
    }
}

public static class XianTu_PetItemSystem
{
	public static void Refresh(this XianTu_PetItem self)
	{
        int max = ConfigSystem.Ins.GetGlobal("FreeArrest");
        int use = DataSystem.Ins.PlayerModel.GetCounter(CounterType.PET_CATCH);
        self.textNum.text = $"{max - use}/{max}";

    }
    public static void ClickItem(this XianTu_PetItem self)
    {
        UISystem.Ins.Show(UIType.UIPetCapture);
    }
    public static void ClickAdd(this XianTu_PetItem self)
    {

    }
    public static void Event_CounterUpdateEvent(this XianTu_PetItem self, CounterUpdateEvent e)
    {
        self.Refresh();
    }
}