using ET;
using Frame;

namespace TBS
{
	public class ClientBattle:Entity,IAwake,IDestroy
	{
		public static ClientBattle bat;
		public static ClientBattle Create(Entity parent)
		{
			bat = parent.AddComponent<ClientBattle>();
			return bat;
		}
		public static ClientBattle Get()
		{
			return bat;
		}
	}
	public class ClientBattleAwakeSystem : AwakeSystem<ClientBattle>
	{
		protected override void Awake(ClientBattle self)
		{
			self.AddComponent<BattleActorsComponent>();
			self.AddComponent<BattleEffectShowComponent>();
			self.AddComponent<BattleRuleComponent>();
			self.AddComponent<BattleReportComponent>();
			self.AddComponent<BattleSceneComponent>();
			self.AddComponent<BattleEventComponent>();

		}
	}
    public class ClientBattleDestorySystem : DestroySystem<ClientBattle>
    {
        protected override void Destroy(ClientBattle self)
        {
            //ClientBattle.bat = null;
        }
    }
    public static class ClientBattleSystem
	{
		public static void Start(this ClientBattle self,string cmd)
		{
			self.GetComponent<BattleReportComponent>().ParseReport(cmd);
			self.GetComponent<BattleRuleComponent>().StartFight();
			self.GetComponent<BattleEventComponent>().Publish(new BattleStartEvent());
		}
		public static void Destroy(this ClientBattle self)
		{
			self.Dispose();
		}
	}
}
