using System;
using System.Runtime.CompilerServices;
using HawkNetworking;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x02000005 RID: 5
[Serializable]
internal class AsteroidDefenceAsteroidSplitData
{
	// Token: 0x06000017 RID: 23 RVA: 0x000025F4 File Offset: 0x000007F4
	public void Spawn(AsteroidDefenceAsteroidSync sync, Transform attackPoint, Action<AsteroidDefenceAsteroid> onSpawnedAsteroid)
	{
		if (this.spawnTransform)
		{
			NetworkPrefab.SpawnNetworkPrefab(this.prefab, delegate(HawkNetworkBehaviour x)
			{
				AsteroidDefenceAsteroid asteroidDefenceAsteroid = x as AsteroidDefenceAsteroid;
				if (asteroidDefenceAsteroid)
				{
					sync.position = this.spawnTransform.position;
					if (attackPoint)
					{
						sync.velocity = (attackPoint.position - this.spawnTransform.position).normalized;
					}
					else
					{
						x.networkObject.Destroy(1f);
					}
					asteroidDefenceAsteroid.ServerSetData(attackPoint, sync, true);
					Action<AsteroidDefenceAsteroid> onSpawnedAsteroid2 = onSpawnedAsteroid;
					if (onSpawnedAsteroid2 == null)
					{
						return;
					}
					onSpawnedAsteroid2(asteroidDefenceAsteroid);
				}
			}, new Vector3?(this.spawnTransform.position), new Quaternion?(this.spawnTransform.rotation), null, true, true, false, true);
		}
	}

	// Token: 0x06000018 RID: 24 RVA: 0x0000266C File Offset: 0x0000086C
	public AsteroidDefenceAsteroidSplitData()
	{
	}

	// Token: 0x04000016 RID: 22
	[SerializeField]
	private AssetReference prefab;

	// Token: 0x04000017 RID: 23
	[SerializeField]
	private Transform spawnTransform;

	// Token: 0x0200005B RID: 91
	[CompilerGenerated]
	private sealed class <>c__DisplayClass2_0
	{
		// Token: 0x060002B9 RID: 697 RVA: 0x0000E753 File Offset: 0x0000C953
		public <>c__DisplayClass2_0()
		{
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000E75C File Offset: 0x0000C95C
		internal void <Spawn>b__0(HawkNetworkBehaviour x)
		{
			AsteroidDefenceAsteroid asteroidDefenceAsteroid = x as AsteroidDefenceAsteroid;
			if (asteroidDefenceAsteroid)
			{
				this.sync.position = this.<>4__this.spawnTransform.position;
				if (this.attackPoint)
				{
					this.sync.velocity = (this.attackPoint.position - this.<>4__this.spawnTransform.position).normalized;
				}
				else
				{
					x.networkObject.Destroy(1f);
				}
				asteroidDefenceAsteroid.ServerSetData(this.attackPoint, this.sync, true);
				Action<AsteroidDefenceAsteroid> action = this.onSpawnedAsteroid;
				if (action == null)
				{
					return;
				}
				action(asteroidDefenceAsteroid);
			}
		}

		// Token: 0x04000230 RID: 560
		public AsteroidDefenceAsteroidSync sync;

		// Token: 0x04000231 RID: 561
		public AsteroidDefenceAsteroidSplitData <>4__this;

		// Token: 0x04000232 RID: 562
		public Transform attackPoint;

		// Token: 0x04000233 RID: 563
		public Action<AsteroidDefenceAsteroid> onSpawnedAsteroid;
	}
}
