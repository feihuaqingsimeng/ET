using FairyGUI;
using ET;
using Frame;

public enum UIBottomButtonType
{
	None = -1,
	UIHome,
	UIBag,
	UIXianTu,
	UIFaBao,
	UIPet
}

public class UIBottomButton : UIBase
{
	public Controller stateCtrl;
	public GButton btnTraining;
	public GButton btnBag;
	public GButton btnXianTu;
	public GButton btnFaBao;
	public GButton btnPet;

	public UIBottomButtonType index;
	public string[] uiNames = new string[] { UIType.UIHome, UIType.UIEquipWear, UIType.UIXianTu, UIType.UIFaBaoWear, UIType.UIPetFormation };
}
class UIBottomButtonAwakeSystem : AwakeSystem<UIBottomButton, GComponent>
{
	protected override void Awake(UIBottomButton self, GComponent a)
	{
		self.Awake(a);
		self.stateCtrl = a.GetController("state");
		self.btnTraining = a.GetButton("btnXiuLian");
		self.btnBag = a.GetButton("btnBag");
		self.btnXianTu = a.GetButton("btnXianTu");
		self.btnFaBao = a.GetButton("btnFaBao");
		self.btnPet = a.GetButton("btnLingCong");

		self.stateCtrl.onChanged.Add(self.PageChange);

		self.index = UIBottomButtonType.None;
	}
}
public class UIBottomButtonChangeSystem : ChangeSystem<UIBottomButton, IUIDataParam>
{
	protected override void Change(UIBottomButton self, IUIDataParam a)
	{
		var param = (UIBottomButtonParam)a;
		self.stateCtrl.selectedIndex = (int)param.index;
		if(self.index == UIBottomButtonType.None)
		{
			self.PageChange();
		}
	}
}
public static class UIBottomButtonSystem
{
	public static void PageChange(this UIBottomButton self)
	{
		
		int index = self.stateCtrl.selectedIndex;
		if ((int)self.index == index) return;
		if (self.index != UIBottomButtonType.None)
		{
			UISystem.Ins.Close(self.uiNames[(int)self.index]);
		}
		self.index = (UIBottomButtonType)index;
		UISystem.Ins.Show(self.uiNames[index]);
	}
	
}