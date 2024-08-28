using System;
using System.Runtime.CompilerServices;
using HawkNetworking;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x0200000F RID: 15
[Serializable]
internal class SpaceMechanicSpawnData
{
	// Token: 0x06000065 RID: 101 RVA: 0x00003BB8 File Offset: 0x00001DB8
	public bool Spawn(Action<SpaceMechanicSpawnData> onSpawned)
	{
		if (this.shipPrefab != null && this.shipPrefab.RuntimeKeyIsValid() && !this.bPendingSpawn)
		{
			this.bSpawned = true;
			this.bPendingSpawn = true;
			NetworkPrefab.SpawnNetworkPrefab(this.shipPrefab, delegate(HawkNetworkBehaviour shipInstance)
			{
				this.bPendingSpawn = false;
				SpaceMechanicJobShip spaceMechanicJobShip = shipInstance as SpaceMechanicJobShip;
				if (spaceMechanicJobShip)
				{
					if (this.bSpawned)
					{
						Animator component = spaceMechanicJobShip.gameObject.GetComponent<Animator>();
						component.updateMode = 1;
						component.runtimeAnimatorController = this.controller;
						this.jobShip = spaceMechanicJobShip;
					}
					else
					{
						shipInstance.networkObject.Destroy();
					}
				}
				else if (shipInstance)
				{
					shipInstance.networkObject.Destroy();
				}
				Action<SpaceMechanicSpawnData> onSpawned2 = onSpawned;
				if (onSpawned2 == null)
				{
					return;
				}
				onSpawned2(this);
			}, new Vector3?(Vector3.up * 10000f), null, null, true, true, false, true);
			return true;
		}
		return false;
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00003C40 File Offset: 0x00001E40
	public void Destroy()
	{
		this.bSpawned = false;
		if (this.jobShip)
		{
			this.jobShip.DestroyShip();
		}
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00003C61 File Offset: 0x00001E61
	public SpaceMechanicBays GetBayNumMask()
	{
		return this.bayNumMask;
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00003C69 File Offset: 0x00001E69
	public SpaceMechanicJobShip GetJobShip()
	{
		return this.jobShip;
	}

	// Token: 0x06000069 RID: 105 RVA: 0x00003C71 File Offset: 0x00001E71
	public SpaceMechanicSpawnData()
	{
	}

	// Token: 0x04000059 RID: 89
	[SerializeField]
	private SpaceMechanicBays bayNumMask;

	// Token: 0x0400005A RID: 90
	[SerializeField]
	private RuntimeAnimatorController controller;

	// Token: 0x0400005B RID: 91
	[SerializeField]
	private AssetReference shipPrefab;

	// Token: 0x0400005C RID: 92
	private SpaceMechanicJobShip jobShip;

	// Token: 0x0400005D RID: 93
	private bool bSpawned;

	// Token: 0x0400005E RID: 94
	private bool bPendingSpawn;

	// Token: 0x02000060 RID: 96
	[CompilerGenerated]
	private sealed class <>c__DisplayClass6_0
	{
		// Token: 0x060002C7 RID: 711 RVA: 0x0000EA6D File Offset: 0x0000CC6D
		public <>c__DisplayClass6_0()
		{
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0000EA78 File Offset: 0x0000CC78
		internal void <Spawn>b__0(HawkNetworkBehaviour shipInstance)
		{
			this.<>4__this.bPendingSpawn = false;
			SpaceMechanicJobShip spaceMechanicJobShip = shipInstance as SpaceMechanicJobShip;
			if (spaceMechanicJobShip)
			{
				if (this.<>4__this.bSpawned)
				{
					Animator component = spaceMechanicJobShip.gameObject.GetComponent<Animator>();
					component.updateMode = 1;
					component.runtimeAnimatorController = this.<>4__this.controller;
					this.<>4__this.jobShip = spaceMechanicJobShip;
				}
				else
				{
					shipInstance.networkObject.Destroy();
				}
			}
			else if (shipInstance)
			{
				shipInstance.networkObject.Destroy();
			}
			Action<SpaceMechanicSpawnData> action = this.onSpawned;
			if (action == null)
			{
				return;
			}
			action(this.<>4__this);
		}

		// Token: 0x0400023D RID: 573
		public SpaceMechanicSpawnData <>4__this;

		// Token: 0x0400023E RID: 574
		public Action<SpaceMechanicSpawnData> onSpawned;
	}
}
