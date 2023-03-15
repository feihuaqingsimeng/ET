using ET;
using Frame;
namespace TBS
{
	public class SkillNodeBase:ManagedEntity
	{
		public Skill skill;

		public virtual void OnSkillStart() { }
		public virtual void OnUpdate(float dt) { }
		public virtual void OnSkillEnd() { }

	}
}
