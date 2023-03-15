using FairyGUI;
using ET;
using GameEvent;
using System;
using Frame;

public class PetFormationSlotCom : UIBase
{
    public Controller ctrlState;
    public GTextField textLock;

    public PetItemCom item;
    public Action<PetFormationSlotCom, EventContext> moveCallback;
    public Action<PetFormationSlotCom, EventContext> endCallback;
    public PetInfo pet;
    public bool isTouch;
}
class PetFormationSlotComAwakeSystem : AwakeSystem<PetFormationSlotCom, GComponent>
{
    protected override void Awake(PetFormationSlotCom self, GComponent a)
	{
		self.Awake(a);
        self.ctrlState = a.GetController("state");
        self.textLock = a.GetText("tips");
        self.item = self.AddChild<PetItemCom, GComponent>(a.GetCom("item"));

        self.gCom.onClick.Add(self.ClickItem);
        self.gCom.onTouchBegin.Add(self.TouchBegin);
        self.gCom.onTouchMove.Add(self.TouchMove);
        self.gCom.onTouchEnd.Add(self.TouchEnd);
    }
}

public static class PetFormationSlotComSystem
{
	public static void Empty(this PetFormationSlotCom self)
	{
        self.ctrlState.selectedIndex = 0;
        self.pet = null;
	}
    public static void Set(this PetFormationSlotCom self,PetInfo pet)
    {
        if(pet == null)
        {
            self.Empty();
            return;
        }
        self.ctrlState.selectedIndex = 1;
        self.item.Set(pet);
        self.pet = pet;
    }
    public static void Lock(this PetFormationSlotCom self,int lockLevel)
    {
        self.ctrlState.selectedIndex = 2;
        var cfg = ConfigSystem.Ins.GetLvConfig(lockLevel);
        self.textLock.text = cfg.Get<string>("Describe");
    }
    public static void ClickItem(this PetFormationSlotCom self)
    {
        if (self.ctrlState.selectedIndex != 1) return;
        UISystem.Ins.Show(UIType.UIPetDetail, new UIPetDetailParam() { pet = self.item.pet });
    }
    public static void SetTouchCallback(this PetFormationSlotCom self, Action<PetFormationSlotCom, EventContext> move, Action<PetFormationSlotCom, EventContext> end)
    {
        self.moveCallback = move;
        self.endCallback = end;
    }
    
    public static void TouchBegin(this PetFormationSlotCom self, EventContext e)
    {
        self.isTouch = true;
    }
    public static void TouchMove(this PetFormationSlotCom self, EventContext e)
    {
        if (!self.isTouch) return;
        if (self.ctrlState.selectedIndex != 1) return;
        self.moveCallback?.Invoke(self, e);
    }
    public static void TouchEnd(this PetFormationSlotCom self, EventContext e)
    {
        if (!self.isTouch) return;
        self.isTouch = false;
        if (self.ctrlState.selectedIndex != 1) return;
        self.endCallback?.Invoke(self, e);
    }
}