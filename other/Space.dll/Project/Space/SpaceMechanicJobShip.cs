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

// Token: 0x02000012 RID: 18
internal class SpaceMechanicJobShip : HawkNetworkBehaviour
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x0600007C RID: 124 RVA: 0x0000442C File Offset: 0x0000262C
	// (remove) Token: 0x0600007D RID: 125 RVA: 0x00004464 File Offset: 0x00002664
	public event Action<SpaceMechanicJobShip, bool> onServerTaskCompleted
	{
		[CompilerGenerated]
		add
		{
			Action<SpaceMechanicJobShip, bool> action = this.onServerTaskCompleted;
			Action<SpaceMechanicJobShip, bool> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip, bool> action3 = (Action<SpaceMechanicJobShip, bool>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip, bool>>(ref this.onServerTaskCompleted, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<SpaceMechanicJobShip, bool> action = this.onServerTaskCompleted;
			Action<SpaceMechanicJobShip, bool> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip, bool> action3 = (Action<SpaceMechanicJobShip, bool>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip, bool>>(ref this.onServerTaskCompleted, action3, action2);
			}
			while (action != action2);
		}
	}

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x0600007E RID: 126 RVA: 0x0000449C File Offset: 0x0000269C
	// (remove) Token: 0x0600007F RID: 127 RVA: 0x000044D4 File Offset: 0x000026D4
	public event Action<SpaceMechanicJobShip> onServerShipPreLanded
	{
		[CompilerGenerated]
		add
		{
			Action<SpaceMechanicJobShip> action = this.onServerShipPreLanded;
			Action<SpaceMechanicJobShip> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip> action3 = (Action<SpaceMechanicJobShip>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip>>(ref this.onServerShipPreLanded, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<SpaceMechanicJobShip> action = this.onServerShipPreLanded;
			Action<SpaceMechanicJobShip> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip> action3 = (Action<SpaceMechanicJobShip>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip>>(ref this.onServerShipPreLanded, action3, action2);
			}
			while (action != action2);
		}
	}

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x06000080 RID: 128 RVA: 0x0000450C File Offset: 0x0000270C
	// (remove) Token: 0x06000081 RID: 129 RVA: 0x00004544 File Offset: 0x00002744
	public event Action<SpaceMechanicJobShip> onServerShipLanded
	{
		[CompilerGenerated]
		add
		{
			Action<SpaceMechanicJobShip> action = this.onServerShipLanded;
			Action<SpaceMechanicJobShip> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip> action3 = (Action<SpaceMechanicJobShip>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip>>(ref this.onServerShipLanded, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<SpaceMechanicJobShip> action = this.onServerShipLanded;
			Action<SpaceMechanicJobShip> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip> action3 = (Action<SpaceMechanicJobShip>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip>>(ref this.onServerShipLanded, action3, action2);
			}
			while (action != action2);
		}
	}

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x06000082 RID: 130 RVA: 0x0000457C File Offset: 0x0000277C
	// (remove) Token: 0x06000083 RID: 131 RVA: 0x000045B4 File Offset: 0x000027B4
	public event Action<SpaceMechanicJobShip> onServerShipTakenOff
	{
		[CompilerGenerated]
		add
		{
			Action<SpaceMechanicJobShip> action = this.onServerShipTakenOff;
			Action<SpaceMechanicJobShip> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip> action3 = (Action<SpaceMechanicJobShip>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip>>(ref this.onServerShipTakenOff, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<SpaceMechanicJobShip> action = this.onServerShipTakenOff;
			Action<SpaceMechanicJobShip> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip> action3 = (Action<SpaceMechanicJobShip>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip>>(ref this.onServerShipTakenOff, action3, action2);
			}
			while (action != action2);
		}
	}

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x06000084 RID: 132 RVA: 0x000045EC File Offset: 0x000027EC
	// (remove) Token: 0x06000085 RID: 133 RVA: 0x00004624 File Offset: 0x00002824
	public event Action<SpaceMechanicJobShip> onServerShipFlownAway
	{
		[CompilerGenerated]
		add
		{
			Action<SpaceMechanicJobShip> action = this.onServerShipFlownAway;
			Action<SpaceMechanicJobShip> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip> action3 = (Action<SpaceMechanicJobShip>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip>>(ref this.onServerShipFlownAway, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<SpaceMechanicJobShip> action = this.onServerShipFlownAway;
			Action<SpaceMechanicJobShip> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip> action3 = (Action<SpaceMechanicJobShip>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip>>(ref this.onServerShipFlownAway, action3, action2);
			}
			while (action != action2);
		}
	}

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x06000086 RID: 134 RVA: 0x0000465C File Offset: 0x0000285C
	// (remove) Token: 0x06000087 RID: 135 RVA: 0x00004694 File Offset: 0x00002894
	public event Action<SpaceMechanicJobShip> onServerShipCompleted
	{
		[CompilerGenerated]
		add
		{
			Action<SpaceMechanicJobShip> action = this.onServerShipCompleted;
			Action<SpaceMechanicJobShip> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip> action3 = (Action<SpaceMechanicJobShip>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip>>(ref this.onServerShipCompleted, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<SpaceMechanicJobShip> action = this.onServerShipCompleted;
			Action<SpaceMechanicJobShip> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShip> action3 = (Action<SpaceMechanicJobShip>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShip>>(ref this.onServerShipCompleted, action3, action2);
			}
			while (action != action2);
		}
	}

	// Token: 0x06000088 RID: 136 RVA: 0x000046C9 File Offset: 0x000028C9
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x06000089 RID: 137 RVA: 0x000046D1 File Offset: 0x000028D1
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_SET_SOUND_MODE = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSetSoundMode), 1);
	}

	// Token: 0x0600008A RID: 138 RVA: 0x000046F4 File Offset: 0x000028F4
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.allTasks = base.GetComponentsInChildren<SpaceMechanicJobShipTask>();
		this.whiteBoardData.bOpen = true;
		this.whiteBoardData.partsFixedOutOf = 0;
		this.serverSendWhiteboardData = new Action(this.ServerUpdateWhiteBoard);
		for (int i = 0; i < this.allTasks.Length; i++)
		{
			if (this.allTasks[i].IsTaskEnabled())
			{
				this.allTasks[i].SetWhiteboardData(this.whiteBoardData, this.serverSendWhiteboardData);
			}
		}
	}

	// Token: 0x0600008B RID: 139 RVA: 0x00004779 File Offset: 0x00002979
	protected override void NetworkPost(HawkNetworkObject networkObject)
	{
		base.NetworkPost(networkObject);
	}

	// Token: 0x0600008C RID: 140 RVA: 0x00004782 File Offset: 0x00002982
	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.UnassignWhiteboards();
		if (this.soundInstance.isValid())
		{
			this.soundInstance.stop(1);
			this.soundInstance.release();
		}
	}

	// Token: 0x0600008D RID: 141 RVA: 0x000047B6 File Offset: 0x000029B6
	public void DestroyShip()
	{
		this.ReplaceAllNpcsWithSpawnedDynamicNPCs();
		VanishComponent.VanishAndDestroy(base.gameObject);
	}

	// Token: 0x0600008E RID: 142 RVA: 0x000047CC File Offset: 0x000029CC
	internal void TaskCompleted(SpaceMechanicJobShipTask spaceMechanicJobShipTask, bool bCompleteSubSets)
	{
		if (this.completedTasks == null)
		{
			this.completedTasks = new HashSet<SpaceMechanicJobShipTask>();
		}
		Action<SpaceMechanicJobShip, bool> action = this.onServerTaskCompleted;
		if (action != null)
		{
			action(this, bCompleteSubSets);
		}
		if (this.completedTasks.Add(spaceMechanicJobShipTask))
		{
			for (int i = 0; i < this.allTasks.Length; i++)
			{
				if (this.allTasks[i].IsTaskEnabled() && !this.allTasks[i].IsTaskComplete())
				{
					return;
				}
			}
			this.ServerShipCompleted();
		}
	}

	// Token: 0x0600008F RID: 143 RVA: 0x00004848 File Offset: 0x00002A48
	public void ServerUpdateWhiteBoard()
	{
		if (this.whiteboards != null)
		{
			for (int i = 0; i < this.whiteboards.Length; i++)
			{
				this.whiteboards[i].UpdateWhiteboard(this.whiteBoardData);
			}
		}
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00004888 File Offset: 0x00002A88
	public void ReplaceAllNpcsWithSpawnedDynamicNPCs()
	{
		PlayerNPCController[] componentsInChildren = base.GetComponentsInChildren<PlayerNPCController>();
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			PlayerNPCController[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				PlayerNPCVehicle.ReplaceWithSpawnedDynamicNPC(array[i]);
			}
		}
	}

	// Token: 0x06000091 RID: 145 RVA: 0x000048BB File Offset: 0x00002ABB
	private void AnimationEvent_ShipStarted()
	{
		if (this.networkObject != null && this.networkObject.IsServer())
		{
			this.networkObject.SendRPC(this.RPC_CLIENT_SET_SOUND_MODE, true, 6, new object[] { 1 });
		}
	}

	// Token: 0x06000092 RID: 146 RVA: 0x000048F4 File Offset: 0x00002AF4
	private void AnimationEvent_ShipPreLanded()
	{
		if (this.allTasks != null)
		{
			SpaceMechanicJobShipTask[] array = this.allTasks;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnServerShipPreLanded();
			}
		}
		Action<SpaceMechanicJobShip> action = this.onServerShipPreLanded;
		if (action == null)
		{
			return;
		}
		action(this);
	}

	// Token: 0x06000093 RID: 147 RVA: 0x00004938 File Offset: 0x00002B38
	private void AnimationEvent_ShipLanded()
	{
		if (this.networkObject != null && this.networkObject.IsServer())
		{
			this.networkObject.SendRPC(this.RPC_CLIENT_SET_SOUND_MODE, true, 6, new object[] { 0 });
		}
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			Object.Destroy(component);
		}
		if (this.allTasks != null)
		{
			SpaceMechanicJobShipTask[] array = this.allTasks;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnServerShipLanded();
			}
		}
		this.ServerUpdateWhiteBoard();
		Action<SpaceMechanicJobShip> action = this.onServerShipLanded;
		if (action == null)
		{
			return;
		}
		action(this);
	}

	// Token: 0x06000094 RID: 148 RVA: 0x000049D0 File Offset: 0x00002BD0
	private void AnimationEvent_ShipTakenOff()
	{
		if (this.networkObject != null && this.networkObject.IsServer())
		{
			this.networkObject.SendRPC(this.RPC_CLIENT_SET_SOUND_MODE, true, 6, new object[] { 2 });
		}
		Rigidbody orAddComponent = UnityExtensions.GetOrAddComponent<Rigidbody>(base.gameObject);
		if (orAddComponent)
		{
			orAddComponent.isKinematic = true;
		}
		if (this.allTasks != null)
		{
			SpaceMechanicJobShipTask[] array = this.allTasks;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnServerShipTakenOff();
			}
		}
		Action<SpaceMechanicJobShip> action = this.onServerShipTakenOff;
		if (action == null)
		{
			return;
		}
		action(this);
	}

	// Token: 0x06000095 RID: 149 RVA: 0x00004A68 File Offset: 0x00002C68
	private void AnimationEvent_ShipFlownAway()
	{
		if (this.networkObject != null && this.networkObject.IsServer())
		{
			this.networkObject.SendRPC(this.RPC_CLIENT_SET_SOUND_MODE, true, 6, new object[] { 0 });
		}
		if (this.allTasks != null)
		{
			SpaceMechanicJobShipTask[] array = this.allTasks;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnServerShipFlownAway();
			}
		}
		Action<SpaceMechanicJobShip> action = this.onServerShipFlownAway;
		if (action == null)
		{
			return;
		}
		action(this);
	}

	// Token: 0x06000096 RID: 150 RVA: 0x00004AE4 File Offset: 0x00002CE4
	private void ServerShipCompleted()
	{
		Animator component = base.GetComponent<Animator>();
		if (component)
		{
			component.SetTrigger("tTakeOff");
		}
		Action<SpaceMechanicJobShip> action = this.onServerShipCompleted;
		if (action == null)
		{
			return;
		}
		action(this);
	}

	// Token: 0x06000097 RID: 151 RVA: 0x00004B1C File Offset: 0x00002D1C
	internal void AssignWhiteboards(SpaceMechanicWhiteboard[] whiteboards)
	{
		if (this.whiteboards != null)
		{
			Debug.LogError("Can't assign whiteboards as they have already been assigned");
			return;
		}
		this.whiteboards = whiteboards;
		if (whiteboards != null)
		{
			for (int i = 0; i < whiteboards.Length; i++)
			{
				whiteboards[i].AssignCallback(new Action<GUIDComponent>(this.OnWhiteboardAssigned));
			}
		}
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00004B6C File Offset: 0x00002D6C
	internal void UnassignWhiteboards()
	{
		if (this.whiteboards != null)
		{
			for (int i = 0; i < this.whiteboards.Length; i++)
			{
				this.whiteboards[i].UnassignCallback(new Action<GUIDComponent>(this.OnWhiteboardAssigned));
				this.whiteboards[i].UpdateWhiteboard(null);
			}
		}
		this.whiteboards = null;
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00004BCC File Offset: 0x00002DCC
	private void OnWhiteboardAssigned(GUIDComponent component)
	{
		if (this.networkObject != null && this.networkObject.IsServer())
		{
			SpaceMechanicJobWhiteBoard component2 = component.GetComponent<SpaceMechanicJobWhiteBoard>();
			if (component2)
			{
				component2.ServerSendWhiteBoardData(this.whiteBoardData);
			}
		}
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00004C09 File Offset: 0x00002E09
	private IEnumerator UpdateSoundInstance()
	{
		YieldInstruction instruction;
		if (this.networkObject.IsServer())
		{
			instruction = new WaitForFixedUpdate();
		}
		else
		{
			instruction = new WaitForEndOfFrame();
		}
		Vector3 previousPos = base.transform.position;
		for (;;)
		{
			yield return instruction;
			Vector3 position = base.transform.position;
			float num;
			if (this.networkObject.IsServer())
			{
				num = (position - previousPos).magnitude / Time.fixedDeltaTime;
			}
			else
			{
				num = (position - previousPos).magnitude / Time.deltaTime;
			}
			previousPos = position;
			if (this.soundInstance.isValid())
			{
				this.soundInstance.setParameterByName("VehicleSpeed", num, false);
			}
		}
		yield break;
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00004C18 File Offset: 0x00002E18
	private void ClientSetSoundMode(HawkNetReader reader, HawkRPCInfo info)
	{
		SpaceMechanicJobShip.SpaceMechanicJobShipSoundMode spaceMechanicJobShipSoundMode = (SpaceMechanicJobShip.SpaceMechanicJobShipSoundMode)reader.ReadByte();
		if (this.soundMode != spaceMechanicJobShipSoundMode)
		{
			this.soundMode = spaceMechanicJobShipSoundMode;
			if (this.updateSoundInstanceCoroutine != null)
			{
				base.StopCoroutine(this.updateSoundInstanceCoroutine);
			}
			if (this.soundInstance.isValid())
			{
				this.soundInstance.triggerCue();
				this.soundInstance.release();
				this.soundInstance.clearHandle();
			}
			if (spaceMechanicJobShipSoundMode != SpaceMechanicJobShip.SpaceMechanicJobShipSoundMode.Landing)
			{
				if (spaceMechanicJobShipSoundMode == SpaceMechanicJobShip.SpaceMechanicJobShipSoundMode.TakeOff && !string.IsNullOrEmpty(this.takeOffSoundLoop))
				{
					this.soundInstance = RuntimeManager.CreateInstance(this.takeOffSoundLoop);
					RuntimeManager.AttachInstanceToGameObject(this.soundInstance, base.transform, null);
					this.soundInstance.start();
				}
			}
			else if (!string.IsNullOrEmpty(this.landingSoundLoop))
			{
				this.soundInstance = RuntimeManager.CreateInstance(this.landingSoundLoop);
				RuntimeManager.AttachInstanceToGameObject(this.soundInstance, base.transform, null);
				this.soundInstance.start();
			}
			if (spaceMechanicJobShipSoundMode != SpaceMechanicJobShip.SpaceMechanicJobShipSoundMode.None)
			{
				this.updateSoundInstanceCoroutine = base.StartCoroutine(this.UpdateSoundInstance());
			}
		}
	}

	// Token: 0x0600009C RID: 156 RVA: 0x00004D1A File Offset: 0x00002F1A
	public SpaceMechanicJobShip()
	{
	}

	// Token: 0x04000073 RID: 115
	private byte RPC_CLIENT_SET_SOUND_MODE;

	// Token: 0x04000074 RID: 116
	[CompilerGenerated]
	private Action<SpaceMechanicJobShip, bool> onServerTaskCompleted;

	// Token: 0x04000075 RID: 117
	[CompilerGenerated]
	private Action<SpaceMechanicJobShip> onServerShipPreLanded;

	// Token: 0x04000076 RID: 118
	[CompilerGenerated]
	private Action<SpaceMechanicJobShip> onServerShipLanded;

	// Token: 0x04000077 RID: 119
	[CompilerGenerated]
	private Action<SpaceMechanicJobShip> onServerShipTakenOff;

	// Token: 0x04000078 RID: 120
	[CompilerGenerated]
	private Action<SpaceMechanicJobShip> onServerShipFlownAway;

	// Token: 0x04000079 RID: 121
	[CompilerGenerated]
	private Action<SpaceMechanicJobShip> onServerShipCompleted;

	// Token: 0x0400007A RID: 122
	[SerializeField]
	[EventRef]
	private string landingSoundLoop = "event:/Vehicles_Space/Spaceships/MechanicJob_Spaceship_Speeder_Landing";

	// Token: 0x0400007B RID: 123
	[SerializeField]
	[EventRef]
	private string takeOffSoundLoop = "event:/Vehicles_Space/Spaceships/MechanicJob_Spaceship_Speeder_TakeOff";

	// Token: 0x0400007C RID: 124
	private HashSet<SpaceMechanicJobShipTask> completedTasks;

	// Token: 0x0400007D RID: 125
	private SpaceMechanicJobShipTask[] allTasks;

	// Token: 0x0400007E RID: 126
	private SpaceMechanicWhiteboard[] whiteboards;

	// Token: 0x0400007F RID: 127
	private SpaceMechanicJobWhiteBoardData whiteBoardData = new SpaceMechanicJobWhiteBoardData();

	// Token: 0x04000080 RID: 128
	private Action serverSendWhiteboardData;

	// Token: 0x04000081 RID: 129
	private EventInstance soundInstance;

	// Token: 0x04000082 RID: 130
	private SpaceMechanicJobShip.SpaceMechanicJobShipSoundMode soundMode;

	// Token: 0x04000083 RID: 131
	private Coroutine updateSoundInstanceCoroutine;

	// Token: 0x02000062 RID: 98
	private enum SpaceMechanicJobShipSoundMode : byte
	{
		// Token: 0x04000245 RID: 581
		None,
		// Token: 0x04000246 RID: 582
		Landing,
		// Token: 0x04000247 RID: 583
		TakeOff
	}

	// Token: 0x02000063 RID: 99
	[CompilerGenerated]
	private sealed class <UpdateSoundInstance>d__48 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x060002CF RID: 719 RVA: 0x0000EBBB File Offset: 0x0000CDBB
		[DebuggerHidden]
		public <UpdateSoundInstance>d__48(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0000EBCA File Offset: 0x0000CDCA
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0000EBCC File Offset: 0x0000CDCC
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpaceMechanicJobShip spaceMechanicJobShip = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				Vector3 position = spaceMechanicJobShip.transform.position;
				float num2;
				if (spaceMechanicJobShip.networkObject.IsServer())
				{
					num2 = (position - previousPos).magnitude / Time.fixedDeltaTime;
				}
				else
				{
					num2 = (position - previousPos).magnitude / Time.deltaTime;
				}
				previousPos = position;
				if (spaceMechanicJobShip.soundInstance.isValid())
				{
					spaceMechanicJobShip.soundInstance.setParameterByName("VehicleSpeed", num2, false);
				}
			}
			else
			{
				this.<>1__state = -1;
				if (spaceMechanicJobShip.networkObject.IsServer())
				{
					instruction = new WaitForFixedUpdate();
				}
				else
				{
					instruction = new WaitForEndOfFrame();
				}
				previousPos = spaceMechanicJobShip.transform.position;
			}
			this.<>2__current = instruction;
			this.<>1__state = 1;
			return true;
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x0000ECCA File Offset: 0x0000CECA
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0000ECD2 File Offset: 0x0000CED2
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x0000ECD9 File Offset: 0x0000CED9
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000248 RID: 584
		private int <>1__state;

		// Token: 0x04000249 RID: 585
		private object <>2__current;

		// Token: 0x0400024A RID: 586
		public SpaceMechanicJobShip <>4__this;

		// Token: 0x0400024B RID: 587
		private YieldInstruction <instruction>5__2;

		// Token: 0x0400024C RID: 588
		private Vector3 <previousPos>5__3;
	}
}
