

namespace TBS
{
	public struct PlayerHpChangedEvent
	{
		public Actor actor;
	}
	public struct DamageShowEvent
	{
		public Actor actor;
		public int damage;
		public DamageType damageType;
		public bool isCrit;
	}
	public struct ActorDeadEvent
	{
		public Actor actor;
	}
	//---------------skill
	public struct SkillHitBeforeEvent
	{
		public DamageData data;
	}
	public struct SkillHitAfterEvent//技能命中后
	{
		public DamageData data;
	}
	public struct SkillActionFrontMoveEvent
	{
		public Skill skill;
		public Actor target;
		public float time;
	}//动作移动到目标身边
	public struct SkillActionBackMoveEvent
	{
		public Skill skill;
		public float time;
	}//动作后撤原点

	public struct SkillActionAttackPlayEvent//动作攻击播放
	{
		public Skill skill;
	}
	public struct SkillActionHitPlayEvent//动作受击播放
	{
		public Actor target;
	}
	public struct SkillStartEvent
	{
		public Skill skill;
	}
	public struct SkillEndEvent
	{
		public Skill skill;
	}
	public struct BattleCmdParseFinished { }
	public struct BattleStartEvent { }
	public struct BattleTurnStartEvent { public int turn; }
    public struct BattleFinishedEvent { public bool isWin; };
	//-----------buff
	public struct BuffChangeEvent
	{
		public Buffer buff;
	}
	public struct BuffDestroyEvent
	{
		public Buffer buff;
	}
}
