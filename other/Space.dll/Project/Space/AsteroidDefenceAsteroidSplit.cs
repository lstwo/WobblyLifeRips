using System;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class AsteroidDefenceAsteroidSplit : AsteroidDefenceAsteroid
{
	// Token: 0x06000019 RID: 25 RVA: 0x00002674 File Offset: 0x00000874
	protected override void OnExploded()
	{
		base.OnExploded();
		if (this.networkObject.IsServer() && this.splitDatas != null)
		{
			for (int i = 0; i < this.splitDatas.Length; i++)
			{
				this.splitDatas[i].Spawn(this.syncData, this.serverTarget, this.onSpawnedAsteroid);
			}
		}
	}

	// Token: 0x0600001A RID: 26 RVA: 0x000026CE File Offset: 0x000008CE
	public AsteroidDefenceAsteroidSplit()
	{
	}

	// Token: 0x04000018 RID: 24
	public Action<AsteroidDefenceAsteroid> onSpawnedAsteroid;

	// Token: 0x04000019 RID: 25
	[SerializeField]
	private AsteroidDefenceAsteroidSplitData[] splitDatas;
}
