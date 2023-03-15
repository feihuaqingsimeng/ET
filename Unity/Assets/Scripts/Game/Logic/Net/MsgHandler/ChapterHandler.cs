using ET;
using GameEvent;
using Frame;
using module.checkpoints.message;

class CheckPointsInitRespHandler : AMHandler<CheckPointsInitResp>
{
    protected override async ETTask Run(Session session, CheckPointsInitResp message)
    {
        DataSystem.Ins.ChapterModel.InitChapter(message.currentId);
        await ETTask.CompletedTask;
    }
}
class CheckPointsBattleRespHandler : AMHandler<CheckPointsBattleResp>
{
    protected override async ETTask Run(Session session, CheckPointsBattleResp message)
    {
        var data = DataSystem.Ins.ChapterModel;
        if(message.record.winner == 1)
        {
            data.InitChapter(message.checkPointsId);
            Frame.UIEventSystem.Ins.Publish(new ChapterLevelChangeEvent() { });
        }
        data.record = message.record;
        data.rewards.Clear();
        foreach(var v in message.rewards)
        {
            var info = new ItemInfo();
            info.UpdateByDB(v);
            data.rewards.Add(info);
        }
        await ETTask.CompletedTask;
    }
}
