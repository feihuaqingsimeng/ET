public enum RewardType
{
    NONE = 0,
    ITEM = 1,
    FABAO = 2,
    PET = 3,
    EQUIP = 4,
}
public abstract class RewardInfo
{
    public long uid;
    public virtual RewardType rewardType { get; }
    public int modelId { get { return _modelId; } set { _modelId = value; InitModelId(); } }
    private int _modelId;
    public int num;
    public int type;
    public int quality;
    public string name;
    public string des;
    public virtual string ColorName {
        get {
            var str = ConfigSystem.Ins.GetItemQualityColorStr(quality);
            var cname = $"[color=#{str}]{name}[/color]";
            return cname;
        }
    }
    public abstract void InitModelId();
}
