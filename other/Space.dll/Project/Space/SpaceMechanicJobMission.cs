using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FMODUnity;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000011 RID: 17
internal class SpaceMechanicJobMission : JobMission
{
	// Token: 0x0600006A RID: 106 RVA: 0x00003C7C File Offset: 0x00001E7C
	protected override void Awake()
	{
		base.Awake();
		if (Application.isPlaying)
		{
			this.shuffledSpawnData = new List<SpaceMechanicSpawnData>(this.spawnDatas);
			for (int i = 0; i < this.shuffledSpawnData.Count; i++)
			{
				int num = Random.Range(0, this.shuffledSpawnData.Count);
				SpaceMechanicSpawnData spaceMechanicSpawnData = this.shuffledSpawnData[i];
				SpaceMechanicSpawnData spaceMechanicSpawnData2 = this.shuffledSpawnData[num];
				this.shuffledSpawnData[i] = spaceMechanicSpawnData2;
				this.shuffledSpawnData[i] = spaceMechanicSpawnData;
			}
			this.spawnDataInBayDic = new Dictionary<SpaceMechanicBays, List<SpaceMechanicSpawnData>>(this.spawnDatas.Length);
			Array values = Enum.GetValues(typeof(SpaceMechanicBays));
			this.shuffledBays = new List<SpaceMechanicBays>(values.Length);
			foreach (object obj in values)
			{
				SpaceMechanicBays spaceMechanicBays = (SpaceMechanicBays)obj;
				this.shuffledBays.Add(spaceMechanicBays);
			}
			for (int j = 0; j < this.spawnDatas.Length; j++)
			{
				SpaceMechanicBays bayNumMask = this.spawnDatas[j].GetBayNumMask();
				foreach (object obj2 in values)
				{
					SpaceMechanicBays spaceMechanicBays2 = (SpaceMechanicBays)obj2;
					if ((spaceMechanicBays2 & bayNumMask) != (SpaceMechanicBays)0)
					{
						List<SpaceMechanicSpawnData> list;
						if (this.spawnDataInBayDic.TryGetValue(spaceMechanicBays2, out list))
						{
							list.Add(this.spawnDatas[j]);
						}
						else
						{
							this.spawnDataInBayDic.Add(spaceMechanicBays2, new List<SpaceMechanicSpawnData> { this.spawnDatas[j] });
						}
					}
				}
			}
		}
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00003E50 File Offset: 0x00002050
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_SHIP_COMPLETE = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientShipComplete), 1);
		this.RPC_CLIENT_TASK_COMPLETE = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientTaskComplete), 1);
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00003E8B File Offset: 0x0000208B
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		if (networkObject.IsServer())
		{
			this.serverSpawnShipsCoroutine = base.StartCoroutine(this.ServerSpawnShipsUpdate());
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00003EB0 File Offset: 0x000020B0
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (!this.bIsApplicationQuit && this.spawnDatas != null)
		{
			SpaceMechanicSpawnData[] array = this.spawnDatas;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Destroy();
			}
		}
		if (this.toolSpawns != null)
		{
			for (int j = 0; j < this.toolSpawns.Length; j++)
			{
				this.toolSpawns[j].Destroy();
			}
		}
	}

	// Token: 0x0600006E RID: 110 RVA: 0x00003F18 File Offset: 0x00002118
	public override void StartJob(JobDispensorBehaviour jobDispensor, List<PlayerController> controllers)
	{
		base.StartJob(jobDispensor, controllers);
		if (this.toolSpawns != null)
		{
			for (int i = 0; i < this.toolSpawns.Length; i++)
			{
				this.toolSpawns[i].Spawn();
			}
		}
		this.TrySpawnShip();
	}

	// Token: 0x0600006F RID: 111 RVA: 0x00003F5C File Offset: 0x0000215C
	private IEnumerator ServerSpawnShipsUpdate()
	{
		for (;;)
		{
			float time = Time.time;
			float seconds = Random.Range(3f, 6f);
			while (Time.time - time < seconds)
			{
				yield return null;
			}
			this.TrySpawnShip();
		}
		yield break;
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00003F6C File Offset: 0x0000216C
	private bool TrySpawnShip()
	{
		if (this.freeBaysMask == (SpaceMechanicBays)0)
		{
			return false;
		}
		if (this.controllersOnJob.Count <= this.activeSpawns.Count)
		{
			return false;
		}
		if (this.activeSpawns.Count >= this.maxShipsAtOnce)
		{
			return false;
		}
		for (int i = 0; i < this.shuffledBays.Count; i++)
		{
			int num = Random.Range(0, this.shuffledBays.Count);
			SpaceMechanicBays spaceMechanicBays = this.shuffledBays[i];
			SpaceMechanicBays spaceMechanicBays2 = this.shuffledBays[num];
			this.shuffledBays[i] = spaceMechanicBays2;
			this.shuffledBays[num] = spaceMechanicBays;
		}
		SpaceMechanicBays? spaceMechanicBays3 = null;
		for (int j = 0; j < this.shuffledBays.Count; j++)
		{
			if ((this.shuffledBays[j] & this.freeBaysMask) != (SpaceMechanicBays)0)
			{
				spaceMechanicBays3 = new SpaceMechanicBays?(this.shuffledBays[j]);
				List<SpaceMechanicSpawnData> list;
				if (this.spawnDataInBayDic.TryGetValue(spaceMechanicBays3.Value, out list))
				{
					for (int k = 0; k < list.Count; k++)
					{
						int num2 = Random.Range(0, list.Count);
						SpaceMechanicSpawnData spaceMechanicSpawnData = list[k];
						SpaceMechanicSpawnData spaceMechanicSpawnData2 = list[num2];
						list[num2] = spaceMechanicSpawnData;
						list[k] = spaceMechanicSpawnData2;
					}
					for (int l = 0; l < list.Count; l++)
					{
						SpaceMechanicSpawnData spaceMechanicSpawnData3 = list[l];
						if (spaceMechanicSpawnData3.Spawn(new Action<SpaceMechanicSpawnData>(this.OnShipSpawned)))
						{
							this.activeSpawns.Add(spaceMechanicSpawnData3);
							this.freeBaysMask &= ~spaceMechanicSpawnData3.GetBayNumMask();
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000071 RID: 113 RVA: 0x0000412C File Offset: 0x0000232C
	private void OnShipSpawned(SpaceMechanicSpawnData spawnData)
	{
		if (!this)
		{
			if (spawnData.GetJobShip())
			{
				spawnData.GetJobShip().networkObject.Destroy();
			}
			return;
		}
		SpaceMechanicJobShip jobShip = spawnData.GetJobShip();
		if (jobShip)
		{
			List<SpaceMechanicWhiteboard> list = new List<SpaceMechanicWhiteboard>();
			if (this.whiteboards != null)
			{
				for (int i = 0; i < this.whiteboards.Length; i++)
				{
					if (this.whiteboards[i].IsInBay(spawnData.GetBayNumMask()))
					{
						list.Add(this.whiteboards[i]);
					}
				}
			}
			jobShip.AssignWhiteboards(list.ToArray());
			jobShip.onServerTaskCompleted += this.OnServerTaskCompleted;
			jobShip.onServerShipFlownAway += this.OnServerShipFlownAway;
			jobShip.onServerShipCompleted += this.OnServerShipCompleted;
			return;
		}
		this.freeBaysMask |= spawnData.GetBayNumMask();
		this.activeSpawns.Remove(spawnData);
		this.TrySpawnShip();
	}

	// Token: 0x06000072 RID: 114 RVA: 0x0000422A File Offset: 0x0000242A
	private void OnServerShipCompleted(SpaceMechanicJobShip obj)
	{
		base.IterateControllersOnJob(delegate(PlayerController x)
		{
			if (x)
			{
				this.networkObject.SendRPC(this.RPC_CLIENT_SHIP_COMPLETE, x.networkObject.GetOwner(), Array.Empty<object>());
			}
		});
	}

	// Token: 0x06000073 RID: 115 RVA: 0x0000423E File Offset: 0x0000243E
	private void OnServerTaskCompleted(SpaceMechanicJobShip ship, bool bCompleteSubSets)
	{
		if (bCompleteSubSets)
		{
			base.IterateControllersOnJob(delegate(PlayerController x)
			{
				if (x)
				{
					this.networkObject.SendRPC(this.RPC_CLIENT_TASK_COMPLETE, x.networkObject.GetOwner(), Array.Empty<object>());
				}
			});
		}
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00004255 File Offset: 0x00002455
	protected override void ServerJobFinished()
	{
		base.ServerJobFinished();
		if (this.serverSpawnShipsCoroutine != null)
		{
			base.StopCoroutine(this.serverSpawnShipsCoroutine);
			this.serverSpawnShipsCoroutine = null;
		}
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00004278 File Offset: 0x00002478
	private void OnServerShipFlownAway(SpaceMechanicJobShip jobShip)
	{
		if (this.activeSpawns != null)
		{
			foreach (SpaceMechanicSpawnData spaceMechanicSpawnData in this.activeSpawns)
			{
				if (spaceMechanicSpawnData.GetJobShip() == jobShip)
				{
					this.freeBaysMask |= spaceMechanicSpawnData.GetBayNumMask();
					this.activeSpawns.Remove(spaceMechanicSpawnData);
					break;
				}
			}
			if (this.activeSpawns.Count == 0)
			{
				this.TrySpawnShip();
			}
		}
		jobShip.networkObject.Destroy();
	}

	// Token: 0x06000076 RID: 118 RVA: 0x0000431C File Offset: 0x0000251C
	private void ClientTaskComplete(HawkNetReader reader, HawkRPCInfo info)
	{
		if (!string.IsNullOrEmpty(this.taskCompleteSound))
		{
			RuntimeManager.PlayOneShot(this.taskCompleteSound, default(Vector3));
		}
	}

	// Token: 0x06000077 RID: 119 RVA: 0x0000434C File Offset: 0x0000254C
	private void ClientShipComplete(HawkNetReader reader, HawkRPCInfo info)
	{
		if (!string.IsNullOrEmpty(this.shipCompleteSound))
		{
			RuntimeManager.PlayOneShot(this.shipCompleteSound, default(Vector3));
		}
	}

	// Token: 0x06000078 RID: 120 RVA: 0x0000437A File Offset: 0x0000257A
	public override IHawkMessage GetJobInformationMessage()
	{
		return this.jobInformation;
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00004384 File Offset: 0x00002584
	public SpaceMechanicJobMission()
	{
	}

	// Token: 0x0600007A RID: 122 RVA: 0x000043D5 File Offset: 0x000025D5
	[CompilerGenerated]
	private void <OnServerShipCompleted>b__23_0(PlayerController x)
	{
		if (x)
		{
			this.networkObject.SendRPC(this.RPC_CLIENT_SHIP_COMPLETE, x.networkObject.GetOwner(), Array.Empty<object>());
		}
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00004400 File Offset: 0x00002600
	[CompilerGenerated]
	private void <OnServerTaskCompleted>b__24_0(PlayerController x)
	{
		if (x)
		{
			this.networkObject.SendRPC(this.RPC_CLIENT_TASK_COMPLETE, x.networkObject.GetOwner(), Array.Empty<object>());
		}
	}

	// Token: 0x04000064 RID: 100
	private byte RPC_CLIENT_SHIP_COMPLETE;

	// Token: 0x04000065 RID: 101
	private byte RPC_CLIENT_TASK_COMPLETE;

	// Token: 0x04000066 RID: 102
	[SerializeField]
	private SpaceMechanicSpawnData[] spawnDatas;

	// Token: 0x04000067 RID: 103
	[SerializeField]
	private int maxShipsAtOnce = 2;

	// Token: 0x04000068 RID: 104
	[SerializeField]
	private SpaceMechanicWhiteboard[] whiteboards;

	// Token: 0x04000069 RID: 105
	[SerializeField]
	private SpaceMechnaicToolSpawn[] toolSpawns;

	// Token: 0x0400006A RID: 106
	[SerializeField]
	[EventRef]
	private string shipCompleteSound = "event:/UI/UI_Space_Job_Task_Complete";

	// Token: 0x0400006B RID: 107
	[SerializeField]
	[EventRef]
	private string taskCompleteSound = "event:/UI/UI_Space_Job_Task_StageComplete";

	// Token: 0x0400006C RID: 108
	private Dictionary<SpaceMechanicBays, List<SpaceMechanicSpawnData>> spawnDataInBayDic;

	// Token: 0x0400006D RID: 109
	private List<SpaceMechanicSpawnData> activeSpawns = new List<SpaceMechanicSpawnData>();

	// Token: 0x0400006E RID: 110
	private SpaceMechanicJobInformation jobInformation = new SpaceMechanicJobInformation();

	// Token: 0x0400006F RID: 111
	private List<SpaceMechanicSpawnData> shuffledSpawnData;

	// Token: 0x04000070 RID: 112
	private SpaceMechanicBays freeBaysMask = (SpaceMechanicBays)255;

	// Token: 0x04000071 RID: 113
	private List<SpaceMechanicBays> shuffledBays;

	// Token: 0x04000072 RID: 114
	private Coroutine serverSpawnShipsCoroutine;

	// Token: 0x02000061 RID: 97
	[CompilerGenerated]
	private sealed class <ServerSpawnShipsUpdate>d__20 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x060002C9 RID: 713 RVA: 0x0000EB13 File Offset: 0x0000CD13
		[DebuggerHidden]
		public <ServerSpawnShipsUpdate>d__20(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0000EB22 File Offset: 0x0000CD22
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000EB24 File Offset: 0x0000CD24
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpaceMechanicJobMission spaceMechanicJobMission = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				goto IL_0057;
			}
			else
			{
				this.<>1__state = -1;
			}
			IL_001E:
			time = Time.time;
			seconds = Random.Range(3f, 6f);
			IL_0057:
			if (Time.time - time >= seconds)
			{
				spaceMechanicJobMission.TrySpawnShip();
				goto IL_001E;
			}
			this.<>2__current = null;
			this.<>1__state = 1;
			return true;
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x060002CC RID: 716 RVA: 0x0000EBA4 File Offset: 0x0000CDA4
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0000EBAC File Offset: 0x0000CDAC
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x060002CE RID: 718 RVA: 0x0000EBB3 File Offset: 0x0000CDB3
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x0400023F RID: 575
		private int <>1__state;

		// Token: 0x04000240 RID: 576
		private object <>2__current;

		// Token: 0x04000241 RID: 577
		public SpaceMechanicJobMission <>4__this;

		// Token: 0x04000242 RID: 578
		private float <time>5__2;

		// Token: 0x04000243 RID: 579
		private float <seconds>5__3;
	}
}
