
using ET;
using FairyGUI;
using Frame;
using module.login.message;
using UnityEngine;
class UILogin : UIBase
{
	public GTextInput account;
	public GComponent btnSure;
	public GButton btnServer;
}


static class UILoginSystem
{
	[ObjectSystem]
	class AwakeSystem : AwakeSystem<UILogin, GComponent>
	{
		protected override void Awake(UILogin self, GComponent a)
		{
			self.Awake(a);
			var account = a.GetCom("account");
			self.account = account.GetTextInput("text");
			self.btnSure = a.GetButton("btnSure");
			self.btnServer = a.GetButton("btnServer");
			self.btnServer.text = DataSystem.Ins.AccountModel.serverId.ToString();
			self.btnSure.onClick.Add(self.ClickSure);

			self.account.text = PlayerPrefs.GetString("accountName", "");
		}
	}
	public static void ClickSure(this UILogin self)
	{
		var account = self.account.text;
		if (account.Length < 2)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("uilogin_account_short") });
			Log.Error("账号长度太短");
			return;
		}
        Login(ConstValue.LoginAddress, account);
	}
	public static async void Login(string address, string account)
	{
        var serverId = DataSystem.Ins.AccountModel.serverId;
        var req = new LoginReq() { accountId = account, accountName = account, serverId = serverId };
        LoginResp resp = (LoginResp)await NetSystem.Call(req, typeof(LoginResp));

        if (resp == null)
		{
			Log.Error("登录失败");
            UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetLanguage("uilogin_login_fail") });
            return;
		}
		PlayerPrefs.SetString("accountName", account.ToString());
		var player = DataSystem.Ins;
		if (player.AccountModel.isLogin) return;
		player.AccountModel.isLogin = true;
		player.AccountModel.SetAccount(account, account);
        player.PlayerModel.SetPlayerId(resp.playerId, resp.createTime);

        FGUIPackageManager.Ins.AddPackageAsync("ItemAtlas");
        FGUIPackageManager.Ins.AddPackageAsync("Common");
        ConstValue.Init();
	}
}