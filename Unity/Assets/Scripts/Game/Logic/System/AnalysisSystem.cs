using ET;
using System;
using System.Collections.Generic;

namespace Frame
{
    [ComponentOf(typeof(Scene))]
    public class AnalysisSystem:Entity,IAwake
    {
        private Dictionary<string, long> timeRecords = new();
        [StaticField]
        public static AnalysisSystem Ins;


        public void StartRecordTime(string name)
        {
            timeRecords[name] = TimeHelper.ClientNow();
        }
        public void EndRecordTime(string name)
        {
            if(timeRecords.TryGetValue(name,out long time))
            {
                long diff = TimeHelper.ClientNow() - time;
                UnityEngine.Debug.LogWarning($"[Analysis] record time:{name}->{diff}ms");
                timeRecords.Remove(name);
            }
        }

    }
    [FriendOf(typeof(AnalysisSystem))]
    public class AnalysisSystemAwakeSystem : AwakeSystem<AnalysisSystem>
    {
        protected override void Awake(AnalysisSystem self)
        {
            AnalysisSystem.Ins = self;
        }
    }

}
