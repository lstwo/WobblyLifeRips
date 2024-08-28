using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x02000056 RID: 86
public class SpacePlayerControllerEmployment : PlayerControllerEmployment
{
	// Token: 0x06000284 RID: 644 RVA: 0x0000CC9D File Offset: 0x0000AE9D
	protected override void OnPlayerBasedUIAssigned(PlayerBasedUI playerBasedUI)
	{
		base.OnPlayerBasedUIAssigned(playerBasedUI);
		if (playerBasedUI)
		{
			UIPlayerBasedGameplayCanvas uigameplayCanvas = playerBasedUI.GetUIGameplayCanvas();
			if (uigameplayCanvas == null)
			{
				return;
			}
			uigameplayCanvas.GetGenericCanvas().SetMoneyCounterImage(this.moneyCounterImageReference);
		}
	}

	// Token: 0x06000285 RID: 645 RVA: 0x0000CCCC File Offset: 0x0000AECC
	protected override void OnUpdateMoney(int amount, PlayerControllerEmployment.LocalMoneyChanged callback)
	{
		if (this.persistentData != null)
		{
			SaveSpacePlayer data = this.persistentData.ExternalData.GetData<SaveSpacePlayer>("6b31dacc-6295-467a-a1df-4b3ccff9537f");
			if (data != null)
			{
				data.money += amount;
				if (data.money < 0)
				{
					data.money = 0;
				}
				if (callback != null)
				{
					callback.Invoke(amount, data.money);
				}
			}
		}
	}

	// Token: 0x06000286 RID: 646 RVA: 0x0000CD28 File Offset: 0x0000AF28
	public override int GetLocalMoney()
	{
		SaveSpacePlayer data = this.persistentData.ExternalData.GetData<SaveSpacePlayer>("6b31dacc-6295-467a-a1df-4b3ccff9537f");
		if (data != null)
		{
			return data.money;
		}
		return 0;
	}

	// Token: 0x06000287 RID: 647 RVA: 0x0000CD56 File Offset: 0x0000AF56
	public SpacePlayerControllerEmployment()
	{
	}

	// Token: 0x040001FD RID: 509
	[SerializeField]
	private AssetReferenceT<Sprite> moneyCounterImageReference;
}
