using FairyGUI;
using ET;
using GameEvent;
using System.Collections.Generic;
using UnityEngine;
using System;
using module.item.message;
using Frame;

public class UILianDan : UIBase
{
    public LianDanMatCom[] matArr = new LianDanMatCom[5];
    public Controller btnDanYao_CtrlEmpty;
    public Controller ctrlIsEmpty;
    public GButton btnDanYao;
    public GList itemList;
    public GTextField textTip;
    

    public MaterialInfo[] matInfoArr = new MaterialInfo[5];
    public int selectId;
    public List<ItemInfo> infoList = new List<ItemInfo>();
}
class UILianDanAwakeSystem : AwakeSystem<UILianDan, GComponent>
{
    protected override void Awake(UILianDan self, GComponent a)
	{
		self.Awake(a);
        for(int i = 0; i < 5; i++)
        {
            self.matArr[i] = self.AddChild<LianDanMatCom,GComponent>(a.GetCom($"Mat{i}"));
            self.matArr[i].AddClick(self.ClickMatItem);
            self.matInfoArr[i] = new MaterialInfo();
        }
        self.btnDanYao = a.GetButton("btnChoiceDan");
        self.btnDanYao_CtrlEmpty = self.btnDanYao.GetController("isEmpty");
        self.ctrlIsEmpty = a.GetController("isEmpty");
        self.textTip = a.GetText("textTip");
        self.itemList = a.GetList("list");
        var btnLianDan = a.GetButton("btnLianDan");
        var btnReset = a.GetButton("btnReset");
        var btnClose = a.GetButton("btnClose");
        var btnHandBook = a.GetButton("btnHandBook");

        self.btnDanYao.onClick.Add(self.ClickChoiceDanYao);
        btnLianDan.onClick.Add(self.ClickLianDan);
        btnReset.onClick.Add(self.ClickReset);
        btnClose.onClick.Add(self.CloseSelf);
        btnHandBook.onClick.Add(self.ClickHandBook);

        self.itemList.SetItemRender(self.ItemRender);

        self.Refresh();
    }
}

public static class UILianDanSystem
{
	public static void Refresh(this UILianDan self)
	{
        self.InitMatInfoArr();
        self.RefreshMat();
        self.RefreshDanYao();
        self.InitList();
    }
    public static void InitMatInfoArr(this UILianDan self)
    {
        if (self.selectId > 0)
        {
            var cfg = ConfigSystem.Ins.GetDrugsRefining(self.selectId);
            for (int i = 0; i < self.matInfoArr.Length; i++)
            {
                var info = self.matInfoArr[i];
                int.TryParse(cfg.Get<string>("DrugsQuantity"),out info.max);
            }
        }
        else
        {
            for (int i = 0; i < self.matInfoArr.Length; i++)
            {
                var info = self.matInfoArr[i];
                info.info = null;
            }
        }
            
    }
    public static void RefreshDanYao(this UILianDan self)
    {
        if (self.selectId==0)
        {
            self.btnDanYao_CtrlEmpty.selectedIndex = 1;
        }
        else
        {
            self.btnDanYao_CtrlEmpty.selectedIndex = 0;
            self.btnDanYao.title = ItemUtil.GetItemName(self.selectId);
        }
    }
    public static void RefreshMat(this UILianDan self)
    {
        for (int i = 0;i<self.matArr.Length;i++)
        {
            var v = self.matArr[i];
            v.Refresh(self.matInfoArr[i]);
        }
    }
    public static void InitList(this UILianDan self)
    {
        self.infoList.Clear();
        var dic = DataSystem.Ins.ItemModel.GetItems();
        var type = (int)ItemType.CaoYao;
        var quality = 0;
        if(self.selectId > 0)
        {
            var cfg = ConfigSystem.Ins.GetDrugsRefining(self.selectId);
            quality = cfg.Get("AvailableQuality");
        }
       
        foreach (var v in dic)
        {
            if (v.Value.type != type)
                continue;
            if ((self.selectId > 0 &&v.Value.quality >= quality) || self.selectId == 0)
            {
                self.infoList.Add(v.Value.Clone());
            }
        }
        self.infoList.Sort(Sort);
        self.itemList.numItems = self.infoList.Count;
        var isEmpty = self.infoList.Count == 0;
        self.ctrlIsEmpty.selectedIndex = isEmpty ? 1 : 0;
        
    }
    public static int Sort(ItemInfo a,ItemInfo b)
    {
        return (int)(a.uid - b.uid);
    }
    public static void RefreshListData(this UILianDan self)
    {
        self.itemList.numItems = self.infoList.Count;
        var isEmpty = self.infoList.Count == 0;
        self.ctrlIsEmpty.selectedIndex = isEmpty ? 1 : 0;
    }
    public static void ItemRender(this UILianDan self , int index,GObject go)
    {
        var com = go.asCom;
        var data = self.infoList[index];
        var insId = com.displayObject.gameObject.GetInstanceID();
        var item = self.GetChild<IconItemCom>(insId);
        if(item == null)
        {
            item = self.AddChildWithId<IconItemCom, GComponent>(insId, com);
            item.AddClick(self.ClickCaoYao);
        }
        item.Set( data, index);
    }
    public static void ClickMatItem(this UILianDan self,LianDanMatCom item)
    {
        self.ReturnMat(item.info);
        item.Empty();
        self.infoList.Sort(Sort);
        self.RefreshListData();
    }
    public static void ReturnMat(this UILianDan self, MaterialInfo info)
    {
        for (int i = 0; i < self.infoList.Count; i++)
        {
            var v = self.infoList[i];
            if (v == info.info)
            {
                v.num += info.count;
                info.info = null;
                return;
            }
        }

        info.info.num = info.count;
        self.infoList.Add(info.info);
        info.info = null;
    }
    public static void ClickCaoYao(this UILianDan self,IconItemCom item)
    {
        UISystem.Ins.Show(UIType.UIItemDetail, new UIItemDetailParam() {
            type = UIItemDetail.ItemDetailShowType.LianDan,
            info = item.info as ItemInfo,
            putCallback = self.PutCallback
        });
    }
    public static void PutCallback(this UILianDan self,ItemInfo info)
    {
        MaterialInfo matInfo = null;
        for(int i = 0; i < self.matInfoArr.Length; i++)
        {
            if(self.matInfoArr[i].info == null)
            {
                matInfo = self.matInfoArr[i];
                break;
            }
        }
        if(matInfo == null)
        {
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetErrorCode(44));//槽位已满
            return;
        }
        if(self.selectId == 0)
        {
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetErrorCode(45));//请先选择要炼制的丹药
            return;
        }
        int count = Mathf.Min(matInfo.max, info.num);
        matInfo.info = info;
        matInfo.count = count;
        self.RefreshMat();
        info.num -= count;
        if (info.num == 0)
            self.infoList.Remove(info);
        self.RefreshListData();
    }
    public static void ClickChoiceDanYao(this UILianDan self)
    {
        UISystem.Ins.Show(UIType.UIDanYaoFormula, new UIDanYaoFormulaParam() { callback = self.ChoiceDanYaoCallback});
    }

    public static void ChoiceDanYaoCallback(this UILianDan self,int itemId)
    {
        if (self.selectId == itemId) return;
        self.selectId = itemId;
        self.ClickReset();
        self.Refresh();
    }
    public static async void ClickLianDan(this UILianDan self)
    {
        if(self.selectId == 0)
        {
            UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetErrorCode(45));//请先选择要炼制的丹药
            return;
        }
        var req = new LianDanReq();
        req.drugsId = self.selectId;
        for (int i = 0; i < self.matInfoArr.Length; i++)
        {
            var info = self.matInfoArr[i];
            if (info.info != null)
            {
                if(info.count < info.max)
                {
                    UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("common_not_enough_tip1"));
                    return;
                }
                req.cailiao.Add((int)info.info.uid);
            }
            else
            {
                UISystem.Ins.ShowFlowTipView(ConfigSystem.Ins.GetLanguage("common_not_enough_tip1"));
                return;
            }
        }
        var token = self.GetToken(1);
        var resp = (LianDanResp)await NetSystem.Call(req, typeof(LianDanResp),token);
        if (resp == null) return;
        UISystem.Ins.ShowFlowTipView(string.Format(ConfigSystem.Ins.GetLanguage("gain_item_reward_tip"),ItemUtil.GetItemName(self.selectId),1));
        for (int i = 0; i < self.matInfoArr.Length; i++)
        {
            var info = self.matInfoArr[i];
            info.info = null;
        }
        self.Refresh();
    }
    public static void ClickReset(this UILianDan self)
    {
        bool flag = false;
        for(int i = 0; i < self.matInfoArr.Length; i++)
        {
            var info = self.matInfoArr[i];
            if(info.info != null)
            {
                flag = true;
                self.ReturnMat(info);
            }
        }
        if (flag)
        {
            self.RefreshMat();
            self.infoList.Sort(Sort);
            self.RefreshListData();
        }
    }
    public static async void ClickHandBook(this UILianDan self)
    {
        var token = self.GetToken(1);
        var resp = await NetSystem.Call(new DrugsTuJianReq(), typeof(DrugsTuJianResp),token);
        if (resp == null) return;
        UISystem.Ins.Show(UIType.UIDanYaoHandBook);
    }
}