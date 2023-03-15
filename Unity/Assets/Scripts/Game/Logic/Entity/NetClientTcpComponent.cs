using ET;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEditor.UI;
using UnityEngine;

namespace Game
{
    public class NetClientTcpComponent : Entity,IAwake<AddressFamily>
    {
        public int ServiceId;
        public Session session;
    }
    public static class NetClientTcpComponentSystem
    {
        class AwakeSystem : AwakeSystem<NetClientTcpComponent,AddressFamily>
        {
            protected override void Awake(NetClientTcpComponent self,AddressFamily address)
            {
                self.ServiceId = NetServices.Instance.AddService(new TService(address, ServiceType.Outer));
                NetServices.Instance.RegisterReadCallback(self.ServiceId, self.OnRead);
                NetServices.Instance.RegisterErrorCallback(self.ServiceId, self.OnError);
            }
        }
        public static void OnRead(this NetClientTcpComponent self, long channelId,long actorId,object message)
        {
            Session session = self.GetChild<Session>(channelId);
            if (session == null) return;
            OpcodeHelper.LogMsg(self.DomainZone(), message);
            MessageDispatcherComponent.Instance.Handle(session, message);
            if (message is IResponse response)
            {
                session.OnResponse(response);
            }
        }
        public static void OnError(this NetClientTcpComponent self,long channelId,int error)
        {
            Session session = self.GetChild<Session>(channelId);
            if (session == null) return;
            session.Error = error;
            session.Dispose();
        }

        public static Session Create(this NetClientTcpComponent self,IPEndPoint ipEndPoint)
        {
            uint channelId = NetServices.Instance.CreateConnectChannelId();
            Session session = self.AddChildWithId<Session, int>(channelId, self.ServiceId);
            session.RemoteAddress = ipEndPoint;
            NetServices.Instance.CreateChannel(self.ServiceId, channelId, ipEndPoint);
            return session;
        }
    }
}
