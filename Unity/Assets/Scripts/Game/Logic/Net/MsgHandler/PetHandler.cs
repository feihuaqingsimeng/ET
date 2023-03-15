using ET;
using GameEvent;
using Frame;
using module.pet.message;

//新增一只宠物or宠物信息变化
public class PetAddOrUpdateRespHandler : AMHandler<PetAddOrUpdateResp>
{
    protected override async ETTask Run(Session session, PetAddOrUpdateResp message)
    {
        var model = DataSystem.Ins.PetModel;
        var info = model.GetPet(message.petInfo.uuid);
        if(info != null)
        {
            var oldId = info.modelId;
            info.UpdateByDB( message.petInfo);
            if(oldId != info.modelId)
                Frame.UIEventSystem.Ins.Publish(new PetEvolutionSucEvent() { uid = message.petInfo.uuid });
        }
        else
        {
            info = new PetInfo();
            info.UpdateByDB(message.petInfo);
            model.AddPet(info);
        }
        if (info.battleIndex > 0)
            model.battlePets[info.battleIndex] = info.uid;
        else
            foreach (var v in model.battlePets)
            {
                if(v.Value == info.uid)
                {
                    model.battlePets.Remove(v.Key);
                    break;
                }
            }
        Frame.UIEventSystem.Ins.Publish(new PetUpdateEvent() { uid = message.petInfo.uuid });
        await ETTask.CompletedTask;
    }
}
//上线推送宠物列表
public class PetListRespHandler : AMHandler<PetListResp>
{
    protected override async ETTask Run(Session session, PetListResp message)
    {
        var model = DataSystem.Ins.PetModel;
        model.pets.Clear();
        foreach (var v in message.petList)
        {
            PetInfo info = new PetInfo();
            info.UpdateByDB(v);
            model.AddPet(info);
            if (info.battleIndex > 0)
                model.battlePets[info.battleIndex] = info.uid;
        }
        
        await ETTask.CompletedTask;
    }
}
//删除一只宠物
public class PetDeleteRespHandler : AMHandler<PetDeleteResp>
{
    protected override async ETTask Run(Session session, PetDeleteResp message)
    {
        DataSystem.Ins.PetModel.RemovePet(message.petUuid);
        Frame.UIEventSystem.Ins.Publish(new PetDeleteEvent() { uid = message.petUuid });
        await ETTask.CompletedTask;
    }
}
public class PetChuZhanRespHandler : AMHandler<PetChuZhanResp>
{
    protected override async ETTask Run(Session session, PetChuZhanResp message)
    {
        Frame.UIEventSystem.Ins.Publish(new PetFormationSucEvent());
        await ETTask.CompletedTask;
    }
}
public class PetJinhuaRespHandler : AMHandler<PetJinhuaResp>
{
    protected override async ETTask Run(Session session, PetJinhuaResp message)
    {
        await ETTask.CompletedTask;
    }
}
//如果抓捕成功了,返回结果
public class PetCatchRespHandler : AMHandler<PetCatchResp>
{
    protected override async ETTask Run(Session session, PetCatchResp message)
    {
        DataSystem.Ins.PetModel.InitCaptureList(message.results);
        await ETTask.CompletedTask;
    }
}

