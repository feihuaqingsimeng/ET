using ET;
public static class ConstValue
{
    [StaticField]
    public static bool localServer = false;
    public const bool enableSessionLog = true;
//#if UNITY_ANDROID && !UNITY_EDITOR
//    public const string LoginAddress = "10.0.2.2:7501";
//#else
    public const string LoginAddress = "192.168.71.51:7501";
//#endif

    public const string BattleURL = "http://192.168.2.133:8080/battles/";

	public static string GetBattleReportUrl(string reportPath)
	{
		var url = $"{BattleURL}{reportPath}.bt";
		return url;
	}
    [StaticField]
	public static AcceptAllCertificate certificateHandler = new AcceptAllCertificate();

	public const int LobbyZone = 1;
	public const int ResolusionX = 1080;
	public const int ResolusionY = 1920;
    public const bool debugAnalysis = true;//打印性能分析

    //-------------法宝
    public const int FABAO_EQUIP_COUNT = 6;//法宝装备数量
    public const int FABAO_EQUIP_TYPE_LIMIT = 2;//法宝同类型装备数量
    //-------------域外战场
    public const int YWRow = 10;
	public const int YWCol = 10;
	public const int YWWidth = 90;
	//--------------战斗
	public const float SkillAttackTime = 0.34f;//攻击来回，比动画时间多一点
	public const float SkillMeleeMoveTime = 0.4f;
    public const int PLAYER_SITE = 4;//英雄站位
    //--------------宠物
    public const int PET_CAPTURE_COUNT = 12;//抓捕最大数量
    public const int PET_QUALITY_MAX = 5;//最大品质
    public const int PET_BATTLE_MAX = 5;//上阵个数
    [StaticField]
    public static int PET_SKILL_MAX = 0;

    public static void Init()
    {
        //var NumberSkills = ConfigSystem.Ins.GetGlobal<int[][]>("NumberSkills");
        //PET_SKILL_MAX = NumberSkills[NumberSkills.Length - 1][1];
    }
}
