

namespace TBS
{
	public enum SkillType
	{
		Normal = 1,		//普攻
		Skill = 2,		//技能
		Passive = 3,	//被动
	}
    //特效类型
    public enum SkillPosType
    {
        Caster, //施法者位置
        Target, //目标位置
    }
    public enum SkillReleaseType
    {
        None = 0,
        Melee = 1,
        Remote = 2,
    }
    //特效类型
    public enum SkillEffectType
    {
        Single, //单体特效
        Scene, //场景特效,固定4号位播放
    }
}
