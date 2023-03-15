using ET;
using System;

namespace Frame
{
    [ComponentOf]
    public class ManagedEntity: Entity, IAwake, IDestroy
    {
    }
    [FriendOf(typeof(ManagedEntity))]
    public static class ManagedEntitySystem
    {
        #region token
        public static ETCancellationToken GetToken(this ManagedEntity self, int tokenId,bool autoCreate = true)
        {
            if (autoCreate)
                return self.GetCom<CancelTokenComponent>().GetOrCreateToken(tokenId);
            else
                return self.GetCom<CancelTokenComponent>().GetToken(tokenId);
        }
        public static int CreateToken(this ManagedEntity self,out ETCancellationToken token)
        {
            return self.GetCom<CancelTokenComponent>().CreateToken(out token);
        }
        public static void CancelToken(this ManagedEntity self, int tokenId)
        {
            self.GetCom<CancelTokenComponent>().TokenCancel(tokenId);
        }
        private static T GetCom<T>(this ManagedEntity self,bool add = true) where T : Entity, IAwake, new()
        {
            var t = self.GetComponent<T>();
            if (t == null)
                t = self.AddComponent<T>();
            return t;
        }
        #endregion token
        #region time
        public static int TimeCall(this ManagedEntity self,long millsecond,Action callback,int times = 1)
        {
            
            int id = self.CreateToken(out ETCancellationToken token);
            self._TimeCall(millsecond, callback, token,times);
            return id;
        }
        public static async ETTask<bool> WaitAsync(this ManagedEntity self,long millsecond, ETCancellationToken token = null)
        {
            if(token == null)
                self.CreateToken(out token);
            await TimerComponent.Instance.WaitAsync(millsecond, token);
            if (token != null && token.IsCancel())
                return false;
            return true;
        }
        public static void RemoveTime(this ManagedEntity self,int id)
        {
            self.CancelToken(id);
        }
        private static async void _TimeCall(this ManagedEntity self,long millsecond,Action callback, ETCancellationToken token, int times = 1)
        {
            if (times == 0) return;
            while (true)
            {
                var flag = await WaitAsync(self,millsecond, token);
                if (!flag) return;
                callback?.Invoke();
                if (times > 0)
                {
                    times--;
                    if (times == 0)
                        return;
                }
                
            }
            
        }
        #endregion time
        #region event
        public static void RegisterEvent<T>(this ManagedEntity self, Action<T> callback) where T : struct
        {
            self.GetCom<EventComponent>().Register(callback);
        }
        public static void UnRegisterEvent<T>(this ManagedEntity self, Action<T> callback) where T : struct
        {
            self.GetCom<EventComponent>().UnRegister(callback);
        }
        #endregion event
    }
}
