using ET;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using FairyGUI;
using System;
using Frame;

namespace TBS
{
	public class BattleEffectShowComponent:Entity,IAwake,IDestroy
	{
        public Dictionary<long, GGraph> effectGoList = new Dictionary<long, GGraph>();
        public Dictionary<long, GGraph> effectGraphList = new Dictionary<long, GGraph>();
        public Dictionary<string, List<GGraph>> pool = new Dictionary<string, List<GGraph>>();
	}
    public class BattleEffectShowComponentAwakeSystem : AwakeSystem<BattleEffectShowComponent>
    {
        protected override void Awake(BattleEffectShowComponent self)
        {
            self.AddComponent<CancelTokenComponent>();
        }
    }
    public class BattleEffectShowComponentDestroySystem : DestroySystem<BattleEffectShowComponent>
    {
        protected override void Destroy(BattleEffectShowComponent self)
        {
            foreach(var v in self.effectGoList.Values)
            {
                ResourceManager.Ins.ReleaseInstantiate(v.displayObject.gameObject);
                v.displayObject.Dispose();
            }
            foreach (var v in self.pool.Values)
            {
                while (v.Count > 0)
                {
                    var go = v[v.Count - 1];
                    v.RemoveAt(v.Count - 1);
                    ResourceManager.Ins.ReleaseInstantiate(go.displayObject.gameObject);
                    go.displayObject.Dispose();
                }
            }
            self.effectGoList.Clear();
            self.pool.Clear();
        }
    }
    public static class BattleEffectShowComponentSystem
    {
        public static void PlayEffect(this BattleEffectShowComponent self,string name,Vector2 pos ,string action = "animation")
        {
           self.GetPool(name,(gGraph)=> {
               gGraph.xy = pos;
               gGraph.displayObject.gameObject.name = name;
               self.effectGoList.Add(IdGenerater.Instance.GenerateId(), gGraph);
               var go = gGraph.displayObject.gameObject;
               var skeleton = go.GetComponentInChildren<SkeletonGraphic>();
               skeleton.AnimationState.SetAnimation(0, action, false);
               var anim = skeleton.SkeletonData.FindAnimation(action);
               self.PlayFinished(gGraph, anim.Duration);
           });
        }
        private static async void PlayFinished(this BattleEffectShowComponent self,GGraph gGraph, float time)
        {
            var token = self.GetComponent<CancelTokenComponent>().GetOrCreateToken(1);
            var flag = await TimerComponent.Instance.WaitAsync((long)(time * 1000), token);
            if (!flag) return;
            self.ReturnPool(gGraph.displayObject.gameObject.name, gGraph);
        }
        private static void GetPool(this BattleEffectShowComponent self, string name,Action<GGraph> callback)
        {
            if(self.pool.TryGetValue(name,out var list))
            {
                if (list.Count > 0)
                {
                    var go = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                    go.visible = true;
                    callback?.Invoke(go);
                }
            }
            ResourceManager.Ins.InstantiateAsync($"Effect/Skill/{name}.prefab",(obj)=> {
                var bat = ClientBattle.Get();
                if (bat == null) return;
                var graph = new GGraph();
                graph.SetSize(2, 2);
                graph.visible = false;
                graph.visible = true;
                graph.SetNativeObject(new GoWrapper(obj));
                obj.transform.localPosition = Vector3.zero;
                ClientBattle.Get().GetComponent<BattleSceneComponent>().AddGComChild(graph);
                callback?.Invoke(graph);
            });
        }
        private static void ReturnPool(this BattleEffectShowComponent self,string name,GGraph go)
        {
            if(!self.pool.TryGetValue(name,out var list))
            {
                list = new List<GGraph>();
                self.pool.Add(name, list);
            }
            go.visible = false;
            list.Add(go);
        }
    }
}
