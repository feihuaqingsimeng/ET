using FairyGUI;
using ET;
using GameEvent;
using module.item.message;
using Frame;

public class SkillScrollItemCom : UIBase
{
    public GTextField textName;
    public GTextField textType;
    public GTextField textNum;
    public GTextField textDes;
    public GButton btnLearn;

    public ItemInfo info;
    public PetInfo pet;
}
class SkillScrollItemComAwakeSystem : AwakeSystem<SkillScrollItemCom, GComponent>
{
    protected override void Awake(SkillScrollItemCom self, GComponent a)
	{
		self.Awake(a);
        self.textName = a.GetText("name");
        self.textType = a.GetText("type");
        self.textNum = a.GetText("num");
        self.textDes = a.GetText("des");
        self.btnLearn = a.GetButton("btnLearn");

        self.btnLearn.onClick.Add(self.ClickLearn);

        self.RegisterEvent<ItemOneChangeEvent>(self.Event_ItemOneChangeEvent);
    }
}

public static class SkillScrollItemComSystem
{
    public static void Refresh(this SkillScrollItemCom self, PetInfo pet, ItemInfo info)
	{
        self.info = info;
        self.pet = pet;
        var cfg = ConfigSystem.Ins.GetItemConfig(info.modelId);
        self.textName.text = info.name;
        self.textType.text = cfg.Get<string>("PropTypeDescribe"); 
        self.textDes.text = info.des;
        self.RefreshCount();
    }
    public static void RefreshCount(this SkillScrollItemCom self)
    {
        self.textNum.text = $"x{self.info.num}";
    }
    public static void ClickLearn(this SkillScrollItemCom self)
    {
        var req = new ItemUseReq()
        {
            itemIndex = (int)self.info.uid,
            petUuid = self.pet.uid,
            useNum = 1,
        };
        NetSystem.Send(req);

    }
    public static void Event_ItemOneChangeEvent(this SkillScrollItemCom self, ItemOneChangeEvent e)
    {
        var info = DataSystem.Ins.ItemModel.GetItemBySlot(e.index);
        if(info == null)
        {
            self.GetParent<UIPetLearnSkill>().RefreshScroll();
            return;
        }
        if (self.info.uid != info.uid) return;
        self.RefreshCount();
    }
}