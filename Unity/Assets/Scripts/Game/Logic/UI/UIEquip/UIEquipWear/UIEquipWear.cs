using FairyGUI;
using ET;
using GameEvent;
using Frame;
using System.Collections.Generic;
using module.equip.message;

public class UIEquipWear : UIBase
{
    public GList itemList;

    public ManagedEntity slotRoot;
    public ManagedEntity equipRoot;

    public List<EquipInfo> equipList = new List<EquipInfo>();
    public bool isClickSort;
}
class UIEquipWearAwakeSystem : AwakeSystem<UIEquipWear, GComponent>
{
    protected override void Awake(UIEquipWear self, GComponent a)
	{
		self.Awake(a);
        self.itemList = a.GetList("list");
        self.slotRoot = self.AddChildWithId<ManagedEntity>(1);
        for (int i = 1; i <= 8; i++)
        {
            self.slotRoot.AddChildWithId<EquipWearSlotCom, GComponent>(i,a.GetCom($"slot{i-1}"));
        }
        self.equipRoot = self.AddChildWithId<ManagedEntity>(2);
        var btnOpen = a.GetButton("btnOpen");
        var btnSort = a.GetButton("btnSort");
        btnOpen.onClick.Add(self.ClickOpen);
        btnSort.onClick.Add(self.ClickSort);

        self.itemList.SetItemRender(self.EquipItemRender);
        self.RegisterEvent<EquipChangeEvent>(self.Event_EquipChangeEvent);
        self.Refresh();
    }
}
class UIEquipWearDestroySystem : DestroySystem<UIEquipWear>
{
    protected override void Destroy(UIEquipWear self)
    {
        UISystem.Ins.Close(UIType.UIBag);
    }
}
public static class UIEquipWearSystem
{
	public static void Refresh(this UIEquipWear self)
	{
        self.InitEquipedSlot();
        self.InitEquipList();

    }
    public static void InitEquipedSlot(this UIEquipWear self)
    {
        for(int i = 1; i <= 8; i++)
        {
            self.RefreshEquipedSlot((EquipType)i);
        }
    }
    public static void RefreshEquipedSlot(this UIEquipWear self,EquipType type)
    {
        var child = self.slotRoot.GetChild<EquipWearSlotCom>((int)type);
        var info = DataSystem.Ins.EquipModel.GetEquiped(type);
        if (info == null)
            child.Empty(type);
        else
            child.Set(info);
    }
    public static void InitEquipList(this UIEquipWear self)
    {
        self.equipList.Clear();
        var model = DataSystem.Ins.EquipModel;
        model.GetEquips(EquipPackType.EQUIP, self.equipList);
        model.GetEquips(EquipPackType.PACK, self.equipList);
        self.equipList.Sort(model.Sort);
        self.itemList.numItems = model.size_pack;
    }
    public static void EquipItemRender(this UIEquipWear self,int index,GObject go)
    {
        var com = self.GetChildWithGComponent<IconEquipCom>(go.asCom,true, self.equipRoot);
        if(index < self.equipList.Count)
        {
            com.Set(self.equipList[index]);
        }
        else
        {
            com.SetEmpty();
        }
    }
    public static void ClickOpen(this UIEquipWear self)
    {
        UISystem.Ins.Show(UIType.UIBag);
    }
    public static async void ClickSort(this UIEquipWear self)
    {
        if (self.isClickSort) return;
        self.isClickSort = true;
        NetSystem.Send(new ClassifyEquipReq() { packType = module.equip.model.EquipPackType.Equip_PACK_TYPE });
        await self.WaitAsync(1000);
        self.isClickSort = false;
    }
    public static void Event_EquipChangeEvent(this UIEquipWear self, EquipChangeEvent e)
    {
        self.InitEquipList();
        self.InitEquipedSlot();
    }
}