using System;
using FMODUnity;
using UnityEngine;

// Token: 0x02000008 RID: 8
[Serializable]
internal class AsteroidDefenceWaveSpawnData
{
	// Token: 0x0600001E RID: 30 RVA: 0x00002724 File Offset: 0x00000924
	public void Initalize(AsteroidDefenceJobMission jobMission)
	{
		this.jobMission = jobMission;
		int count = jobMission.GetControllersOnJob().Count;
		if (count > 1)
		{
			this.spawnCount = (int)((float)this.spawnCount * (1f + 0.25f * (float)count));
		}
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00002768 File Offset: 0x00000968
	internal void SubWaveStarted()
	{
		if (this.jobMission && this.jobMission.GetLocalControllersOnJob().Count > 0 && !string.IsNullOrEmpty(this.startSound2D))
		{
			RuntimeManager.PlayOneShot(this.startSound2D, default(Vector3));
		}
	}

	// Token: 0x06000020 RID: 32 RVA: 0x000027B6 File Offset: 0x000009B6
	public void WaveStarted()
	{
		this.waveStartedTime = Time.time;
	}

	// Token: 0x06000021 RID: 33 RVA: 0x000027C4 File Offset: 0x000009C4
	public void Update()
	{
		if (Time.time - this.waveStartedTime < this.delay)
		{
			return;
		}
		if (!this.bInitSubWaveStarted)
		{
			this.bInitSubWaveStarted = true;
			if (this.jobMission)
			{
				this.jobMission.ServerSubWaveStarted(this);
			}
		}
		if (Time.time - this.time >= this.spawnRateSeconds)
		{
			this.time = Time.time;
			if (this.spawnCount > 0)
			{
				this.SpawnAsteroid();
			}
		}
	}

	// Token: 0x06000022 RID: 34 RVA: 0x0000283C File Offset: 0x00000A3C
	private void SpawnAsteroid()
	{
		if (this.prefabs != null && this.prefabs.Length != 0)
		{
			this.jobMission.ServerSpawnAsteroid(this.prefabs[Random.Range(0, this.prefabs.Length)], this.spawnAngleMin, this.spawnAngleMax);
			this.spawnCount--;
		}
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00002894 File Offset: 0x00000A94
	public bool HasFinished()
	{
		return this.spawnCount == 0;
	}

	// Token: 0x06000024 RID: 36 RVA: 0x0000289F File Offset: 0x00000A9F
	public AsteroidDefenceWaveSpawnData()
	{
	}

	// Token: 0x0400001C RID: 28
	[SerializeField]
	private int spawnCount;

	// Token: 0x0400001D RID: 29
	[SerializeField]
	private AsteroidDefenceAsteroid[] prefabs;

	// Token: 0x0400001E RID: 30
	[SerializeField]
	private float spawnAngleMin = 45f;

	// Token: 0x0400001F RID: 31
	[SerializeField]
	private float spawnAngleMax = 45f;

	// Token: 0x04000020 RID: 32
	[SerializeField]
	private float spawnRateSeconds = 0.3f;

	// Token: 0x04000021 RID: 33
	[SerializeField]
	private float delay;

	// Token: 0x04000022 RID: 34
	[SerializeField]
	[EventRef]
	private string startSound2D;

	// Token: 0x04000023 RID: 35
	private AsteroidDefenceJobMission jobMission;

	// Token: 0x04000024 RID: 36
	private float time;

	// Token: 0x04000025 RID: 37
	private float waveStartedTime;

	// Token: 0x04000026 RID: 38
	private bool bInitSubWaveStarted;
}
