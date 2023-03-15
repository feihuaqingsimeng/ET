using FairyGUI;
using ET;
using System.Collections.Generic;
using System;
using Frame;

public class ItemData
{
    public int type;
    public int id;
    public ItemData(int type,int id)
    {
        this.type = type;
        this.id = id;
    }
}
public class UIDanYaoFormula : UIBase
{
    public GList itemList;

    public List<ItemData> dataList = new List<ItemData>();
    public GTreeNode expandedNode;
    public Action<int> callback;
}
class UIDanYaoFormulaAwakeSystem : AwakeSystem<UIDanYaoFormula, GComponent>
{
    protected override void Awake(UIDanYaoFormula self, GComponent a)
	{
		self.Awake(a);
        self.itemList = a.GetList("list");
        
        var btnClose = a.GetButton("btnClose");
        var btnBg = a.GetChild("_bgfull");

        btnClose.onClick.Add(self.CloseSelf);
        btnBg.onClick.Add(self.CloseSelf);

        self.itemList.SetItemRender(self.ItemRender);
        self.itemList.itemProvider = self.ItemProvider;
        self.Refresh();
	}
}
class UIDanYaoFormulaChangeSystem : ChangeSystem<UIDanYaoFormula, IUIDataParam>
{
    protected override void Change(UIDanYaoFormula self, IUIDataParam a)
    {
        var param = (UIDanYaoFormulaParam)a;
        self.callback = param.callback;
    }
}
public static class UIDanYaoFormulaSystem
{
	public static void Refresh(this UIDanYaoFormula self)
	{
        var idDic = new SortedDictionary<int, List<int>>();
        var dic = ConfigSystem.Ins.GetDrugsRefining();
        foreach(var v in dic)
        {
            int level = v.Value.Get("DrugsLv");
            if(level > 0)
            {
                if (!idDic.TryGetValue(level, out var list))
                {
                    list = new List<int>();
                    idDic.Add(level, list);
                }
                list.Add(v.Key);
            }
        }
        foreach(var v in idDic)
        {
            self.dataList.Add(new ItemData(0, v.Key));
            foreach(var v2 in v.Value)
            {
                self.dataList.Add(new ItemData(1, v2));
            }
        }
        self.itemList.numItems = self.dataList.Count;
    }
    
    public static string ItemProvider(this UIDanYaoFormula self,int index)
    {
        var data = self.dataList[index];
        if (data.type == 0)
            return "ui://UILianDan/Com_Folder";
        else
            return "ui://UILianDan/Com_ItemIcon";
    }
    public static void ItemRender(this UIDanYaoFormula self,int index,GObject go)
    {
        var data = self.dataList[index];
        var com = go.asCom;
        if(data.type == 0)
        {
            com.GetText("title").text = ConfigSystem.Ins.GetLanguage($"drugs_class_{data.id}");
        }
        else
        {
            var itemId = data.id;
            com.GetText("title").text = ItemUtil.GetItemName(itemId);
            com.GetText("num").text = ItemUtil.GetItemDes(itemId);

            com.GetController("isLock").selectedIndex = DataSystem.Ins.HandBookModel.IsUnlock(itemId) ? 0 : 1;
            com.onClick.Clear();
            com.onClick.Add(() =>
            {
                if (!DataSystem.Ins.HandBookModel.IsUnlock(itemId))
                {
                    return;
                }
                self.callback?.Invoke(itemId);
                self.CloseSelf();
            });
        }
    }
}