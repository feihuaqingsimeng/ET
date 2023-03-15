using ET;
using GameEvent;
using System;
using System.Collections.Generic;

namespace TBS
{
	public class CommandBase
	{
		public int cid;
		public int frame;
		public BattleActionType action;
		public bool isExecute;
		public bool isActive;
		public bool isEnd;
		public Action activeCallback;
		public static void Init()
		{
			
		}
		public async void DelayCall(long millisecond, Action callback)
		{
			var flag = await TimerComponent.Instance.WaitAsync(millisecond);
			if (!flag) return;
			callback?.Invoke();
		}
		public void Reset()
		{
			isExecute = false;
			isActive = false;
			isEnd = false;
		}
		public void RegisterPreCmdActive( CommandBase preCmd, Action callback)
		{
			if (preCmd == null)
				callback?.Invoke();
			else
				preCmd.RegisterActiveCallback(callback);
		}
		public void RegisterActiveCallback(Action callback)
		{
			if (isActive)
			{
				callback?.Invoke();
				return;
			}
			activeCallback += callback;
		}
		public void ActiveCall()
		{
			activeCallback?.Invoke();
		}
		public void CmdEnd()
		{
			isEnd = true;
		}
		public virtual bool Execute()
		{
			if (isExecute) return false;
			isExecute = true;
			return true;
		}
		public virtual void Active()
		{
			isActive = true;
			ActiveCall();
		}

		public override string ToString()
		{
			return $"frame:{frame},cid:{cid},action:{action}";
		}
		
	}
}
