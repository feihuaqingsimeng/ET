using ET;
using GameEvent;
using Frame;
using module.pack.message;
using module.pack.model;

public class ItemPackageHandler : AMHandler<PackageResp>
{
	protected override async ETTask Run(Session session, PackageResp message)
	{
		var data = DataSystem.Ins.ItemModel;
		data.size_pack = message.size_pack;
		data.size_store = message.size_store;
		data.AddItemList(PackType.PACK, message.items_pack);
		data.AddItemList(PackType.STORE, message.items_store);
		Frame.UIEventSystem.Ins.Publish(new ItemChangeEvent());
		await ETTask.CompletedTask;
	}
}
public class ItemChangeHandler : AMHandler<ItemChangeResp>
{
	protected override async ETTask Run(Session session, ItemChangeResp message)
	{
		var data = DataSystem.Ins.ItemModel;
		data.AddItemList((PackType)message.packType, message.changeItems);
        foreach(var v in message.changeItems)
        {
            Frame.UIEventSystem.Ins.Publish(new ItemOneChangeEvent() {  index = v.index,type = (PackType)message.packType});
        }
        Frame.UIEventSystem.Ins.Publish(new ItemChangeEvent() {type = (PackType)message.packType});
		await ETTask.CompletedTask;
	}
}
public class PackSizeChangeHandler : AMHandler<PackSizeChangeResp>
{
	protected override async ETTask Run(Session session, PackSizeChangeResp message)
	{
		var data = DataSystem.Ins.ItemModel;
		if ((int)message.packType == (int)PackType.STORE)
			data.size_store = message.size;
		else
			data.size_pack = message.size;
		await ETTask.CompletedTask;
	}
}
