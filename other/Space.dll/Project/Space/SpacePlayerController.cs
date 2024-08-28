using System;
using HawkNetworking;

// Token: 0x02000055 RID: 85
internal class SpacePlayerController : FreemodePlayerController
{
	// Token: 0x0600027C RID: 636 RVA: 0x0000CAF3 File Offset: 0x0000ACF3
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_SERVER_SYNC_SPACEPLAYER = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ServerSyncSpacePlayer), 2);
	}

	// Token: 0x0600027D RID: 637 RVA: 0x0000CB15 File Offset: 0x0000AD15
	protected override void OnLoadedSaveData(SavePlayerPersistentData playerPersistentData)
	{
		base.OnLoadedSaveData(playerPersistentData);
		this.spacePlayer = playerPersistentData.ExternalData.GetData<SaveSpacePlayer>("6b31dacc-6295-467a-a1df-4b3ccff9537f");
		this.ClientSyncSpacePlayer();
	}

	// Token: 0x0600027E RID: 638 RVA: 0x0000CB3A File Offset: 0x0000AD3A
	protected override void OnServerLoadedSaveData(SavePlayerPersistentData playerPersistentData)
	{
		base.OnServerLoadedSaveData(playerPersistentData);
		if (this.spacePlayer == null)
		{
			this.spacePlayer = new SaveSpacePlayer();
		}
	}

	// Token: 0x0600027F RID: 639 RVA: 0x0000CB58 File Offset: 0x0000AD58
	public void ClientSyncSpacePlayer()
	{
		if (this.networkObject == null || !this.networkObject.IsOwner() || this.spacePlayer == null)
		{
			return;
		}
		foreach (ClothingPieceData clothingPieceData in this.playerPersistentData.WardrobeData.GetUnlockedClothingData(1))
		{
			Guid clothingPrefabGUID = clothingPieceData.clothingPrefabGUID;
			if (clothingPrefabGUID.ToString() == this.spacePlayer.currentHelmetGuid)
			{
				this.spacePlayer.currentHelmetColor = clothingPieceData.clothingPrimaryColor;
			}
		}
		this.networkObject.SendRPC(this.RPC_SERVER_SYNC_SPACEPLAYER, 3, new object[] { SerializerDeserializerExtensions.SerializeNonAlloc(this.spacePlayer) });
		if (!this.networkObject.IsServer())
		{
			this.OnSpacePlayerUpdated();
		}
	}

	// Token: 0x06000280 RID: 640 RVA: 0x0000CC48 File Offset: 0x0000AE48
	private void OnSpacePlayerUpdated()
	{
		Action<SaveSpacePlayer> action = this.onSpacePlayerUpdated;
		if (action == null)
		{
			return;
		}
		action(this.spacePlayer);
	}

	// Token: 0x06000281 RID: 641 RVA: 0x0000CC60 File Offset: 0x0000AE60
	private void ServerSyncSpacePlayer(HawkNetReader reader, HawkRPCInfo info)
	{
		if (this.spacePlayer == null)
		{
			this.spacePlayer = new SaveSpacePlayer();
		}
		SerializerDeserializerExtensions.Deserialize<SaveSpacePlayer>(this.spacePlayer, reader.ReadBytesAndSize());
		this.OnSpacePlayerUpdated();
	}

	// Token: 0x06000282 RID: 642 RVA: 0x0000CC8D File Offset: 0x0000AE8D
	internal SaveSpacePlayer GetSaveSpacePlayer()
	{
		return this.spacePlayer;
	}

	// Token: 0x06000283 RID: 643 RVA: 0x0000CC95 File Offset: 0x0000AE95
	public SpacePlayerController()
	{
	}

	// Token: 0x040001FA RID: 506
	private byte RPC_SERVER_SYNC_SPACEPLAYER;

	// Token: 0x040001FB RID: 507
	public Action<SaveSpacePlayer> onSpacePlayerUpdated;

	// Token: 0x040001FC RID: 508
	private SaveSpacePlayer spacePlayer;
}
