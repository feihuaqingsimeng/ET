
using Frame;
using module.yw.message.vo;
using System;
using UnityEngine;

public struct UIBottomButtonParam : IUIDataParam
{
	public UIBottomButtonType index;
	public bool isOpen;
}
public struct UIFlowTipParam :IUIDataParam
{
	public string content;
}
public struct UIItemSelectParam : IUIDataParam
{
	public ItemInfo itemInfo;
	public bool isAdd;
	public int limit;
	public Action<ItemInfo,int> callback;
}
public struct UIFaBaoQualitySelectParam : IUIDataParam
{
	public Action<int> callback;
}

public struct UIFaBaoDuanZaoParam : IUIDataParam
{
	//背包位置
	public int index;
}
public struct UIFabaoDetailParam : IUIDataParam
{
	public FaBaoInfo info;
	public bool showFuLing;
}

public struct UICommonTipParam : IUIDataParam
{
	public string des;
	public UICommonTip.ButtonType type;
	public Action sureCallback;
}
public struct UICostCommonTipParam : IUIDataParam
{
	public string des;
	public string costDes;
	public Action sureCallback;
}
public struct UIItemDetailParam : IUIDataParam
{
    public UIItemDetail.ItemDetailShowType type;
    public ItemInfo info;
    public Action<ItemInfo> putCallback;

}
public struct UIEquipDetailParam : IUIDataParam
{
    public EquipInfo info;
}
public struct UIItemUseParam : IUIDataParam
{
	public int index;//背包位置
}
public struct UIItemSellParam : IUIDataParam
{
	public RewardInfo info;//背包位置
}
public struct UIYWBattleParam : IUIDataParam
{
    public YwPlayerVo player;//战斗记录
}
public struct UIEmailDetailParam : IUIDataParam
{
    public EmailInfo info;
}
public struct UIPetDetailParam : IUIDataParam
{
    public PetInfo pet;
}
public struct UIPetEvolutionParam : IUIDataParam
{
    public PetInfo pet;
}
public struct UIPetLevelUpParam : IUIDataParam
{
    public PetInfo pet;
}
public struct UIPetLearnSkillParam : IUIDataParam
{
    public PetInfo pet;
}
public struct UISkillTipsParam : IUIDataParam
{
    public int skillId;
    public Vector2 globalPos;
}
public struct UIDanYaoFormulaParam : IUIDataParam
{
    public Action<int> callback;
}


