using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FMODUnity;
using HawkNetworking;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

// Token: 0x0200000A RID: 10
internal class AsteroidDefenceJobMission : JobMission
{
	// Token: 0x0600002A RID: 42 RVA: 0x000029B4 File Offset: 0x00000BB4
	protected override void Awake()
	{
		base.Awake();
		this.cannons = new List<ActionEnterExitInteract>();
		if (this.spacePortCannonsGuids != null)
		{
			string[] array = this.spacePortCannonsGuids;
			for (int i = 0; i < array.Length; i++)
			{
				Guid guid;
				if (Guid.TryParse(array[i], out guid))
				{
					GUIDComponent firstGUIDComponent = UnitySingleton<GUIDComponentManager>.Instance.GetFirstGUIDComponent(guid);
					if (firstGUIDComponent)
					{
						SpacePortCannon component = firstGUIDComponent.GetComponent<SpacePortCannon>();
						if (component)
						{
							ActionEnterExitInteract actionEnterExitInteract = component.GetActionEnterExitInteract();
							if (actionEnterExitInteract && !actionEnterExitInteract.GetDriverController())
							{
								this.cannons.Add(actionEnterExitInteract);
							}
						}
					}
				}
			}
		}
		this.maxPlayers = (uint)this.cannons.Count;
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002A60 File Offset: 0x00000C60
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_ON_ASTERIOD_SPAWNED = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientOnAsteriodSpawned), 1);
		this.RPC_CLIENT_SUBWAVE_STARTED = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSubWaveStarted), 1);
		this.RPC_CLIENT_STATION_HIT = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientStationHit), 1);
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002AC0 File Offset: 0x00000CC0
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		for (int i = 0; i < this.waves.Length; i++)
		{
			this.waves[i].Initalize(this);
		}
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00002AF8 File Offset: 0x00000CF8
	protected override void ServerUpdate()
	{
		base.ServerUpdate();
		if (this.jobInformation.waveNumber >= 0 && (int)this.jobInformation.waveNumber < this.waves.Length)
		{
			if (this.waves[(int)this.jobInformation.waveNumber].HasFinished() && this.pendingSpawnRef == 0 && this.allAsteriods.Count == 0)
			{
				this.ServerNextWave();
			}
			else if (Time.time - this.timeSinceNewWave > 2f)
			{
				this.waves[(int)this.jobInformation.waveNumber].Update();
			}
		}
		this.ServerCheckFinished();
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002B94 File Offset: 0x00000D94
	private void ServerNextWave()
	{
		if (this.nextwaveCoroutine == null)
		{
			this.nextwaveCoroutine = base.StartCoroutine(this.<ServerNextWave>g__Delay|27_0());
		}
	}

	// Token: 0x0600002F RID: 47 RVA: 0x00002BB0 File Offset: 0x00000DB0
	private void ServerCheckFinished()
	{
		if ((int)this.jobInformation.waveNumber != this.waves.Length)
		{
			return;
		}
		if (this.pendingSpawnRef != 0 || this.allAsteriods.Count != 0)
		{
			return;
		}
		this.ServerJobCompleted();
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00002BE4 File Offset: 0x00000DE4
	public void ServerSpawnAsteroid(AsteroidDefenceAsteroid asteroidPrefab, float spawnAngleMin, float spawnAngleMax)
	{
		if (asteroidPrefab && this.spawnAsteroidRadiusTransform)
		{
			Vector3 spawnPoint = HawkMathUtils.GetRandomPointInsideTwoSphereArc(this.spawnAsteroidRadiusTransform.position, this.spawnAsteroidRadiusMin, this.spawnAsteroidRadiusMax, 0f, spawnAngleMin, spawnAngleMax, -45f, -20f);
			Quaternion rotation = Random.rotation;
			this.pendingSpawnRef++;
			NetworkPrefab.SpawnNetworkPrefab(asteroidPrefab.gameObject, delegate(HawkNetworkBehaviour x)
			{
				this.pendingSpawnRef--;
				if (x)
				{
					AsteroidDefenceAsteroid asteroidDefenceAsteroid = x as AsteroidDefenceAsteroid;
					if (asteroidDefenceAsteroid)
					{
						Transform child = this.attackPointsParent.GetChild(Random.Range(0, this.attackPointsParent.childCount));
						AsteroidDefenceAsteroidSync asteroidDefenceAsteroidSync = default(AsteroidDefenceAsteroidSync);
						asteroidDefenceAsteroidSync.position = spawnPoint;
						asteroidDefenceAsteroidSync.velocity = (child.position - asteroidDefenceAsteroidSync.position).normalized;
						asteroidDefenceAsteroid.ServerSetData(child, asteroidDefenceAsteroidSync, true);
						this.OnServerSpawnedAsteroid(asteroidDefenceAsteroid);
					}
					AsteroidDefenceAsteroidSplit asteroidDefenceAsteroidSplit = x as AsteroidDefenceAsteroidSplit;
					if (asteroidDefenceAsteroidSplit)
					{
						AsteroidDefenceAsteroidSplit asteroidDefenceAsteroidSplit2 = asteroidDefenceAsteroidSplit;
						asteroidDefenceAsteroidSplit2.onSpawnedAsteroid = (Action<AsteroidDefenceAsteroid>)Delegate.Combine(asteroidDefenceAsteroidSplit2.onSpawnedAsteroid, new Action<AsteroidDefenceAsteroid>(this.OnServerSpawnedAsteroid));
					}
				}
			}, new Vector3?(spawnPoint), new Quaternion?(rotation), null, true, false, false, true);
		}
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00002C8C File Offset: 0x00000E8C
	private void OnServerSpawnedAsteroid(AsteroidDefenceAsteroid asteroid)
	{
		if (asteroid)
		{
			asteroid.onExploded = (Action<AsteroidDefenceAsteroid, Collider>)Delegate.Combine(asteroid.onExploded, new Action<AsteroidDefenceAsteroid, Collider>(this.OnAsteroidExploded));
			asteroid.onDestroy.AddCallback(new Action<HawkNetworkBehaviour>(this.OnAsteroidDestroyed));
			this.serverCurrentAsteroids.Add(asteroid);
			this.networkObject.SendRPC(this.RPC_CLIENT_ON_ASTERIOD_SPAWNED, 0, new object[] { asteroid.networkObject.GetNetworkID() });
		}
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00002D14 File Offset: 0x00000F14
	private void OnAsteroidExploded(AsteroidDefenceAsteroid asteroid, Collider collider)
	{
		if (!collider)
		{
			return;
		}
		if (collider.GetComponentInParent<SpaceStation>() && asteroid)
		{
			AsteroidDefenceJobInformation asteroidDefenceJobInformation = this.jobInformation;
			asteroidDefenceJobInformation.health -= asteroid.GetDamage();
			this.networkObject.SendRPC(this.RPC_CLIENT_STATION_HIT, 0, new object[] { asteroid.transform.position });
			if (this.jobInformation.health <= 0)
			{
				this.ServerJobFailed("Ran out of health");
			}
			base.ServerSendJobInformation();
		}
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00002DA4 File Offset: 0x00000FA4
	private void OnAsteroidDestroyed(HawkNetworkBehaviour networkBehaviour)
	{
		AsteroidDefenceAsteroid asteroidDefenceAsteroid = networkBehaviour as AsteroidDefenceAsteroid;
		this.serverCurrentAsteroids.Remove(asteroidDefenceAsteroid);
		this.allAsteriods.Remove(asteroidDefenceAsteroid);
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00002DD2 File Offset: 0x00000FD2
	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.DestroyAllAsteroids();
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00002DE0 File Offset: 0x00000FE0
	protected override void DestoryJob()
	{
		base.DestoryJob();
		this.DestroyAllAsteroids();
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00002DF0 File Offset: 0x00000FF0
	public override void StartJob(JobDispensorBehaviour jobDispensor, List<PlayerController> controllers)
	{
		base.StartJob(jobDispensor, controllers);
		if (this.cannons != null && this.cannons.Count > 0)
		{
			for (int i = controllers.Count - 1; i >= 0; i--)
			{
				if (this.cannons.Count > i)
				{
					ActionEnterExitInteract actionEnterExitInteract = this.cannons[i];
					PlayerController playerController = controllers[i];
					actionEnterExitInteract.RequestEnter(playerController);
					actionEnterExitInteract.SetLocked(true);
				}
			}
		}
		this.ServerNextWave();
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00002E64 File Offset: 0x00001064
	protected override void OnClientJobStarted(PlayerController playerController, ulong jobStartedTimeStep)
	{
		base.OnClientJobStarted(playerController, jobStartedTimeStep);
		PlayerControllerUI playerControllerUI = playerController.GetPlayerControllerUI();
		if (playerControllerUI)
		{
			this.uiInstances.Add(playerController, null);
			playerControllerUI.CreateUIOnGameplayCanvas(this.jobUIPrefab, delegate(UIElement x)
			{
				if (x)
				{
					AsteroidDefenceUI asteroidDefenceUI = x as AsteroidDefenceUI;
					if (this && this.uiInstances.ContainsKey(playerController) && asteroidDefenceUI)
					{
						this.uiInstances[playerController] = asteroidDefenceUI;
						asteroidDefenceUI.Show(this);
						return;
					}
					AddressablesHelper.DestroyRelease(x.gameObject);
				}
			});
		}
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00002ED0 File Offset: 0x000010D0
	public void ServerSubWaveStarted(AsteroidDefenceWaveSpawnData subWave)
	{
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		if (this.jobInformation.waveNumber >= 0 && (int)this.jobInformation.waveNumber < this.waves.Length && this.waves.Length != 0)
		{
			sbyte b = (sbyte)Array.IndexOf<AsteroidDefenceWaveSpawnData>(this.waves[(int)this.jobInformation.waveNumber].spawnDatas, subWave);
			if (b >= 0)
			{
				this.networkObject.SendRPC(this.RPC_CLIENT_SUBWAVE_STARTED, 0, new object[]
				{
					this.jobInformation.waveNumber,
					b
				});
			}
		}
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00002F76 File Offset: 0x00001176
	public override void ServerJobStarted(PlayerController playerController)
	{
		base.ServerJobStarted(playerController);
		playerController.onServerPlayerSpawned += this.OnServerPlayerSpawned;
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00002F94 File Offset: 0x00001194
	protected override void ServerJobFinished(PlayerController playerController)
	{
		base.ServerJobFinished(playerController);
		playerController.onServerPlayerSpawned -= this.OnServerPlayerSpawned;
		if (this.cannons != null)
		{
			for (int i = 0; i < this.cannons.Count; i++)
			{
				if (this.cannons[i] && this.cannons[i].GetDriverController() == playerController)
				{
					this.cannons[i].SetLocked(false);
					this.cannons[i].RequestExit(playerController);
				}
			}
		}
		if (this.exitPoints.Length != 0)
		{
			BaseGamemode gamemode = UnitySingleton<GameInstance>.Instance.GetGamemode();
			if (gamemode)
			{
				PlayerCharacterSpawnPoint[] array = this.exitPoints;
				int num = this.exitPointIndex;
				this.exitPointIndex = num + 1;
				PlayerCharacterSpawnPoint playerCharacterSpawnPoint = array[num];
				if (playerCharacterSpawnPoint)
				{
					gamemode.RespawnPlayerCharacter(playerController, playerCharacterSpawnPoint, true);
				}
			}
		}
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00003070 File Offset: 0x00001270
	private void OnServerPlayerSpawned(PlayerController controller, PlayerCharacter character)
	{
		AsteroidDefenceJobMission.<>c__DisplayClass40_0 CS$<>8__locals1 = new AsteroidDefenceJobMission.<>c__DisplayClass40_0();
		CS$<>8__locals1.controller = controller;
		CS$<>8__locals1.<>4__this = this;
		if (base.gameObject.activeInHierarchy)
		{
			base.StartCoroutine(CS$<>8__locals1.<OnServerPlayerSpawned>g__Delay|0());
		}
	}

	// Token: 0x0600003C RID: 60 RVA: 0x000030AC File Offset: 0x000012AC
	protected override void OnServerJobFinished()
	{
		base.OnServerJobFinished();
		if (this.cannons != null)
		{
			for (int i = 0; i < this.cannons.Count; i++)
			{
				if (this.cannons[i])
				{
					this.cannons[i].SetLocked(false);
				}
			}
		}
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00003104 File Offset: 0x00001304
	protected override void OnClientJobFinished(PlayerController playerController)
	{
		base.OnClientJobFinished(playerController);
		AsteroidDefenceUI asteroidDefenceUI;
		if (this.uiInstances.TryGetValue(playerController, out asteroidDefenceUI))
		{
			if (asteroidDefenceUI)
			{
				AddressablesHelper.DestroyRelease(asteroidDefenceUI.gameObject);
			}
			this.uiInstances.Remove(playerController);
		}
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00003148 File Offset: 0x00001348
	protected override void OnClientJobInformation(PlayerController playerController)
	{
		base.OnClientJobInformation(playerController);
		this.objective.SetJobObjective(this.healthText.GetLocalizedString(new object[] { this.jobInformation.health }), playerController, true);
		AsteroidDefenceUI asteroidDefenceUI;
		if (this.uiInstances.TryGetValue(playerController, out asteroidDefenceUI) && asteroidDefenceUI && this.jobInformation.waveNumber >= 0 && (int)this.jobInformation.waveNumber < this.waves.Length)
		{
			asteroidDefenceUI.SetWave(this.jobInformation.waveNumber, this.waves[(int)this.jobInformation.waveNumber].waveText);
		}
	}

	// Token: 0x0600003F RID: 63 RVA: 0x000031F0 File Offset: 0x000013F0
	private void DestroyAllAsteroids()
	{
		List<AsteroidDefenceAsteroid> list = new List<AsteroidDefenceAsteroid>(this.serverCurrentAsteroids);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i])
			{
				VanishComponent.VanishAndDestroy(list[i].gameObject);
			}
		}
		this.serverCurrentAsteroids.Clear();
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00003244 File Offset: 0x00001444
	public override IHawkMessage GetJobInformationMessage()
	{
		return this.jobInformation;
	}

	// Token: 0x06000041 RID: 65 RVA: 0x0000324C File Offset: 0x0000144C
	private void ClientStationHit(HawkNetReader reader, HawkRPCInfo info)
	{
		Vector3 vector = reader.ReadVector3();
		List<PlayerController> playerControllers = UnitySingleton<GameInstance>.Instance.GetPlayerControllers();
		for (int i = 0; i < playerControllers.Count; i++)
		{
			if (playerControllers[i].IsLocal())
			{
				GameplayCamera gameplayCamera = playerControllers[i].GetGameplayCamera();
				if (gameplayCamera && Vector3.Distance(vector, gameplayCamera.transform.position) < 300f)
				{
					playerControllers[i].ShakeCamera(this.stationHitShake);
				}
			}
		}
		if (this.cannons != null)
		{
			foreach (ActionEnterExitInteract actionEnterExitInteract in this.cannons)
			{
				if (actionEnterExitInteract)
				{
					SpacePortCannon componentInParent = actionEnterExitInteract.GetComponentInParent<SpacePortCannon>();
					if (componentInParent)
					{
						componentInParent.PlayOrbAlarm();
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(this.stationHitSound))
		{
			RuntimeManager.PlayOneShot(this.stationHitSound, vector);
		}
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00003350 File Offset: 0x00001550
	private void ClientSubWaveStarted(HawkNetReader reader, HawkRPCInfo info)
	{
		sbyte b = reader.ReadSByte();
		sbyte b2 = reader.ReadSByte();
		if (b >= 0 && (int)b < this.waves.Length && this.waves.Length != 0)
		{
			AsteroidDefenceWave asteroidDefenceWave = this.waves[(int)b];
			if (b2 >= 0 && (int)b2 < asteroidDefenceWave.spawnDatas.Length && asteroidDefenceWave.spawnDatas.Length != 0)
			{
				asteroidDefenceWave.spawnDatas[(int)b2].SubWaveStarted();
			}
		}
	}

	// Token: 0x06000043 RID: 67 RVA: 0x000033B4 File Offset: 0x000015B4
	private void ClientOnAsteriodSpawned(HawkNetReader reader, HawkRPCInfo info)
	{
		uint num = reader.ReadUInt32();
		if (base.gameObject.activeInHierarchy)
		{
			base.StartCoroutine(NetworkCoroutine.WaitUntillNetworkObjectAvaliable<AsteroidDefenceAsteroid>(num, delegate(AsteroidDefenceAsteroid x)
			{
				if (x)
				{
					x.onDestroy.AddCallback(new Action<HawkNetworkBehaviour>(this.OnAsteroidDestroyed));
					this.allAsteriods.Add(x);
				}
			}));
		}
	}

	// Token: 0x06000044 RID: 68 RVA: 0x000033EE File Offset: 0x000015EE
	public List<AsteroidDefenceAsteroid> GetAllAsteroids()
	{
		return this.allAsteriods;
	}

	// Token: 0x06000045 RID: 69 RVA: 0x000033F8 File Offset: 0x000015F8
	private void OnDrawGizmosSelected()
	{
		if (this.spawnAsteroidRadiusTransform)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(this.spawnAsteroidRadiusTransform.position, this.spawnAsteroidRadiusMax);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this.spawnAsteroidRadiusTransform.position, this.spawnAsteroidRadiusMin);
		}
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00003454 File Offset: 0x00001654
	public AsteroidDefenceJobMission()
	{
	}

	// Token: 0x06000047 RID: 71 RVA: 0x000034B4 File Offset: 0x000016B4
	[CompilerGenerated]
	private IEnumerator <ServerNextWave>g__Delay|27_0()
	{
		yield return new WaitForSeconds(2f);
		AsteroidDefenceJobInformation asteroidDefenceJobInformation = this.jobInformation;
		asteroidDefenceJobInformation.waveNumber += 1;
		this.timeSinceNewWave = Time.time;
		if (this.jobInformation.waveNumber >= 0 && (int)this.jobInformation.waveNumber < this.waves.Length)
		{
			this.waves[(int)this.jobInformation.waveNumber].WaveStarted();
		}
		base.ServerSendJobInformation();
		this.nextwaveCoroutine = null;
		yield break;
	}

	// Token: 0x06000048 RID: 72 RVA: 0x000034C3 File Offset: 0x000016C3
	[CompilerGenerated]
	private void <ClientOnAsteriodSpawned>b__48_0(AsteroidDefenceAsteroid x)
	{
		if (x)
		{
			x.onDestroy.AddCallback(new Action<HawkNetworkBehaviour>(this.OnAsteroidDestroyed));
			this.allAsteriods.Add(x);
		}
	}

	// Token: 0x04000029 RID: 41
	private byte RPC_CLIENT_ON_ASTERIOD_SPAWNED;

	// Token: 0x0400002A RID: 42
	private byte RPC_CLIENT_SUBWAVE_STARTED;

	// Token: 0x0400002B RID: 43
	private byte RPC_CLIENT_STATION_HIT;

	// Token: 0x0400002C RID: 44
	[SerializeField]
	private string[] spacePortCannonsGuids;

	// Token: 0x0400002D RID: 45
	[SerializeField]
	private Transform attackPointsParent;

	// Token: 0x0400002E RID: 46
	[SerializeField]
	private Transform spawnAsteroidRadiusTransform;

	// Token: 0x0400002F RID: 47
	[SerializeField]
	private float spawnAsteroidRadiusMin = 50f;

	// Token: 0x04000030 RID: 48
	[SerializeField]
	private float spawnAsteroidRadiusMax = 100f;

	// Token: 0x04000031 RID: 49
	[SerializeField]
	private LocalizedString healthText;

	// Token: 0x04000032 RID: 50
	[SerializeField]
	private AssetReference jobUIPrefab;

	// Token: 0x04000033 RID: 51
	[SerializeField]
	private PlayerCharacterSpawnPoint[] exitPoints;

	// Token: 0x04000034 RID: 52
	[SerializeField]
	private CameraShakeSettings stationHitShake;

	// Token: 0x04000035 RID: 53
	[SerializeField]
	[EventRef]
	private string stationHitSound = "event:/Objects_Space/AsteroidDefenceJob/Objects_Space_AsteroidDefenceJob_StationHit";

	// Token: 0x04000036 RID: 54
	[Header("Waves")]
	[SerializeField]
	private AsteroidDefenceWave[] waves;

	// Token: 0x04000037 RID: 55
	private AsteroidDefenceJobInformation jobInformation = new AsteroidDefenceJobInformation();

	// Token: 0x04000038 RID: 56
	private List<AsteroidDefenceAsteroid> serverCurrentAsteroids = new List<AsteroidDefenceAsteroid>();

	// Token: 0x04000039 RID: 57
	private List<AsteroidDefenceAsteroid> allAsteriods = new List<AsteroidDefenceAsteroid>();

	// Token: 0x0400003A RID: 58
	private Dictionary<PlayerController, AsteroidDefenceUI> uiInstances = new Dictionary<PlayerController, AsteroidDefenceUI>();

	// Token: 0x0400003B RID: 59
	private List<ActionEnterExitInteract> cannons;

	// Token: 0x0400003C RID: 60
	private int pendingSpawnRef;

	// Token: 0x0400003D RID: 61
	private int exitPointIndex;

	// Token: 0x0400003E RID: 62
	private float timeSinceNewWave;

	// Token: 0x0400003F RID: 63
	private Coroutine nextwaveCoroutine;

	// Token: 0x0200005C RID: 92
	[CompilerGenerated]
	private sealed class <>c__DisplayClass29_0
	{
		// Token: 0x060002BB RID: 699 RVA: 0x0000E80C File Offset: 0x0000CA0C
		public <>c__DisplayClass29_0()
		{
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000E814 File Offset: 0x0000CA14
		internal void <ServerSpawnAsteroid>b__0(HawkNetworkBehaviour x)
		{
			this.<>4__this.pendingSpawnRef = this.<>4__this.pendingSpawnRef - 1;
			if (x)
			{
				AsteroidDefenceAsteroid asteroidDefenceAsteroid = x as AsteroidDefenceAsteroid;
				if (asteroidDefenceAsteroid)
				{
					Transform child = this.<>4__this.attackPointsParent.GetChild(Random.Range(0, this.<>4__this.attackPointsParent.childCount));
					AsteroidDefenceAsteroidSync asteroidDefenceAsteroidSync = default(AsteroidDefenceAsteroidSync);
					asteroidDefenceAsteroidSync.position = this.spawnPoint;
					asteroidDefenceAsteroidSync.velocity = (child.position - asteroidDefenceAsteroidSync.position).normalized;
					asteroidDefenceAsteroid.ServerSetData(child, asteroidDefenceAsteroidSync, true);
					this.<>4__this.OnServerSpawnedAsteroid(asteroidDefenceAsteroid);
				}
				AsteroidDefenceAsteroidSplit asteroidDefenceAsteroidSplit = x as AsteroidDefenceAsteroidSplit;
				if (asteroidDefenceAsteroidSplit)
				{
					AsteroidDefenceAsteroidSplit asteroidDefenceAsteroidSplit2 = asteroidDefenceAsteroidSplit;
					asteroidDefenceAsteroidSplit2.onSpawnedAsteroid = (Action<AsteroidDefenceAsteroid>)Delegate.Combine(asteroidDefenceAsteroidSplit2.onSpawnedAsteroid, new Action<AsteroidDefenceAsteroid>(this.<>4__this.OnServerSpawnedAsteroid));
				}
			}
		}

		// Token: 0x04000234 RID: 564
		public AsteroidDefenceJobMission <>4__this;

		// Token: 0x04000235 RID: 565
		public Vector3 spawnPoint;
	}

	// Token: 0x0200005D RID: 93
	[CompilerGenerated]
	private sealed class <>c__DisplayClass36_0
	{
		// Token: 0x060002BD RID: 701 RVA: 0x0000E8FB File Offset: 0x0000CAFB
		public <>c__DisplayClass36_0()
		{
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0000E904 File Offset: 0x0000CB04
		internal void <OnClientJobStarted>b__0(UIElement x)
		{
			if (x)
			{
				AsteroidDefenceUI asteroidDefenceUI = x as AsteroidDefenceUI;
				if (this.<>4__this && this.<>4__this.uiInstances.ContainsKey(this.playerController) && asteroidDefenceUI)
				{
					this.<>4__this.uiInstances[this.playerController] = asteroidDefenceUI;
					asteroidDefenceUI.Show(this.<>4__this);
					return;
				}
				AddressablesHelper.DestroyRelease(x.gameObject);
			}
		}

		// Token: 0x04000236 RID: 566
		public AsteroidDefenceJobMission <>4__this;

		// Token: 0x04000237 RID: 567
		public PlayerController playerController;
	}

	// Token: 0x0200005E RID: 94
	[CompilerGenerated]
	private sealed class <>c__DisplayClass40_0
	{
		// Token: 0x060002BF RID: 703 RVA: 0x0000E97C File Offset: 0x0000CB7C
		public <>c__DisplayClass40_0()
		{
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0000E984 File Offset: 0x0000CB84
		internal IEnumerator <OnServerPlayerSpawned>g__Delay|0()
		{
			AsteroidDefenceJobMission.<>c__DisplayClass40_0.<<OnServerPlayerSpawned>g__Delay|0>d <<OnServerPlayerSpawned>g__Delay|0>d = new AsteroidDefenceJobMission.<>c__DisplayClass40_0.<<OnServerPlayerSpawned>g__Delay|0>d(0);
			<<OnServerPlayerSpawned>g__Delay|0>d.<>4__this = this;
			return <<OnServerPlayerSpawned>g__Delay|0>d;
		}

		// Token: 0x04000238 RID: 568
		public PlayerController controller;

		// Token: 0x04000239 RID: 569
		public AsteroidDefenceJobMission <>4__this;

		// Token: 0x02000087 RID: 135
		private sealed class <<OnServerPlayerSpawned>g__Delay|0>d : IEnumerator<object>, IEnumerator, IDisposable
		{
			// Token: 0x06000359 RID: 857 RVA: 0x000107FC File Offset: 0x0000E9FC
			[DebuggerHidden]
			public <<OnServerPlayerSpawned>g__Delay|0>d(int <>1__state)
			{
				this.<>1__state = <>1__state;
			}

			// Token: 0x0600035A RID: 858 RVA: 0x0001080B File Offset: 0x0000EA0B
			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			// Token: 0x0600035B RID: 859 RVA: 0x00010810 File Offset: 0x0000EA10
			bool IEnumerator.MoveNext()
			{
				int num = this.<>1__state;
				AsteroidDefenceJobMission.<>c__DisplayClass40_0 CS$<>8__locals1 = this.<>4__this;
				if (num == 0)
				{
					this.<>1__state = -1;
					this.<>2__current = null;
					this.<>1__state = 1;
					return true;
				}
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				if (CS$<>8__locals1.controller)
				{
					CS$<>8__locals1.<>4__this.QuitJob(CS$<>8__locals1.controller);
				}
				return false;
			}

			// Token: 0x17000023 RID: 35
			// (get) Token: 0x0600035C RID: 860 RVA: 0x00010871 File Offset: 0x0000EA71
			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x0600035D RID: 861 RVA: 0x00010879 File Offset: 0x0000EA79
			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			// Token: 0x17000024 RID: 36
			// (get) Token: 0x0600035E RID: 862 RVA: 0x00010880 File Offset: 0x0000EA80
			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.<>2__current;
				}
			}

			// Token: 0x040002B2 RID: 690
			private int <>1__state;

			// Token: 0x040002B3 RID: 691
			private object <>2__current;

			// Token: 0x040002B4 RID: 692
			public AsteroidDefenceJobMission.<>c__DisplayClass40_0 <>4__this;
		}
	}

	// Token: 0x0200005F RID: 95
	[CompilerGenerated]
	private sealed class <<ServerNextWave>g__Delay|27_0>d : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x060002C1 RID: 705 RVA: 0x0000E993 File Offset: 0x0000CB93
		[DebuggerHidden]
		public <<ServerNextWave>g__Delay|27_0>d(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0000E9A2 File Offset: 0x0000CBA2
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000E9A4 File Offset: 0x0000CBA4
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			AsteroidDefenceJobMission asteroidDefenceJobMission = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(2f);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			AsteroidDefenceJobInformation jobInformation = asteroidDefenceJobMission.jobInformation;
			jobInformation.waveNumber += 1;
			asteroidDefenceJobMission.timeSinceNewWave = Time.time;
			if (asteroidDefenceJobMission.jobInformation.waveNumber >= 0 && (int)asteroidDefenceJobMission.jobInformation.waveNumber < asteroidDefenceJobMission.waves.Length)
			{
				asteroidDefenceJobMission.waves[(int)asteroidDefenceJobMission.jobInformation.waveNumber].WaveStarted();
			}
			asteroidDefenceJobMission.ServerSendJobInformation();
			asteroidDefenceJobMission.nextwaveCoroutine = null;
			return false;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x060002C4 RID: 708 RVA: 0x0000EA56 File Offset: 0x0000CC56
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000EA5E File Offset: 0x0000CC5E
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x0000EA65 File Offset: 0x0000CC65
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x0400023A RID: 570
		private int <>1__state;

		// Token: 0x0400023B RID: 571
		private object <>2__current;

		// Token: 0x0400023C RID: 572
		public AsteroidDefenceJobMission <>4__this;
	}
}
