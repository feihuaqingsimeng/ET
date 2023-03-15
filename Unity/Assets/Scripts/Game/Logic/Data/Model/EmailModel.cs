using ET;
using module.mail.message.vo;
using System.Collections.Generic;

//邮件状态
public enum MailStatus
{
    NONE = 0,
    CREATE = 1,
    READ = 2,
    RECV = 3,
    DELETE = 4,
}
public class EmailInfo
{
    // 唯一id
    public long id;
    // 邮件标题
    public string title;
    // 邮件内容
    public string content;
    // 邮件状态
    public MailStatus status;
    // 发送时间,毫秒
    public long createTime;
    // 过期时间,毫秒
    public long expireTimeMillis;
    // 附件奖励
    public List<RewardInfo> rewards = new List<RewardInfo>();
}
public class EmailModel:Entity,IAwake
{
    public Dictionary<long, EmailInfo> emailDic = new Dictionary<long, EmailInfo>();

}
public static class EmailModelSystem
{
    public static void AddEmail(this EmailModel self,MailVO vo)
    {
        if (self.emailDic.TryGetValue(vo.id, out var v))
        {
            UpdateEmailInfo(v, vo);
        }
        else
        {
            EmailInfo info = new EmailInfo();
            UpdateEmailInfo(info, vo);
            self.emailDic.Add(info.id, info);
        }
    }
    public static void UpdateEmailInfo(EmailInfo info, MailVO vo)
    {
        info.id = vo.id;
        info.title = vo.title;
        info.content = vo.content;
        info.status = (MailStatus)((int)vo.status);
        info.createTime = vo.createTime;
        info.expireTimeMillis = vo.expireTimeMillis;
        info.rewards.Clear();
        foreach(var v in vo.attachs)
        {
            var r = RewardInfoFactory.Create((RewardType)((int)v.type));
            r.modelId = v.modelId;
            r.num = v.num;
            info.rewards.Add(r);
        }
    }
    public static void DelEmail(this EmailModel self,long id)
    {
        self.emailDic.Remove(id);
    }
}
