using ET;
using GameEvent;
using Frame;
using module.equip.message;


class EquipListRespHandler : AMHandler<EquipListResp>
{
    protected override async ETTask Run(Session session, EquipListResp message)
    {
        var model = DataSystem.Ins.EquipModel;
        model.size_pack = message.size_pack;
        model.size_store = message.size_store;
        if(message.items_pack != null)
        {
            foreach (var v in message.items_pack)
            {
                model.AddEquip(v, EquipPackType.PACK);
            }
        }
        if (message.items_store != null)
        {
            foreach (var v in message.items_store)
            {
                model.AddEquip(v, EquipPackType.STORE);
            }
        }
        if (message.items_equip != null)
        {
            foreach (var v in message.items_equip)
            {
                model.AddEquip(v, EquipPackType.EQUIP);
            }
        }
        await ETTask.CompletedTask;

    }
}
class EquipChangeRespHandler : AMHandler<EquipChangeResp>
{
    protected override async ETTask Run(Session session, EquipChangeResp message)
    {
        if(message.changeItems != null)
        {
            foreach(var v in message.changeItems)
            {
                if(v.equipId == 0)
                {
                    DataSystem.Ins.EquipModel.RemoveEquip(v.index, (int)message.packType);
                }
                else
                {
                    DataSystem.Ins.EquipModel.AddEquip(v, (EquipPackType)message.packType);
                }
            }
        }
        Frame.UIEventSystem.Ins.Publish(new EquipChangeEvent());
        await ETTask.CompletedTask;
    }
}

