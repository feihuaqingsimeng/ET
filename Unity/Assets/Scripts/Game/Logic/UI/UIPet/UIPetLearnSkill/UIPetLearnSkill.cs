using FairyGUI;
using ET;
using GameEvent;
using System.Collections.Generic;
using Frame;

public class UIPetLearnSkill : UIBase
{
    public GList skillItemList;
    public GList scrollItemList;

    public List<ItemInfo> scrollDataList = new List<ItemInfo>();
    public PetInfo pet;
    public int maxSkill;
}
class UIPetLearnSkillAwakeSystem : AwakeSystem<UIPetLearnSkill, GComponent>
{
    protected override void Awake(UIPetLearnSkill self, GComponent a)
	{
		self.Awake(a);
        self.skillItemList = a.GetList("skillList");
        self.scrollItemList = a.GetList("scrollList");

        var btnClose = a.GetButton("btnClose");
        btnClose.onClick.Add(self.CloseSelf);

        self.skillItemList.SetItemRender(self.SkillItemRender,false);
        self.scrollItemList.SetItemRender(self.ScrollItemRender);

        self.RegisterEvent<PetUpdateEvent>(self.Event_PetUpdateEvent);

    }
}
class UIPetLearnSkillChangeSystem : ChangeSystem<UIPetLearnSkill, IUIDataParam>
{
    protected override void Change(UIPetLearnSkill self, IUIDataParam a)
    {
        var param = (UIPetLearnSkillParam)a;
        self.pet = param.pet;
        self.Refresh();
    }
}
public static class UIPetLearnSkillSystem
{
	public static void Refresh(this UIPetLearnSkill self)
	{
        self.RefreshSkill();
        self.RefreshScroll();
	}
    public static void RefreshSkill(this UIPetLearnSkill self)
    {
        var sys = ConfigSystem.Ins;
        var param = sys.GetGlobal<int[][]>("NumberSkills");
        var cfg = sys.GetLCPetAttribute(self.pet.modelId);
        var grade = cfg.Get("Grade");
        self.maxSkill = param[grade - 1][1];
        self.skillItemList.numItems = ConstValue.PET_SKILL_MAX;
    }
    public static void SkillItemRender(this UIPetLearnSkill self, int index, GObject go)
    {
        var com = go.asCom;
        var insId = com.displayObject.gameObject.GetInstanceID();
        var child = self.GetChild<PetSkillItemCom>(insId);
        if (child == null)
            child = self.AddChildWithId<PetSkillItemCom, GComponent>(insId, com);
        if (index < self.pet.skills.Count)
        {
            child.Set(false, self.pet.skills[index]);
        }
        else if (index < self.maxSkill)
        {
            child.Empty();
        }
        else
        {
            child.Lock();
        }
    }
    public static void RefreshScroll(this UIPetLearnSkill self)
    {
        self.scrollDataList.Clear();
        DataSystem.Ins.ItemModel.GetItemsByType((int)ItemType.PetSkill, self.scrollDataList);
        self.scrollItemList.numItems = self.scrollDataList.Count;
    }
    public static void ScrollItemRender(this UIPetLearnSkill self, int index, GObject go)
    {
        var com = go.asCom;
        var insId = com.displayObject.gameObject.GetInstanceID();
        var child = self.GetChild<SkillScrollItemCom>(insId);
        if (child == null)
            child = self.AddChildWithId<SkillScrollItemCom, GComponent>(insId, com);
        child.Refresh(self.pet, self.scrollDataList[index]);
    }
    public static void Event_PetUpdateEvent(this UIPetLearnSkill self, PetUpdateEvent e)
    {
        self.RefreshSkill();
    }

}