using ET;
using SFaBaoInfo = module.fabao.message.vo.FaBaoInfo;
using SItemInfo = module.item.message.ItemInfo;
using System.Collections.Generic;

public class ItemModel:Entity,IAwake
{
    public Dictionary<int, long> resources = new Dictionary<int, long>();

    //背包大小
    public int size_pack;
    //仓库大小
    public int size_store;
    public Dictionary<int, string> storeNames = new Dictionary<int, string>();
    public MultiDictionary<PackType, int, ItemInfo> items = new MultiDictionary<PackType, int, ItemInfo>();
    public MultiDictionary<FaBaoPackType, int, FaBaoInfo> fabaos = new MultiDictionary<FaBaoPackType, int, FaBaoInfo>();
}
public static class ItemModelSystem
{
    public static void SetResourceChange(this ItemModel self, Dictionary<int, long> resource)
    {
        if (resource == null) return;
        foreach (var a in resource)
        {
            self.resources[a.Key] = a.Value;
        }
    }
    public static long GetResource(this ItemModel self, ResourceType type)
    {
        self.resources.TryGetValue((int)type, out long value);
        return value;
    }
    public static bool IsResource(this ItemModel self,int itemId)
    {
        var t = (ResourceType)itemId;
        if ((t >= ResourceType.MAX))
            return false;
        return true;
    }
    public static void AddItemList(this ItemModel self, PackType type, List<SItemInfo> changes)
    {
        if (changes == null || changes.Count == 0) return;
        for (int i = 0; i < changes.Count; i++)
        {
            var info = changes[i];
            if (info.itemId == 0 || info.amount == 0)
            {
                self.items.Remove(type, info.index);
            }
            else
            {
                if (!self.items.TryGetValue(type, info.index, out ItemInfo own))
                {
                    own = new ItemInfo();
                    self.items.Add(type, info.index, own);
                }
                own.UpdateByDB(info,type);
            }

        }

    }
    public static long GetItemCount(this ItemModel self, int id, PackType type = PackType.PACK)
    {
        if (self.IsResource(id))
            return self.GetResource((ResourceType)id);
        var dic = self.GetItems(type);
        long count = 0;
        if (dic == null) return 0;
        foreach (var v in dic)
        {
            if (v.Value.modelId == id)
                count += v.Value.num;
        }
        return count;
    }
    public static Dictionary<int, ItemInfo> GetItems(this ItemModel self, PackType type = PackType.PACK)
    {
        self.items.TryGetDic(type, out var value);
        return value;
    }
    public static ItemInfo GetItem(this ItemModel self, int id, PackType type = PackType.PACK)
    {
        var items = self.GetItems(type);
        foreach (var v in items)
        {
            if (v.Value.modelId == id)
                return v.Value;
        }
        return null;
    }

    public static ItemInfo GetItemBySlot(this ItemModel self, int slot, PackType type = PackType.PACK)
    {
        self.items.TryGetValue(type, slot, out var v);
        return v;
    }
    public static List<ItemInfo> GetItemsByType(this ItemModel self, int type, List<ItemInfo> outList, PackType pType = PackType.PACK)
    {
        var items = self.GetItems(pType);
        if (items == null) return outList;
        if (outList == null) outList = new List<ItemInfo>();
        foreach(var v in items)
        {
            if(v.Value.type == type)
            {
                outList.Add(v.Value);
            }
        }
        return outList;
    }
    public static string GetStoreName(this ItemModel self, int page)
    {
        //load local
        if (self.storeNames.Count == 0)
        {
            var param = UnityEngine.PlayerPrefs.GetString("STORE_NAME", "");
            if (string.IsNullOrEmpty(param)) return "";
            self.storeNames = LitJson.JsonMapper.ToObject<Dictionary<int, string>>(param);
        }
        if (self.storeNames.TryGetValue(page, out string name))
        {
            return name;
        }
        return "";
    }
    public static void SetStoreName(this ItemModel self, int page, string name)
    {
        self.storeNames[page] = name;
        string param = LitJson.JsonMapper.ToJson(self.storeNames);
        UnityEngine.PlayerPrefs.SetString("STORE_NAME", param);
    }

    public static void AddFaBaoList(this ItemModel self, FaBaoPackType type, List<SFaBaoInfo> changes)
    {
        if (changes == null || changes.Count == 0) return;
        for (int i = 0; i < changes.Count; i++)
        {
            var info = changes[i];
            if (info.faBaoId == 0)
            {
                self.fabaos.Remove(type, info.index);
            }
            else
            {
                if (!self.fabaos.TryGetValue(type, info.index, out FaBaoInfo own))
                {
                    own = new FaBaoInfo();
                    self.fabaos.Add(type, info.index, own);
                }
                own.UpdateByDB(info, type);
            }

        }
    }
    public static Dictionary<int, FaBaoInfo> GetFabaoList(this ItemModel self, FaBaoPackType type = FaBaoPackType.PACK)
    {
        self.fabaos.TryGetDic(type, out var value);
        return value;
    }
    public static FaBaoInfo GetFabao(this ItemModel self, int index, FaBaoPackType type = FaBaoPackType.PACK)
    {
        self.fabaos.TryGetValue(type, index, out var v);
        return v;
    }
}
