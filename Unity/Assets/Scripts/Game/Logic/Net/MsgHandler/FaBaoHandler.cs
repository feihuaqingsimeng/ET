using ET;
using GameEvent;
using Frame;
using module.fabao.message;


public class FaBaoHandler
{
	public static void SendWearReq(FaBaoInfo info)
	{
		var cfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(info.modelId);
		int type = cfg.Get("FBType");
		int count = 0;
		int index = -1;
		for (int i = 1; i <= ConstValue.FABAO_EQUIP_COUNT; i++)
		{
			var f = DataSystem.Ins.ItemModel.GetFabao(i, FaBaoPackType.EQUIP);

			if (f == null && index == -1)
			{
				index = i;
			}
			if (f == null) continue;
			var cfg2 = ConfigSystem.Ins.GetFBMagicWeaponConfig(f.modelId);
			int type2 = cfg2.Get("FBType");
			if (type == type2)
				count++;
		}
		if (index == -1)
		{
			return;
		}
		if (count >= ConstValue.FABAO_EQUIP_TYPE_LIMIT)
		{
			//Game.EventSystem.Publish(new OpenUI(UIType.UIFlowTip, new UIFlowTipParam() { content = "无法佩戴3个同类型法宝" }));
			//return;
		}
		var req = new FaBaoWearReq() { packetIndex = (int)info.uid, wearIndex = index,actionType = 1 };
		NetSystem.Send(req);
	}
}

public class FaBaoPacketHandler : AMHandler<FaBaoPacketResp>
{
	protected override async ETTask Run(Session session, FaBaoPacketResp message)
	{
		var data = DataSystem.Ins.ItemModel;
		data.AddFaBaoList(FaBaoPackType.PACK, message.faBaos_pack);
		data.AddFaBaoList(FaBaoPackType.EQUIP, message.faBaos_equip);
        Frame.UIEventSystem.Ins.Publish(new FaBaoChangeEvent());
		await ETTask.CompletedTask;
	}
}
public class FaBaoChangeHandler : AMHandler<FaBaoChangeResp>
{
	protected override async ETTask Run(Session session, FaBaoChangeResp message)
	{
		var data = DataSystem.Ins.ItemModel;
		var old = data.GetFabaoList((FaBaoPackType) message.packType);
		int count = old == null ? 0 : old.Count;
		data.AddFaBaoList((FaBaoPackType)message.packType, message.changeItems);
		old = data.GetFabaoList((FaBaoPackType)message.packType);
		int count2 = old == null ? 0 : old.Count;
        Frame.UIEventSystem.Ins.Publish(new FaBaoChangeEvent());
		if (count != count2)
            Frame.UIEventSystem.Ins.Publish(new FaBaoCountChangeEvent());
		await ETTask.CompletedTask;
	}
}
public class FabaoWearHandler : AMHandler<FaBaoWearResp>
{
	protected override async ETTask Run(Session session, FaBaoWearResp message)
	{
        Frame.UIEventSystem.Ins.Publish(new FaBaoWearSucEvent());
		await ETTask.CompletedTask;
	}
}
