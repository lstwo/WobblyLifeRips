using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using FMOD.Studio;
using FMODUnity;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000026 RID: 38
[RequireComponent(typeof(ConfigurableJoint))]
public class SpaceFuelPumpPump : GrabStat, IHawkNetworkSubBehaviour
{
	// Token: 0x14000008 RID: 8
	// (add) Token: 0x0600011C RID: 284 RVA: 0x00006A64 File Offset: 0x00004C64
	// (remove) Token: 0x0600011D RID: 285 RVA: 0x00006A9C File Offset: 0x00004C9C
	public event SpaceFuelPumpPump.SpaceFuelPumpPumpCallback onPumpCallback
	{
		[CompilerGenerated]
		add
		{
			SpaceFuelPumpPump.SpaceFuelPumpPumpCallback spaceFuelPumpPumpCallback = this.onPumpCallback;
			SpaceFuelPumpPump.SpaceFuelPumpPumpCallback spaceFuelPumpPumpCallback2;
			do
			{
				spaceFuelPumpPumpCallback2 = spaceFuelPumpPumpCallback;
				SpaceFuelPumpPump.SpaceFuelPumpPumpCallback spaceFuelPumpPumpCallback3 = (SpaceFuelPumpPump.SpaceFuelPumpPumpCallback)Delegate.Combine(spaceFuelPumpPumpCallback2, value);
				spaceFuelPumpPumpCallback = Interlocked.CompareExchange<SpaceFuelPumpPump.SpaceFuelPumpPumpCallback>(ref this.onPumpCallback, spaceFuelPumpPumpCallback3, spaceFuelPumpPumpCallback2);
			}
			while (spaceFuelPumpPumpCallback != spaceFuelPumpPumpCallback2);
		}
		[CompilerGenerated]
		remove
		{
			SpaceFuelPumpPump.SpaceFuelPumpPumpCallback spaceFuelPumpPumpCallback = this.onPumpCallback;
			SpaceFuelPumpPump.SpaceFuelPumpPumpCallback spaceFuelPumpPumpCallback2;
			do
			{
				spaceFuelPumpPumpCallback2 = spaceFuelPumpPumpCallback;
				SpaceFuelPumpPump.SpaceFuelPumpPumpCallback spaceFuelPumpPumpCallback3 = (SpaceFuelPumpPump.SpaceFuelPumpPumpCallback)Delegate.Remove(spaceFuelPumpPumpCallback2, value);
				spaceFuelPumpPumpCallback = Interlocked.CompareExchange<SpaceFuelPumpPump.SpaceFuelPumpPumpCallback>(ref this.onPumpCallback, spaceFuelPumpPumpCallback3, spaceFuelPumpPumpCallback2);
			}
			while (spaceFuelPumpPumpCallback != spaceFuelPumpPumpCallback2);
		}
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00006AD1 File Offset: 0x00004CD1
	public void RegisterRPCs(HawkNetworkObject networkObject)
	{
		this.RPC_CLIENT_ON_PUMPED = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientOnPumped), 1);
		this.RPC_CLIENT_SET_ALLOWED_TO_USE = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSetReadyToUse), 1);
	}

	// Token: 0x0600011F RID: 287 RVA: 0x00006B05 File Offset: 0x00004D05
	public void NetworkStart(HawkNetworkObject networkObject)
	{
		this.networkObject = networkObject;
		this.OnAllowedToUseChanged(this.bAllowedToUse);
	}

	// Token: 0x06000120 RID: 288 RVA: 0x00006B1A File Offset: 0x00004D1A
	public void NetworkPost(HawkNetworkObject networkObject)
	{
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00006B1C File Offset: 0x00004D1C
	private void Start()
	{
		ConfigurableJoint component = base.GetComponent<ConfigurableJoint>();
		this.startPosition = base.transform.position;
		float num = component.linearLimit.limit - 0.1f;
		this.endPosition = this.startPosition + base.transform.forward * num;
		this.startPosition -= base.transform.forward * num;
		base.transform.position = this.endPosition;
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00006BA9 File Offset: 0x00004DA9
	private void OnDestroy()
	{
		if (this.pumpAttachedInstance.isValid())
		{
			this.pumpAttachedInstance.stop(0);
			this.pumpAttachedInstance.release();
		}
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00006BD4 File Offset: 0x00004DD4
	protected override void OnFirstGrab(PlayerCharacter character, RagdollHandJoint handJoint)
	{
		base.OnFirstGrab(character, handJoint);
		if (this.updatePumpCoroutine != null)
		{
			base.StopCoroutine(this.updatePumpCoroutine);
		}
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.isKinematic = false;
		}
		this.updatePumpCoroutine = base.StartCoroutine(this.UpdatePump());
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00006C28 File Offset: 0x00004E28
	protected override void OnLastRelease(PlayerCharacter character, RagdollHandJoint handJoint)
	{
		base.OnLastRelease(character, handJoint);
		if (this.updatePumpCoroutine != null)
		{
			base.StopCoroutine(this.updatePumpCoroutine);
			this.updatePumpCoroutine = null;
		}
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.isKinematic = true;
		}
	}

	// Token: 0x06000125 RID: 293 RVA: 0x00006C6E File Offset: 0x00004E6E
	private IEnumerator UpdatePump()
	{
		WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
		for (;;)
		{
			yield return fixedUpdate;
			if (this.bPumpIn)
			{
				if (Vector3.Dot(base.transform.forward, this.startPosition - base.transform.position) <= -0.5f)
				{
					this.bPumpIn = !this.bPumpIn;
					this.networkObject.SendRPC(this.RPC_CLIENT_ON_PUMPED, 0, new object[] { true });
				}
			}
			else if (Vector3.Dot(-base.transform.forward, this.endPosition - base.transform.position) <= -0.5f)
			{
				this.bPumpIn = !this.bPumpIn;
				this.networkObject.SendRPC(this.RPC_CLIENT_ON_PUMPED, 0, new object[] { false });
			}
		}
		yield break;
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00006C80 File Offset: 0x00004E80
	private void OnAllowedToUseChanged(bool bReadyToUse)
	{
		if (this.readyToUseGameObject)
		{
			this.readyToUseGameObject.SetActive(bReadyToUse);
		}
		if (this.notReadyToUseGameObject)
		{
			this.notReadyToUseGameObject.SetActive(!bReadyToUse);
		}
		if (bReadyToUse)
		{
			if (!this.pumpAttachedInstance.isValid() && !string.IsNullOrEmpty(this.pumpAttachedLoop))
			{
				this.pumpAttachedInstance = RuntimeManager.CreateInstance(this.pumpAttachedLoop);
				this.pumpAttachedInstance.set3DAttributes(RuntimeUtils.To3DAttributes(base.transform.position));
				this.pumpAttachedInstance.start();
				return;
			}
		}
		else if (this.pumpAttachedInstance.isValid())
		{
			this.pumpAttachedInstance.stop(0);
			this.pumpAttachedInstance.release();
			this.pumpAttachedInstance.clearHandle();
		}
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00006D4C File Offset: 0x00004F4C
	private void ClientSetReadyToUse(HawkNetReader reader, HawkRPCInfo info)
	{
		ulong num = reader.ReadUInt64();
		if (num < this.readyToUseTimestep)
		{
			return;
		}
		this.readyToUseTimestep = num;
		this.OnAllowedToUseChanged(reader.ReadBoolean());
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00006D80 File Offset: 0x00004F80
	private void ClientOnPumped(HawkNetReader reader, HawkRPCInfo info)
	{
		bool flag = reader.ReadBoolean();
		if (flag)
		{
			if (!string.IsNullOrEmpty(this.pumpedInOneshot))
			{
				RuntimeManager.PlayOneShot(this.pumpedInOneshot, base.transform.position);
			}
		}
		else if (!string.IsNullOrEmpty(this.pumpedOutOneshot))
		{
			RuntimeManager.PlayOneShot(this.pumpedOutOneshot, base.transform.position);
		}
		SpaceFuelPumpPump.SpaceFuelPumpPumpCallback spaceFuelPumpPumpCallback = this.onPumpCallback;
		if (spaceFuelPumpPumpCallback == null)
		{
			return;
		}
		spaceFuelPumpPumpCallback(flag);
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00006DF0 File Offset: 0x00004FF0
	public void ServerSetAllowedToUse(bool bAllowedToUse)
	{
		if (this.bAllowedToUse == bAllowedToUse)
		{
			return;
		}
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		this.bAllowedToUse = bAllowedToUse;
		if (!bAllowedToUse)
		{
			base.ForceUngrabAll();
		}
		this.networkObject.SendRPC(this.RPC_CLIENT_SET_ALLOWED_TO_USE, true, 7, new object[]
		{
			HawkNetworkManager.DefaultInstance.GetTimestep(),
			bAllowedToUse
		});
		this.OnAllowedToUseChanged(bAllowedToUse);
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00006E68 File Offset: 0x00005068
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00006E70 File Offset: 0x00005070
	public override bool IsGrabbable()
	{
		return base.IsGrabbable() && this.bAllowedToUse;
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00006E82 File Offset: 0x00005082
	public SpaceFuelPumpPump()
	{
	}

	// Token: 0x040000F6 RID: 246
	private byte RPC_CLIENT_ON_PUMPED;

	// Token: 0x040000F7 RID: 247
	private byte RPC_CLIENT_SET_ALLOWED_TO_USE;

	// Token: 0x040000F8 RID: 248
	[CompilerGenerated]
	private SpaceFuelPumpPump.SpaceFuelPumpPumpCallback onPumpCallback;

	// Token: 0x040000F9 RID: 249
	[SerializeField]
	[EventRef]
	private string pumpAttachedLoop;

	// Token: 0x040000FA RID: 250
	[SerializeField]
	[EventRef]
	private string pumpedInOneshot;

	// Token: 0x040000FB RID: 251
	[SerializeField]
	[EventRef]
	private string pumpedOutOneshot;

	// Token: 0x040000FC RID: 252
	[SerializeField]
	private GameObject readyToUseGameObject;

	// Token: 0x040000FD RID: 253
	[SerializeField]
	private GameObject notReadyToUseGameObject;

	// Token: 0x040000FE RID: 254
	[SerializeField]
	private bool bAllowedToUse = true;

	// Token: 0x040000FF RID: 255
	private Coroutine updatePumpCoroutine;

	// Token: 0x04000100 RID: 256
	private Vector3 startPosition;

	// Token: 0x04000101 RID: 257
	private Vector3 endPosition;

	// Token: 0x04000102 RID: 258
	private HawkNetworkObject networkObject;

	// Token: 0x04000103 RID: 259
	private bool bPumpIn;

	// Token: 0x04000104 RID: 260
	private EventInstance pumpAttachedInstance;

	// Token: 0x04000105 RID: 261
	private ulong readyToUseTimestep;

	// Token: 0x0200006D RID: 109
	// (Invoke) Token: 0x06000301 RID: 769
	public delegate void SpaceFuelPumpPumpCallback(bool bPumpedIn);

	// Token: 0x0200006E RID: 110
	[CompilerGenerated]
	private sealed class <UpdatePump>d__25 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x06000304 RID: 772 RVA: 0x0000F556 File Offset: 0x0000D756
		[DebuggerHidden]
		public <UpdatePump>d__25(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0000F565 File Offset: 0x0000D765
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0000F568 File Offset: 0x0000D768
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpaceFuelPumpPump spaceFuelPumpPump = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				if (spaceFuelPumpPump.bPumpIn)
				{
					if (Vector3.Dot(spaceFuelPumpPump.transform.forward, spaceFuelPumpPump.startPosition - spaceFuelPumpPump.transform.position) <= -0.5f)
					{
						spaceFuelPumpPump.bPumpIn = !spaceFuelPumpPump.bPumpIn;
						spaceFuelPumpPump.networkObject.SendRPC(spaceFuelPumpPump.RPC_CLIENT_ON_PUMPED, 0, new object[] { true });
					}
				}
				else if (Vector3.Dot(-spaceFuelPumpPump.transform.forward, spaceFuelPumpPump.endPosition - spaceFuelPumpPump.transform.position) <= -0.5f)
				{
					spaceFuelPumpPump.bPumpIn = !spaceFuelPumpPump.bPumpIn;
					spaceFuelPumpPump.networkObject.SendRPC(spaceFuelPumpPump.RPC_CLIENT_ON_PUMPED, 0, new object[] { false });
				}
			}
			else
			{
				this.<>1__state = -1;
				fixedUpdate = new WaitForFixedUpdate();
			}
			this.<>2__current = fixedUpdate;
			this.<>1__state = 1;
			return true;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000307 RID: 775 RVA: 0x0000F68D File Offset: 0x0000D88D
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0000F695 File Offset: 0x0000D895
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000309 RID: 777 RVA: 0x0000F69C File Offset: 0x0000D89C
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000267 RID: 615
		private int <>1__state;

		// Token: 0x04000268 RID: 616
		private object <>2__current;

		// Token: 0x04000269 RID: 617
		public SpaceFuelPumpPump <>4__this;

		// Token: 0x0400026A RID: 618
		private WaitForFixedUpdate <fixedUpdate>5__2;
	}
}
