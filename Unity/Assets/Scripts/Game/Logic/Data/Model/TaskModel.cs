using ET;
using module.task.message.vo;
using System.Collections.Generic;

public enum TaskState
{
    None = 0,
    //已接受,未完成
    Accept = 1,
    //已完成,未领取奖励
    Reward = 2,
    //已领取奖励
    Done = 3,
}
//任务类型
public enum TaskType
{
    NONE = 0,
    DAILY = 1,
    MAIN = 2,
    Achieve = 3,
}
public class TaskInfo
{
    // 任务id,前端自己判断一下,如果这个任务id,是当前已存在任务的后置任务,替换已存在的那个任务
    public int taskId;
    public int process;
    public TaskState state;
}
public class TaskModel:Entity,IAwake
{
    public MultiDictionary<int, int, TaskInfo> taskDic = new MultiDictionary<int, int, TaskInfo>();
    public Dictionary<int, int> preMappingDic = new Dictionary<int, int>();//前置任务
}

public static class TaskModelSystem
{
    public static void InitTaskList(this TaskModel self,List<TaskVo> list)
    {
        if (list == null) return;
        self.InitPreMapping();
        foreach (var v in list)
        {
            var cfg = ConfigSystem.Ins.GetTaskConfig(v.taskId);
            if (cfg == null)
            {
                Log.Error($"找不到该任务：{v.taskId}");
                continue;
            }
            int type = cfg.Get("Type");
            if(!self.taskDic.TryGetValue(type, v.taskId, out var info))
            {
                info = new TaskInfo();
                self.taskDic.Add(type, v.taskId, info);
            }
            self.UpdateTaskInfo(info, v);
            if(self.preMappingDic.TryGetValue(v.taskId,out var preId))
            {
                self.DeleteTask(preId);
            }
        }
    }
    public static void UpdateTaskInfo(this TaskModel self,TaskInfo info,TaskVo db)
    {
        info.taskId = db.taskId;
        info.process = db.process;
        info.state = (TaskState)db.state;
    }
    private static void InitPreMapping(this TaskModel self)
    {
        if (self.preMappingDic.Count > 0) return;
        var dic = ConfigSystem.Ins.GetTaskConfig();
        foreach(var v in dic)
        {
            var id = v.Value.Get("Ascription");
            if(id > 0)
            {
                self.preMappingDic[id] = v.Key;
            }
        }
    }
    public static void DeleteTask(this TaskModel self,int id)
    {
        var cfg = ConfigSystem.Ins.GetTaskConfig(id);
        if (cfg == null)
        {
            Log.Error($"找不到该任务：{id}");
            return;
        }
        int type = cfg.Get("Type");
        self.taskDic.Remove(type, id);
    }
    public static TaskInfo GetTask(this TaskModel self, TaskType type,int id)
    {
        self.taskDic.TryGetValue((int)type, id, out var info);
        return info;
    }
    public static void GetTaskList(this TaskModel self, TaskType type, List<TaskInfo> taskList)
    {
        if (taskList == null)
            return;
        self.taskDic.TryGetDic((int)type, out var dic);
        if (dic == null) return ;
        taskList.AddRange(dic.Values);
        taskList.Sort(Sort);
    }
    private static int Sort(TaskInfo a,TaskInfo b)
    {
        int aw = a.state == TaskState.Reward ? 0 : a.state == TaskState.Done ? 10 : 5;
        int bw = b.state == TaskState.Reward ? 0 : b.state == TaskState.Done ? 10 : 5; ;
        if (aw != bw) return aw - bw;
        return a.taskId - b.taskId;
    }
}