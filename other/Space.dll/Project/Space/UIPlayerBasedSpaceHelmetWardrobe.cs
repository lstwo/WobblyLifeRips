using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Rewired;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000059 RID: 89
public class UIPlayerBasedSpaceHelmetWardrobe : UIPlayerBasedCanvas
{
	// Token: 0x0600029D RID: 669 RVA: 0x0000D8B2 File Offset: 0x0000BAB2
	protected override void OnEnable()
	{
		base.OnEnable();
		SaveGameManager.onApplySettings = (Action)Delegate.Combine(SaveGameManager.onApplySettings, new Action(this.OnApplySettings));
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0000D8DA File Offset: 0x0000BADA
	protected override void OnDisable()
	{
		base.OnDisable();
		SaveGameManager.onApplySettings = (Action)Delegate.Remove(SaveGameManager.onApplySettings, new Action(this.OnApplySettings));
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0000D902 File Offset: 0x0000BB02
	private void OnApplySettings()
	{
		this.Refresh();
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0000D90A File Offset: 0x0000BB0A
	public void Show(SpaceHelmetWardrobe wardrobe)
	{
		this.wardrobe = wardrobe;
		if (this.IsShowing())
		{
			return;
		}
		this.Show();
		base.SetCursorVisible(true);
		base.SetGameplayInputEnabled(false);
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x0000D930 File Offset: 0x0000BB30
	public override void Show()
	{
		base.Show();
		if (this.characterUIPrefab && !this.characterUI)
		{
			this.characterUI = Object.Instantiate<CharacterUIRenderer>(this.characterUIPrefab);
			this.helmetCustomize = this.characterUI.GetComponentInChildren<CharacterCustomize>();
		}
		this.SetToCurrentHelmet();
		base.StartCoroutine(this.<Show>g__Delay|21_0());
		if (this.playerImage && this.characterUI)
		{
			this.playerImage.texture = this.characterUI.GetRenderTexture();
		}
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x0000D9C4 File Offset: 0x0000BBC4
	private void SetToCurrentHelmet()
	{
		if (this.helmetCustomize)
		{
			this.helmetCustomize.SetClothesData(2, this.playerController.GetPlayerPersistentData().CurrentClothes.ClothingTop);
			this.helmetCustomize.SetClothesData(3, this.playerController.GetPlayerPersistentData().CurrentClothes.ClothingBottom);
			SpacePlayerController spacePlayerController = this.playerController as SpacePlayerController;
			if (spacePlayerController)
			{
				SaveSpacePlayer spacePlayer = spacePlayerController.GetSaveSpacePlayer();
				if (spacePlayer != null)
				{
					bool bUsedDefault = false;
					ClothingAssetReference clothingAssetReference = UnitySingleton<ClothingManager>.Instance.GetClothingReference(spacePlayer.currentHelmetGuid);
					if (clothingAssetReference == null)
					{
						clothingAssetReference = UnitySingleton<ClothingManager>.Instance.GetClothingReference(this.defaultHelmetPrefab);
						bUsedDefault = true;
					}
					this.helmetCustomize.SetClothingPiece(clothingAssetReference, delegate(CharacterCustomize cc, ClothingPiece cp)
					{
						if (cp && !bUsedDefault)
						{
							cp.SetPrimaryColor(spacePlayer.currentHelmetColor);
						}
					}, null);
				}
			}
		}
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x0000DAB1 File Offset: 0x0000BCB1
	protected override void OnLastActiveControllerChanged(ControllerType controllerType, Controller controller)
	{
		base.OnLastActiveControllerChanged(controllerType, controller);
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x0000DABC File Offset: 0x0000BCBC
	public override void Hide()
	{
		if (!this.IsShowing())
		{
			return;
		}
		this.DestroyAllIcons();
		if (this.characterUI)
		{
			Object.Destroy(this.characterUI.gameObject);
		}
		base.Hide();
		base.SetCursorVisible(false);
		base.SetGameplayInputEnabled(true);
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x0000DB0C File Offset: 0x0000BD0C
	public void Apply()
	{
		if (this.wardrobe)
		{
			SpacePlayerController spacePlayerController = this.playerController as SpacePlayerController;
			if (spacePlayerController)
			{
				SaveSpacePlayer saveSpacePlayer = spacePlayerController.GetSaveSpacePlayer();
				if (saveSpacePlayer != null && this.helmetCustomize)
				{
					ClothingPiece clothingHat = this.helmetCustomize.GetClothingHat();
					if (clothingHat != null)
					{
						saveSpacePlayer.currentHelmetGuid = clothingHat.GetGUIDString();
						saveSpacePlayer.currentHelmetColor = clothingHat.GetPrimaryColor();
					}
				}
				spacePlayerController.ClientSyncSpacePlayer();
			}
			this.wardrobe.RequestExit(base.GetPlayerController());
			this.Hide();
		}
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x0000DB9B File Offset: 0x0000BD9B
	public void Back()
	{
		if (this.wardrobe)
		{
			this.wardrobe.RequestExit(base.GetPlayerController());
			this.Hide();
		}
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x0000DBC4 File Offset: 0x0000BDC4
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
				this.characterUI.GetCameraPivot().localRotation = Quaternion.AngleAxis(num * 5f, Vector3.up);
			}
		}
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x0000DCE8 File Offset: 0x0000BEE8
	public void Refresh()
	{
		if (this.wardrobe)
		{
			this.DestroyAllIcons();
			ClothingAssetReference clothingReference = UnitySingleton<ClothingManager>.Instance.GetClothingReference(this.defaultHelmetPrefab);
			List<ClothingAssetReference> list = new List<ClothingAssetReference>();
			if (clothingReference != null)
			{
				list.Add(clothingReference);
			}
			if (this.playerController)
			{
				PlayerControllerUnlocker playerControllerUnlocker = this.playerController.GetPlayerControllerUnlocker();
				if (playerControllerUnlocker && this.allHelmets)
				{
					foreach (AssetReference assetReference in this.allHelmets.GetClothing())
					{
						ClothingAssetReference clothingReference2 = UnitySingleton<ClothingManager>.Instance.GetClothingReference(assetReference);
						if (clothingReference2 != null && playerControllerUnlocker.IsClothingUnlocked(clothingReference2) && clothingReference2.selectionType == 1 && clothingReference.clothingGuid != clothingReference2.clothingGuid)
						{
							list.Add(clothingReference2);
						}
					}
				}
			}
			this.CreateButtonIcons(list);
		}
		this.SetFirstSelected();
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
	private void CreateButtonIcons(List<ClothingAssetReference> clothingPieces)
	{
		if (!this.uiClothingIconPrefab || !this.uiEmptyClothingIconPrefab)
		{
			return;
		}
		if (this.uiClothingIconPrefab_Default)
		{
			UIClothingIcon uiclothingIcon = Object.Instantiate<UIClothingIcon>(this.uiClothingIconPrefab_Default, this.content, false);
			if (uiclothingIcon)
			{
				UIClothingIcon uiclothingIcon2 = uiclothingIcon;
				uiclothingIcon2.onClothingButtonClicked = (Action<ClothingAssetReference>)Delegate.Combine(uiclothingIcon2.onClothingButtonClicked, new Action<ClothingAssetReference>(this.OnClothingButtonClicked_Empty));
				this.clothingIconInstances.Add(uiclothingIcon.gameObject);
			}
		}
		for (int i = 0; i < clothingPieces.Count; i++)
		{
			ClothingAssetReference clothingAssetReference = clothingPieces[i];
			UIClothingIcon uiclothingIcon3 = Object.Instantiate<UIClothingIcon>(this.uiClothingIconPrefab, this.content, false);
			if (uiclothingIcon3)
			{
				uiclothingIcon3.Assign(clothingAssetReference);
				UIClothingIcon uiclothingIcon4 = uiclothingIcon3;
				uiclothingIcon4.onClothingButtonClicked = (Action<ClothingAssetReference>)Delegate.Combine(uiclothingIcon4.onClothingButtonClicked, new Action<ClothingAssetReference>(this.OnClothingButtonClicked));
				this.clothingIconInstances.Add(uiclothingIcon3.gameObject);
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

	// Token: 0x060002AA RID: 682 RVA: 0x0000DF0C File Offset: 0x0000C10C
	private void HandleControllerSupport(List<GameObject> buttons)
	{
		UIPlayerBasedSpaceHelmetWardrobe.<>c__DisplayClass32_0 CS$<>8__locals1;
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
						selectable = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0((i + x > CS$<>8__locals1.buttons.Count) ? CS$<>8__locals1.buttons.Count : (i + x), ref CS$<>8__locals1);
						if (!selectable)
						{
							selectable = this.applyButton;
						}
						selectable2 = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(i + 1, ref CS$<>8__locals1);
						if (!selectable2)
						{
							selectable2 = this.backButton;
						}
					}
					else if (num >= x - 1)
					{
						selectable = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(i - 1, ref CS$<>8__locals1);
						if (!selectable)
						{
							selectable = this.applyButton;
						}
						selectable2 = this.backButton;
					}
					else
					{
						selectable = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(i - 1, ref CS$<>8__locals1);
						if (!selectable)
						{
							selectable = this.applyButton;
						}
						selectable2 = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(i + 1, ref CS$<>8__locals1);
						if (!selectable2)
						{
							selectable2 = this.backButton;
						}
					}
					Selectable selectable3 = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(i - x, ref CS$<>8__locals1);
					if (!selectable3)
					{
						selectable3 = this.backButton;
					}
					Selectable selectable4 = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(i + x, ref CS$<>8__locals1);
					if (!selectable4)
					{
						for (int j = 1; j <= num; j++)
						{
							selectable4 = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(i + x - j, ref CS$<>8__locals1);
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
			navigation2.selectOnRight = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(0, ref CS$<>8__locals1);
			if (!navigation2.selectOnRight)
			{
				navigation2.selectOnRight = this.backButton;
			}
			navigation2.selectOnUp = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(CS$<>8__locals1.buttons.Count - 1, ref CS$<>8__locals1);
			navigation2.selectOnDown = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(0, ref CS$<>8__locals1);
			this.applyButton.navigation = navigation2;
		}
		if (this.backButton)
		{
			Navigation navigation3 = this.backButton.navigation;
			navigation3.mode = 4;
			navigation3.selectOnRight = this.applyButton;
			navigation3.selectOnLeft = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(0, ref CS$<>8__locals1);
			if (!navigation3.selectOnLeft)
			{
				navigation3.selectOnLeft = this.backButton;
			}
			navigation3.selectOnUp = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(CS$<>8__locals1.buttons.Count - 1, ref CS$<>8__locals1);
			navigation3.selectOnDown = UIPlayerBasedSpaceHelmetWardrobe.<HandleControllerSupport>g__GetButtonAtIndex|32_0(0, ref CS$<>8__locals1);
			this.backButton.navigation = navigation3;
		}
	}

	// Token: 0x060002AB RID: 683 RVA: 0x0000E21D File Offset: 0x0000C41D
	private void OnClothingButtonClicked_Empty(ClothingAssetReference clothingPiece)
	{
		this.SetToCurrentHelmet();
	}

	// Token: 0x060002AC RID: 684 RVA: 0x0000E228 File Offset: 0x0000C428
	private void OnClothingButtonClicked(ClothingAssetReference clothingPiece)
	{
		if (this.helmetCustomize && this.playerController)
		{
			List<ClothingPieceData> unlockedClothingData = this.playerController.GetPlayerPersistentData().WardrobeData.GetUnlockedClothingData(1);
			ClothingPieceData? clothingPieceData = null;
			foreach (ClothingPieceData clothingPieceData2 in unlockedClothingData)
			{
				if (clothingPieceData2.clothingPrefabGUID == clothingPiece.clothingGuid)
				{
					clothingPieceData = new ClothingPieceData?(clothingPieceData2);
					break;
				}
			}
			this.helmetCustomize.SetClothingPiece(clothingPiece, null, clothingPieceData);
		}
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0000E2D0 File Offset: 0x0000C4D0
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

	// Token: 0x060002AE RID: 686 RVA: 0x0000E319 File Offset: 0x0000C519
	private void OnDestroy()
	{
		if (this.characterUI)
		{
			Object.Destroy(this.characterUI.gameObject);
		}
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0000E338 File Offset: 0x0000C538
	public UIPlayerBasedSpaceHelmetWardrobe()
	{
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x0000E36E File Offset: 0x0000C56E
	[CompilerGenerated]
	private IEnumerator <Show>g__Delay|21_0()
	{
		yield return null;
		this.Refresh();
		yield break;
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0000E380 File Offset: 0x0000C580
	[CompilerGenerated]
	internal static Button <HandleControllerSupport>g__GetButtonAtIndex|32_0(int index, ref UIPlayerBasedSpaceHelmetWardrobe.<>c__DisplayClass32_0 A_1)
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

	// Token: 0x04000216 RID: 534
	private const float RotatePlayer = 5f;

	// Token: 0x04000217 RID: 535
	[SerializeField]
	private UIClothingIcon uiClothingIconPrefab_Default;

	// Token: 0x04000218 RID: 536
	[SerializeField]
	private UIClothingIcon uiClothingIconPrefab;

	// Token: 0x04000219 RID: 537
	[SerializeField]
	private GameObject uiEmptyClothingIconPrefab;

	// Token: 0x0400021A RID: 538
	[SerializeField]
	private Transform content;

	// Token: 0x0400021B RID: 539
	[SerializeField]
	private GridLayoutGroup gridLayout;

	// Token: 0x0400021C RID: 540
	[SerializeField]
	private RawImage playerImage;

	// Token: 0x0400021D RID: 541
	[SerializeField]
	private int minIconSlots = 12;

	// Token: 0x0400021E RID: 542
	[SerializeField]
	private Button applyButton;

	// Token: 0x0400021F RID: 543
	[SerializeField]
	private Button backButton;

	// Token: 0x04000220 RID: 544
	[SerializeField]
	private CharacterUIRenderer characterUIPrefab;

	// Token: 0x04000221 RID: 545
	[SerializeField]
	private ClothingAssetReferenceScriptableObject allHelmets;

	// Token: 0x04000222 RID: 546
	[SerializeField]
	private AssetReference defaultHelmetPrefab;

	// Token: 0x04000223 RID: 547
	private List<GameObject> clothingIconInstances = new List<GameObject>();

	// Token: 0x04000224 RID: 548
	private SpaceHelmetWardrobe wardrobe;

	// Token: 0x04000225 RID: 549
	private CharacterUIRenderer characterUI;

	// Token: 0x04000226 RID: 550
	private CharacterCustomize helmetCustomize;

	// Token: 0x04000227 RID: 551
	private PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

	// Token: 0x04000228 RID: 552
	private List<RaycastResult> results = new List<RaycastResult>();

	// Token: 0x02000084 RID: 132
	[CompilerGenerated]
	private sealed class <>c__DisplayClass22_0
	{
		// Token: 0x06000351 RID: 849 RVA: 0x0001075D File Offset: 0x0000E95D
		public <>c__DisplayClass22_0()
		{
		}

		// Token: 0x06000352 RID: 850 RVA: 0x00010765 File Offset: 0x0000E965
		internal void <SetToCurrentHelmet>b__0(CharacterCustomize cc, ClothingPiece cp)
		{
			if (cp && !this.bUsedDefault)
			{
				cp.SetPrimaryColor(this.spacePlayer.currentHelmetColor);
			}
		}

		// Token: 0x040002AC RID: 684
		public SaveSpacePlayer spacePlayer;

		// Token: 0x040002AD RID: 685
		public bool bUsedDefault;
	}

	// Token: 0x02000085 RID: 133
	[CompilerGenerated]
	[StructLayout(LayoutKind.Auto)]
	private struct <>c__DisplayClass32_0
	{
		// Token: 0x040002AE RID: 686
		public List<GameObject> buttons;
	}

	// Token: 0x02000086 RID: 134
	[CompilerGenerated]
	private sealed class <<Show>g__Delay|21_0>d : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x06000353 RID: 851 RVA: 0x00010788 File Offset: 0x0000E988
		[DebuggerHidden]
		public <<Show>g__Delay|21_0>d(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x00010797 File Offset: 0x0000E997
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0001079C File Offset: 0x0000E99C
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			UIPlayerBasedSpaceHelmetWardrobe uiplayerBasedSpaceHelmetWardrobe = this;
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
			uiplayerBasedSpaceHelmetWardrobe.Refresh();
			return false;
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000356 RID: 854 RVA: 0x000107E5 File Offset: 0x0000E9E5
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x06000357 RID: 855 RVA: 0x000107ED File Offset: 0x0000E9ED
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000358 RID: 856 RVA: 0x000107F4 File Offset: 0x0000E9F4
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x040002AF RID: 687
		private int <>1__state;

		// Token: 0x040002B0 RID: 688
		private object <>2__current;

		// Token: 0x040002B1 RID: 689
		public UIPlayerBasedSpaceHelmetWardrobe <>4__this;
	}
}
