using ET;
namespace TBS
{
	public class CommandInit:CommandBase
	{
		public int fighterId;
		public int modelId;
		public ActorType actorType;
		public ActorCampType camp;
		public int hpMax;
		public int hp;
		public int posIndex;
		public string playerName;
		public long playerId;
		public int level;

		public override bool Execute()
		{
			var flag = base.Execute();
			if (!flag) return false;
			var bat = ClientBattle.Get();
			var sys = bat.GetComponent<BattleActorsComponent>();
			var actor = sys.CreateActor(fighterId, modelId, actorType, camp);
			actor.GetComponent<ActorAttrComponent>().SetHp(hp);
			actor.GetComponent<ActorAttrComponent>().SetHpMax(hpMax);
			actor.level = level;
			
			actor.fighterId = fighterId;
			actor.posIndex = posIndex;
            actor.pos = bat.GetComponent<BattleSceneComponent>().GetPos(camp,posIndex);

            if (actorType == ActorType.PLAYER)
			{
				actor.name = playerName;
                actor.playerId = playerId;
			}
			else
			{
				if(actorType == ActorType.PET)
				{
					var cfg = ConfigSystem.Ins.GetLCPetAttribute(modelId);
					if (cfg != null)
					{
						actor.name = cfg.Get<string>("Name");
						return true;
					}
				}else if(actorType == ActorType.MONSTER)
                {
                    var monsterCfg = ConfigSystem.Ins.GetMonster(modelId);
                    actor.name = monsterCfg == null ? "Monster" : monsterCfg.Get<string>("Name");
                }else
				    actor.name = actorType.ToString();
			}
			return true;
		}

		public override string ToString()
		{
			var str = base.ToString();
			return $"[初始化]  {str},fighterId:{fighterId},modelId:{modelId},hp:{hp},actorType:{actorType},camp:{camp},posIndex:{posIndex},playerName:{playerName},level:{level}";

		}
	}
}
