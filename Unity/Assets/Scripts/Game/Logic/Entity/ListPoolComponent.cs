using ET;
using System.Collections.Generic;

public class ListPoolComponent:Entity,IAwake,IDestroy
{
	public List<System.IDisposable> list = new List<System.IDisposable>();
}
[ObjectSystem]
public class ListPoolComponentDestroySystem : DestroySystem<ListPoolComponent>
{
	protected override void Destroy(ListPoolComponent self)
	{
		for(int i = 0; i < self.list.Count; i++)
		{
			self.list[i].Dispose();
		}
	}
}
public static class ListPoolComponentSystem
{
	public static ListComponent<T> Create<T>(this ListPoolComponent self)
	{
		var list = ListComponent<T>.Create();
		self.list.Add(list);
		return list;
	}
}