using System;
using UnityEngine;

// Token: 0x0200003F RID: 63
[RequireComponent(typeof(PlayerSpaceHovercraftMovement))]
[RequireComponent(typeof(PlayerSpaceHovercraftInput))]
public class PlayerSpaceHovercraft : PlayerVehicle
{
	// Token: 0x060001D4 RID: 468 RVA: 0x0000982B File Offset: 0x00007A2B
	protected override void OnPlayerDriverChanged(ActionEnterExitInteract actionInteract, PlayerController previousDriver, PlayerController currentDriver)
	{
		base.OnPlayerDriverChanged(actionInteract, previousDriver, currentDriver);
		if (base.GetComponent<PlayerSpaceHovercraftMovement>())
		{
			this.SetVehicleEnabled(currentDriver, false);
		}
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x00009850 File Offset: 0x00007A50
	public PlayerSpaceHovercraft()
	{
	}
}
