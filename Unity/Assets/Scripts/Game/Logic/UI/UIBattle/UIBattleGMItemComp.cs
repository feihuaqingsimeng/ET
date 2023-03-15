using ET;
using FairyGUI;
using Frame;
using TBS;

public class UIBattleGMItemComp : UIBase
{
    public GTextInput input;
}
class UIBattleGMItemCompAwakeSystem : AwakeSystem<UIBattleGMItemComp, GComponent>
{
    protected override void Awake(UIBattleGMItemComp self, GComponent a)
    {
        self.Awake(a);

        var btnLog = a.GetButton("btnLog");
        var btnlog2 = a.GetButton("btnLog2");
        var btnReport = a.GetButton("btnReport");
        self.input = a.GetTextInput("textReport");

        btnLog.onClick.Add(self.ClickLog);
        btnlog2.onClick.Add(self.ClickLog2);
        btnReport.onClick.Add(self.ClickGetReport);

    }
}
public static class UIBattleGMItemCompSystem
{
    public static void ClickLog(this UIBattleGMItemComp self)
    {
        var bat = ClientBattle.Get();
        if (bat == null) return;
        var report = bat.GetComponent<BattleReportComponent>();
        foreach (var cmd in report.initCmdList)
        {
            var str = cmd.isActive ? "已执行" : "   ";
            Log.Info($"{str} {cmd.ToString()}");
        }
        foreach (var v in report.commandList)
        {
            foreach (var cmd in v.Value)
            {
                var str = cmd.isActive ? "已执行" : "   ";
                if (cmd.action == BattleActionType.DAMAGE)
                    Log.Info($"<color=#CC3300>{str} {cmd.ToString()}</color>");
                else if (cmd.action == BattleActionType.ROUND_NODE)
                    Log.Info($"<color=#3399CC>{str} {cmd.ToString()}</color>");
                else
                    Log.Info($"{str} {cmd.ToString()}");
            }
        }
    }
    public static void ClickLog2(this UIBattleGMItemComp self)
    {
        var bat = ClientBattle.Get();
        if (bat == null) return;
        var rule = bat.GetComponent<BattleRuleComponent>();
        foreach (var cmd in rule.executeCmdList)
        {
            var str = cmd.isActive ? "已执行" : "   ";
            if (cmd.action == BattleActionType.DAMAGE)
                Log.Info($"<color=#CC3300>{str} {cmd.ToString()}</color>");
            else
                Log.Info($"{str} {cmd.ToString()}");
        }
    }
    public static void ClickGetReport(this UIBattleGMItemComp self)
    {
        var p = self.Parent as UIBattle;
        p.RequestReport(self.input.text);
    }
}
