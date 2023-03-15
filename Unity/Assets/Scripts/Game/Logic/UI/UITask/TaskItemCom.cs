using FairyGUI;
using ET;
using module.task.message;
using Frame;

public class TaskItemCom : UIBase
{
    public GTextField textTitle;
    public GTextField textName;
    public GTextField textNum;
    public GTextField textReward;
    public Controller ctrlState;

    public TaskInfo info;
}
class TaskItemComAwakeSystem : AwakeSystem<TaskItemCom, GComponent>
{
    protected override void Awake(TaskItemCom self, GComponent a)
	{
		self.Awake(a);
        self.textTitle = a.GetText("title");
        self.textName = a.GetText("name");
        self.textNum = a.GetText("num");
        self.textReward = a.GetText("reward");
        self.ctrlState = a.GetController("state");
        var btnReward = a.GetButton("btnReward");

        btnReward.onClick.Add(self.ClickReward);

    }
}
public static class TaskItemComSystem
{
	public static void Refresh(this TaskItemCom self,TaskInfo info)
	{
        self.info = info;
        var cfg = ConfigSystem.Ins.GetTaskConfig(info.taskId);
        self.textName.text = cfg.Get<string>("Introduce");
        var param = cfg.Get<int[]>("Value");
        self.textNum.text = string.Format($"({self.info.process}/{param[0]})");
        var param2 = cfg.Get<int[][]>("Reward");
        string str = "";
        for(int i = 0; i < param2.Length; i++)
        {
            var p = param2[i];
            str  += $"{RewardInfoFactory.GetRewardName((RewardType) p[0], p[1])}x{p[2]}   ";
        }
        var type =  cfg.Get("Type");
        if (type == (int)TaskType.DAILY)
            self.textTitle.text = ConfigSystem.Ins.GetLanguage("uitask_daily_title");
        else if (type == (int)TaskType.MAIN)
            self.textTitle.text = ConfigSystem.Ins.GetLanguage("uitask_main_title");
        else
            self.textTitle.text = ConfigSystem.Ins.GetLanguage("uitask_achieve_title");

        self.textReward.text = str;
        self.ctrlState.selectedIndex = (int)info.state-1;

    }
    public static void ClickReward(this TaskItemCom self)
    {
        var req = new TaskTakeRewardReq() { taskId = self.info.taskId };
        NetSystem.Send(req);
    }
   
}