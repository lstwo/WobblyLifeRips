using System;
using System.Runtime.CompilerServices;
using HawkNetworking;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x02000053 RID: 83
public class SpacePlayerCharacter : HawkNetworkSubBehaviour
{
	// Token: 0x0600026A RID: 618 RVA: 0x0000C659 File Offset: 0x0000A859
	public override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_PLAY_CHANGE_HAT = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientPlayChangeHat), 1);
	}

	// Token: 0x0600026B RID: 619 RVA: 0x0000C67C File Offset: 0x0000A87C
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.playerCharacter = base.GetComponent<PlayerCharacter>();
		PlayerCharacter playerCharacter = this.playerCharacter;
		playerCharacter.onPlayerControllerAssigned = (Action<PlayerController>)Delegate.Combine(playerCharacter.onPlayerControllerAssigned, new Action<PlayerController>(this.OnPlayerControllerAssigned));
		this.characterCustomize = base.GetComponent<CharacterCustomize>();
		CharacterCustomize characterCustomize = this.characterCustomize;
		characterCustomize.onClothesChanged = (Action<CharacterCustomize>)Delegate.Combine(characterCustomize.onClothesChanged, new Action<CharacterCustomize>(this.OnClothesChanged));
	}

	// Token: 0x0600026C RID: 620 RVA: 0x0000C6F6 File Offset: 0x0000A8F6
	private void OnDestroy()
	{
		this.OnUnassignPlayerController(this.playerController);
	}

	// Token: 0x0600026D RID: 621 RVA: 0x0000C704 File Offset: 0x0000A904
	private void OnPlayerControllerAssigned(PlayerController playerController)
	{
		if (this.playerController)
		{
			return;
		}
		this.playerController = playerController;
		this.OnAssignPlayerController(playerController);
	}

	// Token: 0x0600026E RID: 622 RVA: 0x0000C724 File Offset: 0x0000A924
	private void OnAssignPlayerController(PlayerController playerController)
	{
		SpacePlayerController spacePlayerController = playerController as SpacePlayerController;
		if (spacePlayerController)
		{
			SpacePlayerController spacePlayerController2 = spacePlayerController;
			spacePlayerController2.onSpacePlayerUpdated = (Action<SaveSpacePlayer>)Delegate.Combine(spacePlayerController2.onSpacePlayerUpdated, new Action<SaveSpacePlayer>(this.OnSpacePlayerUpdated));
		}
	}

	// Token: 0x0600026F RID: 623 RVA: 0x0000C764 File Offset: 0x0000A964
	private void OnUnassignPlayerController(PlayerController playerController)
	{
		SpacePlayerController spacePlayerController = playerController as SpacePlayerController;
		if (spacePlayerController)
		{
			SpacePlayerController spacePlayerController2 = spacePlayerController;
			spacePlayerController2.onSpacePlayerUpdated = (Action<SaveSpacePlayer>)Delegate.Remove(spacePlayerController2.onSpacePlayerUpdated, new Action<SaveSpacePlayer>(this.OnSpacePlayerUpdated));
		}
	}

	// Token: 0x06000270 RID: 624 RVA: 0x0000C7A4 File Offset: 0x0000A9A4
	internal void ServerSetAirMode(SpaceAirMode airMode)
	{
		SpaceAirMode? spaceAirMode = this.currentAirMode;
		if (((spaceAirMode.GetValueOrDefault() == airMode) & (spaceAirMode != null)) || this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		this.ServerSetAirMode_Internal(airMode);
	}

	// Token: 0x06000271 RID: 625 RVA: 0x0000C7EC File Offset: 0x0000A9EC
	private void ServerSetAirMode_Internal(SpaceAirMode airMode)
	{
		this.currentAirMode = new SpaceAirMode?(airMode);
		if (this.playerCharacter)
		{
			SpacePlayerController spacePlayerController = this.playerCharacter.GetPlayerController() as SpacePlayerController;
			if (spacePlayerController)
			{
				if (airMode == SpaceAirMode.NoAir)
				{
					SaveSpacePlayer saveSpacePlayer = spacePlayerController.GetSaveSpacePlayer();
					if (saveSpacePlayer != null && this.allHelmets)
					{
						AssetReference clothing = this.allHelmets.GetClothing(saveSpacePlayer.currentHelmetGuid);
						if (!this.TryHelmet(clothing, new Color?(saveSpacePlayer.currentHelmetColor)))
						{
							this.TryHelmet(this.defaultHelmetPrefab, null);
						}
					}
				}
				else
				{
					spacePlayerController.ServerLoadSavedClothesOnActivePlayer(1, false);
				}
			}
		}
		if (this.networkObject != null)
		{
			this.networkObject.SendRPC(this.RPC_CLIENT_PLAY_CHANGE_HAT, 0, Array.Empty<object>());
		}
	}

	// Token: 0x06000272 RID: 626 RVA: 0x0000C8AC File Offset: 0x0000AAAC
	private bool TryHelmet(AssetReference helmetPrefab, Color? color)
	{
		if (helmetPrefab != null)
		{
			ClothingAssetReference clothingReference = UnitySingleton<ClothingManager>.Instance.GetClothingReference(helmetPrefab);
			if (clothingReference != null && clothingReference.selectionType == 1)
			{
				if (this.characterCustomize)
				{
					this.characterCustomize.SetClothingPiece(helmetPrefab, delegate(CharacterCustomize cc, ClothingPiece cp)
					{
						if (cp && color != null)
						{
							cp.SetPrimaryColor(color.Value);
						}
					}, null);
				}
				return true;
			}
			Debug.LogError("Can't put helmet on because you probably haven't baked all clothing Tools/Bake All Clothing");
		}
		return false;
	}

	// Token: 0x06000273 RID: 627 RVA: 0x0000C91C File Offset: 0x0000AB1C
	private void OnClothesChanged(CharacterCustomize characterCustomize)
	{
		if (this.currentAirMode != null && this.currentAirMode.Value == SpaceAirMode.NoAir)
		{
			SpacePlayerController spacePlayerController = this.playerCharacter.GetPlayerController() as SpacePlayerController;
			if (spacePlayerController)
			{
				SaveSpacePlayer saveSpacePlayer = spacePlayerController.GetSaveSpacePlayer();
				if (saveSpacePlayer != null)
				{
					ClothingPiece clothingHat = characterCustomize.GetClothingHat();
					if (clothingHat && (clothingHat.GetGUIDString() != saveSpacePlayer.currentHelmetGuid || clothingHat.GetPrimaryColor() != saveSpacePlayer.currentHelmetColor))
					{
						this.ServerSetAirMode_Internal(this.currentAirMode.Value);
					}
				}
			}
		}
	}

	// Token: 0x06000274 RID: 628 RVA: 0x0000C9AC File Offset: 0x0000ABAC
	private void OnSpacePlayerUpdated(SaveSpacePlayer spacePlayer)
	{
		this.OnClothesChanged(this.characterCustomize);
	}

	// Token: 0x06000275 RID: 629 RVA: 0x0000C9BA File Offset: 0x0000ABBA
	private void ClientPlayChangeHat(HawkNetReader reader, HawkRPCInfo info)
	{
		this.changeHatParticle.PopPlayPush(0);
	}

	// Token: 0x06000276 RID: 630 RVA: 0x0000C9C8 File Offset: 0x0000ABC8
	internal SpaceAirMode? GetCurrentAirMode()
	{
		return this.currentAirMode;
	}

	// Token: 0x06000277 RID: 631 RVA: 0x0000C9D0 File Offset: 0x0000ABD0
	public SpacePlayerCharacter()
	{
	}

	// Token: 0x040001EF RID: 495
	private byte RPC_CLIENT_PLAY_CHANGE_HAT;

	// Token: 0x040001F0 RID: 496
	[SerializeField]
	private ClothingAssetReferenceScriptableObject allHelmets;

	// Token: 0x040001F1 RID: 497
	[SerializeField]
	private AssetReference defaultHelmetPrefab;

	// Token: 0x040001F2 RID: 498
	[SerializeField]
	private BaseParticleInstance changeHatParticle;

	// Token: 0x040001F3 RID: 499
	private SpaceAirMode? currentAirMode;

	// Token: 0x040001F4 RID: 500
	private PlayerCharacter playerCharacter;

	// Token: 0x040001F5 RID: 501
	private CharacterCustomize characterCustomize;

	// Token: 0x040001F6 RID: 502
	private PlayerController playerController;

	// Token: 0x02000081 RID: 129
	[CompilerGenerated]
	private sealed class <>c__DisplayClass16_0
	{
		// Token: 0x0600034D RID: 845 RVA: 0x00010702 File Offset: 0x0000E902
		public <>c__DisplayClass16_0()
		{
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0001070A File Offset: 0x0000E90A
		internal void <TryHelmet>b__0(CharacterCustomize cc, ClothingPiece cp)
		{
			if (cp && this.color != null)
			{
				cp.SetPrimaryColor(this.color.Value);
			}
		}

		// Token: 0x040002A8 RID: 680
		public Color? color;
	}
}
