using System.Collections.Generic;
using SFaBaoInfo = module.fabao.message.vo.FaBaoInfo;
//法宝背包类型
public enum FaBaoPackType
{
    NONE = 0,
    //仓库
    STORE = 1,
    //背包
    PACK = 2,
    //装备栏
    EQUIP = 3,
}
public class FaBaoInfo : RewardInfo
{
    public override RewardType rewardType => RewardType.FABAO;
    // 法宝阶数,=0表示是个胚子
    public int stage;
    // 基础属性 key value 属性值
    public Dictionary<int, long> attri = new Dictionary<int, long>();
    // 附加属性 key value 属性值
    public Dictionary<int, long> attri_Spe = new Dictionary<int, long>();
    // 评分阶段的得分情况,可能为null--胚子从来没开始评分,或当法宝打造完成,已经不是胚子,这个就一定是null
    public List<int> gotScores = new List<int>();
    // 技能
    public int skillId;
    // 打造时间戳,秒
    public long createTime;
    public FaBaoPackType packType;

    public override void InitModelId()
    {
        var cfg = ConfigSystem.Ins.GetFBMagicWeaponConfig(modelId);
        if (cfg == null) return;
        type = cfg.Get("FBType");
        quality = cfg.Get("FBGrade");
        name = cfg.Get<string>("FBName");
        des = cfg.Get<string>("FBDescribe");
    }

}
public static class FaBaoInfoSystem
{
    public static void UpdateByDB(this FaBaoInfo self, SFaBaoInfo db, FaBaoPackType packType)
    {
        self.uid = db.index;
        self.modelId = db.faBaoId;
        self.stage = db.stage;
        self.attri = db.attri;
        self.attri_Spe = db.attri_Spe;
        self.skillId = db.skillId;
        self.createTime = db.createTime;
        self.packType = packType;
    }
}
