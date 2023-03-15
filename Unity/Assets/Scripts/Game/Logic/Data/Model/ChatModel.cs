using ET;
using System.Collections.Generic;

public enum ChatType
{
    None = 0,
    Current = 1,    //当前服
    NearBy = 2,     //附近
    Team = 3,
    Clan = 4,       //家族
    World = 5,
    System = 6,
    Loop = 7,       //
    Friend = 8,     //好友
    Rumor = 11,     //传说
    Horn = 12,      //喇叭
}
public class ChatInfo
{
    public int id;
    public int chatType;
    public int serverId;
    public long senderId;       //发送者id
    public string senderName;
    public int senderLevel;
    public long receivedId;     //接收者id
    public long receivedName;  
    
    public string content;

    public long sendTime;
    
}
public class ChatModel:Entity,IAwake
{
    public Dictionary<int, List<ChatInfo>> chatDic = new Dictionary<int, List<ChatInfo>>();
}

public static class ChatModelSystem
{
    public static void AddChatMsg(this ChatModel self)
    {

    }
    public static List<ChatInfo> GetChatList(this ChatModel self,ChatType type)
    {
        self.chatDic.TryGetValue((int)type, out var list);
        return list;
    }
}

