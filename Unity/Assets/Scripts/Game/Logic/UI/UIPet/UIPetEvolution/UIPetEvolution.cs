using FairyGUI;
using ET;
using GameEvent;
using System.Collections.Generic;
using UnityEngine;
using module.pet.message;
using Frame;

public class UIPetEvolution : UIBase
{
    public GTextField textName;
    public GTextField textStage;
    public GTextField textType;
    public GTextField textNextType;
    public GProgressBar progress;
    public GList attrItemList;
    public GList petItemList;

    public PetInfo pet;
    public List<AttributeType> attrList = new List<AttributeType>();
    public List<PetInfo> petInfoList = new List<PetInfo>();

    public PetItemCom selectItem;

}
class UIPetEvolutionAwakeSystem : AwakeSystem<UIPetEvolution, GComponent>
{
    protected override void Awake(UIPetEvolution self, GComponent a)
	{
		self.Awake(a);
        self.textName = a.GetText("textName");
        self.textStage = a.GetText("textStage");
        self.textType = a.GetText("textType");
        self.textNextType = a.GetText("nextType");
        self.progress = a.GetProgressBar("progress");
        self.attrItemList = a.GetList("attrList");
        self.petItemList = a.GetList("petList");
        var btnClose = a.GetButton("btnClose");
        var btnEvolution = a.GetButton("btnEvolution");

        btnClose.onClick.Add(self.CloseSelf);
        btnEvolution.onClick.Add(self.ClickEvolution);

        self.attrItemList.itemRenderer = self.AttrItemRender;
        self.petItemList.itemRenderer = self.PetItemRender;

    }
}
class UIPetEvolutionChangeSystem : ChangeSystem<UIPetEvolution, IUIDataParam>
{
	protected override void Change(UIPetEvolution self, IUIDataParam a)
	{
        var param = (UIPetEvolutionParam)a;
        self.pet = param.pet;
        self.Refresh();
        
	}
}
public static class UIPetEvolutionSystem
{
	public static void Refresh(this UIPetEvolution self)
	{
        var sys = ConfigSystem.Ins;
        var cfg = sys.GetLCPetAttribute(self.pet.modelId);
        var lvCfg = sys.GetLCPetLv(self.pet.level);
        self.textName.text = self.pet.name;
        self.textStage.text = lvCfg.Get<string>("Describe") + lvCfg.Get<string>("Describe2");
        self.textType.text = sys.GetLanguage($"pet_grade_{self.pet.quality}");
        if (self.pet.IsMaxQuality())
        {
            self.textNextType.text = "";
        }
        else
        {
            self.textNextType.text = sys.GetLanguage($"pet_grade_{self.pet.quality +1}");
            self.progress.value = self.pet.jinhuaExp;
            var param = sys.GetGlobal<int[][]>("EvolutionExe");
            self.progress.max = param[self.pet.quality - 1][1];
        }
        self.RefreshAttr();
        self.RefreshPetList();
    }
    public static void RefreshExp(this UIPetEvolution self)
    {
        self.progress.value = self.pet.jinhuaExp;
    }
    public static void RefreshAttr(this UIPetEvolution self)
    {
        self.attrList.Clear();
        AttributeTypeUtil.GetBaseList(self.attrList);
        AttributeTypeUtil.GetSpecialList(self.attrList);
        self.attrItemList.numItems = self.attrList.Count;
    }
    public static void AttrItemRender(this UIPetEvolution self,int index,GObject go)
    {
        var com = go.asCom;
        var type = self.attrList[index];
        self.pet.attrs.TryGetValue((int)type, out var value);
        var before = com.GetCom("before");
        var next = com.GetCom("next");
        var textAdd = com.GetText("add");
        before.GetText("name").text = AttributeTypeUtil.GetNameStr(type);
        before.GetText("value").text = AttributeTypeUtil.GetValueStr(type, value);
        if(self.pet.quality == ConstValue.PET_QUALITY_MAX)
        {
            textAdd.visible = false;
            next.visible = false;
            return;
        }
        var sys = ConfigSystem.Ins;
        var cfg = sys.GetLCPetAttribute(self.pet.modelId);
        var nextCfg = ConfigSystem.Ins.GetLCPetAttribute(cfg.Get("EvolutionLCID"));
        if (nextCfg == null) return;
        var lvCfg = sys.GetLCPetLv(self.pet.level);
        next.GetText("name").text = AttributeTypeUtil.GetNameStr(type);
        
        if (AttributeTypeUtil.IsBaseAttr(type))
        {
            int factor = self.GetFactor(nextCfg.Get<int[][]>("BasicsCoefficient"), (int)type) - self.GetFactor(cfg.Get<int[][]>("BasicsCoefficient"), (int)type);
            textAdd.text = $"{factor/100}%";
            next.GetText("value").text = $"+{AttributeTypeUtil.GetValueStr(type, (long)( (1+factor/10000.0f)* value))}";
        }
        else
        {
            int factor = self.GetFactor(nextCfg.Get<int[][]>("SpecialCoefficient"), (int)type) - self.GetFactor(cfg.Get<int[][]>("SpecialCoefficient"), (int)type);
            long nextValue = value + factor;
            next.GetText("value").text = $"+{AttributeTypeUtil.GetValueStr(type, nextValue)}";
            textAdd.text = $"{(factor/100)}%";
        }
    }
    public static int GetFactor(this UIPetEvolution self,int[][] factors,int type)
    {
        if (factors == null) return 0;
        for(int i = 0; i < factors.Length; i++)
        {
            if (factors[i][0] == type)
                return factors[i][1];
        }
        return 0;
    }
    public static void RefreshPetList(this UIPetEvolution self)
    {
        self.petInfoList.Clear();
        foreach (var v in DataSystem.Ins.PetModel.pets)
        {
            if (v.Value != self.pet && v.Value.battleIndex == 0 && v.Value.quality == self.pet.quality)
                self.petInfoList.Add(v.Value);
        }
        self.petItemList.numItems = self.petInfoList.Count;
    }
    public static void PetItemRender(this UIPetEvolution self, int index, GObject go)
    {
        var data = self.petInfoList[index];
        var com = go.asCom;
        var insId = com.displayObject.gameObject.GetInstanceID();
        var child = self.GetChild<PetItemCom>(insId);
        if (child == null)
            child = self.AddChildWithId<PetItemCom, GComponent>(insId, com);
        child.Set(data);
        child.AddClick(self.ClickPetItem);
    }
    public static void ClickPetItem(this UIPetEvolution self, PetItemCom item)
    {
        if (self.selectItem != null && self.selectItem != item)
            self.selectItem.SetSelect(false);
        self.selectItem = item;
        self.selectItem.SetSelect(true);
        self.progress.value = self.pet.jinhuaExp + 1;
    }
    public static async void ClickEvolution(this UIPetEvolution self)
    {
        if(self.selectItem == null)
        {
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("uipet_choicepet_evolution"));
            return;
        }
        var token = self.GetToken(1);
        var req = new PetJinhuaReq
        {
            petUuid = self.pet.uid,
            eatPetUuid = self.selectItem.pet.uid
        };
        var resp = (PetJinhuaResp) await NetSystem.Call(req, typeof(PetJinhuaResp), token);
        if (resp == null) return;
        if (self.pet.modelId != self.pet.oldPetId)
        {
            self.Refresh();
            var cfg = ConfigSystem.Ins.GetLCPetAttribute(self.pet.modelId);
            var str = ConfigSystem.Ins.GetLanguage("uipet_evolution_suc");
            UISystem.Ins.ShowFlowTipView(string.Format(str,ConfigSystem.Ins.GetLanguage($"pet_grade_{cfg.Get("Grade")}")));
        }
        else
        {
            self.RefreshExp();
            self.RefreshPetList();
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("uipet_evolution_exp_add"));
        }
    }
}