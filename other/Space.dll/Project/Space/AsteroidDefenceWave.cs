using System;
using UnityEngine.Localization;

// Token: 0x02000009 RID: 9
[Serializable]
internal class AsteroidDefenceWave
{
	// Token: 0x06000025 RID: 37 RVA: 0x000028C8 File Offset: 0x00000AC8
	public void Initalize(AsteroidDefenceJobMission jobMission)
	{
		if (this.spawnDatas != null)
		{
			for (int i = 0; i < this.spawnDatas.Length; i++)
			{
				this.spawnDatas[i].Initalize(jobMission);
			}
		}
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00002900 File Offset: 0x00000B00
	public void WaveStarted()
	{
		if (this.spawnDatas != null)
		{
			for (int i = 0; i < this.spawnDatas.Length; i++)
			{
				this.spawnDatas[i].WaveStarted();
			}
		}
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002938 File Offset: 0x00000B38
	public void Update()
	{
		if (this.spawnDatas != null)
		{
			for (int i = 0; i < this.spawnDatas.Length; i++)
			{
				this.spawnDatas[i].Update();
			}
		}
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00002970 File Offset: 0x00000B70
	public bool HasFinished()
	{
		if (this.spawnDatas != null)
		{
			for (int i = 0; i < this.spawnDatas.Length; i++)
			{
				if (!this.spawnDatas[i].HasFinished())
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06000029 RID: 41 RVA: 0x000029AA File Offset: 0x00000BAA
	public AsteroidDefenceWave()
	{
	}

	// Token: 0x04000027 RID: 39
	public LocalizedString waveText;

	// Token: 0x04000028 RID: 40
	public AsteroidDefenceWaveSpawnData[] spawnDatas;
}
