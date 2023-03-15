using FairyGUI;
using ET;
using Frame;

public class HandBookEffectCom : UIBase
{
    public GTextField txtName;
    public GTextField txtNum;
    public GTextField txtAttrName;
    public GTextField txtAttrValue;
    public GTextField txtEffect;
}
class HandBookEffectComAwakeSystem : AwakeSystem<HandBookEffectCom, GComponent>
{
    protected override void Awake(HandBookEffectCom self, GComponent a)
	{
		self.Awake(a);
        self.txtName = a.GetText("title");
        self.txtNum = a.GetText("num");
        self.txtAttrName = a.GetText("attrName");
        self.txtAttrValue = a.GetText("attrValue");
        self.txtEffect = a.GetText("effect");

    }
}

public static class HandBookEffectComSystem
{
	public static void Refresh(this HandBookEffectCom self,DrugsUseInfo info)
	{
        self.txtName.text = ItemUtil.GetItemName(info.itemId);
        self.txtName.color = ItemUtil.GetItemQualityColorById(info.itemId);
        var refine = ConfigSystem.Ins.GetDrugsRefining(info.itemId);
        int limit = refine.Get("UseUpperLimit");
        self.txtNum.text = string.Format($"({info.effectCount}/{limit})");
        self.txtAttrName.text = string.Format($"{AttributeTypeUtil.GetNameStr(info.attrType)}：");
        
        var itemCfg = ConfigSystem.Ins.GetItemConfig(info.itemId);
        int max = 0;
        if (itemCfg!=null)
        {
            max = itemCfg.Get<int[]>("UseParameter")[1];
            max *= limit;
        }
        var curStr = AttributeTypeUtil.GetValueStr(info.attrType, info.effectValue);

        self.txtAttrValue.text = $"{curStr}/{AttributeTypeUtil.GetValueStr(info.attrType, max)}";
        int e = info.minEffect / 1000;
        if (e <= 0) e = 1;
        if (e > 10) e = 10;
        self.txtEffect.text = string.Format(ConfigSystem.Ins.GetLanguage($"drugs_efficacy_{e}"));
    }
}