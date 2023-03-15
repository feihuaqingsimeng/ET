using ET;
using FairyGUI;
using Frame;
using UnityEngine;

public class FaBaoCursorComponent : UIBase
{
	public GGraph cursor;

	public ListComponent<GGraph> colorList;

	public bool isFront;
	public int index;
	public int[] defaultScores = new int[] { 6, 7, 8, 9, 10, 9, 8, 7, 6 };
}

public class FaBaoCursorComponentAwakeSystem : AwakeSystem<FaBaoCursorComponent, GComponent>
{
	protected override void Awake(FaBaoCursorComponent self, GComponent a)
	{
		self.Awake(a);
		self.cursor = a.GetGraph("cursor");

		var pool = self.AddComponent<ListPoolComponent>();
		self.colorList = pool.Create<GGraph>();
		for(int i = 0; i <= 8; i++)
		{
			self.colorList.Add(a.GetGraph($"color{i}"));

		}
	}
}
public static class FaBaoCursorComponentSystem
{
	public static void RandomPos(this FaBaoCursorComponent self)
	{
		var first = self.colorList[0];
		float ox = first.x;
		var last = self.colorList[self.colorList.Count - 1];
		float x = Random.Range(0, 1000 - (last.x + last.width - ox));
		for (int i = 0; i < self.colorList.Count; i++)
		{
			var g = self.colorList[i];
			g.x = x + g.x-ox;
		}
		self.cursor.x = 0;
		self.isFront = false;
	}
	public static void StartCursor(this FaBaoCursorComponent self, int time)
	{
		self.isFront = !self.isFront;
		if (self.isFront)
			self.cursor.TweenMoveX(1000, time / 1000).SetEase(EaseType.Linear);
		else
			self.cursor.TweenMoveX(0, time / 1000).SetEase(EaseType.Linear);
		
	}
	public static void StopCursor(this FaBaoCursorComponent self)
	{
		GTween.Kill(self.cursor);

		self.index = -1;
		float x = self.cursor.x;
		for (int i = 0; i < self.colorList.Count; i++)
		{
			var g = self.colorList[i];
			if (g.x <= x && g.x + g.width >= x)
			{
				self.index = i;
				return;
			}
		}
		
	}
	public static int GetScore(this FaBaoCursorComponent self)
	{
		if (self.index == -1) return 5;
		return self.defaultScores[self.index];
		
	}
	public static Color GetColor(this FaBaoCursorComponent self,int score)
	{
		for(int i = 0; i < self.defaultScores.Length; i++)
		{
			if(score == self.defaultScores[i])
				return self.colorList[i].color;
		}
		return Color.white;
	}
	public static Color GetColor(this FaBaoCursorComponent self)
	{
		if (self.index == -1) return Color.white;
		return self.colorList[self.index].color;
	}
}