using ET;

[ComponentOf(typeof(Scene))]
public class DataSystem:Entity,IAwake
{
	public static DataSystem Ins { get; set; }

    public PlayerModel PlayerModel;
    public ChapterModel ChapterModel;
    public ItemModel ItemModel;
    public AccountModel AccountModel;
    public YuWaiModel YuWaiModel;
    public PetModel PetModel;
    public EmailModel EmailModel;
    public HandBookModel HandBookModel;
    public TaskModel TaskModel;
    public ChatModel ChatModel;
    public EquipModel EquipModel;
}
public class DataSystemAwakeSystem : AwakeSystem<DataSystem>
{
    protected override void Awake(DataSystem self)
    {
        DataSystem.Ins = self;
        self.ChapterModel = self.AddComponent<ChapterModel>();
        self.PlayerModel = self.AddComponent<PlayerModel>();
        self.ItemModel = self.AddComponent<ItemModel>();
        self.AccountModel = self.AddComponent<AccountModel>();
        self.YuWaiModel = self.AddComponent<YuWaiModel>();
        self.PetModel = self.AddComponent<PetModel>();
        self.EmailModel = self.AddComponent<EmailModel>();
        self.HandBookModel = self.AddComponent<HandBookModel>();
        self.TaskModel = self.AddComponent<TaskModel>();
        self.ChatModel = self.AddComponent<ChatModel>();
        self.EquipModel = self.AddComponent<EquipModel>();
    }
}

