using ET;
using FairyGUI;
using Frame;

public class ExpItemComponent : UIBase
{
	public GProgressBar gBar;
	public GTextField textTip;
}

[ObjectSystem]
public class ExpItemComponentAwakeSystem : AwakeSystem<ExpItemComponent,GComponent>
{
	protected override void Awake(ExpItemComponent self,GComponent a)
	{
		self.Awake(a);
		self.gBar = a.asProgress;
		self.textTip = a.GetText("text");

		self.Refresh();
	}
}

public static class ExpItemComponentSystem
{
	public static void Refresh(this ExpItemComponent self)
	{
		var data = DataSystem.Ins.PlayerModel;
		if (data.CheckExpLimit())
		{
			self.gBar.value = 100;
			self.textTip.text = ConfigSystem.Ins.GetLanguage("exp_max_tip");
		}
		else
		{
            float t = (TimeHelper.ServerNow() - data.expUpdateTime) / 1000.0f;
            self.gBar.value = t*10;
			self.gBar.TweenValue(100, 10-t).SetEase(EaseType.Linear) ;
			var config = ConfigSystem.Ins.GetLvConfig(data.level);
			var exp = config.Get("HarvestExp");
            self.textTip.text = string.Format(ConfigSystem.Ins.GetLanguage("uihome_learn_rate_tip"), exp);
		}
	}

}