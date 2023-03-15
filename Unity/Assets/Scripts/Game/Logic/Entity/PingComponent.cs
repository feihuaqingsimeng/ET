using ET;
using module.session.message;
using System;

namespace Game
{
    [ComponentOf(typeof(Session))]
    public class PingComponent: Entity, IAwake, IDestroy
    {
        public long Ping; //延迟值
    }

    public class PingComponentAwakeSystem : AwakeSystem<PingComponent>
    {
        protected override void Awake(PingComponent self)
        {
            PingAsync(self).Coroutine();
        }

        private static async ETTask PingAsync(PingComponent self)
        {
            Session session = self.GetParent<Session>();
            long instanceId = self.InstanceId;

            while (true)
            {
                if (self.InstanceId != instanceId)
                {
                    return;
                }

                long time1 = TimeHelper.ClientNow();
                try
                {
                    HeartResp response = await session.Call(new HeartReq()) as HeartResp;

                    if (self.InstanceId != instanceId)
                    {
                        return;
                    }

                    long time2 = TimeHelper.ClientNow();
                    self.Ping = time2 - time1;

                    TimeInfo.Instance.ServerMinusClientTime = response.time + response.offset + (time2 - time1) / 2 - time2;

                    await TimerComponent.Instance.WaitAsync(5000);
                }
                catch (RpcException e)
                {
                    // session断开导致ping rpc报错，记录一下即可，不需要打成error
                    Log.Info($"ping error: {self.Id} {e.Error}");
                    return;
                }
                catch (Exception e)
                {
                    Log.Error($"ping error: \n{e}");
                }
            }
        }
    }

    [ObjectSystem]
    public class PingComponentDestroySystem : DestroySystem<PingComponent>
    {
        protected override void Destroy(PingComponent self)
        {
            self.Ping = default;
        }
    }
}