using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FMODUnity;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000023 RID: 35
[DisallowMultipleComponent]
[RequireComponent(typeof(ConfigurableJoint))]
public class SpaceFuelHose : GrabStat, IHawkNetworkSubBehaviour, IHandJointOverride
{
	// Token: 0x060000FA RID: 250 RVA: 0x00006064 File Offset: 0x00004264
	public void NetworkStart(HawkNetworkObject networkObject)
	{
		this.networkObject = networkObject;
		this.joint = base.GetComponent<ConfigurableJoint>();
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.lineRenderer.enabled = false;
		base.enabled = false;
	}

	// Token: 0x060000FB RID: 251 RVA: 0x00006098 File Offset: 0x00004298
	public void RegisterRPCs(HawkNetworkObject networkObject)
	{
		this.RPC_CLIENT_SET_ON = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSetOn));
		this.RPC_CLIENT_SET_CONNECTED = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSetConnected), 1);
	}

	// Token: 0x060000FC RID: 252 RVA: 0x000060CB File Offset: 0x000042CB
	public void NetworkPost(HawkNetworkObject networkObject)
	{
	}

	// Token: 0x060000FD RID: 253 RVA: 0x000060D0 File Offset: 0x000042D0
	protected override void OnFirstGrab(PlayerCharacter character, RagdollHandJoint handJoint)
	{
		SpaceFuelHose.<>c__DisplayClass26_0 CS$<>8__locals1 = new SpaceFuelHose.<>c__DisplayClass26_0();
		CS$<>8__locals1.handJoint = handJoint;
		CS$<>8__locals1.<>4__this = this;
		base.OnFirstGrab(character, CS$<>8__locals1.handJoint);
		if (this.awaitingHands.Contains(CS$<>8__locals1.handJoint))
		{
			return;
		}
		if (this.networkObject != null && this.networkObject.IsServer())
		{
			this.networkObject.SendRPC(this.RPC_CLIENT_SET_ON, true, 6, new object[] { true });
		}
		base.StartCoroutine(CS$<>8__locals1.<OnFirstGrab>g__WaitFrame|0());
		if (this.rigidbody)
		{
			Collider[] componentsInChildren = character.GetComponentsInChildren<Collider>();
			Collider[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				foreach (Collider collider2 in componentsInChildren2)
				{
					Physics.IgnoreCollision(collider, collider2, true);
				}
			}
		}
		if (this.joint)
		{
			SoftJointLimit linearLimit = this.joint.linearLimit;
			linearLimit.limit = this.maxHoseDistance;
			this.joint.linearLimit = linearLimit;
		}
		if (this.fixHoseUnderTruckCoroutine != null)
		{
			base.StopCoroutine(this.fixHoseUnderTruckCoroutine);
			this.fixHoseUnderTruckCoroutine = null;
		}
		this.timeSinceChanged = Time.time;
	}

	// Token: 0x060000FE RID: 254 RVA: 0x0000620C File Offset: 0x0000440C
	protected override void OnLastRelease(PlayerCharacter character, RagdollHandJoint handJoint)
	{
		base.OnLastRelease(character, handJoint);
		if (this.awaitingHands.Contains(handJoint))
		{
			return;
		}
		if (!this.attachedConnector && character)
		{
			Collider[] componentsInChildren = character.GetComponentsInChildren<Collider>();
			Collider[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				foreach (Collider collider2 in componentsInChildren2)
				{
					Physics.IgnoreCollision(collider, collider2, false);
				}
			}
			this.OnLetGo();
		}
	}

	// Token: 0x060000FF RID: 255 RVA: 0x00006298 File Offset: 0x00004498
	private void OnLetGo()
	{
		if (this.networkObject != null && this.networkObject.IsServer())
		{
			this.networkObject.SendRPC(this.RPC_CLIENT_SET_ON, true, 6, new object[] { false });
		}
		if (this.joint)
		{
			SoftJointLimit linearLimit = this.joint.linearLimit;
			linearLimit.limit = 0.001f;
			this.joint.linearLimit = linearLimit;
		}
		if (this.fixHoseUnderTruckCoroutine == null && base.gameObject.activeInHierarchy)
		{
			this.fixHoseUnderTruckCoroutine = base.StartCoroutine(this.FixHoseUnderTruck());
		}
		this.rigidbody.detectCollisions = true;
		this.timeSinceChanged = Time.time;
	}

	// Token: 0x06000100 RID: 256 RVA: 0x0000634C File Offset: 0x0000454C
	private void Update()
	{
		if (!this.bOn && Time.time - this.timeSinceChanged >= 3f)
		{
			base.enabled = false;
			this.lineRenderer.enabled = false;
		}
		this.positions[0] = this.hoseStartTransform.position;
		this.positions[1] = this.lineRenderer.transform.position;
		this.lineRenderer.positionCount = 2;
		this.lineRenderer.SetPositions(this.positions);
	}

	// Token: 0x06000101 RID: 257 RVA: 0x000063D7 File Offset: 0x000045D7
	private IEnumerator FixHoseUnderTruck()
	{
		yield return new WaitForSeconds(3f);
		if (this.rigidbody && this.hoseStartAnchorTransform)
		{
			this.rigidbody.position = this.hoseStartAnchorTransform.position;
			this.rigidbody.rotation = this.hoseStartAnchorTransform.rotation;
			this.rigidbody.velocity = Vector3.zero;
			this.rigidbody.angularVelocity = Vector3.zero;
		}
		this.fixHoseUnderTruckCoroutine = null;
		yield break;
	}

	// Token: 0x06000102 RID: 258 RVA: 0x000063E8 File Offset: 0x000045E8
	private void ClientSetConnected(HawkNetReader reader, HawkRPCInfo info)
	{
		ulong num = reader.ReadUInt64();
		if (this.connectedTimestep >= num)
		{
			return;
		}
		this.connectedTimestep = num;
		this.bConnected = reader.ReadBoolean();
		if (this.bConnected)
		{
			if (!string.IsNullOrEmpty(this.connectedOneShot))
			{
				RuntimeManager.PlayOneShot(this.connectedOneShot, base.transform.position);
			}
		}
		else if (!string.IsNullOrEmpty(this.unconnectedOneShot))
		{
			RuntimeManager.PlayOneShot(this.unconnectedOneShot, base.transform.position);
		}
		SpaceFuelHose.SpaceFuelHoseConnectedCallback spaceFuelHoseConnectedCallback = this.onConnectedCallback;
		if (spaceFuelHoseConnectedCallback == null)
		{
			return;
		}
		spaceFuelHoseConnectedCallback(this, this.bConnected);
	}

	// Token: 0x06000103 RID: 259 RVA: 0x00006480 File Offset: 0x00004680
	private void ClientSetOn(HawkNetReader reader, HawkRPCInfo info)
	{
		this.bOn = reader.ReadBoolean();
		if (this.bOn)
		{
			base.enabled = true;
			this.lineRenderer.enabled = true;
		}
	}

	// Token: 0x06000104 RID: 260 RVA: 0x000064AC File Offset: 0x000046AC
	internal void ServerSetConnector(SpaceFuelHoseConnector connector)
	{
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		if (this.attachedConnector == connector)
		{
			return;
		}
		this.attachedConnector = connector;
		if (connector)
		{
			base.ForceUngrabAll();
			if (!this.attachedConnectorJoint)
			{
				Transform connectorTransform = connector.GetConnectorTransform();
				if (connectorTransform)
				{
					base.transform.SetPositionAndRotation(connectorTransform.position, connectorTransform.rotation);
					this.rigidbody.position = connectorTransform.position;
					this.rigidbody.rotation = connectorTransform.rotation;
					this.rigidbody.detectCollisions = false;
					this.attachedConnectorJoint = base.gameObject.AddComponent<FixedJoint>();
				}
			}
		}
		else
		{
			if (this.attachedConnectorJoint)
			{
				this.rigidbody.detectCollisions = true;
				Object.Destroy(this.attachedConnectorJoint);
				this.attachedConnectorJoint = null;
			}
			this.OnLetGo();
		}
		if (connector)
		{
			this.networkObject.SendRPC(this.RPC_CLIENT_SET_CONNECTED, true, 6, new object[]
			{
				HawkNetworkManager.DefaultInstance.GetTimestep(),
				true
			});
			return;
		}
		this.networkObject.ClearBuffered(this.RPC_CLIENT_SET_CONNECTED);
		this.networkObject.SendRPC(this.RPC_CLIENT_SET_CONNECTED, 0, new object[]
		{
			HawkNetworkManager.DefaultInstance.GetTimestep(),
			false
		});
	}

	// Token: 0x06000105 RID: 261 RVA: 0x00006622 File Offset: 0x00004822
	public bool IsConnected()
	{
		return this.bConnected;
	}

	// Token: 0x06000106 RID: 262 RVA: 0x0000662A File Offset: 0x0000482A
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00006632 File Offset: 0x00004832
	public SpaceFuelHoseConnector GetConnectedConnector()
	{
		return this.attachedConnector;
	}

	// Token: 0x06000108 RID: 264 RVA: 0x0000663A File Offset: 0x0000483A
	public bool IsPlayerNonMoveable()
	{
		return false;
	}

	// Token: 0x06000109 RID: 265 RVA: 0x0000663D File Offset: 0x0000483D
	public SpaceFuelHose()
	{
	}

	// Token: 0x040000CF RID: 207
	private const float SnapTime = 3f;

	// Token: 0x040000D0 RID: 208
	private byte RPC_CLIENT_SET_ON;

	// Token: 0x040000D1 RID: 209
	private byte RPC_CLIENT_SET_CONNECTED;

	// Token: 0x040000D2 RID: 210
	internal SpaceFuelHose.SpaceFuelHoseConnectedCallback onConnectedCallback;

	// Token: 0x040000D3 RID: 211
	[SerializeField]
	private LineRenderer lineRenderer;

	// Token: 0x040000D4 RID: 212
	[SerializeField]
	private float maxHoseDistance = 10f;

	// Token: 0x040000D5 RID: 213
	[SerializeField]
	private Transform hoseStartTransform;

	// Token: 0x040000D6 RID: 214
	[SerializeField]
	private Transform handGrabPositionTransform;

	// Token: 0x040000D7 RID: 215
	[SerializeField]
	private Transform hoseStartAnchorTransform;

	// Token: 0x040000D8 RID: 216
	[SerializeField]
	[EventRef]
	private string connectedOneShot;

	// Token: 0x040000D9 RID: 217
	[SerializeField]
	[EventRef]
	private string unconnectedOneShot;

	// Token: 0x040000DA RID: 218
	private HawkNetworkObject networkObject;

	// Token: 0x040000DB RID: 219
	private ConfigurableJoint joint;

	// Token: 0x040000DC RID: 220
	private Rigidbody rigidbody;

	// Token: 0x040000DD RID: 221
	private float timeSinceChanged;

	// Token: 0x040000DE RID: 222
	private Vector3[] positions = new Vector3[2];

	// Token: 0x040000DF RID: 223
	private Coroutine fixHoseUnderTruckCoroutine;

	// Token: 0x040000E0 RID: 224
	private bool bOn;

	// Token: 0x040000E1 RID: 225
	private SpaceFuelHoseConnector attachedConnector;

	// Token: 0x040000E2 RID: 226
	private FixedJoint attachedConnectorJoint;

	// Token: 0x040000E3 RID: 227
	private bool bConnected;

	// Token: 0x040000E4 RID: 228
	private HashSet<RagdollHandJoint> awaitingHands = new HashSet<RagdollHandJoint>();

	// Token: 0x040000E5 RID: 229
	private ulong connectedTimestep;

	// Token: 0x02000068 RID: 104
	// (Invoke) Token: 0x060002E9 RID: 745
	internal delegate void SpaceFuelHoseConnectedCallback(SpaceFuelHose hose, bool bConnected);

	// Token: 0x02000069 RID: 105
	[CompilerGenerated]
	private sealed class <>c__DisplayClass26_0
	{
		// Token: 0x060002EC RID: 748 RVA: 0x0000F29B File Offset: 0x0000D49B
		public <>c__DisplayClass26_0()
		{
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0000F2A3 File Offset: 0x0000D4A3
		internal IEnumerator <OnFirstGrab>g__WaitFrame|0()
		{
			SpaceFuelHose.<>c__DisplayClass26_0.<<OnFirstGrab>g__WaitFrame|0>d <<OnFirstGrab>g__WaitFrame|0>d = new SpaceFuelHose.<>c__DisplayClass26_0.<<OnFirstGrab>g__WaitFrame|0>d(0);
			<<OnFirstGrab>g__WaitFrame|0>d.<>4__this = this;
			return <<OnFirstGrab>g__WaitFrame|0>d;
		}

		// Token: 0x0400025C RID: 604
		public RagdollHandJoint handJoint;

		// Token: 0x0400025D RID: 605
		public SpaceFuelHose <>4__this;

		// Token: 0x02000088 RID: 136
		private sealed class <<OnFirstGrab>g__WaitFrame|0>d : IEnumerator<object>, IEnumerator, IDisposable
		{
			// Token: 0x0600035F RID: 863 RVA: 0x00010888 File Offset: 0x0000EA88
			[DebuggerHidden]
			public <<OnFirstGrab>g__WaitFrame|0>d(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			// Token: 0x06000360 RID: 864 RVA: 0x00010897 File Offset: 0x0000EA97
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x06000361 RID: 865 RVA: 0x0001089C File Offset: 0x0000EA9C
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				SpaceFuelHose.<>c__DisplayClass26_0 CS$<>8__locals1 = this.<>4__this;
				switch (num)
				{
				case 0:
					this.<>1__state = -1;
					if (CS$<>8__locals1.handJoint)
					{
						CS$<>8__locals1.<>4__this.awaitingHands.Add(CS$<>8__locals1.handJoint);
						CS$<>8__locals1.handJoint.ForceUngrab();
					}
					this.<>2__current = new WaitForFixedUpdate();
					this.<>1__state = 1;
					return true;
				case 1:
					this.<>1__state = -1;
					if (CS$<>8__locals1.<>4__this.handGrabPositionTransform && CS$<>8__locals1.handJoint)
					{
						CS$<>8__locals1.<>4__this.rigidbody.transform.position = (CS$<>8__locals1.<>4__this.rigidbody.position = CS$<>8__locals1.handJoint.GetAnchorTransform().position + CS$<>8__locals1.<>4__this.handGrabPositionTransform.localPosition);
						CS$<>8__locals1.<>4__this.rigidbody.transform.forward = CS$<>8__locals1.handJoint.GetAnchorTransform().forward + Vector3.up * 0.2f;
						CS$<>8__locals1.<>4__this.rigidbody.rotation = CS$<>8__locals1.<>4__this.rigidbody.transform.rotation;
						CS$<>8__locals1.<>4__this.rigidbody.velocity = Vector3.zero;
						CS$<>8__locals1.<>4__this.rigidbody.angularVelocity = Vector3.zero;
						CS$<>8__locals1.handJoint.ForceGrab(CS$<>8__locals1.<>4__this.handGrabPositionTransform.gameObject, new Vector3?(CS$<>8__locals1.<>4__this.handGrabPositionTransform.transform.localPosition), true);
						CS$<>8__locals1.<>4__this.awaitingHands.Remove(CS$<>8__locals1.handJoint);
					}
					this.<>2__current = new WaitForFixedUpdate();
					this.<>1__state = 2;
					return true;
				case 2:
					this.<>1__state = -1;
					if (CS$<>8__locals1.<>4__this.rigidbody)
					{
						CS$<>8__locals1.<>4__this.rigidbody.velocity = Vector3.zero;
						CS$<>8__locals1.<>4__this.rigidbody.angularVelocity = Vector3.zero;
					}
					return false;
				default:
					return false;
				}
			}

			// Token: 0x17000025 RID: 37
			// (get) Token: 0x06000362 RID: 866 RVA: 0x00010ABB File Offset: 0x0000ECBB
			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x06000363 RID: 867 RVA: 0x00010AC3 File Offset: 0x0000ECC3
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000026 RID: 38
			// (get) Token: 0x06000364 RID: 868 RVA: 0x00010ACA File Offset: 0x0000ECCA
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x040002B5 RID: 693
			private int <>1__state;

			// Token: 0x040002B6 RID: 694
			private object <>2__current;

			// Token: 0x040002B7 RID: 695
			public SpaceFuelHose.<>c__DisplayClass26_0 <>4__this;
		}
	}

	// Token: 0x0200006A RID: 106
	[CompilerGenerated]
	private sealed class <FixHoseUnderTruck>d__30 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x060002EE RID: 750 RVA: 0x0000F2B2 File Offset: 0x0000D4B2
		[DebuggerHidden]
		public <FixHoseUnderTruck>d__30(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x060002EF RID: 751 RVA: 0x0000F2C1 File Offset: 0x0000D4C1
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x0000F2C4 File Offset: 0x0000D4C4
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpaceFuelHose spaceFuelHose = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(3f);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			if (spaceFuelHose.rigidbody && spaceFuelHose.hoseStartAnchorTransform)
			{
				spaceFuelHose.rigidbody.position = spaceFuelHose.hoseStartAnchorTransform.position;
				spaceFuelHose.rigidbody.rotation = spaceFuelHose.hoseStartAnchorTransform.rotation;
				spaceFuelHose.rigidbody.velocity = Vector3.zero;
				spaceFuelHose.rigidbody.angularVelocity = Vector3.zero;
			}
			spaceFuelHose.fixHoseUnderTruckCoroutine = null;
			return false;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x0000F37D File Offset: 0x0000D57D
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0000F385 File Offset: 0x0000D585
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x0000F38C File Offset: 0x0000D58C
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x0400025E RID: 606
		private int <>1__state;

		// Token: 0x0400025F RID: 607
		private object <>2__current;

		// Token: 0x04000260 RID: 608
		public SpaceFuelHose <>4__this;
	}
}
