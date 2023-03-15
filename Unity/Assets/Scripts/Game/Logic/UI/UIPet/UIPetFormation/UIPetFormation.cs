using FairyGUI;
using ET;
using GameEvent;
using System.Collections.Generic;
using UnityEngine;
using module.pet.message;
using Frame;

public class UIPetFormation : UIBase
{
    public GTextField textNum;
    public GList itemList;
    public GComboBox sortBox;

    public List<PetInfo> petInfoList = new List<PetInfo>();
    public GComponent dragItemParent;
    public long showPetUid;
    public int sortType;
}
class UIPetFormationAwakeSystem : AwakeSystem<UIPetFormation, GComponent>
{
    protected override void Awake(UIPetFormation self, GComponent a)
	{
		self.Awake(a);
        self.textNum = a.GetCom("num").GetText("title");
        self.itemList = a.GetList("list");
        self.sortBox = a.GetComboBox("sortBox");
        for (int i = 1; i <= ConstValue.PET_BATTLE_MAX; i++)
        {
            var slotItem = self.AddChildWithId<PetFormationSlotCom, GComponent>(i,a.GetCom($"slot{i-1}"));
            slotItem.SetTouchCallback(self.TouchMove, self.TouchEnd);
        }
        self.itemList.itemRenderer = self.ItemRender;

        self.sortBox.onChanged.Add(self.SortChanged);

        self.RegisterEvent<PetFormationSucEvent>(self.Event_PetFormationSucEvent);
        self.RegisterEvent<PetEvolutionSucEvent>(self.Event_PetEvolutionSucEvent);

        self.sortType = 0;
        self.Refresh();


    }
}

public static class UIPetFormationSystem
{
	public static void Refresh(this UIPetFormation self)
	{
        self.RefreshList();
        self.RefreshSlot();

    }
    public static void RefreshSlot(this UIPetFormation self)
    {
        var param = ConfigSystem.Ins.GetGlobal<int[]>("LSUnlockLv");
        for (int i = 1; i <= ConstValue.PET_BATTLE_MAX; i++)
        {
            var level = param[i-1];
            
            var item = self.GetChild< PetFormationSlotCom>(i);
            if (DataSystem.Ins.PlayerModel.level < level)
            {
                item.Lock(level);
            }
            else
            {
                var pet = DataSystem.Ins.PetModel.GetBattlePet(i);
                if (pet == null)
                    item.Empty();
                else
                    item.Set(pet);
            }
        }
    }
    public static void RefreshList(this UIPetFormation self)
    {
        self.petInfoList.Clear();
        foreach (var v in DataSystem.Ins.PetModel.pets)
        {
            self.petInfoList.Add(v.Value);
        }
        self.DoRefreshList();
    }
    public static void DoRefreshList(this UIPetFormation self)
    {
        self.petInfoList.Sort(self.Sort);
        self.itemList.numItems = self.petInfoList.Count;
        self.textNum.text = string.Format(ConfigSystem.Ins.GetLanguage("common_num_tip2"),self.petInfoList.Count);
    }
    public static int Sort(this UIPetFormation self,PetInfo a,PetInfo b)
    {
        if(self.sortType == 0)//境界
        {
            if (a.level != b.level)
                return b.level - a.level;
            return b.quality - a.quality;
        }
        else
        {
            if (a.quality != b.quality)
                return b.quality - a.quality;
            return b.level - a.level;
        }

    }
    public static void ItemRender(this UIPetFormation self,int index,GObject go)
    {
        var data = self.petInfoList[index];
        var com = go.asCom;
        var insId = com.displayObject.gameObject.GetInstanceID();
        var child = self.GetChild<PetItemCom>(insId);
        if (child == null)
            child = self.AddChildWithId<PetItemCom, GComponent>(insId, com);
        child.Set(data);
        child.SetInBattle(data.battleIndex > 0);
        child.AddClick(self.ClickPetItem);
    }
    public static void SortChanged(this UIPetFormation self)
    {
        self.sortType = self.sortBox.selectedIndex;
        self.DoRefreshList();
    }
    public static void ClickPetItem(this UIPetFormation self,PetItemCom item)
    {
        self.showPetUid = item.pet.uid;
        UISystem.Ins.Show(UIType.UIPetDetail, new UIPetDetailParam() { pet = item.pet });
    }
    public static void TouchMove(this UIPetFormation self, PetFormationSlotCom item, EventContext e)
    {
        if (self.dragItemParent == null)
        {
            self.dragItemParent = item.item.gCom.parent;
            self.gCom.AddChild(item.item.gCom);
        }
        item.item.InvalidateBatchingState();
        var pos = new Vector2(e.inputEvent.x, e.inputEvent.y);
        pos /= UIContentScaler.scaleFactor;
        item.item.SetPos(pos.x - item.item.gCom.width / 2, pos.y - item.item.gCom.height / 2);
    }
    public static void TouchEnd(this UIPetFormation self, PetFormationSlotCom item, EventContext e)
    {
        if (self.dragItemParent == null) return;
        self.dragItemParent.AddChild(item.item.gCom);
        self.dragItemParent = null;
        item.item.SetPos(0, 0);
        var pos = new Vector2(e.inputEvent.x, e.inputEvent.y);
        pos /= UIContentScaler.scaleFactor;
        float x = pos.x;
        float y = pos.y;
        for (int i = 1; i <= ConstValue.PET_BATTLE_MAX; i++)
        {
            var child = self.GetChild<PetFormationSlotCom>(i);
            var com = child.gCom;
            if (x >= com.x && x <= com.x + com.width && y >= com.y && y <= com.y + com.height)
            {
                if (child != item)
                {
                    var req = new PetChuZhanReq() { petUuid = item.pet.uid, zhanIndex = i };
                    NetSystem.Send(req);
                    var info = item.pet;
                    item.Set(child.pet);
                    child.Set(info);
                    return;
                }
                else
                {
                    return;
                }
            }
        }
        NetSystem.Send(new PetChuZhanReq() { petUuid = item.pet.uid, zhanIndex = 0 });
        item.Empty();
        
    }

    public static void Event_PetFormationSucEvent(this UIPetFormation self, PetFormationSucEvent e)
    {
        self.Refresh();
    }
    public static void Event_PetEvolutionSucEvent(this UIPetFormation self, PetEvolutionSucEvent e)
    {
        self.Refresh();
    }
}