using ET;
using FairyGUI;
using GameEvent;
using module.fabao.message;
using Frame;

public class UIFabaoDetail:UIBase
{
	public GTextField textName;
	public GTextField textQuality;
	public GTextField textType;
	public GTextField textDes;
	public GTextField textTime;
	public GTextField textSkillName;
	public GTextField textSkillCD;
	public GTextField textSkillDes;
	public Controller isEquipedCtrl;
	public Controller isShowFuLingCtrl;
	public GTextField[] baseAttr;
	public GTextField[] extraAttr = new GTextField[6];
	public FaBaoInfo info;
	public bool isShowFuLing;

}
public class UIFabaoDetailAwakeSystem : AwakeSystem<UIFabaoDetail, GComponent>
{
	protected override void Awake(UIFabaoDetail self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("name");
		self.textQuality = a.GetText("quality");
		self.textType = a.GetText("type");
		self.textDes = a.GetText("des");
		self.textTime = a.GetText("time");
		self.textSkillName = a.GetText("skillName");
		self.textSkillCD = a.GetText("skillCD");
		self.textSkillDes = a.GetText("skillDes");
		self.isEquipedCtrl = a.GetController("isEquiped");
		self.isShowFuLingCtrl = a.GetController("isShowFuLing");
		self.baseAttr = new GTextField[3] {a.GetText("atk"),a.GetText("def"),a.GetText("hp") };
		var btnClose = a.GetCom("_bgfull");
		var btnEquip = a.GetButton("btnEquip");
		var btnUnEquip = a.GetButton("btnUnEquip");
		var btnDecompose = a.GetButton("btnDecompose");
		var btnFuLing = a.GetButton("btnFuLing");

		for (int i = 0; i < 6; i++)
		{
			self.extraAttr[i] = a.GetText($"attr{i}");
		}
		btnClose.onClick.Add(self.ClickClose);
		btnEquip.onClick.Add(self.ClickEquip);
		btnUnEquip.onClick.Add(self.ClickUnEquip);
		btnDecompose.onClick.Add(self.ClickDecompose);
		btnFuLing.onClick.Add(self.ClickFuLing);

		self.RegisterEvent<FaBaoWearSucEvent>(self.Event_FaBaoWearSuc);

	}
}
public class UIFabaoDetailChangeSystem : ChangeSystem<UIFabaoDetail, IUIDataParam>
{
	protected override void Change(UIFabaoDetail self, IUIDataParam a)
	{
		var p = (UIFabaoDetailParam)a;
		self.info = p.info;
		self.isShowFuLing = p.showFuLing;
		self.Refresh();
	}
}
public static class UIFabaoDetailSystem
{
	public static void Refresh(this UIFabaoDetail self)
	{
		self.isEquipedCtrl.selectedIndex = self.info.packType == FaBaoPackType.EQUIP ? 1 : 0;
		self.isShowFuLingCtrl.selectedIndex = self.isShowFuLing ? 1 : 0;
		var weapon = ConfigSystem.Ins.GetFBMagicWeaponConfig(self.info.modelId);
		var auxiliary = ConfigSystem.Ins.GetFBAuxiliaryConfig(weapon.Get("FBGrade"));
		self.textName.text = string.Format(ConfigSystem.Ins.GetLanguage("fabao_stage"), self.info.stage) + weapon.Get<string>("FBName");
		self.textQuality.text = auxiliary.Get<string>("Name");
		self.textType.text = ConfigSystem.Ins.GetLanguage($"fabao_type_{weapon.Get("FBType")}");
		self.textDes.text = weapon.Get<string>("FBDescribe");
		//base attr
		for(int i = 0; i < self.baseAttr.Length; i++)
		{
			int type = (int)AttributeType.Atk_Per + i;
            self.info.attri.TryGetValue(type, out long value);
			var name = AttributeTypeUtil.GetNameStr((AttributeType)(i+1));
			var vStr = AttributeTypeUtil.GetValueStr((AttributeType)type, value);
			self.baseAttr[i].text = $"{name}+{vStr}"; 
		}
		//special attr
		int index = 0;
		if (self.info.attri_Spe != null)
		{
			foreach (var v in self.info.attri_Spe)
			{
				var name = AttributeTypeUtil.GetNameStr((AttributeType)v.Key);
				var vStr = AttributeTypeUtil.GetValueStr((AttributeType)v.Key, v.Value);
				self.extraAttr[index].text = $"{name}+{vStr}";
				index++;
			}
		}
		for(int i = index; i < self.extraAttr.Length; i++)
		{
			self.extraAttr[i].visible = false;
		}
		self.RefreshSkill();
	}
	public static void RefreshSkill(this UIFabaoDetail self)
	{
		int skillId = self.info.skillId;
		if (skillId == 0)
		{
			self.textSkillName.text = "";
			self.textSkillCD.text = "";
			self.textSkillDes.text = "";
			return;
		}
		var sys = ConfigSystem.Ins;
		var skill = sys.GetSkillConfig(skillId);
		if (skill == null)
		{
			Log.Error($"skill表找不到技能：{skillId} ");
			return;
		}
		self.textSkillName.text = string.Format(sys.GetLanguage("ui_fabao_skill_name"), skill.Get<string>("SkillName"));
		int cd = skill.Get("CD");
		if (cd > 0)
			self.textSkillCD.text = string.Format(sys.GetLanguage("ui_fabao_skill_cd"), cd.ToString());
		else
			self.textSkillCD.text = "";
		int type = skill.Get("SkillType");
		string typeName = sys.GetLanguage($"skill_type_{type}");
		self.textSkillDes.text = $"{typeName}：{skill.Get<string>("SkillDescribe")}";
	}
	public static void ClickClose(this UIFabaoDetail self)
	{
		UISystem.Ins.Close(UIType.UIFabaoDetail);
	}
	public static void ClickEquip(this UIFabaoDetail self)
	{
		if (self.info == null) return;
		FaBaoHandler.SendWearReq(self.info);
	}
	public static void ClickUnEquip(this UIFabaoDetail self)
	{
		if (self.info == null) return;
		var req = new FaBaoWearReq() { packetIndex = (int)self.info.uid ,actionType = 0};
		NetSystem.Send(req);
	}
	public static void ClickDecompose(this UIFabaoDetail self)
	{
		if (self.info == null) return;
		var cfg = ConfigSystem.Ins.GetFBAuxiliaryConfigByFabaoId(self.info.modelId);
		var res = cfg.Get<int[][]>("FBDecompose");
		string des = string.Format(ConfigSystem.Ins.GetLanguage("ui_fabao_decompose_tip"), res[0][1].ToString());
		var param = new UICommonTipParam() { des  = des,sureCallback = self.DecomposeCallback};
		UISystem.Ins.Show(UIType.UICommonTip,param);
	}
	public static void DecomposeCallback(this UIFabaoDetail self)
	{
		self.SendDecomposeReqAsync().Coroutine();
	}
	public static async ETTask SendDecomposeReqAsync(this UIFabaoDetail self)
	{
		var req = new FaBaoDecomposeReq() { packType = (module.fabao.packet.impl.FaBaoPackType)self.info.packType, packetIndex = (int)self.info.uid };
		var resp = (FaBaoChangeResp)await NetSystem.Call(req, typeof(FaBaoChangeResp), self.GetToken(1));
		if (resp == null) return;
		self.ClickClose();
		var sys = ConfigSystem.Ins;
		var cfg = sys.GetFBAuxiliaryConfigByFabaoId(self.info.modelId);
		var res = cfg.Get<int[][]>("FBDecompose");
		string des = string.Format(sys.GetLanguage("common_reward_tip"), ItemUtil.GetItemName(res[0][0]), res[0][1].ToString());
		UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = des });
		
	}
	public static void ClickFuLing(this UIFabaoDetail self)
	{
        Frame.UIEventSystem.Ins.Publish(new FaBaoFuLingSelectEvent() { info = self.info });
		self.ClickClose();
	}
	public static void Event_FaBaoWearSuc(this UIFabaoDetail self,FaBaoWearSucEvent e)
	{
		self.ClickClose();
	}
}