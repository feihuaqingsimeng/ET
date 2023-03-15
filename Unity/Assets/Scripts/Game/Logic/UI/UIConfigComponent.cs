using ET;
using Frame;

[ComponentOf(typeof(Scene))]
public class UIConfigConponent : Entity, IAwake
{
}

[ObjectSystem]
public class UIConfigConponetAwakeSystem : AwakeSystem<UIConfigConponent>
{
    protected override void Awake(UIConfigConponent self)
    {
        UISystem.Ins.AddConfig(UIType.UILogin, new UIConfig("UILogin", "UILogin", typeof(UILogin)));
        //common
        /*self.configs.Add(UIType.UIGM, new UIConfig("UIGM", "UIGM", typeof(UIGM)));
        self.configs.Add(UIType.UIFlowTip, new UIConfig("Common", "UIFlowTip", typeof(UIFlowTip), UIOrder.Top));
        self.configs.Add(UIType.UIItemDetail, new UIConfig("Common", "UIItemDetail", typeof(UIItemDetail)));
        self.configs.Add(UIType.UICommonTip, new UIConfig("Common", "UICommonTip", typeof(UICommonTip)));
        self.configs.Add(UIType.UICostCommonTip, new UIConfig("Common", "UICostCommonTip", typeof(UICostCommonTip)));
        self.configs.Add(UIType.UISkillTips, new UIConfig("Common", "UISkillTips", typeof(UISkillTips)));
        self.configs.Add(UIType.UIEquipDetail, new UIConfig("UIEquipDetail", "UIEquipDetail", typeof(UIEquipDetail)));
        self.configs.Add(UIType.UIItemUse, new UIConfig("UIItemUse", "UIItemUse", typeof(UIItemUse)));
        self.configs.Add(UIType.UIFabaoDetail, new UIConfig("UIFabaoDetail", "UIFabaoDetail", typeof(UIFabaoDetail)));
        //login
        self.configs.Add(UIType.UILogin, new UIConfig("UILogin", "UILogin", typeof(UILogin)));
        //home
        self.configs.Add(UIType.UIHome, new UIConfig("UIHome", "UIHome", typeof(UIHome), UIOrder.Low));
        self.configs.Add(UIType.UILevelUpTip, new UIConfig("UIHome", "UILevelUpTip", typeof(UILevelUpTip)));
        self.configs.Add(UIType.UIAttrDetail, new UIConfig("UIHome", "UIAttrDetail", typeof(UIAttrDetail)));
        self.configs.Add(UIType.UIBottomButton, new UIConfig("UIBottomButton", "UIBottomButton", typeof(UIBottomButton)));
        self.configs.Add(UIType.UIEmail, new UIConfig("UIEmail", "UIEmail", typeof(UIEmail)));
        self.configs.Add(UIType.UIEmailDetail, new UIConfig("UIEmail", "UIEmailDetail", typeof(UIEmailDetail)));


        //法宝
        self.configs.Add(UIType.UIFaBaoLianQi, new UIConfig("UIFaBao", "UIFaBaoLianQi", typeof(UIFaBaoLianQi)));
        self.configs.Add(UIType.UIFaBaoDuanZao, new UIConfig("UIFaBao", "UIFaBaoDuanZao", typeof(UIFaBaoDuanZao)));
        self.configs.Add(UIType.UIFaBaoPeiZi, new UIConfig("UIFaBao", "UIFaBaoPeiZi", typeof(UIFaBaoPeiZi)));
        self.configs.Add(UIType.UIItemSelect, new UIConfig("UIFaBao", "UIItemSelect", typeof(UIItemSelect)));
        self.configs.Add(UIType.UIFaBaoQualitySelect, new UIConfig("UIFaBao", "UIFaBaoQualitySelect", typeof(UIFaBaoQualitySelect)));
        self.configs.Add(UIType.UIFaBaoWear, new UIConfig("UIFaBaoWear", "UIFaBaoWear", typeof(UIFaBaoWear), UIOrder.Low));
        self.configs.Add(UIType.UIFaBaoSkill, new UIConfig("UIFaBaoSkill", "UIFaBaoSkill", typeof(UIFaBaoSkill)));

        //背包
        self.configs.Add(UIType.UIBag, new UIConfig("UIBag", "UIBag", typeof(UIBag), UIOrder.Low));
       
        //域外战场
        self.configs.Add(UIType.UIYuWaiChapter, new UIConfig("UIYuWaiChapter", "UIYuWaiChapter", typeof(UIYuWaiChapter)));
        self.configs.Add(UIType.UIYuWaiMap, new UIConfig("UIYuWaiMap", "UIYuWaiMap", typeof(UIYuWaiMap)));
        self.configs.Add(UIType.UIYuWaiBag, new UIConfig("UIYuWaiBag", "UIYuWaiBag", typeof(UIYuWaiBag)));
        //战斗
        self.configs.Add(UIType.UIYWBattle, new UIConfig("UIYuWaiBattle", "UIYuWaiBattle", typeof(UIYWBattle)));
        //章节
        self.configs.Add(UIType.UIChapter, new UIConfig("UIChapter", "UIChapter", typeof(UIChapter)));
        self.configs.Add(UIType.UIXianTu, new UIConfig("UIXianTu", "UIXianTu", typeof(UIXianTu), UIOrder.Low));
        //宠物
        self.configs.Add(UIType.UIPetCapture, new UIConfig("UIPetCapture", "UIPetCapture", typeof(UIPetCapture)));
        self.configs.Add(UIType.UIPetFormation, new UIConfig("UIPetFormation", "UIPetFormation", typeof(UIPetFormation), UIOrder.Low));
        self.configs.Add(UIType.UIPetEvolution, new UIConfig("UIPetEvolution", "UIPetEvolution", typeof(UIPetEvolution)));
        self.configs.Add(UIType.UIPetDetail, new UIConfig("UIPetDetail", "UIPetDetail", typeof(UIPetDetail)));
        self.configs.Add(UIType.UIPetLearnSkill, new UIConfig("UIPetLearnSkill", "UIPetLearnSkill", typeof(UIPetLearnSkill)));
        self.configs.Add(UIType.UIPetLevelUp, new UIConfig("UIPetLevelUp", "UIPetLevelUp", typeof(UIPetLevelUp)));
        //丹药
        self.configs.Add(UIType.UILianDan, new UIConfig("UILianDan", "UILianDan", typeof(UILianDan)));
        self.configs.Add(UIType.UIDanYaoFormula, new UIConfig("UILianDan", "UIDanYaoFormula", typeof(UIDanYaoFormula)));
        self.configs.Add(UIType.UIDanYaoHandBook, new UIConfig("UILianDan", "UIDanYaoHandBook", typeof(UIDanYaoHandBook)));
        //任务
        self.configs.Add(UIType.UITask, new UIConfig("UITask", "UITask", typeof(UITask)));
        //聊天
        self.configs.Add(UIType.UIChat, new UIConfig("UIChat", "UIChat", typeof(UIChat)));
        self.configs.Add(UIType.UIEquipWear, new UIConfig("UIEquipWear", "UIEquipWear", typeof(UIEquipWear), UIOrder.Low));

        self.configs.Add(UIType.UIItemSell, new UIConfig("UIItemSell", "UIItemSell", typeof(UIItemSell)));
        */
    }
}
