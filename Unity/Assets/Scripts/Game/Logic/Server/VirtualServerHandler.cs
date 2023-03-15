using ET;
using module.login.message;
using module.pet.message;
using module.player.message;
using System.Collections.Generic;

class VirtualServerHandler
{
    public static bool Receive(IMessage req)
    {
        if (!ConstValue.localServer) return false;
        var type = req.GetType();
        if (type == typeof(LoginReq))
        {
            LoginResp resp = new LoginResp();
            resp.playerId = 1;
            resp.createTime = (int)TimeHelper.ClientNowSeconds();
            VirtualServer.Ins.Send(resp);
            SendPlayerInfo();
            return true;
        }
        if ( type == typeof(PetCatchReq))
        {
            var r = req as PetCatchReq;
            PetCatchResp resp = new PetCatchResp();
            for(int i = 0; i < ConstValue.PET_CAPTURE_COUNT; i++)
            {
                if(i == 0)
                    resp.results.Add(1);
                else if(i == 1)
                    resp.results.Add(2);
                else
                    resp.results.Add(4);
            }
            VirtualServer.Ins.Send(resp);
            return true;
        }else if(type == typeof(PetChuZhanReq))
        {
            var r = req as PetChuZhanReq;
            var info = DataSystem.Ins.PetModel.GetPet(r.petUuid);
            var sInfo = new module.pet.message.vo.PetInfo();
            sInfo.uuid = info.uid;
            sInfo.petId = info.modelId;
            sInfo.level = info.level;
            sInfo.exp = info.exp;
            sInfo.zhanIndex = r.zhanIndex;
            sInfo.skills.AddRange(info.skills);
            sInfo.attrs = new Dictionary<int, long>();
            foreach(var v in info.attrs)
            {
                sInfo.attrs.Add(v.Key, v.Value);
            }
            var resp = new PetAddOrUpdateResp() { petInfo = sInfo };
            VirtualServer.Ins.Send(resp);
            return true;
        }
        return false;
    }
    public static void SendPlayerInfo()
    {
        PlayerInfoResp resp = new PlayerInfoResp();
        resp.playerId = 1;
        resp.name = DataSystem.Ins.AccountModel.accountName;
        resp.level = 10;
        resp.exp = 0;
        resp.attributes = new Dictionary<int, long>();
        for(int i = 0; i < 10; i++)
        {
            resp.attributes[i] = i;
        }
        VirtualServer.Ins.Send(resp);
    }
}
