using ET;
using FairyGUI;
using Frame;
using System.Collections.Generic;
using TBS;
using UnityEngine;

public class EffectShowItemComponent: UIBase
{
	public List<GComponent> damageCacheList = new List<GComponent>();
}
class DamageItemComponentAwakeSystem : AwakeSystem<EffectShowItemComponent, GComponent>
{
	protected override void Awake(EffectShowItemComponent self, GComponent a)
	{
		self.Awake(a);
	}
}

public static class DamageItemComponentSystem
{
	public static GComponent GetDamageItem(this EffectShowItemComponent self)
	{
		GComponent item = null;
		if (self.damageCacheList.Count > 0)
		{
			item = self.damageCacheList[self.damageCacheList.Count - 1];
			self.damageCacheList.RemoveAt(self.damageCacheList.Count - 1);
		}
		else
		{
			var go = UIPackage.CreateObject("UIBattle", "Text_Damage");
			item = go.asCom;
			self.gCom.AddChild(go);
		}
		return item;
	}
	public static async void ShowDamage(this EffectShowItemComponent self, int damage,DamageType damageType,bool isCrit,Vector2 pos)
	{
		var item = self.GetDamageItem();
		var text = item.GetText("text");
		var typeCtrl = item.GetController("damageType");
		Transition anim = item.GetTransition("normal");
		item.SetPivot(0.5f, 0.5f);
		item.pivotAsAnchor = true;
		item.xy = pos;
		if (damageType == DamageType.heal)
			typeCtrl.selectedIndex = 2;
		else if (isCrit)
			typeCtrl.selectedIndex = 1;
		else
			typeCtrl.selectedIndex = 0;
		text.text = (damage > 0 ? "-" : "+") + Mathf.Abs(damage).ToString();
		anim.Play();
		var token = self.GetToken(1);
		var flag = await TimerComponent.Instance.WaitAsync(750,token);
		if (!flag) return;
		self.damageCacheList.Add(item);
	}
	public static void ShowEffect(this EffectShowItemComponent self)
	{

	}
}
