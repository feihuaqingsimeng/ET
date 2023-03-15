using ET;
using FairyGUI;
using GameEvent;
using UnityEngine;
using module.yw.message;
using System;
using Frame;

public enum YWMapState
{
	Unexplored,// 未探索
	Explored ,//探索过
}
public class YWMapComponent: UIBase
{
	public GList mapItemList;
	public GGraph player;

	public bool isTween;
	public YWMapItemComponent[] itemsArr = new YWMapItemComponent[ConstValue.YWRow*ConstValue.YWCol];

}
public class YWMapComponentAwakeSystem : AwakeSystem<YWMapComponent, GComponent>
{
	protected override void Awake(YWMapComponent self, GComponent a)
	{
		self.Awake(a);
		self.mapItemList = a.GetList("mapList");
		self.player = a.GetGraph("player");

		self.mapItemList.itemRenderer = self.ItemRender;

		self.RegisterEvent<YWPosInfoPathChangeEvent>(self.Event_YWPosInfoPathChangeEvent);
	}
}
public static class YWMapComponentSystem
{
	public static void Refresh(this YWMapComponent self)
	{
		self.mapItemList.numItems = ConstValue.YWRow * ConstValue.YWCol;
		var data = DataSystem.Ins.YuWaiModel;
		
        self.player.xy = self.GetPos(data.ywCurRow, data.ywCurCol);
		self.RefreshExplorePath();
	}
	public static void RefreshExplorePath(this YWMapComponent self)
	{
		var data = DataSystem.Ins.YuWaiModel;
		foreach (var v in data.ywPosInfoDic)
		{
			self.itemsArr[GetIndex(v.Value.y, v.Value.x)].SetState(YWMapState.Explored);
		}
	}
    private static Vector2 GetPos(this YWMapComponent self, int row,int col)
    {
        float half = ConstValue.YWWidth / 2.0f;
        float x = col * ConstValue.YWWidth + half + col * self.mapItemList.columnGap;
        float y = row * ConstValue.YWWidth + half + row * self.mapItemList.lineGap;
        return new Vector2(x, y);
    }
	public static async void TweenMove(this YWMapComponent self, int row, int col)
	{
		int time = 300;
		self.isTween = true;
		self.player.TweenMove(self.GetPos(row,col), time/1000.0f);
		//var flag = await TimerComponent.Instance.WaitAsync(time / 2, token.GetOrCreateToken(1));
		//if (!flag) return;
		
		var flag = await TimerComponent.Instance.WaitAsync(time, self.GetToken(1));
		if (!flag) return;
		self.isTween = false;
        var model = DataSystem.Ins.YuWaiModel;
        model.ywCurRow = row;
        model.ywCurCol = col;
		self.itemsArr[GetIndex(row, col)].SetState(YWMapState.Explored);
		((UIYuWaiMap)self.Parent).MapMoveFinished();
	}
	public static int GetIndex(int row,int col)
	{
		return row * ConstValue.YWCol + col;
	}
	public static void ItemRender(this YWMapComponent self,int index,GObject go)
	{
		var instId = go.displayObject.gameObject.GetInstanceID();
		var item = self.GetChild<YWMapItemComponent>(instId);
		if (item == null)
		{
			item = self.AddChildWithId<YWMapItemComponent, GComponent>(instId, go.asCom);
			item.SetClick(self.ClickMapItem);
			self.itemsArr[index] = item;
		}
		item.Refresh(index);
		item.SetState(YWMapState.Unexplored);

	}
	public static void ClickMapItem(this YWMapComponent self,YWMapItemComponent item)
	{
		if (self.isTween) return;
		int row = item.index / ConstValue.YWCol;
		int col = item.index % ConstValue.YWCol;
		var data = DataSystem.Ins.YuWaiModel;
		if ((data.ywCurRow == row && Mathf.Abs(data.ywCurCol - col) == 1) || (data.ywCurCol == col && Mathf.Abs(data.ywCurRow - row) == 1))
		{
			self.MoveReq(row,col);
			
		}
		else
		{
			UISystem.Ins.Show(UIType.UIFlowTip, new UIFlowTipParam() { content = ConfigSystem.Ins.GetErrorCode(34) });
		}
	}
	public static async void MoveReq(this YWMapComponent self,int row,int col)
	{
		var data = DataSystem.Ins.YuWaiModel;
		var req = new YwMoveReq() { lowerMapId = data.ywMapId, x = col, y = row };
		var resp = (YwMapPosInfoResp)await NetSystem.Call(req, typeof(YwMapPosInfoResp), self.GetToken(1));
		if (resp == null) return;
		self.TweenMove(resp.y, resp.x);
	}

	public static void Event_YWPosInfoPathChangeEvent(this YWMapComponent self, YWPosInfoPathChangeEvent e)
	{
		self.RefreshExplorePath();
	}
}