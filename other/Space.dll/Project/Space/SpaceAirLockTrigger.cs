using System;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class SpaceAirLockTrigger : WobblyPlayerCharacterTriggerNonNetworked
{
	// Token: 0x060000F8 RID: 248 RVA: 0x00005FE4 File Offset: 0x000041E4
	protected override void OnPlayerEnter(PlayerCharacter playerCharacter)
	{
		base.OnPlayerEnter(playerCharacter);
		if (playerCharacter)
		{
			SpacePlayerCharacter componentInParent = playerCharacter.GetComponentInParent<SpacePlayerCharacter>();
			if (componentInParent)
			{
				if (this.bDontChangeIfInVehicle)
				{
					PlayerController playerController = playerCharacter.GetPlayerController();
					if (playerController)
					{
						PlayerControllerInteractor playerControllerInteractor = playerController.GetPlayerControllerInteractor();
						if (playerControllerInteractor && playerControllerInteractor.HasEnteredAction() && playerControllerInteractor.GetEnteredAction().GetGameObject().GetComponent<IVehicle>() != null)
						{
							return;
						}
					}
				}
				componentInParent.ServerSetAirMode(this.airMode);
			}
		}
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x0000605C File Offset: 0x0000425C
	public SpaceAirLockTrigger()
	{
	}

	// Token: 0x040000CD RID: 205
	[SerializeField]
	private SpaceAirMode airMode;

	// Token: 0x040000CE RID: 206
	[SerializeField]
	private bool bDontChangeIfInVehicle;
}
