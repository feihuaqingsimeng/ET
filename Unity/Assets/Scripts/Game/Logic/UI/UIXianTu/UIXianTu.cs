using FairyGUI;
using ET;
using System;
using System.Collections.Generic;
using Frame;

public class UIXianTu : UIBase
{
    public GList itemList;

    public List<ItemData> datList = new List<ItemData>();

    public struct ItemData
    {
        public string path;
        public Type type;
        public ItemData(string path,Type type)
        {
            this.path = path;
            this.type = type;
        }
    }
}
class UIXianTuAwakeSystem : AwakeSystem<UIXianTu, GComponent>
{
    protected override void Awake(UIXianTu self, GComponent a)
	{
		self.Awake(a);
        self.itemList = a.GetList("list");

        self.itemList.itemProvider = self.ItemProvider;
        self.itemList.itemRenderer = self.ItemRender;

        self.datList.Add(new UIXianTu.ItemData("ui://UIXianTu/Com_Battle", typeof(XianTu_BattleItem)));
        self.datList.Add(new UIXianTu.ItemData("ui://UIXianTu/Com_YuWai", typeof(XianTu_YWItem)));
        self.datList.Add(new UIXianTu.ItemData("ui://UIXianTu/Com_Pet", typeof(XianTu_PetItem)));
        self.datList.Add(new UIXianTu.ItemData("ui://UIXianTu/Com_Task", typeof(XianTu_TaskItem)));

        self.itemList.numItems = self.datList.Count;
    }
}

public static class UIXianTuSystem
{
    public static string ItemProvider(this UIXianTu self,int index)
    {
        return self.datList[index].path;
    }
    public static void ItemRender(this UIXianTu self,int index,GObject go)
    {
        var com = go.asCom;
        var type = self.datList[index].type;
        self.AddChild(type, com);
    }
    
}