using ET;

namespace TBS
{
	public enum BattleActionType
	{
		/**
		* 无
		*/
			NONE = 0,
		/**
		 * 初始化
		 */
		INIT = 1,
		/**
		 * 攻击 (释放技能)
		 */
		ATTACK = 2,

		/**
		 * 目标受击
		 */
		HIT = 4,
		/**
		 * 5 伤害
		 */
		DAMAGE = 5,

		/**
		 * 7 buff
		 */
		BUFF = 7,
		/**
		 * 9 死亡
		 */
		DIE = 9,
		/**
		 * 回合节点(谁出手)
		 */
		ROUND_NODE = 101,
	}
}
