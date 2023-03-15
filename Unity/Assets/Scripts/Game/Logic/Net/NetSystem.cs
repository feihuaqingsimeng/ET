using ET;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class NetSystem
{
	public static Session CreateSession()
	{
		NetClientTcpComponent netTcpComonent = Root.Instance.Scene.GetComponent<NetClientTcpComponent>();
		if(netTcpComonent != null)
			netTcpComonent.Dispose();
		IPEndPoint ipEndPoint =  NetworkHelper.ToIPEndPoint(ConstValue.LoginAddress);
		netTcpComonent = Root.Instance.Scene.AddComponent<NetClientTcpComponent,AddressFamily>(ipEndPoint.AddressFamily);
		var session = netTcpComonent.Create(ipEndPoint);
		//session.AddComponent<PingComponent>();
		netTcpComonent.session = session;
		return session;
	}
	public static Session GetSession()
	{
		var netTcpComonent = Root.Instance.Scene.GetComponent<NetClientTcpComponent>();
		if(netTcpComonent == null|| netTcpComonent.session == null || netTcpComonent.session.IsDisposed)
		{
			var session = CreateSession();
			return session;
		}
		return netTcpComonent.session;
	}
	public static async ETTask<IResponse> Call(IRequest request,Type responeType, ETCancellationToken cancellationToken = null)
	{
        var session = GetSession();
		return await session.Call(request, responeType, cancellationToken);
	}
	public static void Send(IRequest request)
	{
		var session = GetSession();
		session.Send(request);
	}


}

