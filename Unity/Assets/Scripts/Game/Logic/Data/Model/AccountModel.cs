using ET;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class AccountModel:Entity,IAwake
{
    public int serverId = 1002;
    public string accoutId;
    public string accountName;

    public bool isLogin;
}
public static class AccountModelSystem
{
    public static void SetAccount(this AccountModel self, string accountId, string accountName)
    {
        self.accoutId = accountId;
        self.accountName = accountName;
    }
}
