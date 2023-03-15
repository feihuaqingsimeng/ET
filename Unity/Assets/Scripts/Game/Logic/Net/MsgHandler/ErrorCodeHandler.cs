using ET;
using GameEvent;
using Frame;
using module.errorcode.message;

public class ErrorCodeHandler : AMHandler<ErrorCodeResp>
{
	protected override async ETTask Run(Session session, ErrorCodeResp message)
	{
        var str = "";
        var cfg = ConfigSystem.Ins.Get("tips", message.errorId);
        if (cfg == null)
            str= message.errorId.ToString();
        str = cfg.Get<string>("Content");
        string sound = cfg.Get<string>("SoundEffects");
        if (!string.IsNullOrEmpty(sound))
        {
            AudioManager.Ins.PlayAudio($"Sound/{sound}.mp3");
        }
        str = ConfigSystem.Ins.GetErrorCode(message.errorId);
		if(message.paramList == null || message.paramList.Count == 0)
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = str });
		}
		else
		{
			string content = string.Format(str, message.paramList.ToArray());
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = content });
		}
		await ETTask.CompletedTask;
	}
}
