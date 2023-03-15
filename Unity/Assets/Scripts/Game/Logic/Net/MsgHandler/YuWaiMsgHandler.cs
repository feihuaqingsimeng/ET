using ET;
using module.yw.message;
using GameEvent;
using Frame;

//域外战场当前所在的格子信息
public class YwMapPosInfoRespHandler : AMHandler<YwMapPosInfoResp>
{
	protected override async ETTask Run(Session session, YwMapPosInfoResp message)
	{
		var data = DataSystem.Ins.YuWaiModel;
		var info = data.GetYWPosInfo(message.y, message.x);
		if(info == null)
		{
			info = new YWPosInfo();
			data.AddYWPosInfo(message.y,message.x, info);
		}
		info.x = message.x;
		info.y = message.y;
		info.playerList = message.playerList;
		info.tansuo = message.miJing != null? true:message.tansuo;
		info.miJing = message.miJing;
		info.lowerMapId = message.lowerMapId;
        Frame.UIEventSystem.Ins.Publish(new YWPosInfoChangeEvent() { info = info });
		await ETTask.CompletedTask;
	}
}
//进入地图或打开界面时,覆盖更新玩家已经走过的格子
public class YwMovedPosRespHandler : AMHandler<YwMovedPosResp>
{
	protected override async ETTask Run(Session session, YwMovedPosResp message)
	{
		var data = DataSystem.Ins.YuWaiModel;
		if (data.ywMapId != message.lowerMapId)
		{
			data.ClearYWPosDic(data.ywMapId);
		}
		for(int i = 0; i < message.movedPath.Count; i++)
		{
			var p = message.movedPath[i];
			var info = data.GetYWPosInfo(p.y , p.x);
			if (info == null)
			{
				info = new YWPosInfo
				{
					lowerMapId = message.lowerMapId,
					x = p.x,
					y = p.y
				};
				data.AddYWPosInfo(p.y, p.x, info);
			}
		}
        Frame.UIEventSystem.Ins.Publish(new YWPosInfoPathChangeEvent());
		await ETTask.CompletedTask;
	}
}
//域外战场灵力更新
public class YwLingLiUpdateRespHandler : AMHandler<YwLingLiUpdateResp>
{
	protected override async ETTask Run(Session session, YwLingLiUpdateResp message)
	{
		var data = DataSystem.Ins.YuWaiModel;
		data.ywLingli = message.lingLi;
		data.ywLingLiLastUpdateTime = message.lastUpdateTime;
        Frame.UIEventSystem.Ins.Publish(new YWLingLiChangeEvent());
		await ETTask.CompletedTask;
	}
}
//从主界面打开域外战场返回消息
public class YwOpenRespHandler : AMHandler<YwOpenResp>
{
	protected override async ETTask Run(Session session, YwOpenResp message)
	{
		if (message.lowerMapId == 0)
			return;
		var data = DataSystem.Ins.YuWaiModel;
		data.ywMapId = message.lowerMapId;
		data.ywCurRow = message.y;
		data.ywCurCol = message.x;
        Frame.UIEventSystem.Ins.Publish(new YWMapChangeEvent());
        await ETTask.CompletedTask;
	}
}
//传送返回
public class YwEntryRespHandler : AMHandler<YwEntryResp>
{
	protected override async ETTask Run(Session session, YwEntryResp message)
	{
		var data = DataSystem.Ins.YuWaiModel;
		data.ywMapId = message.lowerMapId;
		data.ywCurRow = message.y;
		data.ywCurCol = message.x;
		data.ywNoticeInfoList.Clear();
		data.ywNoticeInfoIsRequest = false;
        Frame.UIEventSystem.Ins.Publish(new YWMapChangeEvent());
        await ETTask.CompletedTask;
	}
}
//直接覆盖域外战场的储物袋
public class YwPacketRespHandler : AMHandler<YwPacketResp>
{
	protected override async ETTask Run(Session session, YwPacketResp message)
	{
		var data = DataSystem.Ins.YuWaiModel;
		data.ywPacketSize = message.packetSize;
		data.ywRewards.Clear();
		if(message.rewards != null)
			foreach (var v in message.rewards)
			{
				data.ywRewards.Add(v);
			}
        Frame.UIEventSystem.Ins.Publish(new YWPacketChangeEvent());
		await ETTask.CompletedTask;
	}
}
//请求探索成功,返回被探索的地图和坐标
public class YwTanSuoRespHandler : AMHandler<YwTanSuoResp>
{
	protected override async ETTask Run(Session session, YwTanSuoResp message)
	{
		var data = DataSystem.Ins.YuWaiModel;
		var info = data.GetYWPosInfo(message.y, message.x);
		if (info != null)
			info.tansuo = true;
		await ETTask.CompletedTask;
	}
}
//一个新的秘境产生了,广播给在这个地图的所有玩家
public class YwMiJingCreateRespHandler : AMHandler<YwMiJingCreateResp>
{
	protected override async ETTask Run(Session session, YwMiJingCreateResp message)
	{
		var data = DataSystem.Ins.YuWaiModel;
		var info = data.GetYWPosInfo(message.miJing.y, message.miJing.x);
		if (info == null) return;
		info.miJing = message.miJing;
		info.tansuo = true;
        Frame.UIEventSystem.Ins.Publish(new YWMiJingChangeEvent());
		await ETTask.CompletedTask;
	}
}
public class YwMiJingUpdateRespHandler : AMHandler<YwMiJingUpdateResp>
{
	protected override async ETTask Run(Session session, YwMiJingUpdateResp message)
	{
		var data = DataSystem.Ins.YuWaiModel;
		var info = data.GetYWPosInfo(message.y, message.x);
		if (info == null||info.miJing == null) return;
		info.miJing.hadTanSuo = true;
        Frame.UIEventSystem.Ins.Publish(new YWMiJingChangeEvent() { x = message.x, y = message.y });
		await ETTask.CompletedTask;
	}
}
//域外战场的频道信息返回
public class YwInformationListRespHandler : AMHandler<YwInformationListResp>
{
	protected override async ETTask Run(Session session, YwInformationListResp message)
	{
		var data = DataSystem.Ins.YuWaiModel;
		data.ywNoticeInfoIsRequest = true;
		data.ywNoticeInfoList.Clear();
		if (message.infoList == null) return;
		foreach(var v in message.infoList)
		{
			data.ywNoticeInfoList.Add(new YWNoticeInfo(v));
		}
        Frame.UIEventSystem.Ins.Publish(new YWNoticeInfoChangeEvent());
		await ETTask.CompletedTask;
	}
}
public class YwInformationAddRespHandler : AMHandler<YwInformationAddResp>
{
	protected override async ETTask Run(Session session, YwInformationAddResp message)
	{
		var data = DataSystem.Ins.YuWaiModel;
		if (!data.ywNoticeInfoIsRequest) return;
		data.ywNoticeInfoList.Add(new YWNoticeInfo(message.addInfo));
		await ETTask.CompletedTask;
	}
}
public class YwPkRespHandler : AMHandler<YwPkResp>
{
    protected override async ETTask Run(Session session, YwPkResp message)
    {
        if(message.record.winner != 1)
        {
            var data = DataSystem.Ins.YuWaiModel;
            data.ywMapId = 0;
            Frame.UIEventSystem.Ins.Publish(new YWMapChangeEvent());
        }
        await ETTask.CompletedTask;
    }
}