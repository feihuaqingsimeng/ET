using FairyGUI;
using ET;
using GameEvent;
using System.Collections.Generic;
using Frame;

public class UIPetLevelUp : UIBase
{
    public GTextField textName;
    public GTextField textStage;
    public GProgressBar expBar;
    public GList itemList;
    public Controller ctrlIsEmpty;
    public List<int> dataList = new List<int>();

    public PetInfo pet;
}
class UIPetLevelUpAwakeSystem : AwakeSystem<UIPetLevelUp, GComponent>
{
    protected override void Awake(UIPetLevelUp self, GComponent a)
	{
		self.Awake(a);
        self.textName = a.GetText("textName");
        self.textStage = a.GetText("textStage");
        self.ctrlIsEmpty = a.GetController("isEmpty");
        var c = a.GetCom("expBar");
        self.expBar = a.GetProgressBar("expBar");
        self.itemList = a.GetList("list");
        var btnClose = a.GetButton("btnClose");

        btnClose.onClick.Add(self.CloseSelf);

        self.itemList.itemRenderer = self.ItemRender;

        var cfgs = ConfigSystem.Ins.GetItemConfig();
        foreach (var cfg in cfgs)
        {
            if(cfg.Value.Get("PropType") == (int)ItemType.PetMedicine)
            {
                self.dataList.Add(cfg.Key);
            }
        }
        self.dataList.Sort();

        self.RegisterEvent<PetUpdateEvent>(self.Event_PetUpdateEvent);
    }
}
class UIPetLevelUpChangeSystem : ChangeSystem<UIPetLevelUp, IUIDataParam>
{
	protected override void Change(UIPetLevelUp self, IUIDataParam a)
	{
        var param = (UIPetLevelUpParam)a;
        self.pet = param.pet;
        self.Refresh();
	}
}
public static class UIPetLevelUpSystem
{
	public static void Refresh(this UIPetLevelUp self)
	{
        self.textName.text =self.pet.name;
        self.RefreshExp();
        self.RefreshList();
    }
    public static void RefreshExp(this UIPetLevelUp self)
    {
        var lvCfg = ConfigSystem.Ins.GetLCPetLv(self.pet.level);
        self.expBar.value = self.pet.exp;
        self.expBar.max = lvCfg.Get("UpgradeExp");
        self.textStage.text = lvCfg.Get<string>("Describe") + lvCfg.Get<string>("Describe2");
    }
    public static void RefreshList(this UIPetLevelUp self)
    {
        self.itemList.numItems = self.dataList.Count;
    }
    public static void ItemRender(this UIPetLevelUp self,int index,GObject go)
    {
        var com = go.asCom;
        var insId = com.displayObject.gameObject.GetInstanceID();
        var item = self.GetChild<PetDanYaoItemCom>(insId);
        if(item == null)
        {
            item = self.AddChild<PetDanYaoItemCom, GComponent>(com);
        }
        item.Refresh(self.dataList[index],self.pet);
    }
    public static void Event_PetUpdateEvent(this UIPetLevelUp self, PetUpdateEvent e)
    {
        self.RefreshExp();
    }
}