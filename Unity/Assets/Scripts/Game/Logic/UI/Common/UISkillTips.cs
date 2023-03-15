using FairyGUI;
using ET;
using GameEvent;
using UnityEngine;
using Frame;

public class UISkillTips : UIBase
{
    public GTextField textName;
    public GTextField textCD;
    public GTextField textDes;
    public GGroup group;
}
class UISkillTipsAwakeSystem : AwakeSystem<UISkillTips, GComponent>
{
    protected override void Awake(UISkillTips self, GComponent a)
	{
		self.Awake(a);
        self.textName = a.GetText("skillName");
        self.textCD = a.GetText("skillCD");
        self.textDes = a.GetText("skillDes");
        self.group = a.GetGroup("group");

        a.onClick.Add(self.CloseSelf);
	}
}
class UISkillTipsChangeSystem : ChangeSystem<UISkillTips, IUIDataParam>
{
	protected override void Change(UISkillTips self, IUIDataParam a)
	{
        var param = (UISkillTipsParam)a;
        self.Refresh(param.skillId);

        self.group.EnsureBoundsCorrect();
        var p = self.gCom.GlobalToLocal(new Vector2(param.globalPos.x - self.group.width / 2, param.globalPos.y - self.group.height));
        p.x = Mathf.Min(Mathf.Max(0, p.x), ConstValue.ResolusionX-self.group.width);
        p.y = Mathf.Min(Mathf.Max(0, p.y), ConstValue.ResolusionY-self.group.height);
        self.group.SetXY(p.x , p.y);
	}
}
public static class UISkillTipsSystem
{
	public static void Refresh(this UISkillTips self,int skillId)
	{
        var cfg = ConfigSystem.Ins.GetSkillConfig(skillId);
        if (cfg == null) return;
        self.textName.text = cfg.Get<string>("SkillName");
        self.textDes.text = cfg.Get<string>("SkillDescribe");
        var cd = cfg.Get("CD");
        if (cd > 0)
            self.textCD.text = string.Format(ConfigSystem.Ins.GetLanguage("ui_fabao_skill_cd"), cd);
        else
            self.textCD.text = "";

    }
}