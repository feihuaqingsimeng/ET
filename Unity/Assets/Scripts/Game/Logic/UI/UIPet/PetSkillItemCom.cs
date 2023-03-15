using FairyGUI;
using ET;
using GameEvent;
using UnityEngine;
using System;
using Frame;

public class PetSkillItemCom : UIBase
{
    public GTextField textName;
    public Controller ctrlState;
    public Controller ctrlBind;

    public int skillId;
    public Action<PetSkillItemCom> clickCallback;
}
class PetSkillItemComAwakeSystem : AwakeSystem<PetSkillItemCom, GComponent>
{
    protected override void Awake(PetSkillItemCom self, GComponent a)
	{
		self.Awake(a);
        self.textName = a.GetText("name");
        self.ctrlState = a.GetController("state");
        self.ctrlBind = a.GetController("isBind");
        a.onClick.Add(self.ClickItem);
	}
}

public static class PetSkillItemComSystem
{
	
    public static void Empty(this PetSkillItemCom self)
    {
        self.ctrlState.selectedIndex = 0;
    }
    public static void Set(this PetSkillItemCom self ,bool isBind,int skillId)
    {
        self.ctrlState.selectedIndex = 1;
        self.ctrlBind.selectedIndex = isBind ? 1 : 0;
        var cfg = ConfigSystem.Ins.GetSkillConfig(skillId);
        self.skillId = skillId;
        if (cfg == null) return;
        self.textName.text = cfg.Get<string>("SkillName");
    }
    public static void Lock(this PetSkillItemCom self)
    {
        self.ctrlState.selectedIndex = 2;
    }
    public static void AddClick(this PetSkillItemCom self, Action<PetSkillItemCom> clickCallback)
    {
        self.clickCallback = clickCallback;
    }
    public static void ClickItem(this PetSkillItemCom self)
    {
        if (self.ctrlState.selectedIndex == 2)
        {
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("pet_unlockbyevolution"));
            return;
        }
        if (self.ctrlState.selectedIndex == 1)
        {
            UISystem.Ins.ShowSkillTips(self.skillId, self.gCom.LocalToGlobal(new Vector2(self.gCom.width / 2, 0)));
            return;
        }
        self.clickCallback?.Invoke(self);
    }
}