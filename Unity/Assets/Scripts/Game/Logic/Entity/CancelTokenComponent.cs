using ET;
using System;
using System.Collections.Generic;

namespace Frame
{

public class CancelTokenComponent:Entity,IAwake,IDestroy
{
	public Dictionary<int,ETCancellationToken> tokens = new Dictionary<int,ETCancellationToken>();
}
[ObjectSystem]
public class CancelTokenDestroySystem : DestroySystem<CancelTokenComponent>
{
	protected override void Destroy(CancelTokenComponent self)
	{
		self.TokenCancel();
	}
}
    public static class CancelTokenComponentSystem
    {
        public static void TokenCancel(this CancelTokenComponent self)
        {
            foreach (var v in self.tokens)
            {
                if (v.Value.IsCancel())
                    continue;
                v.Value.Cancel();
            }
            self.tokens.Clear();
        }
        public static void TokenCancel(this CancelTokenComponent self, int id)
        {
            if (self.tokens.TryGetValue(id, out ETCancellationToken token))
            {
                token.Cancel();
                self.tokens.Remove(id);
            }
        }
        public static ETCancellationToken SetToken(this CancelTokenComponent self, int id, ETCancellationToken token)
        {
            self.tokens.TryGetValue(id, out ETCancellationToken old);
            self.tokens[id] = token;
            return old;
        }
        public static ETCancellationToken GetToken(this CancelTokenComponent self, int id)
        {
            self.tokens.TryGetValue(id, out ETCancellationToken old);
            return old;
        }
        public static ETCancellationToken GetOrCreateToken(this CancelTokenComponent self, int id)
        {
            self.tokens.TryGetValue(id, out ETCancellationToken token);
            if (token == null || token.IsCancel())
            {
                token = new ETCancellationToken();
                self.SetToken(id, token);
            }
            return token;
        }
        public static int CreateToken(this CancelTokenComponent self,out ETCancellationToken token)
        {
            int max = 0;
            foreach(var v in self.tokens)
            {
                max = Math.Max(v.Key,max);
            }
            var t = new ETCancellationToken();
            token = t;
            self.SetToken(max+1, token);
            return max + 1;
        }
    }
}
