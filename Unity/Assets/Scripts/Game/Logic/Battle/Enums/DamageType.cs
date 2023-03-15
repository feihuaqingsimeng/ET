
namespace TBS
{
	public enum DamageType
	{
		none = 0,//不显示

	normal = 1,	//普通

	dodge = 2,	//闪避

	crit = 3,	//暴击

	immunity = 4,	//免疫

	heal = 5,	//治疗

	sp = 6,		//怒气

	shields = 7,//护盾 //跳字吸收

	poison = 9,//中毒

	poisonHero = 10,//玩家中毒

	blood = 11,//流血

	bloodHero = 12,//玩家流血

	crush = 13,//压碎

	crushHero = 14,//玩家压碎


	normalHurt = 15,//对敌人物理伤害

	normalHurtCrit = 16,//对敌人物理伤害暴击

	normalElementCrit = 17,//相克暴击

	normalHurtMCrit = 18,//对敌人法术伤害暴击

	normalCure = 19,//通用治疗

	normalCureCrit = 20,//通用治疗暴击

	heroHurt = 21,//玩家单位受伤

	heroHurtCrit = 22,//玩家单位受伤暴击

	elementHurt = 23,
	}
}
