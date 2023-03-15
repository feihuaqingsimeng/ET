using ET;
using GameEvent;
using FairyGUI;
using Frame;

public class ResourceItemComponent: UIBase
{
	public GTextField textTitle;
	public GTextField textNum;
	public GButton add;

	public ResourceType resType;
}
[ObjectSystem]
public class ResourceItemComponentAwakeSystem : AwakeSystem<ResourceItemComponent, GComponent>
{
	protected override void Awake(ResourceItemComponent self, GComponent a)
	{
		self.Awake(a);
		self.textTitle = a.GetText("title");
		self.textNum = a.GetText("num");
		self.add = a.GetButton("add");

		self.gCom.onClick.Add(self.ClickAdd);

        self.RegisterEvent<ResourceChangedEvent>(self.Event_ResourceChanged);
	}
}

static class ResourceItemComponentSystem
{
	public static void Init(this ResourceItemComponent self, ResourceType type, bool isShowAdd = true)
	{
        AnalysisSystem.Ins.StartRecordTime("ResourceItemComponent Init1");
		self.resType = type;
		long resNum = DataSystem.Ins.ItemModel.GetResource(type);
        AnalysisSystem.Ins.EndRecordTime("ResourceItemComponent Init1");
        AnalysisSystem.Ins.StartRecordTime("ResourceItemComponent Init2");
        self.textTitle.text = ItemUtil.GetItemName((int)type);
		self.textNum.text = resNum.ToString();
		self.add.visible = isShowAdd;
        AnalysisSystem.Ins.EndRecordTime("ResourceItemComponent Init2");

    }
    public static void ClickAdd(this ResourceItemComponent self)
	{
		UISystem.Ins.Show(UIType.UIGM);
	}
	public static void Event_ResourceChanged(this ResourceItemComponent self, ResourceChangedEvent e)
	{
		if (e.resType != self.resType) return;
		self.textNum.text = e.resValue.ToString();
	}
}