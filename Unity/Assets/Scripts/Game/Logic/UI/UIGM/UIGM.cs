using ET;
using GameEvent;
using FairyGUI;
using Frame;
using module.test.message;
using System.Collections.Generic;

public enum GmType
{
    None = 0,
    Item = 1,
    Equip = 2,
    
}
public class GMInfo
{
    public GmType type = GmType.None;
    public string cmd;
    public string name;
    public string content;
    public GMInfo(string content)
    {
        this.content = content;
        var arr = content.Split(' ');
        name = arr[0];
        cmd = arr[1];
        if (cmd == "getItem")
            type = GmType.Item;
    }
}
public class GmSecondInfo
{
    public GMInfo gmInfo;
    public string name;
    public int modelId;
}
class UIGM: UIBase
{
	public GTextInput input;
	public GList itemList;
    public GList itemSecondList;
    public GButton btnSend;
	public GButton btnClose;
    public GTextField textTip;
    public GComboBox comboBox;

    public GMInfo selectInfo;
	public List<GMInfo> gmList = new List<GMInfo>();
    public List<GmSecondInfo> gmSecondList = new List<GmSecondInfo>(1024);
    public List<GmSecondInfo> cacheList = new List<GmSecondInfo>();

}
class UIGMAwakeSystem : AwakeSystem<UIGM, GComponent>
{
    protected override void Awake(UIGM self, GComponent a)
	{
		self.Awake(a);
		self.input = a.GetCom("input").GetTextInput("text");
		self.btnSend = a.GetButton("btnSend");
		self.itemList = a.GetList("list");
        self.itemSecondList = a.GetList("secondList");
        self.textTip = a.GetText("tip");
		self.btnClose = a.GetButton("btnClose");
        self.comboBox = a.GetComboBox("comboBox");

        self.itemList.SetItemRender(self.ItemRender);
        self.itemSecondList.SetItemRender(self.ItemSecondRender);

        self.btnSend.onClick.Add(self.ClickSend);
		self.btnClose.onClick.Add(self.ClickClose);
        self.comboBox.onChanged.Add(self.ComboChange);
		self.SendGMList().Coroutine();
        self.comboBox.visible = false;
	}
}
static class UIGMSystem
{
    public static void ComboChange(this UIGM self)
    {
        self.InitSecondList(self.selectInfo);
        self.itemSecondList.ClearSelection();
    }

    public static void ClickSend(this UIGM self)
	{
		self.Send().Coroutine();
	}
	public static async ETTask Send(this UIGM self)
	{
		string str = self.input.text;
		var resp = (GmResp)await NetSystem.Call(new GmReq() { cmdStr = str }, typeof(GmResp));
	}
	public static void ClickClose(this UIGM self)
	{
		UISystem.Ins.Close(UIType.UIGM);
	}
	public static async ETTask SendGMList(this UIGM self)
	{
		var resp = (GmListResp) await NetSystem.Call(new GmListReq(), typeof(GmListResp));
		if (resp == null) return;
		self.gmList.Clear();
        foreach(var v in resp.cmds)
        {
            var info = new GMInfo(v);
            self.gmList.Add(info);
            if(info.type == GmType.Item)
            {
                info = new GMInfo(v)
                {
                    name = "获取装备",
                    type = GmType.Equip
                };
                self.gmList.Add(info);
            }
        }
		self.itemList.numItems = self.gmList.Count;
	}
    public static void InitComboBox(this UIGM self, GMInfo info)
    {
        self.comboBox.visible = info.type == GmType.Item || info.type == GmType.Equip;
        if (info.type == GmType.Item)
        {
            var cfg = ConfigSystem.Ins.GetItemConfig();
            Dictionary<int, string> typeList = new Dictionary<int, string>();
            foreach (var v in cfg)
            {
                var t = v.Value.Get("PropType");
                if (!typeList.ContainsKey(t))
                {
                    typeList.Add(t, v.Value.Get<string>("PropTypeDescribe"));
                }
            }
            string[] arr = new string[typeList.Count];
            foreach (var v in typeList)
            {
                arr[v.Key - 1] = v.Value;
            }
            self.comboBox.items = arr;
            self.comboBox.values = arr;
            self.comboBox.selectedIndex = 0;
        }
        else if (info.type == GmType.Equip)
        {
            var cfg = ConfigSystem.Ins.GetEquipConfig();
            Dictionary<int,string> typeList = new Dictionary<int,string>();
            foreach (var v in cfg)
            {
                var t = v.Value.Get("EquipType");
                if (!typeList.ContainsKey(t))
                {
                    typeList.Add(t,ConfigSystem.Ins.GetLanguage($"equip_type_{t}"));
                }
            }
            string[] arr = new string[typeList.Count];
            foreach(var v in typeList)
            {
                arr[v.Key-1] = v.Value;
            }
            self.comboBox.items = arr;
            self.comboBox.values = arr;
            self.comboBox.selectedIndex = 0;
        }
    }
	public static void ItemRender(this UIGM self,int index,GObject go)
	{
        var info = self.gmList[index];

        go.text = info.name;
		go.onClick.Add(() => {
			self.input.text = info.cmd;
            self.textTip.text = info.content;
            self.selectInfo = info;
            self.InitComboBox(info);
            self.InitSecondList(info);
		});
	}
    public static GmSecondInfo GetGmSecondInfo(this UIGM self,GMInfo gmInfo,int modelId,string name)
    {
        if (self.cacheList.Count > 0)
        {
            var info = self.cacheList[self.cacheList.Count - 1];
            info.gmInfo = gmInfo;
            info.modelId = modelId;
            info.name = name;
            self.cacheList.RemoveAt(self.cacheList.Count - 1);
            return info;
        }
        return new GmSecondInfo() { gmInfo = gmInfo, modelId = modelId, name = name};
    }
    public static void ClearGmSecondInfo(this UIGM self)
    {
        foreach(var v in self.gmSecondList)
        {
            self.cacheList.Add(v);
        }
        self.gmSecondList.Clear();
    }
    public static void InitSecondList(this UIGM self,GMInfo info)
    {
        if (info.type == GmType.None)
        {
            self.itemSecondList.numItems = 0;
            return;
        }
        self.ClearGmSecondInfo();
        if (info.type == GmType.Item)
        {
            
            var cfg = ConfigSystem.Ins.GetItemConfig();
            foreach (var v in cfg)
            {
                if(v.Value.Get("PropType") == self.comboBox.selectedIndex+1)
                {
                    var d = self.GetGmSecondInfo(info, v.Key, v.Value.Get<string>("Name"));
                    self.gmSecondList.Add(d);
                }
            }
        }
        if(info.type == GmType.Equip)
        {
            var cfg = ConfigSystem.Ins.GetEquipConfig();
            foreach (var v in cfg)
            {
                if (v.Value.Get("EquipType") == self.comboBox.selectedIndex+1)
                {
                    var d = self.GetGmSecondInfo(info, v.Key, v.Value.Get<string>("Name"));
                    self.gmSecondList.Add(d);
                }
                   
            }
        }
        self.itemSecondList.numItems = self.gmSecondList.Count;
    }
    public static void ItemSecondRender(this UIGM self, int index, GObject go)
    {
        var info = self.gmSecondList[index];
        go.text = info.name;
        go.data = info;
        go.onClick.Add(self.ClickSecondItem);
    }
    public static void ClickSecondItem(this UIGM self,EventContext e)
    {
        var info = (e.sender as GObject).data as GmSecondInfo;
        self.input.text = $"{info.gmInfo.cmd} {info.modelId} 1";
    }
}
