using FairyGUI;
using ET;
using GameEvent;
using System.Collections.Generic;
using Frame;

class UIAttrDetail : UIBase
{
	
	public GTextField textName;
	public GTextField textPlayerId;
	public GList attrItemList;
	public GList specialAttrItemList;
	public GButton btnRename;
	public GButton btnClose;

	public List<AttributeType> baseAttList = new List<AttributeType>(16);
	public List<AttributeType> specialAttList = new List<AttributeType>();

}
class UIAttrDetailAwakeSystem : AwakeSystem<UIAttrDetail, GComponent>
{
	protected override void Awake(UIAttrDetail self, GComponent a)
	{
		self.Awake(a);
		self.btnRename = a.GetButton("btnRename");
		self.textName = a.GetText("textName");
		self.textPlayerId = a.GetText("textID");
		self.attrItemList = a.GetList("attrList");
		self.specialAttrItemList = a.GetList("specialAttrList");
		self.btnClose = a.GetButton("btnClose");



		self.attrItemList.itemRenderer = self.BaseAttItemRender;
		self.specialAttrItemList.itemRenderer = self.SpecialItemRender;

		self.btnRename.onClick.Add(self.ClickRename);
		self.btnClose.onClick.Add(self.ClickClose);
		AttributeTypeUtil.GetBaseList(self.baseAttList);
		AttributeTypeUtil.GetSpecialList(self.specialAttList);

		self.Refresh();

	}
}
static class UIAttrDetailSystem
{
	public static void Refresh(this UIAttrDetail self)
	{
		var data = DataSystem.Ins.PlayerModel;
		self.textName.text = data.name;
		self.textPlayerId.text = data.playerId.ToString();


		self.InitAttr();
	}
	public static void InitAttr(this UIAttrDetail self)
	{
		self.attrItemList.numItems = self.baseAttList.Count;
		self.specialAttrItemList.numItems = self.specialAttList.Count;
	}
	public static void BaseAttItemRender(this UIAttrDetail self, int index, GObject item)
	{
		self.UpdateAttr(self.baseAttList[index], item.asCom);
	}
	public static void SpecialItemRender(this UIAttrDetail self, int index, GObject item)
	{
		self.UpdateAttr(self.specialAttList[index], item.asCom);
	}
	private static void UpdateAttr(this UIAttrDetail self, AttributeType type, GComponent go)
	{
		var data = DataSystem.Ins.PlayerModel;
		long value = data.GetAttr(type);
		go.GetText("name").text = AttributeTypeUtil.GetNameStr(type);
		go.GetText("value").text = AttributeTypeUtil.GetValueStr(type, value);
	}
	public static void ClickRename(this UIAttrDetail self)
	{
		UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("undeveloped_tip") });
	}
	public static void ClickClose(this UIAttrDetail self)
	{
		UISystem.Ins.Close(UIType.UIAttrDetail);
	}
}
