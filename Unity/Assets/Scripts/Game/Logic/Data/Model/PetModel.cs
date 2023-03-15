using ET;
using System.Collections.Generic;
using UnityEngine;
public class PetCaptureInfo
{
    public int lcArrestProbabilityId;
    public long startTime;
    public long endTime;
    public int foodId;
    public int[] msg = new int[2];
}

public class PetModel : Entity, IAwake
{
    public List<PetCaptureInfo> captureList = new List<PetCaptureInfo>();//抓捕结果
    public List<string> captureMsgList = new List<string>();

    public Dictionary<long, PetInfo> pets = new Dictionary<long, PetInfo>();
    public Dictionary<int, long> battlePets = new Dictionary<int, long>();
}

public static class PetModelSystem
{
    public static void AddPet(this PetModel self,PetInfo info)
    {
        self.pets.Add(info.uid, info);
    }
    public static PetInfo GetPet(this PetModel self,long uid)
    {
        self.pets.TryGetValue(uid, out var v);
        return v;
    }
    
    public static void RemovePet(this PetModel self, long uid)
    {
        self.pets.Remove(uid);
        foreach (var v in self.battlePets)
        {
            if (v.Value == uid)
            {
                self.battlePets.Remove(v.Key);
                return;
            }
        }
    }
    public static void CaptureClear(this PetModel self)
    {
        self.captureList.Clear();
        self.captureMsgList.Clear();
    }
    public static bool CheckCaptureFinish(this PetModel self)
    {
        var time = TimeHelper.ServerNow();
        for (int i = 0; i < self.captureList.Count; i++)
        {
            if (time < self.captureList[i].endTime)
                return false;
        }
        return true;
    }
    public static bool IsInBattle(this PetModel self,long uid)
    {
        var pet = self.GetPet(uid);
        if (pet == null) return false;
        return pet.battleIndex > 0;
    }
    public static PetInfo GetBattlePet(this PetModel self,int slot)
    {
        self.battlePets.TryGetValue(slot, out var id);
        self.pets.TryGetValue(id, out var info);
        return info;
    }
    public static int GetEmptyBattleSlot(this PetModel self)
    {
        for(int i = 1; i < ConstValue.PET_BATTLE_MAX; i++)
        {
            if (!self.battlePets.ContainsKey(i))
                return i;
        }
        return -1;
    }
    public static void FinishedCapture(this PetModel self,int index)
    {
        if (index < 0 || index >= self.captureList.Count)
            return;
        self.captureList[index].endTime = 0;
    }
    public static PetCaptureInfo GetCpatureInfo(this PetModel self,int index)
    {
        if (index < 0 || index >= self.captureList.Count)
            return null;
        return self.captureList[index];
    }
    public static void InitCaptureList(this PetModel self, List<int> list)
    {
        self.CaptureClear();
        var now = TimeHelper.ServerNow();
        for (int i = 0; i < list.Count; i++)
        {
            var data = new PetCaptureInfo
            {
                lcArrestProbabilityId = list[i],
                startTime = now,
                endTime = now + ConfigSystem.Ins.GetGlobal("LingGuoArrestTime"),
            };
            self.captureList.Add(data);
        }
    }
    public static void CreateCpatureMsg(this PetModel self)
    {
        var list = new List<int>();
        for(int i = 0;i< self.captureList.Count;i++)
        {
            var info = self.captureList[i];
            if (info.endTime == 0) continue;
            if (info.msg[0] == 0 || info.msg[1] == 0)
                list.Add(i);
        }
        if (list.Count == 0) return;
        var r = Random.Range(0, list.Count);
        var data = self.captureList[list[r]];
        var cfg = ConfigSystem.Ins.GetLCArrestdescribe(list[r] + 1);
        if (cfg == null) return;
        int[] param;
        if(data.msg[0] == 0)
        {
            param = cfg.Get<int[]>("Narrator1");
            r = Random.Range(0, param.Length);
            data.msg[0] = param[r];
            self.captureMsgList.Add(ConfigSystem.Ins.GetLanguage(param[r].ToString()));
        }
        else
        {
            param = cfg.Get<int[]>("Narrator2");
            r = Random.Range(0, param.Length);
            data.msg[1] = param[r];
            self.captureMsgList.Add(ConfigSystem.Ins.GetLanguage(param[r].ToString()));
        }
    }
}


