using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

// Token: 0x02000058 RID: 88
public class UIPlayerBasedSpaceShopHelmetCanvas : UIPlayerBasedCanvas
{
	// Token: 0x0600028D RID: 653 RVA: 0x0000CDBC File Offset: 0x0000AFBC
	protected override void Awake()
	{
		base.Awake();
		if (this.applyButton)
		{
			this.applyButton.onClick.AddListener(new UnityAction(this.OnApplyButtonClicked));
		}
		if (this.backButton)
		{
			this.backButton.onClick.AddListener(new UnityAction(this.OnBackButtonClicked));
		}
	}

	// Token: 0x0600028E RID: 654 RVA: 0x0000CE24 File Offset: 0x0000B024
	private void OnApplyButtonClicked()
	{
		int cost = this.GetCost();
		SpacePlayerController spacePlayerController = base.GetPlayerController() as SpacePlayerController;
		if (spacePlayerController)
		{
			PlayerControllerEmployment playerControllerEmployment = spacePlayerController.GetPlayerControllerEmployment();
			if (playerControllerEmployment)
			{
				if (playerControllerEmployment.GetLocalMoney() - cost >= 0)
				{
					PlayerControllerUnlocker playerControllerUnlocker = spacePlayerController.GetPlayerControllerUnlocker();
					if (playerControllerUnlocker)
					{
						playerControllerUnlocker.UnlockClothing(this, this.helmetCustomize.GetClothingHat());
					}
					SaveSpacePlayer saveSpacePlayer = spacePlayerController.GetSaveSpacePlayer();
					if (saveSpacePlayer != null && this.helmetCustomize.GetClothingHat())
					{
						saveSpacePlayer.currentHelmetGuid = this.helmetCustomize.GetClothingHat().GetGUIDString();
						saveSpacePlayer.currentHelmetColor = this.helmetCustomize.GetClothingHat().GetPrimaryColor();
					}
					playerControllerEmployment.UpdateMoney(-cost);
					spacePlayerController.ClientSyncSpacePlayer();
					if (this.spaceHelmetShop)
					{
						this.spaceHelmetShop.RequestExit(base.GetPlayerController());
					}
					this.Hide();
					return;
				}
				this.playerController.GetPlayerBasedUI().GetUIPromptCanvas().ShowPrompt_NotEnoughMoney();
			}
		}
	}

	// Token: 0x0600028F RID: 655 RVA: 0x0000CF24 File Offset: 0x0000B124
	private void OnBackButtonClicked()
	{
		if (this.spaceHelmetShop)
		{
			this.spaceHelmetShop.RequestExit(base.GetPlayerController());
		}
		this.Hide();
	}

	// Token: 0x06000290 RID: 656 RVA: 0x0000CF4A File Offset: 0x0000B14A
	public void Show(SpaceHelmetShop spaceHelmetShop)
	{
		this.spaceHelmetShop = spaceHelmetShop;
		if (this.IsShowing())
		{
			return;
		}
		this.Show();
		if (this.playerController)
		{
			this.playerController.SetMinimapVisible(false);
		}
	}

	// Token: 0x06000291 RID: 657 RVA: 0x0000CF7C File Offset: 0x0000B17C
	public override void Show()
	{
		base.Show();
		base.SetCursorVisible(true);
		base.SetGameplayInputEnabled(false);
		if (!this.characterUI && this.characterUIPrefab)
		{
			this.characterUI = Object.Instantiate<CharacterUIRenderer>(this.characterUIPrefab);
		}
		if (this.characterUI)
		{
			if (this.playerImage)
			{
				this.playerImage.texture = this.characterUI.GetRenderTexture();
			}
			CharacterCustomize componentInChildren = this.characterUI.GetComponentInChildren<CharacterCustomize>();
			if (componentInChildren)
			{
				SpacePlayerController spacePlayerController = base.GetPlayerController() as SpacePlayerController;
				if (spacePlayerController)
				{
					SaveSpacePlayer spacePlayer = spacePlayerController.GetSaveSpacePlayer();
					if (spacePlayer != null)
					{
						componentInChildren.SetClothesData(2, spacePlayerController.GetPlayerPersistentData().CurrentClothes.ClothingTop);
						componentInChildren.SetClothesData(3, spacePlayerController.GetPlayerPersistentData().CurrentClothes.ClothingBottom);
						bool bUsedDefault = false;
						ClothingAssetReference clothingAssetReference = UnitySingleton<ClothingManager>.Instance.GetClothingReference(spacePlayer.currentHelmetGuid);
						if (clothingAssetReference == null)
						{
							clothingAssetReference = UnitySingleton<ClothingManager>.Instance.GetClothingReference(this.defaultSpaceHelmetPrefab);
							bUsedDefault = true;
						}
						componentInChildren.SetClothingPiece(clothingAssetReference, delegate(CharacterCustomize cc, ClothingPiece cp)
						{
							if (cp && !bUsedDefault)
							{
								cp.SetPrimaryColor(spacePlayer.currentHelmetColor);
							}
						}, null);
					}
				}
				this.helmetCustomize = componentInChildren;
			}
		}
		this.DestroyAllIcons();
		this.CreateButtonIcons(this.spaceHelmetShop.GetCatalog());
		this.UpdateCost();
	}

	// Token: 0x06000292 RID: 658 RVA: 0x0000D0F5 File Offset: 0x0000B2F5
	public override void Hide()
	{
		base.Hide();
		base.SetCursorVisible(false);
		base.SetGameplayInputEnabled(true);
		if (this.playerController)
		{
			this.playerController.SetMinimapVisible(true);
		}
	}

	// Token: 0x06000293 RID: 659 RVA: 0x0000D124 File Offset: 0x0000B324
	private void DestroyAllIcons()
	{
		for (int i = 0; i < this.clothingIconInstances.Count; i++)
		{
			GameObject gameObject = this.clothingIconInstances[i];
			if (gameObject)
			{
				Object.Destroy(gameObject);
			}
		}
		this.clothingIconInstances.Clear();
	}

	// Token: 0x06000294 RID: 660 RVA: 0x0000D170 File Offset: 0x0000B370
	private void CreateButtonIcons(ShopClothesTypeCatalog clothingPieces)
	{
		if (!this.uiClothingIconPrefab || !this.uiEmptyClothingIconPrefab)
		{
			return;
		}
		if (clothingPieces != null)
		{
			for (int i = 0; i < clothingPieces.GetCatalogLength(); i++)
			{
				if (clothingPieces[i] != null)
				{
					ClothingAssetReference clothingReference = UnitySingleton<ClothingManager>.Instance.GetClothingReference(clothingPieces[i].clothingPrefabReference);
					UIClothingIcon uiclothingIcon = Object.Instantiate<UIClothingIcon>(this.uiClothingIconPrefab, this.content, false);
					if (uiclothingIcon)
					{
						uiclothingIcon.Assign(clothingReference);
						UIClothingIcon uiclothingIcon2 = uiclothingIcon;
						uiclothingIcon2.onClothingButtonClicked = (Action<ClothingAssetReference>)Delegate.Combine(uiclothingIcon2.onClothingButtonClicked, new Action<ClothingAssetReference>(this.OnClothingButtonClicked));
						this.clothingIconInstances.Add(uiclothingIcon.gameObject);
					}
				}
			}
		}
		this.HandleControllerSupport(this.clothingIconInstances);
		while (this.clothingIconInstances.Count < this.minIconSlots)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.uiEmptyClothingIconPrefab, this.content, false);
			if (gameObject)
			{
				this.clothingIconInstances.Add(gameObject);
			}
		}
	}

	// Token: 0x06000295 RID: 661 RVA: 0x0000D278 File Offset: 0x0000B478
	private void HandleControllerSupport(List<GameObject> buttons)
	{
		UIPlayerBasedSpaceShopHelmetCanvas.<>c__DisplayClass26_0 CS$<>8__locals1;
		CS$<>8__locals1.buttons = buttons;
		int x = GridLayoutGroupExtensions.GetColumRowCount(this.gridLayout).x;
		for (int i = 0; i < CS$<>8__locals1.buttons.Count; i++)
		{
			GameObject gameObject = CS$<>8__locals1.buttons[i];
			if (gameObject)
			{
				Button componentInChildren = gameObject.GetComponentInChildren<Button>();
				if (componentInChildren)
				{
					int num = i % x;
					Selectable selectable;
					Selectable selectable2;
					if (num == 0)
					{
						selectable = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0((i + x > CS$<>8__locals1.buttons.Count) ? CS$<>8__locals1.buttons.Count : (i + x), ref CS$<>8__locals1);
						if (!selectable)
						{
							selectable = this.applyButton;
						}
						selectable2 = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(i + 1, ref CS$<>8__locals1);
						if (!selectable2)
						{
							selectable2 = this.backButton;
						}
					}
					else if (num == x - 1)
					{
						selectable = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(i - 1, ref CS$<>8__locals1);
						if (!selectable)
						{
							selectable = this.applyButton;
						}
						selectable2 = this.backButton;
					}
					else
					{
						selectable = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(i - 1, ref CS$<>8__locals1);
						if (!selectable)
						{
							selectable = this.applyButton;
						}
						selectable2 = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(i + 1, ref CS$<>8__locals1);
						if (!selectable2)
						{
							selectable2 = this.backButton;
						}
					}
					Selectable selectable3 = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(i - x, ref CS$<>8__locals1);
					if (!selectable3)
					{
						selectable3 = this.backButton;
					}
					Selectable selectable4 = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(i + x, ref CS$<>8__locals1);
					if (!selectable4)
					{
						for (int j = 1; j <= num; j++)
						{
							selectable4 = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(i + x - j, ref CS$<>8__locals1);
						}
					}
					if (!selectable4)
					{
						selectable4 = this.backButton;
					}
					Navigation navigation = componentInChildren.navigation;
					navigation.mode = 4;
					navigation.selectOnLeft = selectable;
					navigation.selectOnRight = selectable2;
					navigation.selectOnUp = selectable3;
					navigation.selectOnDown = selectable4;
					componentInChildren.navigation = navigation;
				}
			}
		}
		if (this.applyButton)
		{
			Navigation navigation2 = this.applyButton.navigation;
			navigation2.mode = 4;
			navigation2.selectOnLeft = this.backButton;
			navigation2.selectOnRight = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(0, ref CS$<>8__locals1);
			if (!navigation2.selectOnRight)
			{
				navigation2.selectOnRight = this.backButton;
			}
			navigation2.selectOnUp = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(CS$<>8__locals1.buttons.Count - 1, ref CS$<>8__locals1);
			navigation2.selectOnDown = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(0, ref CS$<>8__locals1);
			this.applyButton.navigation = navigation2;
		}
		if (this.backButton)
		{
			Navigation navigation3 = this.backButton.navigation;
			navigation3.mode = 4;
			navigation3.selectOnRight = this.applyButton;
			navigation3.selectOnLeft = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(0, ref CS$<>8__locals1);
			if (!navigation3.selectOnLeft)
			{
				navigation3.selectOnLeft = this.backButton;
			}
			navigation3.selectOnUp = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(CS$<>8__locals1.buttons.Count - 1, ref CS$<>8__locals1);
			navigation3.selectOnDown = UIPlayerBasedSpaceShopHelmetCanvas.<HandleControllerSupport>g__GetButtonAtIndex|26_0(0, ref CS$<>8__locals1);
			this.backButton.navigation = navigation3;
		}
	}

	// Token: 0x06000296 RID: 662 RVA: 0x0000D58C File Offset: 0x0000B78C
	private void OnClothingButtonClicked(ClothingAssetReference clothingPiece)
	{
		int num = -1;
		if (this.spaceHelmetShop)
		{
			this.spaceHelmetShop.GetCatalog().IsInCatalog(clothingPiece.clothingPrefab, ref num);
		}
		if (num >= 0)
		{
			this.selectedHelmetIndex = num;
			if (this.helmetCustomize)
			{
				this.helmetCustomize.SetClothingPiece(clothingPiece, null, null);
			}
		}
		this.UpdateCost();
	}

	// Token: 0x06000297 RID: 663 RVA: 0x0000D5F8 File Offset: 0x0000B7F8
	private void UpdateCost()
	{
		int cost = this.GetCost();
		if (this.clothesPriceText)
		{
			if (cost > 0)
			{
				this.clothesPriceText.text = "$" + cost.ToString();
				return;
			}
			this.clothesPriceText.text = this.freeText.GetLocalizedString();
		}
	}

	// Token: 0x06000298 RID: 664 RVA: 0x0000D650 File Offset: 0x0000B850
	private void Update()
	{
		Player rewiredPlayer = base.GetRewiredPlayer();
		if (rewiredPlayer != null)
		{
			float num;
			if (this.GetActiveControllerType() != 2)
			{
				bool flag = false;
				if (Input.GetMouseButton(0) && this.playerImage)
				{
					this.eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
					this.results.Clear();
					EventSystem.current.RaycastAll(this.eventDataCurrentPosition, this.results);
					for (int i = 0; i < this.results.Count; i++)
					{
						if (this.results[i].gameObject == this.playerImage.gameObject)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					return;
				}
				num = RewiredExtensions.GetAxisRelative(rewiredPlayer, "LookDeltaX", 50f) * 10f;
			}
			else
			{
				num = RewiredExtensions.GetAxisRelative(rewiredPlayer, "LookDeltaX", 50f);
			}
			if (this.characterUI)
			{
				Transform cameraPivot = this.characterUI.GetCameraPivot();
				if (cameraPivot)
				{
					cameraPivot.localRotation *= Quaternion.AngleAxis(num * 5f, Vector3.up);
				}
			}
		}
	}

	// Token: 0x06000299 RID: 665 RVA: 0x0000D78C File Offset: 0x0000B98C
	private int GetCost()
	{
		int num = 0;
		if (this.spaceHelmetShop)
		{
			ShopClothingItem[] clothingPieces = this.spaceHelmetShop.GetCatalog().GetClothingPieces();
			if (clothingPieces != null && clothingPieces.Length != 0 && clothingPieces.Length > this.selectedHelmetIndex && this.selectedHelmetIndex >= 0)
			{
				ShopClothingItem shopClothingItem = clothingPieces[this.selectedHelmetIndex];
				if (shopClothingItem != null)
				{
					PlayerController playerController = base.GetPlayerController();
					if (playerController)
					{
						PlayerControllerUnlocker playerControllerUnlocker = playerController.GetPlayerControllerUnlocker();
						if (playerControllerUnlocker && !playerControllerUnlocker.IsClothingUnlocked(shopClothingItem.clothingPrefabReference))
						{
							num = shopClothingItem.itemPrice;
						}
					}
				}
			}
		}
		return num;
	}

	// Token: 0x0600029A RID: 666 RVA: 0x0000D817 File Offset: 0x0000BA17
	private void OnDestroy()
	{
		if (this.characterUI)
		{
			Object.Destroy(this.characterUI.gameObject);
		}
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0000D836 File Offset: 0x0000BA36
	public UIPlayerBasedSpaceShopHelmetCanvas()
	{
	}

	// Token: 0x0600029C RID: 668 RVA: 0x0000D874 File Offset: 0x0000BA74
	[CompilerGenerated]
	internal static Button <HandleControllerSupport>g__GetButtonAtIndex|26_0(int index, ref UIPlayerBasedSpaceShopHelmetCanvas.<>c__DisplayClass26_0 A_1)
	{
		if (index < 0 || index >= A_1.buttons.Count)
		{
			return null;
		}
		GameObject gameObject = A_1.buttons[index];
		if (gameObject)
		{
			return gameObject.GetComponentInChildren<Button>();
		}
		return null;
	}

	// Token: 0x04000202 RID: 514
	private const float RotatePlayer = 5f;

	// Token: 0x04000203 RID: 515
	[Header("Settings")]
	[SerializeField]
	private UIClothingIcon uiClothingIconPrefab;

	// Token: 0x04000204 RID: 516
	[SerializeField]
	private GameObject uiEmptyClothingIconPrefab;

	// Token: 0x04000205 RID: 517
	[SerializeField]
	private GridLayoutGroup gridLayout;

	// Token: 0x04000206 RID: 518
	[SerializeField]
	private Transform content;

	// Token: 0x04000207 RID: 519
	[SerializeField]
	private RawImage playerImage;

	// Token: 0x04000208 RID: 520
	[SerializeField]
	private int minIconSlots = 12;

	// Token: 0x04000209 RID: 521
	[SerializeField]
	private Button applyButton;

	// Token: 0x0400020A RID: 522
	[SerializeField]
	private Button backButton;

	// Token: 0x0400020B RID: 523
	[SerializeField]
	private CharacterUIRenderer characterUIPrefab;

	// Token: 0x0400020C RID: 524
	[SerializeField]
	private AssetReference defaultSpaceHelmetPrefab;

	// Token: 0x0400020D RID: 525
	[Header("Text")]
	[SerializeField]
	private TextMeshProUGUI clothesPriceText;

	// Token: 0x0400020E RID: 526
	[SerializeField]
	private LocalizedString freeText;

	// Token: 0x0400020F RID: 527
	private SpaceHelmetShop spaceHelmetShop;

	// Token: 0x04000210 RID: 528
	private List<GameObject> clothingIconInstances = new List<GameObject>();

	// Token: 0x04000211 RID: 529
	private CharacterUIRenderer characterUI;

	// Token: 0x04000212 RID: 530
	private int selectedHelmetIndex = -1;

	// Token: 0x04000213 RID: 531
	private CharacterCustomize helmetCustomize;

	// Token: 0x04000214 RID: 532
	private PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

	// Token: 0x04000215 RID: 533
	private List<RaycastResult> results = new List<RaycastResult>();

	// Token: 0x02000082 RID: 130
	[CompilerGenerated]
	private sealed class <>c__DisplayClass22_0
	{
		// Token: 0x0600034F RID: 847 RVA: 0x00010732 File Offset: 0x0000E932
		public <>c__DisplayClass22_0()
		{
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0001073A File Offset: 0x0000E93A
		internal void <Show>b__0(CharacterCustomize cc, ClothingPiece cp)
		{
			if (cp && !this.bUsedDefault)
			{
				cp.SetPrimaryColor(this.spacePlayer.currentHelmetColor);
			}
		}

		// Token: 0x040002A9 RID: 681
		public SaveSpacePlayer spacePlayer;

		// Token: 0x040002AA RID: 682
		public bool bUsedDefault;
	}

	// Token: 0x02000083 RID: 131
	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <>c__DisplayClass26_0
	{
		// Token: 0x040002AB RID: 683
		public List<GameObject> buttons;
	}
}
