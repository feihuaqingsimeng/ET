using ET;
using GameEvent;
using Frame;
using module.counter.message;
using module.item.message;
using module.level.message;
using module.player.message;

public class AttrChangeHandler : AMHandler<AttrChangeResp>
{
	protected override async ETTask Run(Session session, AttrChangeResp message)
	{
		DataSystem.Ins.PlayerModel.SetAttrChange(message.changes);
		await ETTask.CompletedTask;
	}
}
[MessageHandler(SceneType.Process)]
public class PlayerInfoHandler : AMHandler<PlayerInfoResp>
{
	protected override async ETTask Run(Session session, PlayerInfoResp message)
	{
		var data = DataSystem.Ins.PlayerModel;
		data.playerId = message.playerId;
		data.name = message.name;
		data.level = message.level;
		data.exp = message.exp;
		data.SetAttrChange(message.attributes);
        DataSystem.Ins.ItemModel.SetResourceChange(message.resources);

		UISystem.Ins.Close(UIType.UILogin);
		UISystem.Ins.Show(UIType.UIBottomButton,new UIBottomButtonParam() { index = UIBottomButtonType.UIHome, isOpen = true });

		data.StartExpTick().Coroutine();

        NetSystem.Send(new DrugsTuJianReq());
		await ETTask.CompletedTask;
	}
}
public class LevelUpHandler : AMHandler<LevelUpResp>
{
	protected override async ETTask Run(Session session, LevelUpResp message)
	{
		var playerData = DataSystem.Ins.PlayerModel;
		if(message.newLv > 0)
		{
			playerData.level = message.newLv;
		}
		playerData.exp = message.exp;
		playerData.StartExpTick().Coroutine() ;
        Frame.UIEventSystem.Ins.Publish(new LevelUpEvent() { success = message.newLv > 0 });
		await ETTask.CompletedTask;
	}
}
public class LevelExtHandle : AMHandler<LevelExtResp>
{
	protected override async ETTask Run(Session session, LevelExtResp message)
	{
		var playerData = DataSystem.Ins.PlayerModel;
		playerData.failCount = message.failCount;
		playerData.probAdd = message.probAdd;
		await ETTask.CompletedTask;
	}
}
//计数器更新or上线时推送,更新时只发有变化的
public class CounterRespHandler : AMHandler<CounterResp>
{
    protected override async ETTask Run(Session session, CounterResp message)
    {
        var data = DataSystem.Ins.PlayerModel;
        data.SetCounterMap(message.countMap);
        Frame.UIEventSystem.Ins.Publish(new CounterUpdateEvent());

        await ETTask.CompletedTask;
    }
}
