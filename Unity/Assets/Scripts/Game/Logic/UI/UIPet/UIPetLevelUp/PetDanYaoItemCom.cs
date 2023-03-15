using ET;
using FairyGUI;
using module.item.message;
using GameEvent;
using Frame;

public class PetDanYaoItemCom : UIBase
{
    public GTextField textName;
    public GTextField textDes;
    public GTextField textNum;
    public Controller ctrlSelect;

    public int itemId;
    public long count;
    public int useCount;
    public bool isTrigger;
    public PetInfo pet;
}
class PetDanYaoItemComAwakeSystem : AwakeSystem<PetDanYaoItemCom, GComponent>
{
    protected override void Awake(PetDanYaoItemCom self, GComponent a)
    {
        self.Awake(a);
        self.textName = a.GetText("name");
        self.textDes = a.GetText("des");
        self.textNum = a.GetText("num");

        a.onTouchBegin.Add(self.TouchBegin);
        a.onTouchEnd.Add(self.TouchEnd);
        a.onTouchMove.Add(self.TouchMove);

        self.RegisterEvent<ItemOneChangeEvent>(self.Event_ItemOneChangeEvent);
    }
}

public static class PetDanYaoItemComSystem
{
    public static void Refresh(this PetDanYaoItemCom self, int itemId, PetInfo pet)
    {
        self.pet = pet;
        self.itemId = itemId;
        self.textName.text = ItemUtil.GetItemName(itemId);
        self.textDes.text = ItemUtil.GetItemDes(itemId);
        self.RefreshCount();
    }
    public static void RefreshCount(this PetDanYaoItemCom self)
    {
        self.count = DataSystem.Ins.ItemModel.GetItemCount(self.itemId);
        self.textNum.text = $"*{self.count}";
    }
    public static async void TouchBegin(this PetDanYaoItemCom self)
    {
        if (self.count == 0) return;
        self.useCount = 1;
        var flag = await TimerComponent.Instance.WaitAsync(700, self.GetToken(1));
        if (!flag) return;
        self.isTrigger = true;
        while (true)
        {
            if (self.pet.level >= DataSystem.Ins.PlayerModel.level)
            {
                UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("uipet_levelup_master_limit"));
                return;
            }
            self.useCount++;
            self.textNum.text = $"*{self.count-self.useCount}";
            self.SendDanYao(1);
            if (self.count == self.useCount) return;
            flag = await TimerComponent.Instance.WaitAsync(200, self.GetToken(1));
            if (!flag) return;
        }
    }
    public static void TouchMove(this PetDanYaoItemCom self)
    {
        self.CancelToken(1);
        self.useCount = 0;
    }
    public static void TouchEnd(this PetDanYaoItemCom self)
    {
        if (self.count == 0 || self.useCount == 0) return;
        self.CancelToken(1);
        if (self.isTrigger)
        {
            self.isTrigger = false;
            return;
        }
        if (self.pet.level >= DataSystem.Ins.PlayerModel.level)
        {
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("uipet_levelup_master_limit"));
            return;
        }
        self.SendDanYao(self.useCount);
    }
    public static void SendDanYao(this PetDanYaoItemCom self,int num)
    {
        var info = DataSystem.Ins.ItemModel.GetItem(self.itemId);
        var req = new ItemUseReq() {
            itemIndex = (int)info.uid,
            petUuid = self.pet.uid,
            useNum = num
        };
        NetSystem.Send(req);
    }
    public static void Event_ItemOneChangeEvent(this PetDanYaoItemCom self, ItemOneChangeEvent e)
    {
        var info = DataSystem.Ins.ItemModel.GetItemBySlot(e.index, e.type);
        if(info == null)
        {
            self.RefreshCount();
            return;
        }
        if (info.modelId != self.itemId) return;
        self.RefreshCount();
    }
}
