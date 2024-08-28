using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HawkNetworking;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x02000035 RID: 53
[UnitySingleton(0, true, "", false, true)]
public class SpaceTrafficManager : UnitySingleton<SpaceTrafficManager>
{
	// Token: 0x0600017D RID: 381 RVA: 0x000084DC File Offset: 0x000066DC
	protected override void Awake()
	{
		base.Awake();
		this.vehiclePlayerMask = LayerMask.GetMask(new string[] { "Ragdoll", "DynamicObject", "Wheel", "Vehicle" });
		this.BakeTrafficNodesIntoChunks();
		if (UnitySingleton<WorldManager>.Instance)
		{
			using (HashSet<Vector2Int>.Enumerator enumerator = UnitySingleton<WorldManager>.Instance.GetAllLoadedChunks().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Vector2Int vector2Int = enumerator.Current;
					this.OnChunkLoaded(vector2Int);
				}
				return;
			}
		}
		object obj = this.threadLock;
		lock (obj)
		{
			foreach (KeyValuePair<Vector2Int, List<SpaceTrafficNode>> keyValuePair in this.bakedChunkNodes)
			{
				this.loadedNodes.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00008600 File Offset: 0x00006800
	private void OnEnable()
	{
		if (UnitySingleton<WorldManager>.Instance)
		{
			WorldManager instance = UnitySingleton<WorldManager>.Instance;
			instance.onSceneChunkLoaded = (Action<Vector2Int>)Delegate.Combine(instance.onSceneChunkLoaded, new Action<Vector2Int>(this.OnChunkLoaded));
			WorldManager instance2 = UnitySingleton<WorldManager>.Instance;
			instance2.onSceneChunkUnloaded = (Action<Vector2Int>)Delegate.Combine(instance2.onSceneChunkUnloaded, new Action<Vector2Int>(this.OnChunkUnloaded));
		}
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00008668 File Offset: 0x00006868
	private void OnDisable()
	{
		if (UnitySingleton<WorldManager>.InstanceExists)
		{
			WorldManager instance = UnitySingleton<WorldManager>.Instance;
			instance.onSceneChunkLoaded = (Action<Vector2Int>)Delegate.Remove(instance.onSceneChunkLoaded, new Action<Vector2Int>(this.OnChunkLoaded));
			WorldManager instance2 = UnitySingleton<WorldManager>.Instance;
			instance2.onSceneChunkUnloaded = (Action<Vector2Int>)Delegate.Remove(instance2.onSceneChunkUnloaded, new Action<Vector2Int>(this.OnChunkUnloaded));
		}
	}

	// Token: 0x06000180 RID: 384 RVA: 0x000086C8 File Offset: 0x000068C8
	private void OnChunkLoaded(Vector2Int chunk)
	{
		object obj = this.threadLock;
		lock (obj)
		{
			List<SpaceTrafficNode> list;
			if (this.bakedChunkNodes.TryGetValue(chunk, out list) && !this.loadedNodes.ContainsKey(chunk))
			{
				this.loadedNodes.Add(chunk, list);
			}
		}
	}

	// Token: 0x06000181 RID: 385 RVA: 0x00008730 File Offset: 0x00006930
	private void OnChunkUnloaded(Vector2Int chunk)
	{
		object obj = this.threadLock;
		lock (obj)
		{
			this.loadedNodes.Remove(chunk);
		}
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00008778 File Offset: 0x00006978
	private void BakeTrafficNodesIntoChunks()
	{
		object obj = this.threadLock;
		lock (obj)
		{
			this.bakedChunkNodes.Clear();
			foreach (SpaceTrafficNode spaceTrafficNode in base.GetComponentsInChildren<SpaceTrafficNode>())
			{
				Vector2Int vector2Int = WorldManager.WorldToChunkSpace(spaceTrafficNode.transform.position);
				List<SpaceTrafficNode> list;
				if (this.bakedChunkNodes.TryGetValue(vector2Int, out list))
				{
					list.Add(spaceTrafficNode);
				}
				else
				{
					list = new List<SpaceTrafficNode> { spaceTrafficNode };
					this.bakedChunkNodes.Add(vector2Int, list);
				}
			}
		}
	}

	// Token: 0x06000183 RID: 387 RVA: 0x00008824 File Offset: 0x00006A24
	private void Update()
	{
		if (!HawkNetworkManager.DefaultInstance.IsServer())
		{
			return;
		}
		this.UpdateTraffic();
	}

	// Token: 0x06000184 RID: 388 RVA: 0x00008839 File Offset: 0x00006A39
	private void LateUpdate()
	{
		this.SimulateTraffic();
	}

	// Token: 0x06000185 RID: 389 RVA: 0x00008844 File Offset: 0x00006A44
	private void SimulateTraffic()
	{
		if (this.active_aiVehicles.Count == 0)
		{
			return;
		}
		for (int i = 0; i < 1; i++)
		{
			if (this.simulateAiIndex >= this.active_aiVehicles.Count)
			{
				this.simulateAiIndex = 0;
			}
			List<PlayerSpaceShipAI> list = this.active_aiVehicles;
			int num = this.simulateAiIndex;
			this.simulateAiIndex = num + 1;
			list[num].SimulateVehicle();
		}
	}

	// Token: 0x06000186 RID: 390 RVA: 0x000088A8 File Offset: 0x00006AA8
	public void MakeAvaliable(PlayerSpaceShipAI vehicleAI)
	{
		if (vehicleAI && !this.avaliable_aiVehicles.Contains(vehicleAI))
		{
			this.avaliable_aiVehicles.Add(vehicleAI);
			PlayerVehicle playerVehicle = vehicleAI.GetPlayerVehicle();
			if (playerVehicle)
			{
				playerVehicle.ServerSetGameObjectActive(false);
			}
		}
	}

	// Token: 0x06000187 RID: 391 RVA: 0x000088F0 File Offset: 0x00006AF0
	public void MakeUnavaliable(PlayerSpaceShipAI vehicleRoadAI)
	{
		this.avaliable_aiVehicles.Remove(vehicleRoadAI);
		PlayerVehicle playerVehicle = vehicleRoadAI.GetPlayerVehicle();
		if (playerVehicle)
		{
			playerVehicle.ServerSetGameObjectActive(true);
		}
	}

	// Token: 0x06000188 RID: 392 RVA: 0x00008920 File Offset: 0x00006B20
	private void UpdateTraffic()
	{
		if ((long)this.active_aiVehicles.Count >= (long)((ulong)this.maxVehicles) || !this.bTrafficEnabled)
		{
			return;
		}
		if (Time.time - this.trafficUpdateTime >= 1f && (long)this.active_aiVehicles.Count < (long)((ulong)this.maxVehicles))
		{
			this.SpawnAI();
		}
	}

	// Token: 0x06000189 RID: 393 RVA: 0x0000897C File Offset: 0x00006B7C
	private void SpawnAI()
	{
		this.trafficUpdateTime = Time.time;
		SpaceTrafficNode vehicleSpawn = this.GetVehicleSpawn();
		if (vehicleSpawn)
		{
			AssetReference spawnableVehiclePrefab = vehicleSpawn.GetSpawnableVehiclePrefab(this.genericVehiclesData);
			Guid guid;
			if (spawnableVehiclePrefab != null && Guid.TryParse(spawnableVehiclePrefab.AssetGUID, out guid))
			{
				PlayerSpaceShipAI avaliableVehicleAI = this.GetAvaliableVehicleAI(guid);
				if (avaliableVehicleAI)
				{
					this.MakeUnavaliable(avaliableVehicleAI);
					this.SetupAIVehicle(avaliableVehicleAI, vehicleSpawn);
					return;
				}
				this.SpawnAIVehicle(spawnableVehiclePrefab, vehicleSpawn);
			}
		}
	}

	// Token: 0x0600018A RID: 394 RVA: 0x000089EC File Offset: 0x00006BEC
	private void SpawnAIVehicle(AssetReference vehiclePrefab, SpaceTrafficNode spawnNode)
	{
		if ((long)this.active_aiVehicles.Count >= (long)((ulong)this.maxVehicles) || !vehiclePrefab.RuntimeKeyIsValid() || !spawnNode)
		{
			return;
		}
		NetworkPrefab.SpawnNetworkPrefab(vehiclePrefab, delegate(HawkNetworkBehaviour networkBehaviour)
		{
			if (networkBehaviour != null)
			{
				PlayerSpaceShipAI playerSpaceShipAI = networkBehaviour.gameObject.AddComponent<PlayerSpaceShipAI>();
				this.SetupAIVehicle(playerSpaceShipAI, spawnNode);
			}
		}, new Vector3?(spawnNode.transform.position), new Quaternion?(spawnNode.transform.rotation), null, true, false, false, true);
	}

	// Token: 0x0600018B RID: 395 RVA: 0x00008A7C File Offset: 0x00006C7C
	private void SetupAIVehicle(PlayerSpaceShipAI aiVehicle, SpaceTrafficNode spawnNode)
	{
		PlayerVehicle playerVehicle = aiVehicle.GetPlayerVehicle();
		if (playerVehicle)
		{
			PlayerVehicleCustomizeBase component = playerVehicle.GetComponent<PlayerVehicleCustomizeBase>();
			if (component)
			{
				PlayerVehicleCustomizeColour playerVehicleCustomizeColour = component as PlayerVehicleCustomizeColour;
				if (playerVehicleCustomizeColour)
				{
					playerVehicleCustomizeColour.RndPrimaryColour();
				}
			}
			playerVehicle.transform.SetPositionAndRotation(spawnNode.transform.position, spawnNode.transform.rotation);
		}
		aiVehicle.SetTargetNode(spawnNode);
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00008AE4 File Offset: 0x00006CE4
	public void Assign(PlayerSpaceShipAI aiVehicle)
	{
		if (aiVehicle)
		{
			this.active_aiVehicles.Add(aiVehicle);
		}
	}

	// Token: 0x0600018D RID: 397 RVA: 0x00008AFA File Offset: 0x00006CFA
	public void Unassign(PlayerSpaceShipAI aiVehicle)
	{
		if (aiVehicle)
		{
			this.active_aiVehicles.Remove(aiVehicle);
		}
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00008B14 File Offset: 0x00006D14
	public SpaceTrafficNode GetVehicleSpawn()
	{
		if (this.loadedNodes.Count > 0)
		{
			int num = Random.Range(0, this.loadedNodes.Count);
			List<SpaceTrafficNode> list = this.loadedNodes.Values.ElementAt(num);
			if (list.Count > 0)
			{
				SpaceTrafficNode spaceTrafficNode = list[Random.Range(0, list.Count)];
				if (!spaceTrafficNode)
				{
					return null;
				}
				for (int i = 0; i < this.active_aiVehicles.Count; i++)
				{
					if ((spaceTrafficNode.transform.position - this.active_aiVehicles[i].transform.position).magnitude < 20f)
					{
						return null;
					}
				}
				if (spaceTrafficNode.CanSpawnOnNode() && spaceTrafficNode.IsInsideLoadedWorld() && !Physics.CheckSphere(spaceTrafficNode.transform.position, 20f, this.vehiclePlayerMask, 1))
				{
					bool flag = false;
					List<PlayerController> playerControllers = UnitySingleton<GameInstance>.Instance.GetPlayerControllers();
					for (int j = 0; j < playerControllers.Count; j++)
					{
						Transform playerTransform = playerControllers[j].GetPlayerTransform();
						if (playerTransform)
						{
							flag = (spaceTrafficNode.transform.position - playerTransform.position).magnitude < 20f;
							if (flag)
							{
								return null;
							}
						}
					}
					if (!flag)
					{
						return spaceTrafficNode;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00008C7C File Offset: 0x00006E7C
	public SpaceTrafficNode GetClosestNode(Vector3 position)
	{
		object obj = this.threadLock;
		SpaceTrafficNode spaceTrafficNode2;
		lock (obj)
		{
			Vector2Int vector2Int = WorldManager.WorldToChunkSpace(position);
			float num = float.MaxValue;
			SpaceTrafficNode spaceTrafficNode = null;
			position.y = 0f;
			for (int i = vector2Int.x - 2; i < vector2Int.x + 2; i++)
			{
				for (int j = vector2Int.y - 2; j < vector2Int.y + 2; j++)
				{
					Vector2Int vector2Int2;
					vector2Int2..ctor(i, j);
					List<SpaceTrafficNode> list;
					if (this.bakedChunkNodes.TryGetValue(vector2Int2, out list))
					{
						for (int k = 0; k < list.Count; k++)
						{
							Vector3 position2 = list[k].transform.position;
							position2.y = 0f;
							float sqrMagnitude = (position2 - position).sqrMagnitude;
							if (sqrMagnitude < num)
							{
								spaceTrafficNode = list[k];
								num = sqrMagnitude;
							}
						}
					}
				}
			}
			spaceTrafficNode2 = spaceTrafficNode;
		}
		return spaceTrafficNode2;
	}

	// Token: 0x06000190 RID: 400 RVA: 0x00008D9C File Offset: 0x00006F9C
	private PlayerSpaceShipAI GetAvaliableVehicleAI(Guid prefabGuid)
	{
		for (int i = 0; i < this.avaliable_aiVehicles.Count; i++)
		{
			if (this.avaliable_aiVehicles[i].GetPlayerVehicle().GetAssetId() == prefabGuid)
			{
				return this.avaliable_aiVehicles[i];
			}
		}
		return null;
	}

	// Token: 0x06000191 RID: 401 RVA: 0x00008DEB File Offset: 0x00006FEB
	public int GetTrafficCount()
	{
		return this.active_aiVehicles.Count;
	}

	// Token: 0x06000192 RID: 402 RVA: 0x00008DF8 File Offset: 0x00006FF8
	public void SetTrafficEnabled(bool bTrafficEnabled)
	{
		this.bTrafficEnabled = bTrafficEnabled;
		if (!bTrafficEnabled)
		{
			for (int i = this.active_aiVehicles.Count - 1; i >= 0; i--)
			{
				this.active_aiVehicles[i].GetPlayerVehicle().networkObject.Destroy();
			}
		}
	}

	// Token: 0x06000193 RID: 403 RVA: 0x00008E44 File Offset: 0x00007044
	public SpaceTrafficManager()
	{
	}

	// Token: 0x04000153 RID: 339
	private const float UpdateTrafficInterval = 1f;

	// Token: 0x04000154 RID: 340
	private const float TrafficRadius = 20f;

	// Token: 0x04000155 RID: 341
	private const float SpawnRadius = 20f;

	// Token: 0x04000156 RID: 342
	private const float SpawnRadiusFromPlayers = 20f;

	// Token: 0x04000157 RID: 343
	[SerializeField]
	private VehiclesScriptableObject genericVehiclesData;

	// Token: 0x04000158 RID: 344
	[SerializeField]
	private uint minVehicles = 3U;

	// Token: 0x04000159 RID: 345
	[SerializeField]
	private uint maxVehicles = 5U;

	// Token: 0x0400015A RID: 346
	private LayerMask vehiclePlayerMask;

	// Token: 0x0400015B RID: 347
	private Dictionary<Vector2Int, List<SpaceTrafficNode>> bakedChunkNodes = new Dictionary<Vector2Int, List<SpaceTrafficNode>>();

	// Token: 0x0400015C RID: 348
	private Dictionary<Vector2Int, List<SpaceTrafficNode>> loadedNodes = new Dictionary<Vector2Int, List<SpaceTrafficNode>>();

	// Token: 0x0400015D RID: 349
	private object threadLock = new object();

	// Token: 0x0400015E RID: 350
	private List<PlayerSpaceShipAI> avaliable_aiVehicles = new List<PlayerSpaceShipAI>();

	// Token: 0x0400015F RID: 351
	private List<PlayerSpaceShipAI> active_aiVehicles = new List<PlayerSpaceShipAI>();

	// Token: 0x04000160 RID: 352
	private float trafficUpdateTime;

	// Token: 0x04000161 RID: 353
	private bool bTrafficEnabled = true;

	// Token: 0x04000162 RID: 354
	private const int SimulateAIPerFrame = 1;

	// Token: 0x04000163 RID: 355
	private int simulateAiIndex;

	// Token: 0x02000076 RID: 118
	[CompilerGenerated]
	private sealed class <>c__DisplayClass30_0
	{
		// Token: 0x06000324 RID: 804 RVA: 0x00010265 File Offset: 0x0000E465
		public <>c__DisplayClass30_0()
		{
		}

		// Token: 0x06000325 RID: 805 RVA: 0x00010270 File Offset: 0x0000E470
		internal void <SpawnAIVehicle>b__0(HawkNetworkBehaviour networkBehaviour)
		{
			if (networkBehaviour != null)
			{
				PlayerSpaceShipAI playerSpaceShipAI = networkBehaviour.gameObject.AddComponent<PlayerSpaceShipAI>();
				this.<>4__this.SetupAIVehicle(playerSpaceShipAI, this.spawnNode);
			}
		}

		// Token: 0x0400028F RID: 655
		public SpaceTrafficManager <>4__this;

		// Token: 0x04000290 RID: 656
		public SpaceTrafficNode spawnNode;
	}
}
