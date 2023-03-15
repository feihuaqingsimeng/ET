using FairyGUI;
using ET;
using System;
using Frame;

public class PetItemCom : UIBase
{
    public GTextField textType;
    public GTextField textName;
    public GTextField textStage;
    public Controller ctrlSelect;
    public Controller ctrlBattle;

    public PetInfo pet;
    public Action<PetItemCom> callback;
}
class PetItemComAwakeSystem : AwakeSystem<PetItemCom, GComponent>
{
    protected override void Awake(PetItemCom self, GComponent a)
	{
		self.Awake(a);
        self.textType = a.GetText("type");
        self.textName = a.GetText("name");
        self.textStage = a.GetText("stage");
        self.ctrlSelect = a.GetController("isSelect");
        self.ctrlBattle = a.GetController("isInBattle");
    }
}

public static class PetItemComSystem
{
	public static void Set(this PetItemCom self, PetInfo info)
	{
        self.pet = info;
        var sys = ConfigSystem.Ins;
        var cfg = sys.GetLCPetAttribute(self.pet.modelId);
        self.textName.text = self.pet.name;
        self.textType.text = sys.GetLanguage($"pet_grade_{self.pet.quality}");
        var lvCfg = sys.GetLCPetLv(self.pet.level);
        self.textStage.text = $"[{lvCfg.Get<string>("Describe")}{lvCfg.Get<string>("Describe2")}]";
    }
    public static void SetSelect(this PetItemCom self,bool isSelect)
    {
        self.ctrlSelect.selectedIndex = isSelect ? 1 : 0;
    }
    public static void SetInBattle(this PetItemCom self,bool isInBattle)
    {
        self.ctrlBattle.selectedIndex = isInBattle ? 1 : 0;
    }
    public static void AddClick(this PetItemCom self,Action<PetItemCom> callback)
    {
        if (self.callback == null)
            self.gCom.onClick.Add(self.ClickItem);
        self.callback = callback;
    }
    public static void ClickItem(this PetItemCom self)
    {
        self.callback?.Invoke(self);
    }
    public static void SetPos(this PetItemCom self, float x, float y)
    {
        self.gCom.SetXY(x, y);
    }
    public static void InvalidateBatchingState(this PetItemCom self)
    {
        self.gCom.InvalidateBatchingState();
    }
}