using ET;
using FairyGUI;
using Frame;
using System.Collections.Generic;

public class ResourceMultiItemComponent : UIBase
{
	public List<ResourceItemComponent> resList = new List<ResourceItemComponent>();
	public int index;
}
public class ResourceMultiItemComponentAwakeSystem : AwakeSystem<ResourceMultiItemComponent, GComponent>
{
	protected override void Awake(ResourceMultiItemComponent self, GComponent a)
	{
		self.Awake(a);
		self.resList.Add(self.AddChild<ResourceItemComponent,GComponent>(a.GetCom("res1")));
		self.resList.Add(self.AddChild<ResourceItemComponent, GComponent>(a.GetCom("res2")));
		self.index = 0;
	}
}
public static class ResourceMultiItemComponentSystem
{
	public static void Reset(this ResourceMultiItemComponent self)
	{
		self.index = 0;
	}
	public static void Add(this ResourceMultiItemComponent self, ResourceType type, bool isShowAdd = true)
	{
		self.Change(self.index++, type, isShowAdd);
	}
	public static void Change(this ResourceMultiItemComponent self,int index, ResourceType type, bool isShowAdd = true)
	{
		if (index >= self.resList.Count)
		{
			Log.Error($"数量超出上限 {index}/{self.resList.Count}");
			return;
		}
		self.resList[index].Init(type, isShowAdd);
	}
}
