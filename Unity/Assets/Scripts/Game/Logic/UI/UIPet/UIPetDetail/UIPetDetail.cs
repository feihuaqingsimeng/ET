using FairyGUI;
using ET;
using GameEvent;
using System.Collections.Generic;
using module.pet.message;
using Frame;

public class UIPetDetail : UIBase
{
    public GTextField textName;
    public GTextField textStage;
    public GTextField textType;
    public GList attrItemList;
    public GList skillItemList;
    public Controller ctrlBattleState;

    public PetInfo pet;
    public int maxSkill;
    public List<AttributeType> attrList = new List<AttributeType>();
}
class UIPetDetailAwakeSystem : AwakeSystem<UIPetDetail, GComponent>
{
    protected override void Awake(UIPetDetail self, GComponent a)
	{
		self.Awake(a);
        self.textName = a.GetText("textName");
        self.textStage = a.GetText("textStage");
        self.textType = a.GetText("textType");
        self.attrItemList = a.GetList("attrList");
        self.skillItemList = a.GetList("skillList");
        self.ctrlBattleState = a.GetController("battleState");
        var btnClose = a.GetButton("btnClose");
        var btnEvolution = a.GetButton("btnEvolution");
        var btnLevelUp = a.GetButton("btnLevelUp");
        var btnSkill = a.GetButton("btnSkill");
        var btnEquiped = a.GetButton("btnEquiped");
        var btnUnEquiped = a.GetButton("btnUnEquiped");


        self.attrItemList.itemRenderer = self.AttrItemRender;
        self.skillItemList.itemRenderer = self.SkillItemRender;

        btnClose.onClick.Add(self.CloseSelf);
        btnEvolution.onClick.Add(self.ClickEvolution);
        btnLevelUp.onClick.Add(self.ClickLevelUp);
        btnSkill.onClick.Add(self.ClickSkill);
        btnEquiped.onClick.Add(self.ClickEquiped);
        btnUnEquiped.onClick.Add(self.ClickUnEquiped);

        self.RegisterEvent<PetUpdateEvent>(self.Event_PetUpdateEvent);

    }
}
class UIPetDetailChangeSystem : ChangeSystem<UIPetDetail, IUIDataParam>
{
	protected override void Change(UIPetDetail self, IUIDataParam a)
	{
        var param = (UIPetDetailParam)a;
        self.pet = param.pet;
        self.Refresh();
	}
}
public static class UIPetDetailSystem
{
	public static void Refresh(this UIPetDetail self)
	{
        self.RefreshAttr();
        self.RefreshSkill();
        self.RefreshInBattle();
    }
    public static void RefreshAttr(this UIPetDetail self)
    {
        var sys = ConfigSystem.Ins;
        var cfg = sys.GetLCPetAttribute(self.pet.modelId);
        var lvCfg = sys.GetLCPetLv(self.pet.level);
        self.textStage.text = lvCfg.Get<string>("Describe") + lvCfg.Get<string>("Describe2");
        self.textName.text = self.pet.name;
        self.textType.text = sys.GetLanguage($"pet_grade_{self.pet.quality}");
        self.attrList.Clear();
        AttributeTypeUtil.GetBaseList(self.attrList);
        AttributeTypeUtil.GetSpecialList(self.attrList);
        self.attrItemList.numItems = self.attrList.Count;
    }
    public static void AttrItemRender(this UIPetDetail self,int index,GObject go)
    {
        var com = go.asCom;
        var type = self.attrList[index];
        self.pet.attrs.TryGetValue((int)type, out var value);
        com.GetText("name").text = AttributeTypeUtil.GetNameStr(type);
        com.GetText("value").text = AttributeTypeUtil.GetValueStr(type, value);
    }
    public static void RefreshSkill(this UIPetDetail self)
    {
        var sys = ConfigSystem.Ins;
        var param = sys.GetGlobal<int[][]>("NumberSkills");
        var cfg = sys.GetLCPetAttribute(self.pet.modelId);
        var grade = cfg.Get("Grade");
        self.maxSkill = param[grade - 1][1];
        self.skillItemList.numItems = ConstValue.PET_SKILL_MAX;
    }
    public static void SkillItemRender(this UIPetDetail self, int index, GObject go)
    {
        var com = go.asCom;
        var insId = com.displayObject.gameObject.GetInstanceID();
        var child = self.GetChild<PetSkillItemCom>(insId);
        if (child == null)
        {
            child = self.AddChildWithId<PetSkillItemCom, GComponent>(insId, com);
            child.AddClick((item) => {
                UISystem.Ins.Show(UIType.UIPetLearnSkill, new UIPetLearnSkillParam() { pet = self.pet });
            });
        }
            
        if (index < self.pet.skills.Count)
        {
            child.Set(false, self.pet.skills[index]);
        }else if (index < self.maxSkill)
        {
            child.Empty();
        }
        else
        {
            child.Lock();
        }
    }
    public static void RefreshInBattle(this UIPetDetail self)
    {
        if (self.pet.battleIndex > 0)
            self.ctrlBattleState.selectedIndex = 2;
        else if (DataSystem.Ins.PetModel.GetEmptyBattleSlot() > 0)
            self.ctrlBattleState.selectedIndex = 1;
        else
            self.ctrlBattleState.selectedIndex = 0;
    }
    public static void ClickEvolution(this UIPetDetail self)
    {
        UISystem.Ins.Show(UIType.UIPetEvolution, new UIPetEvolutionParam() { pet = self.pet });
    }
    public static void ClickLevelUp(this UIPetDetail self)
    {
        UISystem.Ins.Show(UIType.UIPetLevelUp, new UIPetLevelUpParam() { pet = self.pet });
    }
    public static void ClickSkill(this UIPetDetail self)
    {
        UISystem.Ins.Show(UIType.UIPetLearnSkill, new UIPetLearnSkillParam() { pet = self.pet });
    }
    public static async void ClickEquiped(this UIPetDetail self)
    {
        var index = DataSystem.Ins.PetModel.GetEmptyBattleSlot();
        if (index == -1) return;
        var token = self.GetToken(1);
        var req = new PetChuZhanReq() { petUuid = self.pet.uid, zhanIndex = index };
        var resp = (PetChuZhanResp) await NetSystem.Call(req, typeof(PetChuZhanResp), token);
        if (resp == null) return;
        self.RefreshInBattle();

    }
    public static async void ClickUnEquiped(this UIPetDetail self)
    {
        var token = self.GetToken(1);
        var req = new PetChuZhanReq() { petUuid = self.pet.uid, zhanIndex = 0 };
        var resp = (PetChuZhanResp)await NetSystem.Call(req, typeof(PetChuZhanResp), token);
        if (resp == null) return;
        self.RefreshInBattle();
    }

    public static void Event_PetUpdateEvent(this UIPetDetail self, PetUpdateEvent e)
    {
        self.RefreshAttr();
        self.RefreshInBattle();
    }
}