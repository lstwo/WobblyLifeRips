using System;
using System.Runtime.CompilerServices;
using HawkNetworking;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x0200000E RID: 14
[Serializable]
internal class SpaceMechnaicToolSpawn
{
	// Token: 0x06000061 RID: 97 RVA: 0x00003AF4 File Offset: 0x00001CF4
	public void Spawn()
	{
		if (this.bSpawn || this.instance)
		{
			return;
		}
		this.bSpawn = true;
		NetworkPrefab.SpawnNetworkPrefab(this.prefab, delegate(HawkNetworkBehaviour x)
		{
			if (x)
			{
				if (this.bSpawn)
				{
					this.instance = x;
					return;
				}
				x.networkObject.Destroy();
			}
		}, new Vector3?(this.spawnTransform.position), new Quaternion?(this.spawnTransform.rotation), null, true, false, false, true);
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00003B5A File Offset: 0x00001D5A
	public void Destroy()
	{
		if (!this.bSpawn)
		{
			return;
		}
		this.bSpawn = false;
		if (this.instance)
		{
			VanishComponent.VanishAndDestroy(this.instance.gameObject);
		}
	}

	// Token: 0x06000063 RID: 99 RVA: 0x00003B89 File Offset: 0x00001D89
	public SpaceMechnaicToolSpawn()
	{
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00003B91 File Offset: 0x00001D91
	[CompilerGenerated]
	private void <Spawn>b__4_0(HawkNetworkBehaviour x)
	{
		if (x)
		{
			if (this.bSpawn)
			{
				this.instance = x;
				return;
			}
			x.networkObject.Destroy();
		}
	}

	// Token: 0x04000055 RID: 85
	[SerializeField]
	private AssetReference prefab;

	// Token: 0x04000056 RID: 86
	[SerializeField]
	private Transform spawnTransform;

	// Token: 0x04000057 RID: 87
	private HawkNetworkBehaviour instance;

	// Token: 0x04000058 RID: 88
	private bool bSpawn;
}
