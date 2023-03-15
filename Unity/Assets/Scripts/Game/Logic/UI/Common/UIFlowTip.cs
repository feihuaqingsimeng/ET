using ET;
using FairyGUI;
using Frame;
using System.Collections.Generic;

public class UIFlowTip : UIBase
{
	public List<GComponent> itemList = new List<GComponent>();

	public int index;
    public Queue<string> infoQueue = new Queue<string>();
    public bool isStart;
}
public class UIFlowTipAwakeSystem : AwakeSystem<UIFlowTip, GComponent>
{
    protected override void Awake(UIFlowTip self, GComponent a)
	{
		self.Awake(a);
		self.index = 0;
		var template = a.GetCom("item");
		self.itemList.Add(a.GetCom("item"));
		var package = UIPackage.GetByName("Common");

		for (int i = 0; i < 4; i++)
		{
			var item = package.CreateObject("flowTipItem").asCom;
			self.gCom.AddChild(item);
			item.SetXY(template.x, template.y);
			item.visible = false;
			self.itemList.Add(item);

		}

	}
}
public class UIFlowTipChangeSystem : ChangeSystem<UIFlowTip, IUIDataParam>
{
	protected override void Change(UIFlowTip self, IUIDataParam a)
	{
		var param = (UIFlowTipParam)a;
        self.infoQueue.Enqueue(param.content);
        if (!self.isStart)
            self.Flow();
	}
}
public static class UIFlowTipSystem
{
    public static async void Flow(this UIFlowTip self)
    {
        self.isStart = true;
        while (self.infoQueue.Count > 0)
        {
            var info = self.infoQueue.Dequeue();
            GComponent go = self.itemList[self.index];
            go.visible = true;
            self.gCom.SetChildIndex(go, self.gCom.numChildren);
            go.GetText("text").text = info;
            var anim = go.GetTransition("t0");
            anim.Play();
            self.index = (self.index + 1) % self.itemList.Count;
            await TimerComponent.Instance.WaitAsync(300);
            
        }
        self.isStart = false;
    }
}