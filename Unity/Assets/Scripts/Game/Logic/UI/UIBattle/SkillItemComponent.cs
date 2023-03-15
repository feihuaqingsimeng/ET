using FairyGUI;
using ET;
using Frame;

public class SkillItemComponent : UIBase
{
	public GTextField textSkillId;
	public GTextField textCd;
	public Controller isEmptyCtrl;

	public FaBaoInfo info;
}
class SkillItemComponentAwakeSystem : AwakeSystem<SkillItemComponent, GComponent>
{
	protected override void Awake(SkillItemComponent self, GComponent a)
	{
		self.Awake(a);
		self.textSkillId = a.GetText("skillId");
		self.textCd = a.GetText("cd");
		self.isEmptyCtrl = a.GetController("isEmpty");
	}
}

public static class SkillItemComponentSystem
{
	
	public static void Empty(this SkillItemComponent self)
	{
		self.isEmptyCtrl.selectedIndex = 1;
	}
	public static void Set(this SkillItemComponent self,FaBaoInfo info)
	{
		self.info = info;
		if(info == null || info.skillId == 0)
		{
			self.Empty();
			return;
		}
		self.isEmptyCtrl.selectedIndex = 0;
		
		var cfg = ConfigSystem.Ins.GetSkillConfig(self.info.skillId);
		if (cfg == null) return;
		self.textSkillId.text = cfg.Get<string>("SkillName");
		self.RefreshCD(0);
	}
	public static void RefreshCD(this SkillItemComponent self,int cd)
	{
        self.textCd.text = string.Format(ConfigSystem.Ins.GetLanguage("ui_fabao_skill_cd2"), cd);
	}
}