using ET;
using System;

public class VirtualServer:Entity,IAwake
{
    public static VirtualServer Ins;
}
public class VirtualServerAwakeSystem : AwakeSystem<VirtualServer>
{
    protected override void Awake(VirtualServer self)
    {
        VirtualServer.Ins = self;
    }
}

public static class VirtualServerSystem
{
    
    public static bool Receive(this VirtualServer self,IMessage req)
    {
        return VirtualServerHandler.Receive(req);
    }
    public static async void Send(this VirtualServer self, IMessage message)
    {
        await TimerComponent.Instance.WaitAsync(50);
        var session = NetSystem.GetSession();
        Log.Error("本地Server发送消息：");
        OpcodeHelper.LogMsg(session.DomainZone(), message);
        // 普通消息或者是Rpc请求消息
        MessageDispatcherComponent.Instance.Handle(session, message);
        if (message is IResponse response)
        {
            session.OnResponse(response);
        }
    }
}
