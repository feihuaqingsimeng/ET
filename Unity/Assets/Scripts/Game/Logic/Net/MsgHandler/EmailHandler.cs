using ET;
using GameEvent;
using Frame;
using module.mail.message;
using SMailStatus = module.mail.message.MailStatus;
//上线初始化/新增邮件,删除,更新邮件都是这一个消息
class MailRespHandler : AMHandler<MailResp>
{
    protected override async ETTask Run(Session session, MailResp message)
    {
        var model = DataSystem.Ins.EmailModel;
        foreach(var v in message.mailList)
        {
            if (v.status == SMailStatus.DELETE)
                model.DelEmail(v.id);
            else
                model.AddEmail(v);
        }
        Frame.UIEventSystem.Ins.Publish(new EmailUpdateEvent());
        await ETTask.CompletedTask;
    }

}
