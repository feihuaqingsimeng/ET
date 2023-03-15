using ET;
using GameEvent;
using module.item.message;

public class DrugsTuJianRespHandler : AMHandler<DrugsTuJianResp>
{
    protected override async ETTask Run(Session session, DrugsTuJianResp message)
    {
        var model = DataSystem.Ins.HandBookModel;
        model.InitTotalAttr(message.totalAttr);
        model.InitDrugsRecords(message.tuJians);
        model.InitFormula(message.peiFangs);
        await ETTask.CompletedTask;
    }
}
public class LearnDrugsPeiFangRespHandler : AMHandler<LearnDrugsPeiFangResp>
{
    protected override async ETTask Run(Session session, LearnDrugsPeiFangResp message)
    {
        var model = DataSystem.Ins.HandBookModel;
        model.formulaDic.Add(message.drugsId);
        await ETTask.CompletedTask;
    }
}
