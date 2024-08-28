using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HawkNetworking;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x02000034 RID: 52
public class SpaceHelmetShop : HawkNetworkBehaviour
{
	// Token: 0x06000176 RID: 374 RVA: 0x000082FC File Offset: 0x000064FC
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.controllerUIDic.Count > 0)
		{
			foreach (KeyValuePair<PlayerController, UIPlayerBasedSpaceShopHelmetCanvas> keyValuePair in this.controllerUIDic)
			{
				if (keyValuePair.Value)
				{
					keyValuePair.Value.Hide();
					AddressablesHelper.DestroyRelease(keyValuePair.Value.gameObject);
				}
			}
		}
	}

	// Token: 0x06000177 RID: 375 RVA: 0x00008388 File Offset: 0x00006588
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.actionEnter.onPlayerEntered += this.OnPlayerEntered;
		this.actionEnter.onPlayerExited += this.OnPlayerExited;
	}

	// Token: 0x06000178 RID: 376 RVA: 0x000083C0 File Offset: 0x000065C0
	public void OnPlayerEntered(PlayerController playerController)
	{
		if (playerController == null || !playerController.IsLocal())
		{
			return;
		}
		if (!this.controllerUIDic.ContainsKey(playerController))
		{
			PlayerControllerUI playerControllerUI = playerController.GetPlayerControllerUI();
			if (playerControllerUI)
			{
				this.controllerUIDic.Add(playerController, null);
				playerControllerUI.CreateUIOnGameplayCanvas(this.clothingUIPrefab, delegate(UIElement x)
				{
					if (x)
					{
						if (this)
						{
							UIPlayerBasedSpaceShopHelmetCanvas uiplayerBasedSpaceShopHelmetCanvas;
							if (!this.controllerUIDic.TryGetValue(playerController, out uiplayerBasedSpaceShopHelmetCanvas))
							{
								AddressablesHelper.DestroyRelease(x.gameObject);
								return;
							}
							if (uiplayerBasedSpaceShopHelmetCanvas)
							{
								AddressablesHelper.DestroyRelease(x.gameObject);
								return;
							}
							UIPlayerBasedSpaceShopHelmetCanvas uiplayerBasedSpaceShopHelmetCanvas2 = x as UIPlayerBasedSpaceShopHelmetCanvas;
							if (uiplayerBasedSpaceShopHelmetCanvas2)
							{
								uiplayerBasedSpaceShopHelmetCanvas2.Show(this);
								this.controllerUIDic[playerController] = uiplayerBasedSpaceShopHelmetCanvas2;
								return;
							}
							AddressablesHelper.DestroyRelease(x.gameObject);
							return;
						}
						else
						{
							AddressablesHelper.DestroyRelease(x.gameObject);
						}
					}
				});
			}
		}
	}

	// Token: 0x06000179 RID: 377 RVA: 0x00008450 File Offset: 0x00006650
	public void OnPlayerExited(PlayerController playerController)
	{
		if (playerController == null || !playerController.IsLocal())
		{
			return;
		}
		UIPlayerBasedSpaceShopHelmetCanvas uiplayerBasedSpaceShopHelmetCanvas;
		if (this.controllerUIDic.TryGetValue(playerController, out uiplayerBasedSpaceShopHelmetCanvas))
		{
			if (uiplayerBasedSpaceShopHelmetCanvas)
			{
				uiplayerBasedSpaceShopHelmetCanvas.Hide();
				AddressablesHelper.DestroyRelease(uiplayerBasedSpaceShopHelmetCanvas.gameObject);
			}
			this.controllerUIDic.Remove(playerController);
		}
	}

	// Token: 0x0600017A RID: 378 RVA: 0x000084A5 File Offset: 0x000066A5
	internal void RequestExit(PlayerController playerController)
	{
		if (this.actionEnter)
		{
			this.actionEnter.RequestExit(playerController);
		}
	}

	// Token: 0x0600017B RID: 379 RVA: 0x000084C0 File Offset: 0x000066C0
	internal ShopClothesTypeCatalog GetCatalog()
	{
		return this.catalog;
	}

	// Token: 0x0600017C RID: 380 RVA: 0x000084C8 File Offset: 0x000066C8
	public SpaceHelmetShop()
	{
	}

	// Token: 0x0400014F RID: 335
	[SerializeField]
	private ShopClothesHatCatalog catalog;

	// Token: 0x04000150 RID: 336
	[SerializeField]
	private AssetReference clothingUIPrefab;

	// Token: 0x04000151 RID: 337
	[SerializeField]
	private ActionEnterExitInteract actionEnter;

	// Token: 0x04000152 RID: 338
	private Dictionary<PlayerController, UIPlayerBasedSpaceShopHelmetCanvas> controllerUIDic = new Dictionary<PlayerController, UIPlayerBasedSpaceShopHelmetCanvas>();

	// Token: 0x02000075 RID: 117
	[CompilerGenerated]
	private sealed class <>c__DisplayClass6_0
	{
		// Token: 0x06000322 RID: 802 RVA: 0x000101B4 File Offset: 0x0000E3B4
		public <>c__DisplayClass6_0()
		{
		}

		// Token: 0x06000323 RID: 803 RVA: 0x000101BC File Offset: 0x0000E3BC
		internal void <OnPlayerEntered>b__0(UIElement x)
		{
			if (x)
			{
				if (this.<>4__this)
				{
					UIPlayerBasedSpaceShopHelmetCanvas uiplayerBasedSpaceShopHelmetCanvas;
					if (!this.<>4__this.controllerUIDic.TryGetValue(this.playerController, out uiplayerBasedSpaceShopHelmetCanvas))
					{
						AddressablesHelper.DestroyRelease(x.gameObject);
						return;
					}
					if (uiplayerBasedSpaceShopHelmetCanvas)
					{
						AddressablesHelper.DestroyRelease(x.gameObject);
						return;
					}
					UIPlayerBasedSpaceShopHelmetCanvas uiplayerBasedSpaceShopHelmetCanvas2 = x as UIPlayerBasedSpaceShopHelmetCanvas;
					if (uiplayerBasedSpaceShopHelmetCanvas2)
					{
						uiplayerBasedSpaceShopHelmetCanvas2.Show(this.<>4__this);
						this.<>4__this.controllerUIDic[this.playerController] = uiplayerBasedSpaceShopHelmetCanvas2;
						return;
					}
					AddressablesHelper.DestroyRelease(x.gameObject);
					return;
				}
				else
				{
					AddressablesHelper.DestroyRelease(x.gameObject);
				}
			}
		}

		// Token: 0x0400028D RID: 653
		public SpaceHelmetShop <>4__this;

		// Token: 0x0400028E RID: 654
		public PlayerController playerController;
	}
}
