using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x02000037 RID: 55
[DisallowMultipleComponent]
public class SpaceTrafficNode : MonoBehaviour
{
	// Token: 0x14000009 RID: 9
	// (add) Token: 0x06000194 RID: 404 RVA: 0x00008EA4 File Offset: 0x000070A4
	// (remove) Token: 0x06000195 RID: 405 RVA: 0x00008EDC File Offset: 0x000070DC
	public event SpaceTrafficNode.SpaceTrafficNodeCallback onVehicleHeadingTowardsNode
	{
		[CompilerGenerated]
		add
		{
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback = this.onVehicleHeadingTowardsNode;
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback2;
			do
			{
				spaceTrafficNodeCallback2 = spaceTrafficNodeCallback;
				SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback3 = (SpaceTrafficNode.SpaceTrafficNodeCallback)Delegate.Combine(spaceTrafficNodeCallback2, value);
				spaceTrafficNodeCallback = Interlocked.CompareExchange<SpaceTrafficNode.SpaceTrafficNodeCallback>(ref this.onVehicleHeadingTowardsNode, spaceTrafficNodeCallback3, spaceTrafficNodeCallback2);
			}
			while (spaceTrafficNodeCallback != spaceTrafficNodeCallback2);
		}
		[CompilerGenerated]
		remove
		{
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback = this.onVehicleHeadingTowardsNode;
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback2;
			do
			{
				spaceTrafficNodeCallback2 = spaceTrafficNodeCallback;
				SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback3 = (SpaceTrafficNode.SpaceTrafficNodeCallback)Delegate.Remove(spaceTrafficNodeCallback2, value);
				spaceTrafficNodeCallback = Interlocked.CompareExchange<SpaceTrafficNode.SpaceTrafficNodeCallback>(ref this.onVehicleHeadingTowardsNode, spaceTrafficNodeCallback3, spaceTrafficNodeCallback2);
			}
			while (spaceTrafficNodeCallback != spaceTrafficNodeCallback2);
		}
	}

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06000196 RID: 406 RVA: 0x00008F14 File Offset: 0x00007114
	// (remove) Token: 0x06000197 RID: 407 RVA: 0x00008F4C File Offset: 0x0000714C
	public event SpaceTrafficNode.SpaceTrafficNodeCallback onVehicleReachedNode
	{
		[CompilerGenerated]
		add
		{
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback = this.onVehicleReachedNode;
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback2;
			do
			{
				spaceTrafficNodeCallback2 = spaceTrafficNodeCallback;
				SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback3 = (SpaceTrafficNode.SpaceTrafficNodeCallback)Delegate.Combine(spaceTrafficNodeCallback2, value);
				spaceTrafficNodeCallback = Interlocked.CompareExchange<SpaceTrafficNode.SpaceTrafficNodeCallback>(ref this.onVehicleReachedNode, spaceTrafficNodeCallback3, spaceTrafficNodeCallback2);
			}
			while (spaceTrafficNodeCallback != spaceTrafficNodeCallback2);
		}
		[CompilerGenerated]
		remove
		{
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback = this.onVehicleReachedNode;
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback2;
			do
			{
				spaceTrafficNodeCallback2 = spaceTrafficNodeCallback;
				SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback3 = (SpaceTrafficNode.SpaceTrafficNodeCallback)Delegate.Remove(spaceTrafficNodeCallback2, value);
				spaceTrafficNodeCallback = Interlocked.CompareExchange<SpaceTrafficNode.SpaceTrafficNodeCallback>(ref this.onVehicleReachedNode, spaceTrafficNodeCallback3, spaceTrafficNodeCallback2);
			}
			while (spaceTrafficNodeCallback != spaceTrafficNodeCallback2);
		}
	}

	// Token: 0x1400000B RID: 11
	// (add) Token: 0x06000198 RID: 408 RVA: 0x00008F84 File Offset: 0x00007184
	// (remove) Token: 0x06000199 RID: 409 RVA: 0x00008FBC File Offset: 0x000071BC
	public event SpaceTrafficNode.SpaceTrafficNodeCallback onVehicleLeftNode
	{
		[CompilerGenerated]
		add
		{
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback = this.onVehicleLeftNode;
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback2;
			do
			{
				spaceTrafficNodeCallback2 = spaceTrafficNodeCallback;
				SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback3 = (SpaceTrafficNode.SpaceTrafficNodeCallback)Delegate.Combine(spaceTrafficNodeCallback2, value);
				spaceTrafficNodeCallback = Interlocked.CompareExchange<SpaceTrafficNode.SpaceTrafficNodeCallback>(ref this.onVehicleLeftNode, spaceTrafficNodeCallback3, spaceTrafficNodeCallback2);
			}
			while (spaceTrafficNodeCallback != spaceTrafficNodeCallback2);
		}
		[CompilerGenerated]
		remove
		{
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback = this.onVehicleLeftNode;
			SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback2;
			do
			{
				spaceTrafficNodeCallback2 = spaceTrafficNodeCallback;
				SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback3 = (SpaceTrafficNode.SpaceTrafficNodeCallback)Delegate.Remove(spaceTrafficNodeCallback2, value);
				spaceTrafficNodeCallback = Interlocked.CompareExchange<SpaceTrafficNode.SpaceTrafficNodeCallback>(ref this.onVehicleLeftNode, spaceTrafficNodeCallback3, spaceTrafficNodeCallback2);
			}
			while (spaceTrafficNodeCallback != spaceTrafficNodeCallback2);
		}
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00008FF4 File Offset: 0x000071F4
	private void Bake()
	{
		if (this.callbacks == null)
		{
			this.callbacks = base.GetComponentsInChildren<SpaceTrafficNodeCallbacks>();
			for (int i = 0; i < this.callbacks.Length; i++)
			{
				this.callbacks[i].SetNode(this);
			}
		}
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00009036 File Offset: 0x00007236
	private void Awake()
	{
		this.Bake();
	}

	// Token: 0x0600019C RID: 412 RVA: 0x00009040 File Offset: 0x00007240
	public void VehicleNodeUpdate(PlayerSpaceShipAI spaceShipAI)
	{
		this.Bake();
		for (int i = 0; i < this.callbacks.Length; i++)
		{
			this.callbacks[i].VehicleNodeUpdate(spaceShipAI);
		}
	}

	// Token: 0x0600019D RID: 413 RVA: 0x00009074 File Offset: 0x00007274
	public void VehicleHeadingTowardsNode(PlayerSpaceShipAI spaceShipAI)
	{
		if (spaceShipAI.IsGearsDown())
		{
			spaceShipAI.ApplyNodeSettings(this.settings);
		}
		this.Bake();
		SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback = this.onVehicleHeadingTowardsNode;
		if (spaceTrafficNodeCallback != null)
		{
			spaceTrafficNodeCallback(this, spaceShipAI);
		}
		this.OnVehicleHeadingTowardsNode(spaceShipAI);
		for (int i = 0; i < this.callbacks.Length; i++)
		{
			this.callbacks[i].VehicleHeadingTowardsNode(spaceShipAI);
		}
	}

	// Token: 0x0600019E RID: 414 RVA: 0x000090D8 File Offset: 0x000072D8
	public void VehicleReachedNode(PlayerSpaceShipAI spaceShipAI)
	{
		if (!spaceShipAI.IsGearsDown())
		{
			spaceShipAI.ApplyNodeSettings(this.settings);
		}
		this.Bake();
		SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback = this.onVehicleReachedNode;
		if (spaceTrafficNodeCallback != null)
		{
			spaceTrafficNodeCallback(this, spaceShipAI);
		}
		this.OnVehicleReachedNode(spaceShipAI);
		for (int i = 0; i < this.callbacks.Length; i++)
		{
			this.callbacks[i].VehicleReachedNode(spaceShipAI);
		}
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000913C File Offset: 0x0000733C
	public void VehicleLeftNode(PlayerSpaceShipAI spaceShipAI)
	{
		this.Bake();
		SpaceTrafficNode.SpaceTrafficNodeCallback spaceTrafficNodeCallback = this.onVehicleLeftNode;
		if (spaceTrafficNodeCallback != null)
		{
			spaceTrafficNodeCallback(this, spaceShipAI);
		}
		this.OnVehicleLeftNode(spaceShipAI);
		for (int i = 0; i < this.callbacks.Length; i++)
		{
			this.callbacks[i].VehicleLeftNode(spaceShipAI);
		}
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000918A File Offset: 0x0000738A
	public void StopAnyTravelToThisNode(object handle)
	{
		if (this.travelLockHandles == null)
		{
			this.travelLockHandles = new List<object> { handle };
			return;
		}
		this.travelLockHandles.Add(handle);
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x000091B3 File Offset: 0x000073B3
	public void AllowAnyTravelToThisNode(object handle)
	{
		if (this.travelLockHandles == null)
		{
			return;
		}
		this.travelLockHandles.Remove(handle);
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x000091CB File Offset: 0x000073CB
	protected virtual void OnVehicleHeadingTowardsNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x000091CD File Offset: 0x000073CD
	protected virtual void OnVehicleReachedNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x000091CF File Offset: 0x000073CF
	protected virtual void OnVehicleLeftNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x000091D4 File Offset: 0x000073D4
	public virtual AssetReference GetSpawnableVehiclePrefab(VehiclesScriptableObject genericVehiclesData)
	{
		SpaceTrafficNode.prefabCache.Clear();
		if (genericVehiclesData)
		{
			SpaceTrafficNode.prefabCache.AddRange(genericVehiclesData.GetVehiclePrefabs());
		}
		if (this.trafficArea)
		{
			VehiclesScriptableObject vehiclesData = this.trafficArea.GetVehiclesData();
			if (vehiclesData)
			{
				SpaceTrafficNode.prefabCache.AddRange(vehiclesData.GetVehiclePrefabs());
			}
		}
		int num = Random.Range(0, SpaceTrafficNode.prefabCache.Count);
		if (num < SpaceTrafficNode.prefabCache.Count)
		{
			return SpaceTrafficNode.prefabCache[num];
		}
		return null;
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x00009260 File Offset: 0x00007460
	public bool CanTravelToNode()
	{
		if (this.travelLockHandles != null && this.travelLockHandles.Count != 0)
		{
			return false;
		}
		this.Bake();
		for (int i = 0; i < this.callbacks.Length; i++)
		{
			if (!this.callbacks[i].CanTravelToNode())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x000092B0 File Offset: 0x000074B0
	public bool IsInsideLoadedWorld()
	{
		if (UnitySingleton<WorldManager>.Instance)
		{
			Vector2Int vector2Int = WorldManager.WorldToChunkSpace(base.transform.position);
			return UnitySingleton<WorldManager>.GetRawInstance().IsChunkLoaded(vector2Int);
		}
		return true;
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x000092E7 File Offset: 0x000074E7
	public bool CanSpawnOnNode()
	{
		return this.bCanSpawnOnNode;
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x000092EF File Offset: 0x000074EF
	public Color GetGizmosColor()
	{
		if (!this.CanTravelToNode())
		{
			return Color.red;
		}
		if (!this.CanSpawnOnNode())
		{
			return Color.yellow;
		}
		return Color.green;
	}

	// Token: 0x060001AA RID: 426 RVA: 0x00009312 File Offset: 0x00007512
	public SpaceTrafficNode()
	{
	}

	// Token: 0x060001AB RID: 427 RVA: 0x00009321 File Offset: 0x00007521
	// Note: this type is marked as 'beforefieldinit'.
	static SpaceTrafficNode()
	{
	}

	// Token: 0x04000169 RID: 361
	[CompilerGenerated]
	private SpaceTrafficNode.SpaceTrafficNodeCallback onVehicleHeadingTowardsNode;

	// Token: 0x0400016A RID: 362
	[CompilerGenerated]
	private SpaceTrafficNode.SpaceTrafficNodeCallback onVehicleReachedNode;

	// Token: 0x0400016B RID: 363
	[CompilerGenerated]
	private SpaceTrafficNode.SpaceTrafficNodeCallback onVehicleLeftNode;

	// Token: 0x0400016C RID: 364
	[SerializeField]
	private SpaceTrafficNodeSettings settings;

	// Token: 0x0400016D RID: 365
	[SerializeField]
	private bool bCanSpawnOnNode = true;

	// Token: 0x0400016E RID: 366
	[SerializeField]
	private TrafficArea trafficArea;

	// Token: 0x0400016F RID: 367
	private SpaceTrafficNodeCallbacks[] callbacks;

	// Token: 0x04000170 RID: 368
	private List<object> travelLockHandles;

	// Token: 0x04000171 RID: 369
	private static List<AssetReference> prefabCache = new List<AssetReference>(10);

	// Token: 0x02000077 RID: 119
	// (Invoke) Token: 0x06000327 RID: 807
	public delegate void SpaceTrafficNodeCallback(SpaceTrafficNode node, PlayerSpaceShipAI spaceShipAI);
}
