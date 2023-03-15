using ET;
using FairyGUI;
using System.Collections.Generic;
using UnityEngine;

namespace TBS
{
	public class BattleSceneComponent:Entity,IAwake
	{
        public GComponent root;
        public List<Vector2> bluePosList = new List<Vector2>();
        public List<Vector2> redPosList = new List<Vector2>();

    }
    public static class BattleSceneComponentSystem
    {
        public static void Init(this BattleSceneComponent self,GComponent root, List<Vector2> bluePosList, List<Vector2> redPosList)
        {
            self.root = root;
            foreach(var v in bluePosList)
            {
                self.bluePosList.Add(v);
            }
            foreach (var v in redPosList)
            {
                self.redPosList.Add(v);
            }
        }
        public static Vector2 GetPos(this BattleSceneComponent self,ActorCampType camp,int index)
        {
            if (camp == ActorCampType.Blue)
                return self.bluePosList[index];
            else
                return self.redPosList[index];
        }
        public static void AddGComChild(this BattleSceneComponent self, GObject go)
        {
            self.root.AddChild(go);
        }
    }
}
