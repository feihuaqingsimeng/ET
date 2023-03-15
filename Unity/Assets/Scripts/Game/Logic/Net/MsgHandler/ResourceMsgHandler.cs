
using ET;
using GameEvent;
using Frame;
using module.player.message;

public class ResourceMsgHandler : AMHandler<ResourceResp>
{
	protected override async ETTask Run(Session session, ResourceResp message)
	{
		if (message.changes == null) return;
		var data = DataSystem.Ins.ItemModel;
		data.SetResourceChange(message.changes);
		foreach(var v in message.changes)
		{
            Frame.UIEventSystem.Ins.Publish(new ResourceChangedEvent() { resType = (ResourceType)v.Key, resValue = v.Value });
		}
        Frame.UIEventSystem.Ins.Publish(new ResourceAllChangedEvent());
		await ETTask.CompletedTask;
	}
}
