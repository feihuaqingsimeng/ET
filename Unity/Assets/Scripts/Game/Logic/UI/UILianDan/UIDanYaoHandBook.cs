using FairyGUI;
using ET;
using GameEvent;
using System.Collections.Generic;
using Frame;

public class UIDanYaoHandBook : UIBase
{
    public GList baseItemList;
    public GList specialItemList;
    public GList pageItemList;
    public GList effectItemList;

    public List<AttributeType> baseList = new List<AttributeType>();
    public List<AttributeType> specialList = new List<AttributeType>();
    public List<int> pageList = new List<int>();
    public Dictionary<int, List<DrugsUseInfo>> effectList = new Dictionary<int, List<DrugsUseInfo>>();
    public int selectPage;

}
class UIDanYaoHandBookAwakeSystem : AwakeSystem<UIDanYaoHandBook, GComponent>
{
	protected override void Awake(UIDanYaoHandBook self, GComponent a)
	{
		self.Awake(a);
        self.baseItemList = a.GetList("baseList");
        self.specialItemList = a.GetList("specialList");
        self.pageItemList = a.GetList("pageList");
        self.effectItemList = a.GetList("effectList");
        var btnClose = a.GetButton("btnClose");
        btnClose.onClick.Add(self.CloseSelf);

        self.baseItemList.SetItemRender(self.BaseItemRender, false);
        self.specialItemList.SetItemRender(self.SpecialItemRender, false);
        self.pageItemList.SetItemRender(self.PageItemRender);
        self.effectItemList.SetItemRender(self.EffectItemRender);

        self.Refresh();
    }
}

public static class UIDanYaoHandBookSystem
{
	public static void Refresh(this UIDanYaoHandBook self)
	{
        var sys = DataSystem.Ins;
        self.baseList.Sort();
        self.specialList.Sort();
        AttributeTypeUtil.GetBaseList(self.baseList);
        AttributeTypeUtil.GetSpecialList(self.specialList);

        foreach(var v in sys.HandBookModel.drugsRecords)
        {
            var cfg = ConfigSystem.Ins.GetDrugsRefining(v.itemId);
            if(cfg != null)
            {
                var lv = cfg.Get("DrugsLv");
                if(!self.effectList.TryGetValue(lv,out var list))
                {
                    list = new List<DrugsUseInfo>();
                    self.effectList.Add(lv, list);
                    self.pageList.Add(lv);
                }
                list.Add(v);
            }
        }
        self.pageList.Sort();

        if (self.pageList.Count > 0)
            self.selectPage = self.pageList[0];

        self.baseItemList.numItems = self.baseList.Count;
        self.specialItemList.numItems = self.specialList.Count;
        self.pageItemList.numItems = self.pageList.Count;
        if (self.selectPage > 0)
        {
            self.effectItemList.numItems = self.effectList[self.selectPage].Count;
        }
        self.pageItemList.selectedIndex = 0;
    }
    
    public static void BaseItemRender(this UIDanYaoHandBook self,int index,GObject go)
    {
        self.UpdateAttr(self.baseList[index], go.asCom);
    }
    public static void SpecialItemRender(this UIDanYaoHandBook self, int index, GObject go)
    {
        self.UpdateAttr(self.specialList[index], go.asCom);
    }
    private static void UpdateAttr(this UIDanYaoHandBook self, AttributeType type, GComponent go)
    {
        var data = DataSystem.Ins.HandBookModel;
        long value = data.GetAttr(type);
        go.GetText("name").text = AttributeTypeUtil.GetNameStr(type);
        go.GetText("value").text = AttributeTypeUtil.GetValueStr(type, value);
    }
    public static void PageItemRender(this UIDanYaoHandBook self, int index, GObject go)
    {
        int page = self.pageList[index];
        go.asButton.title = ConfigSystem.Ins.GetLanguage($"drugs_class_{page}");
    }
    public static void EffectItemRender(this UIDanYaoHandBook self, int index, GObject go)
    {
        var dataList = self.effectList[self.selectPage];
        var data = dataList[index];
        var com = go.asCom;
        var insId = com.displayObject.gameObject.GetInstanceID();
        var item = self.GetChild<HandBookEffectCom>(insId);
        if(item == null)
        {
            item = self.AddChildWithId<HandBookEffectCom, GComponent>(insId, com);
        }
        item.Refresh(data);
    }
}