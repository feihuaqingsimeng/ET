using ET;
using GameEvent;
using Frame;
using module.task.message;

class TaskAddOrUpdateRespHandler : AMHandler<TaskAddOrUpdateResp>
{
    protected override async ETTask Run(Session session, TaskAddOrUpdateResp message)
    {
        DataSystem.Ins.TaskModel.InitTaskList(message.taskList);
        Frame.UIEventSystem.Ins.Publish(new TaskUpdateEvent());
        await ETTask.CompletedTask;
    }
}
class TaskDeleteRespHandler : AMHandler<TaskDeleteResp>
{
    protected override async ETTask Run(Session session, TaskDeleteResp message)
    {
        foreach(var v in message.deletes)
        {
            DataSystem.Ins.TaskModel.DeleteTask(v);
        }
        Frame.UIEventSystem.Ins.Publish(new TaskUpdateEvent());
        await ETTask.CompletedTask;
    }
}
