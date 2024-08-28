using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FMOD.Studio;
using HawkNetworking;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000027 RID: 39
public class SpaceGravityLift : HawkNetworkBehaviour
{
	// Token: 0x0600012D RID: 301 RVA: 0x00006E91 File Offset: 0x00005091
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_SET_ON = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSetOn));
		this.RPC_CLIENT_IS_LIFTING_OBJECTS = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientIsLiftingObjects), 1);
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00006ECC File Offset: 0x000050CC
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.rigidbodyTriggerEvent.onTriggerEnter.AddListener(new UnityAction<RigidbodyTriggerEvent, Rigidbody>(this.OnRigidbodyEnter));
		this.rigidbodyTriggerEvent.onTriggerExit.AddListener(new UnityAction<RigidbodyTriggerEvent, Rigidbody>(this.OnRigidbodyExit));
		this.soundTrigger.onFirstCameraEntered += new WobblyAudioCameraTrigger.WobblyWobblyAudioCameraTriggerCallback(this.OnSoundTriggerFirstCameraEnter);
		this.soundTrigger.onLastCameraExited += new WobblyAudioCameraTrigger.WobblyWobblyAudioCameraTriggerCallback(this.OnSoundTriggerLastCameraExit);
		if (this.soundEmitter.IsUsingColliderTriggers())
		{
			Debug.LogError("You must disable use collider triggers on the emitter");
		}
		this.Set_Internal(this.bOn);
	}

	// Token: 0x0600012F RID: 303 RVA: 0x00006F69 File Offset: 0x00005169
	public void Toggle()
	{
		this.Set(!this.bOn);
	}

	// Token: 0x06000130 RID: 304 RVA: 0x00006F7A File Offset: 0x0000517A
	private void OnSoundTriggerFirstCameraEnter(GameplayCamera gameplayCamera)
	{
		if (this.bOn && this.soundEmitter && !this.soundEmitter.IsUsingColliderTriggers())
		{
			this.soundEmitter.Play();
			this.OnSoundLiftingObjectChanged();
		}
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00006FAF File Offset: 0x000051AF
	private void OnSoundTriggerLastCameraExit(GameplayCamera gameplayCamera)
	{
		if (this.soundEmitter && !this.soundEmitter.IsUsingColliderTriggers())
		{
			this.soundEmitter.Stop(0);
		}
	}

	// Token: 0x06000132 RID: 306 RVA: 0x00006FD8 File Offset: 0x000051D8
	private void OnSoundLiftingObjectChanged()
	{
		if (this.soundEmitter)
		{
			EventInstance eventInstance = this.soundEmitter.GetEventInstance();
			if (eventInstance.isValid())
			{
				eventInstance.setParameterByName("IsActivated", Convert.ToSingle(this.bIsLiftingObjects), false);
			}
		}
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00007020 File Offset: 0x00005220
	private void OnRigidbodyEnter(RigidbodyTriggerEvent triggerEvent, Rigidbody rigidbody)
	{
		if (triggerEvent.GetRigidbodies().Count == 1)
		{
			if (this.serverFixedUpdateCoroutine != null)
			{
				base.StopCoroutine(this.serverFixedUpdateCoroutine);
			}
			this.serverFixedUpdateCoroutine = base.StartCoroutine(this.ServerFixedUpdate());
			this.ServerSetLiftingObjects(true);
		}
	}

	// Token: 0x06000134 RID: 308 RVA: 0x0000705D File Offset: 0x0000525D
	private void OnRigidbodyExit(RigidbodyTriggerEvent triggerEvent, Rigidbody rigidbody)
	{
		if (triggerEvent.GetRigidbodies().Count == 0)
		{
			if (this.serverFixedUpdateCoroutine != null)
			{
				base.StopCoroutine(this.serverFixedUpdateCoroutine);
				this.serverFixedUpdateCoroutine = null;
			}
			this.ServerSetLiftingObjects(false);
		}
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00007090 File Offset: 0x00005290
	private void ServerSetLiftingObjects(bool bLifting)
	{
		SpaceGravityLift.<>c__DisplayClass21_0 CS$<>8__locals1 = new SpaceGravityLift.<>c__DisplayClass21_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.bLifting = bLifting;
		if (this.delayIsLiftingSyncCoroutine != null)
		{
			base.StopCoroutine(this.delayIsLiftingSyncCoroutine);
		}
		this.delayIsLiftingSyncCoroutine = base.StartCoroutine(CS$<>8__locals1.<ServerSetLiftingObjects>g__Delay|0());
	}

	// Token: 0x06000136 RID: 310 RVA: 0x000070D7 File Offset: 0x000052D7
	private IEnumerator ServerFixedUpdate()
	{
		WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
		for (;;)
		{
			yield return fixedUpdate;
			List<Rigidbody> rigidbodies = this.rigidbodyTriggerEvent.GetRigidbodies();
			for (int i = rigidbodies.Count - 1; i >= 0; i--)
			{
				Rigidbody rigidbody = rigidbodies[i];
				if (rigidbody)
				{
					Vector3 vector = rigidbody.velocity;
					vector.y = 0f;
					vector = base.transform.InverseTransformVector(vector);
					vector.y = this.upForce;
					vector = base.transform.TransformVector(vector);
					rigidbody.velocity = vector;
				}
				else
				{
					rigidbodies.RemoveAt(i);
				}
			}
		}
		yield break;
	}

	// Token: 0x06000137 RID: 311 RVA: 0x000070E8 File Offset: 0x000052E8
	private void ClientSetOn(HawkNetReader reader, HawkRPCInfo info)
	{
		if (info.sender.IsHost)
		{
			return;
		}
		bool flag = reader.ReadBoolean();
		this.Set_Internal(flag);
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00007114 File Offset: 0x00005314
	private void ClientIsLiftingObjects(HawkNetReader reader, HawkRPCInfo info)
	{
		ulong num = reader.ReadUInt64();
		if (num <= this.liftingObjectTimestep)
		{
			return;
		}
		this.liftingObjectTimestep = num;
		this.bIsLiftingObjects = reader.ReadBoolean();
		this.OnSoundLiftingObjectChanged();
	}

	// Token: 0x06000139 RID: 313 RVA: 0x0000714C File Offset: 0x0000534C
	public void Set(bool bValue)
	{
		if (this.bOn == bValue)
		{
			return;
		}
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		this.networkObject.SendRPC(this.RPC_CLIENT_SET_ON, true, 7, new object[] { bValue });
		this.Set_Internal(bValue);
		if (bValue)
		{
			if (this.rigidbodyTriggerEvent && this.rigidbodyTriggerEvent.GetRigidbodies().Count > 0 && this.serverFixedUpdateCoroutine == null)
			{
				this.serverFixedUpdateCoroutine = base.StartCoroutine(this.ServerFixedUpdate());
			}
			if (this.soundTrigger && this.soundTrigger.ContainsCameras() && !this.soundEmitter.IsUsingColliderTriggers())
			{
				this.soundEmitter.Play();
				this.OnSoundLiftingObjectChanged();
				return;
			}
		}
		else
		{
			if (this.serverFixedUpdateCoroutine != null)
			{
				base.StopCoroutine(this.serverFixedUpdateCoroutine);
				this.serverFixedUpdateCoroutine = null;
			}
			if (this.soundEmitter && !this.soundEmitter.IsUsingColliderTriggers())
			{
				this.soundEmitter.Stop(0);
			}
		}
	}

	// Token: 0x0600013A RID: 314 RVA: 0x0000725C File Offset: 0x0000545C
	private void Set_Internal(bool bValue)
	{
		this.bOn = bValue;
		if (this.particleSystems != null)
		{
			if (this.bOn)
			{
				for (int i = 0; i < this.particleSystems.Length; i++)
				{
					this.particleSystems[i].Play();
					this.particleSystems[i].emission.enabled = true;
				}
				return;
			}
			for (int j = 0; j < this.particleSystems.Length; j++)
			{
				this.particleSystems[j].emission.enabled = false;
			}
		}
	}

	// Token: 0x0600013B RID: 315 RVA: 0x000072E0 File Offset: 0x000054E0
	public SpaceGravityLift()
	{
	}

	// Token: 0x04000106 RID: 262
	private byte RPC_CLIENT_SET_ON;

	// Token: 0x04000107 RID: 263
	private byte RPC_CLIENT_IS_LIFTING_OBJECTS;

	// Token: 0x04000108 RID: 264
	[SerializeField]
	private RigidbodyTriggerEvent rigidbodyTriggerEvent;

	// Token: 0x04000109 RID: 265
	[SerializeField]
	private WobblyAudioCameraTrigger soundTrigger;

	// Token: 0x0400010A RID: 266
	[SerializeField]
	private WobblySoundEmitter soundEmitter;

	// Token: 0x0400010B RID: 267
	[SerializeField]
	private float upForce = 10f;

	// Token: 0x0400010C RID: 268
	[SerializeField]
	private bool bOn = true;

	// Token: 0x0400010D RID: 269
	[SerializeField]
	private ParticleSystem[] particleSystems;

	// Token: 0x0400010E RID: 270
	private Coroutine serverFixedUpdateCoroutine;

	// Token: 0x0400010F RID: 271
	private bool bIsLiftingObjects;

	// Token: 0x04000110 RID: 272
	private ulong liftingObjectTimestep;

	// Token: 0x04000111 RID: 273
	private Coroutine delayIsLiftingSyncCoroutine;

	// Token: 0x04000112 RID: 274
	private float timeSinceLiftingSynced;

	// Token: 0x0200006F RID: 111
	[CompilerGenerated]
	private sealed class <>c__DisplayClass21_0
	{
		// Token: 0x0600030A RID: 778 RVA: 0x0000F6A4 File Offset: 0x0000D8A4
		public <>c__DisplayClass21_0()
		{
		}

		// Token: 0x0600030B RID: 779 RVA: 0x0000F6AC File Offset: 0x0000D8AC
		internal IEnumerator <ServerSetLiftingObjects>g__Delay|0()
		{
			SpaceGravityLift.<>c__DisplayClass21_0.<<ServerSetLiftingObjects>g__Delay|0>d <<ServerSetLiftingObjects>g__Delay|0>d = new SpaceGravityLift.<>c__DisplayClass21_0.<<ServerSetLiftingObjects>g__Delay|0>d(0);
			<<ServerSetLiftingObjects>g__Delay|0>d.<>4__this = this;
			return <<ServerSetLiftingObjects>g__Delay|0>d;
		}

		// Token: 0x0400026B RID: 619
		public SpaceGravityLift <>4__this;

		// Token: 0x0400026C RID: 620
		public bool bLifting;

		// Token: 0x02000089 RID: 137
		private sealed class <<ServerSetLiftingObjects>g__Delay|0>d : IEnumerator<object>, IEnumerator, IDisposable
		{
			// Token: 0x06000365 RID: 869 RVA: 0x00010AD2 File Offset: 0x0000ECD2
			[DebuggerHidden]
			public <<ServerSetLiftingObjects>g__Delay|0>d(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			// Token: 0x06000366 RID: 870 RVA: 0x00010AE1 File Offset: 0x0000ECE1
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x06000367 RID: 871 RVA: 0x00010AE4 File Offset: 0x0000ECE4
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				SpaceGravityLift.<>c__DisplayClass21_0 CS$<>8__locals1 = this.<>4__this;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					this.<>1__state = -1;
				}
				else
				{
					this.<>1__state = -1;
				}
				if (Time.time - CS$<>8__locals1.<>4__this.timeSinceLiftingSynced > 0.2f)
				{
					if (CS$<>8__locals1.<>4__this.bIsLiftingObjects != CS$<>8__locals1.bLifting)
					{
						CS$<>8__locals1.<>4__this.timeSinceLiftingSynced = Time.time;
						CS$<>8__locals1.<>4__this.networkObject.SendRPC(CS$<>8__locals1.<>4__this.RPC_CLIENT_IS_LIFTING_OBJECTS, 0, new object[]
						{
							HawkNetworkManager.DefaultInstance.GetTimestep(),
							CS$<>8__locals1.bLifting
						});
					}
					return false;
				}
				this.<>2__current = null;
				this.<>1__state = 1;
				return true;
			}

			// Token: 0x17000027 RID: 39
			// (get) Token: 0x06000368 RID: 872 RVA: 0x00010BA6 File Offset: 0x0000EDA6
			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000369 RID: 873 RVA: 0x00010BAE File Offset: 0x0000EDAE
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000028 RID: 40
			// (get) Token: 0x0600036A RID: 874 RVA: 0x00010BB5 File Offset: 0x0000EDB5
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x040002B8 RID: 696
			private int <>1__state;

			// Token: 0x040002B9 RID: 697
			private object <>2__current;

			// Token: 0x040002BA RID: 698
			public SpaceGravityLift.<>c__DisplayClass21_0 <>4__this;
		}
	}

	// Token: 0x02000070 RID: 112
	[CompilerGenerated]
	private sealed class <ServerFixedUpdate>d__22 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x0600030C RID: 780 RVA: 0x0000F6BB File Offset: 0x0000D8BB
		[DebuggerHidden]
		public <ServerFixedUpdate>d__22(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0000F6CA File Offset: 0x0000D8CA
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0000F6CC File Offset: 0x0000D8CC
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpaceGravityLift spaceGravityLift = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				List<Rigidbody> rigidbodies = spaceGravityLift.rigidbodyTriggerEvent.GetRigidbodies();
				for (int i = rigidbodies.Count - 1; i >= 0; i--)
				{
					Rigidbody rigidbody = rigidbodies[i];
					if (rigidbody)
					{
						Vector3 vector = rigidbody.velocity;
						vector.y = 0f;
						vector = spaceGravityLift.transform.InverseTransformVector(vector);
						vector.y = spaceGravityLift.upForce;
						vector = spaceGravityLift.transform.TransformVector(vector);
						rigidbody.velocity = vector;
					}
					else
					{
						rigidbodies.RemoveAt(i);
					}
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

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600030F RID: 783 RVA: 0x0000F7A5 File Offset: 0x0000D9A5
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0000F7AD File Offset: 0x0000D9AD
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000311 RID: 785 RVA: 0x0000F7B4 File Offset: 0x0000D9B4
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x0400026D RID: 621
		private int <>1__state;

		// Token: 0x0400026E RID: 622
		private object <>2__current;

		// Token: 0x0400026F RID: 623
		public SpaceGravityLift <>4__this;

		// Token: 0x04000270 RID: 624
		private WaitForFixedUpdate <fixedUpdate>5__2;
	}
}
