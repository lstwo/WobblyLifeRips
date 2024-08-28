using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HawkNetworking;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x02000028 RID: 40
public class SpaceHelmetWardrobe : HawkNetworkBehaviour
{
	// Token: 0x0600013C RID: 316 RVA: 0x000072FC File Offset: 0x000054FC
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.controllerUIDic.Count > 0)
		{
			foreach (KeyValuePair<PlayerController, UIPlayerBasedSpaceHelmetWardrobe> keyValuePair in this.controllerUIDic)
			{
				if (keyValuePair.Value)
				{
					keyValuePair.Value.Hide();
					AddressablesHelper.DestroyRelease(keyValuePair.Value.gameObject);
				}
			}
		}
	}

	// Token: 0x0600013D RID: 317 RVA: 0x00007388 File Offset: 0x00005588
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.actionEnter.onPlayerEntered += this.OnPlayerEntered;
		this.actionEnter.onPlayerExited += this.OnPlayerExited;
	}

	// Token: 0x0600013E RID: 318 RVA: 0x000073C0 File Offset: 0x000055C0
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
				playerControllerUI.CreateUIOnGameplayCanvas(this.uiPrefab, delegate(UIElement x)
				{
					if (x)
					{
						if (this)
						{
							UIPlayerBasedSpaceHelmetWardrobe uiplayerBasedSpaceHelmetWardrobe;
							if (!this.controllerUIDic.TryGetValue(playerController, out uiplayerBasedSpaceHelmetWardrobe))
							{
								AddressablesHelper.DestroyRelease(x.gameObject);
								return;
							}
							if (uiplayerBasedSpaceHelmetWardrobe)
							{
								AddressablesHelper.DestroyRelease(x.gameObject);
								return;
							}
							UIPlayerBasedSpaceHelmetWardrobe uiplayerBasedSpaceHelmetWardrobe2 = x as UIPlayerBasedSpaceHelmetWardrobe;
							if (uiplayerBasedSpaceHelmetWardrobe2)
							{
								uiplayerBasedSpaceHelmetWardrobe2.Show(this);
								this.controllerUIDic[playerController] = uiplayerBasedSpaceHelmetWardrobe2;
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

	// Token: 0x0600013F RID: 319 RVA: 0x00007450 File Offset: 0x00005650
	public void OnPlayerExited(PlayerController playerController)
	{
		if (playerController == null || !playerController.IsLocal())
		{
			return;
		}
		UIPlayerBasedSpaceHelmetWardrobe uiplayerBasedSpaceHelmetWardrobe;
		if (this.controllerUIDic.TryGetValue(playerController, out uiplayerBasedSpaceHelmetWardrobe))
		{
			if (uiplayerBasedSpaceHelmetWardrobe)
			{
				uiplayerBasedSpaceHelmetWardrobe.Hide();
				AddressablesHelper.DestroyRelease(uiplayerBasedSpaceHelmetWardrobe.gameObject);
			}
			this.controllerUIDic.Remove(playerController);
		}
	}

	// Token: 0x06000140 RID: 320 RVA: 0x000074A5 File Offset: 0x000056A5
	internal void RequestExit(PlayerController playerController)
	{
		if (this.actionEnter)
		{
			this.actionEnter.RequestExit(playerController);
		}
	}

	// Token: 0x06000141 RID: 321 RVA: 0x000074C0 File Offset: 0x000056C0
	public SpaceHelmetWardrobe()
	{
	}

	// Token: 0x04000113 RID: 275
	[SerializeField]
	private AssetReference uiPrefab;

	// Token: 0x04000114 RID: 276
	[SerializeField]
	private ActionEnterExitInteract actionEnter;

	// Token: 0x04000115 RID: 277
	private Dictionary<PlayerController, UIPlayerBasedSpaceHelmetWardrobe> controllerUIDic = new Dictionary<PlayerController, UIPlayerBasedSpaceHelmetWardrobe>();

	// Token: 0x02000071 RID: 113
	[CompilerGenerated]
	private sealed class <>c__DisplayClass5_0
	{
		// Token: 0x06000312 RID: 786 RVA: 0x0000F7BC File Offset: 0x0000D9BC
		public <>c__DisplayClass5_0()
		{
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0000F7C4 File Offset: 0x0000D9C4
		internal void <OnPlayerEntered>b__0(UIElement x)
		{
			if (x)
			{
				if (this.<>4__this)
				{
					UIPlayerBasedSpaceHelmetWardrobe uiplayerBasedSpaceHelmetWardrobe;
					if (!this.<>4__this.controllerUIDic.TryGetValue(this.playerController, out uiplayerBasedSpaceHelmetWardrobe))
					{
						AddressablesHelper.DestroyRelease(x.gameObject);
						return;
					}
					if (uiplayerBasedSpaceHelmetWardrobe)
					{
						AddressablesHelper.DestroyRelease(x.gameObject);
						return;
					}
					UIPlayerBasedSpaceHelmetWardrobe uiplayerBasedSpaceHelmetWardrobe2 = x as UIPlayerBasedSpaceHelmetWardrobe;
					if (uiplayerBasedSpaceHelmetWardrobe2)
					{
						uiplayerBasedSpaceHelmetWardrobe2.Show(this.<>4__this);
						this.<>4__this.controllerUIDic[this.playerController] = uiplayerBasedSpaceHelmetWardrobe2;
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

		// Token: 0x04000271 RID: 625
		public SpaceHelmetWardrobe <>4__this;

		// Token: 0x04000272 RID: 626
		public PlayerController playerController;
	}
}
