using UnityEngine;
using System.Collections.Generic;
using FairyGUI;
using System;

public class PanelManager
{
	enum ViewState
	{
		loading,
		opened,
		closing,
		close,
	}
	public static PanelManager Ins { get; private set; }
	private List<ViewStack> viewStackList = new List<ViewStack>();
	public Dictionary<string, GComponent> UIs = new Dictionary<string, GComponent>();
	public Dictionary<string, int> uiLayerIdx = new Dictionary<string, int>();
	public Dictionary<string, int> uiLayerOpen = new Dictionary<string, int>();

	public bool isNeedFit
	{
		get
		{
			float height = 0;
#if UNITY_IOS || UNITY_ANDROID
            height = Screen.height - Screen.safeArea.height; //  获取刘海高度
            Debug.Log("fit height == " + height);
#endif
			if (height > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
	private void Awake()
	{
		uiLayerIdx.Add("Mid", 100);
		Ins = this;
	}
	public void ShowUI(string package, string name)
	{
		try
		{
			if (name == "") return;
			var stack = GetStack(name);
			if (stack != null && stack.state == ViewState.loading)
			{
				return;
			}
			var layer = "Mid";
			if (stack == null)
			{
				int uiIdx = uiLayerIdx[layer];
				uiLayerIdx[layer] += 1;
				uiLayerOpen[name] = 1;
				stack = new ViewStack();
				stack.Load(name, uiIdx);
				AddStack(stack);
			}
			if (UIs.ContainsKey(name))
			{
				int order = uiLayerIdx[layer];
				if (uiLayerOpen[name] != 1)
				{
					uiLayerIdx[layer] += 1;
				}
				uiLayerOpen[name] = 1;
				AddStack(stack);
				stack.Create(order, true);
				stack.SetState(ViewState.opened);
				stack.Enable();
				_OpenUI(UIs[name], false);
			}
			else
			{
				CreatePanel(package,name);
			}
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
		}
	}
	private void _OpenUI(GComponent obj, bool anim = true)
	{
		if (anim)
		{
			FairyGUI.Transition openTrans = obj.GetTransition("Effect_JumpOut");
			if (openTrans != null)
			{
				openTrans.invalidateBatchingEveryFrame = true;

				openTrans.Play(() =>
				{
					obj.fairyBatching = true;
				});
			}
			else
			{
				obj.fairyBatching = true;
			}
		}
	}
	public void CloseUI(string package, string name, bool isDestroy = true, Action onFinish = null)
	{
		var stack = GetStack(name);
		if (stack == null) return;
		if (stack.state == ViewState.loading)
		{
			PackageLoader.Ins.UnLoadFUI(package, name);
			RemoveStack(name);
			return;
		}
		if (stack.state == ViewState.closing) return;
		stack.Closing();
		Transition closeTrans = UIs[name].GetTransition("Effect_JumpIn");
		if (closeTrans != null)
		{
			closeTrans.invalidateBatchingEveryFrame = true;

			closeTrans.Play(() =>
			{
				CloseUIReal(name, package, isDestroy, onFinish);
			});
		}
		else
		{
			CloseUIReal(name, package, isDestroy, onFinish);
		}

	}
	private void CloseUIReal(string name, string package, bool isDestroy, Action onFinshed)
	{
		var stack = GetStack(name);
		if (stack != null && stack.state != ViewState.closing)
		{
			return;
		}
		stack.Closed(isDestroy);
		if (UIs.ContainsKey(name))
		{
			if (isDestroy)
			{
				DestroyUI(package, name);

			}
			else
			{
				HideUI(name);
			}
			onFinshed?.Invoke();
		}
	}
	private void DestroyUI(string packageName, string name)
	{
		if (!UIs.ContainsKey(name)) return;

		var layer = "Mid";
		if (uiLayerOpen[name] != 0)
		{
			uiLayerIdx[layer] -= 1;
		}
		uiLayerOpen[name] = 0;
		PackageLoader.Ins.UnLoadFUI(packageName, name);
		RemoveStack(name);
		UIs.Remove(name);
	}
	private void HideUI(string name)
	{
		var layer = "Mid";
		if (uiLayerOpen[name] != 0)
		{
			uiLayerIdx[layer] -= 1;
		}
		uiLayerOpen[name] = 0;
	}
	private ViewStack GetStack(string name)
	{
		for (int i = 0; i < viewStackList.Count; i++)
		{
			if (viewStackList[i].name == name)
				return viewStackList[i];
		}
		return null;
	}
	private void AddStack(ViewStack stack)
	{
		RemoveStack(stack.name);
		viewStackList.Add(stack);
	}
	private void RemoveStack(string name)
	{
		for (int i = 0; i < viewStackList.Count; i++)
		{
			if (viewStackList[i].name == name)
			{
				viewStackList.RemoveAt(i);
				return;
			}
		}
	}
		
	public void OnLoadUIFinished(GComponent uiprefab, string package, string assetName, bool isOpen)
	{
		if (uiprefab == null) return;
		var layer = "Mid";
		var uiIdx = uiLayerIdx[layer];
		//uiprefab.sortingOrder = uiIdx;
		if (!UIs.ContainsKey(assetName))
		{
			UIs.Add(assetName, uiprefab);
		}
		var stack = GetStack(assetName);
		stack.Create(uiprefab, isOpen);
		if (isOpen)
		{
			_OpenUI(uiprefab);
			stack.SetState(ViewState.opened);
			stack.Enable();
		}
	}
	public void CreatePanel(string package, string name, bool isOpen = true,Action<GComponent> callback = null)
	{
		PackageLoader.Ins.LoadFUI(package, name, delegate (GComponent viewGot)
		{
			//{
				int width = Screen.width;
				int height = Screen.height;
				width = Mathf.CeilToInt(width / UIContentScaler.scaleFactor);
				height = Mathf.CeilToInt(height / UIContentScaler.scaleFactor);
				if (viewGot != null)
				{
					//Debug.Log("界面资源宽====" + viewGot.sourceWidth);
					viewGot.SetSize(viewGot.sourceWidth, height);
					//Debug.Log("(width - viewGot.sourceWidth) / 2===" + (width - viewGot.sourceWidth) / 2);
					viewGot.SetXY((int)((width - viewGot.sourceWidth) / 2), 0, true);
				}
			//}
			//else
			//{
			//	viewGot.MakeFullScreen();
			//}
			OnLoadUIFinished(viewGot, package, name, isOpen);
			callback?.Invoke(viewGot);
		});
	}

	private class ViewStack
	{
		public int order;
		public ViewState state;
		public string name;
		public GComponent go;
		public bool isEnable = false;
		public bool isCreate = false;
		public void SetState(ViewState state)
		{
			this.state = state;
		}
		public void Load(string name, int order)
		{
			this.name = name;
			this.order = order;
			state = ViewState.loading;
		}
		public void Create(int order, bool show = false)
		{
			this.order = order;
			if (go != null)
			{
				go.sortingOrder = order;
				go.visible = false;
				if ( show && !isCreate)
				{
					isCreate = true;
					//var f = luaView.GetLuaFunction("Create");
					//if (f != null)
					//{
					//	f.Call(luaView, go);
					//	f.Dispose();
					//}
				}
			}
		}
		//first create
		public void Create(GComponent go, bool show = true)
		{
			if (go != null)
			{
				this.go = go;
				go.name = go.gameObjectName;
				GRoot.inst.AddChild(go);
			}
			Create(order, show);
		}
		public void Enable()
		{
			if (go != null)
				go.visible = true;
			//if (luaView != null)
			//{
			//	if (!isEnable)
			//	{
			//		isEnable = true;
			//		var f = luaView.GetLuaFunction("Enable");
			//		if (f != null)
			//		{
			//			f.Call(luaView);
			//			f.Dispose();
			//		}
			//	}
			//}
		}
		public void Closing()
		{
			state = ViewState.closing;
		}
		public void Closed(bool isDestroy)
		{
			state = ViewState.close;
			//if (luaView != null)
			//{
			//	if (isEnable)
			//	{
			//		isEnable = false;
			//		var f = luaView.GetLuaFunction("Disable");
			//		if (f != null)
			//		{
			//			f.Call(luaView);
			//			f.Dispose();
			//		}
			//	}
			//}
			if (isDestroy)
			{
				//if (luaView != null)
				//{
				//	var f = luaView.GetLuaFunction("Destroy");
				//	if (f != null)
				//	{
				//		f.Call(luaView);
				//		f.Dispose();
				//	}
				//	f = luaView.GetLuaFunction("OnDestroyAfter");
				//	if (f != null)
				//	{
				//		f.Call(luaView);
				//		f.Dispose();
				//	}
				//	luaView.Dispose();
				//	luaView = null;
				//}
				go.Dispose();
				go = null;
			}
			else
				go.visible = false;

		}
	}
}

