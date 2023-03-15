using System;
using UnityEngine.Networking;

namespace ET
{
	
	public class UnityWebRequestUpdateSystem : UpdateSystem<UnityWebRequestAsync>
	{
		protected override void Update(UnityWebRequestAsync self)
		{
			self.Update();
		}
	}
	
	public class UnityWebRequestAsync : Entity, IUpdate,IAwake
	{
		public static AcceptAllCertificate certificateHandler = new AcceptAllCertificate();
		
		public UnityWebRequest Request;

		public bool isCancel;

		public ETTask<bool> tcs;
		
		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			this.Request?.Dispose();
			this.Request = null;
			this.isCancel = false;
		}

		public float Progress
		{
			get
			{
				if (this.Request == null)
				{
					return 0;
				}
				return this.Request.downloadProgress;
			}
		}

		public ulong ByteDownloaded
		{
			get
			{
				if (this.Request == null)
				{
					return 0;
				}
				return this.Request.downloadedBytes;
			}
		}

		public void Update()
		{
			if (this.isCancel)
			{
				tcs.SetResult(false);
				//this.tcs.SetException(new Exception($"request error: {this.Request.error}"));
				return;
			}
			
			if (!this.Request.isDone)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.Request.error))
			{
				Log.Error($"request error: {this.Request.error}");
				//this.tcs.SetException(new Exception($"request error: {this.Request.error}"));
				this.tcs.SetResult(false);
				return;
			}

			this.tcs.SetResult(true);
		}

		public async ETTask<bool> DownloadAsync(string url,ETCancellationToken cancel = null)
		{
			this.tcs = ETTask<bool>.Create(true);
			
			url = url.Replace(" ", "%20");
			this.Request = UnityWebRequest.Get(url);
			this.Request.certificateHandler = certificateHandler;
			this.Request.SendWebRequest();

			bool flag = false;
			void CancelAction()
			{
				tcs.SetResult(false);
			}
			try
			{
				cancel?.Add(CancelAction);
				flag = await this.tcs;
			}
			catch (RpcException e)
			{
				Log.Error($"DownloadAsync Exception {e.Message}");
			}
			finally
			{
				cancel?.Remove(CancelAction);
			}
			return flag;

		}
	}
}
