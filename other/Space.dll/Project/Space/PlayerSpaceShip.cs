using System;
using UnityEngine;

// Token: 0x02000046 RID: 70
[RequireComponent(typeof(PlayerSpaceShipMovement))]
[RequireComponent(typeof(PlayerSpaceShipInput))]
[RequireComponent(typeof(PlayerSpaceShipSound))]
public class PlayerSpaceShip : PlayerVehicle, IVehicleSpace, IVehicle
{
	// Token: 0x060001F1 RID: 497 RVA: 0x00009FB8 File Offset: 0x000081B8
	protected override void OnPlayerEntered(PlayerController playerController)
	{
		base.OnPlayerEntered(playerController);
		if (playerController)
		{
			PlayerCharacter playerCharacter = playerController.GetPlayerCharacter();
			if (playerCharacter)
			{
				SpacePlayerCharacter component = playerCharacter.GetComponent<SpacePlayerCharacter>();
				if (component)
				{
					component.ServerSetAirMode(SpaceAirMode.NoAir);
				}
			}
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x00009FF9 File Offset: 0x000081F9
	public PlayerSpaceShip()
	{
	}
}
