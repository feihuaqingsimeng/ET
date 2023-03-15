using ET;
using FairyGUI;
using GameEvent;
using module.fabao.packet.impl;
using module.fabao.message;
using Frame;
using module.pack.message;

public class UIEquipDetail:UIBase
{
	public GTextField textName;
	public GTextField textLevel;
	public GTextField textType;
	public GTextField textDes;
	public GTextField textSkillName;
	public GTextField textSkillCD;
	public GTextField textSkillDes;
	public Controller isEquipedCtrl;
    public Controller isShowSellCtrl;
	public GTextField[] baseAttr;
	public GTextField[] extraAttr = new GTextField[6];
	//背包位置
	public EquipInfo info;

}
public class UIEquipDetailAwakeSystem : AwakeSystem<UIEquipDetail, GComponent>
{
	protected override void Awake(UIEquipDetail self, GComponent a)
	{
		self.Awake(a);
		self.textName = a.GetText("name");
		self.textLevel = a.GetText("level");
		self.textType = a.GetText("type");
		self.textDes = a.GetText("des");
		self.isEquipedCtrl = a.GetController("isEquiped");
		self.isShowSellCtrl = a.GetController("isShowSell");
		self.baseAttr = new GTextField[3] {a.GetText("atk"),a.GetText("def"),a.GetText("hp") };
		var btnClose = a.GetCom("_bgfull");
		var btnEquip = a.GetButton("btnEquip");
		var btnUnEquip = a.GetButton("btnUnEquip");
		var btnSell = a.GetButton("btnSell");

		for (int i = 0; i < 6; i++)
		{
			self.extraAttr[i] = a.GetText($"attr{i}");
		}
		btnClose.onClick.Add(self.CloseSelf);
		btnEquip.onClick.Add(self.ClickEquip);
		btnUnEquip.onClick.Add(self.ClickUnEquip);
        btnSell.onClick.Add(self.ClickSell);

	}
}
public class UIEquipDetailChangeSystem : ChangeSystem<UIEquipDetail, IUIDataParam>
{
	protected override void Change(UIEquipDetail self, IUIDataParam a)
	{
		var p = (UIEquipDetailParam)a;
        self.info = p.info;
		self.Refresh();
	}
}
public static class UIEquipDetailSystem
{
	public static void Refresh(this UIEquipDetail self)
	{
        self.isEquipedCtrl.selectedIndex = self.info.IsEquiped ? 1 : 0;
        self.isShowSellCtrl.selectedIndex = self.info.IsEquiped ? 0 : 1;
        self.textName.text = self.info.name;
        var cfg = ConfigSystem.Ins.GetEquipConfig(self.info.modelId);
        self.textLevel.text = $"{self.info.level}";

        self.textType.text = RewardInfoFactory.GetTypeStr(RewardType.EQUIP, self.info.modelId);
        self.textDes.text = self.info.des;
		//base attr
		for(int i = 0; i < self.baseAttr.Length; i++)
		{
			int type = (int)AttributeType.Atk_Per + i;
			self.info.attri.TryGetValue(type, out long value);
			var name = AttributeTypeUtil.GetNameStr((AttributeType)(i+1));
			var vStr = AttributeTypeUtil.GetValueStr((AttributeType)type, value);
			self.baseAttr[i].text = $"{name} +{vStr}"; 
		}
		//special attr
		int index = 0;
		foreach (var v in self.info.attri)
		{
			var name = AttributeTypeUtil.GetNameStr((AttributeType)v.Key);
			var vStr = AttributeTypeUtil.GetValueStr((AttributeType)v.Key, v.Value);
			self.extraAttr[index].text = $"{name} +{vStr}";
			index++;
		}
		for(int i = index; i < self.extraAttr.Length; i++)
		{
			self.extraAttr[i].visible = false;
		}
	}
	public static void ClickEquip(this UIEquipDetail self)
	{
        var req = new WearEquipReq()
        {
            opType = 0,
            index = (int)self.info.uid
        };
        NetSystem.Send(req);
        self.CloseSelf();

    }
	public static void ClickUnEquip(this UIEquipDetail self)
	{
        var req = new WearEquipReq()
        {
            opType = 1,
            index = (int)self.info.uid
        };
        NetSystem.Send(req);
        self.CloseSelf();
    }
	
	public static void ClickSell(this UIEquipDetail self)
	{
		
	}
}