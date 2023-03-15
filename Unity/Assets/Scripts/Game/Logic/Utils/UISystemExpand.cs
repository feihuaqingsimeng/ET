using ET;
using Frame;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class UISystemExpand
{
    public static void ShowFlowTipView(this UISystem uiSys,string content)
    {
        uiSys.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = content });
    }
    public static void ShowGainReward(this UISystem uiSys,RewardType type,int modelId,int num)
    {
        uiSys.ShowFlowTipView(RewardInfoFactory.GetGainRewardTipStr(type, modelId, num));
    }
    public static void ShowGainReward(this UISystem uiSys, List<RewardInfo> rewards)
    {
        for (int i = 0; i < rewards.Count; i++)
        {
            var info = rewards[i];
            uiSys.ShowGainReward(info.rewardType, info.modelId, info.num);
        }
    }
    public static void ShowFlowTipView(this UISystem uiSys, List<ItemInfo> rewards)
    {
        for (int i = 0; i < rewards.Count; i++)
        {
            var info = rewards[i];
            uiSys.ShowFlowTipView( string.Format(ConfigSystem.Ins.GetLanguage("gain_item_reward_tip"),ItemUtil.GetItemName(info.modelId),info.num));
        }
    }
    public static void ShowCommonTipView(this UISystem uiSys, string des, Action sureCallback, UICommonTip.ButtonType type = UICommonTip.ButtonType.Double)
    {
        var data = new UICommonTipParam() {
            des = des,
            sureCallback = sureCallback,
            type = type,
        };
        uiSys.Show(UIType.UICommonTip, data);
    }
    public static void ShowSkillTips(this UISystem uiSys,int skillId,Vector2 pos)
    {
        uiSys.Show(UIType.UISkillTips, new UISkillTipsParam() { skillId = skillId, globalPos = pos });
    }
}
