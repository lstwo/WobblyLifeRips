using System;
using HawkNetworking;
using Rewired;
using UnityEngine;

// Token: 0x02000041 RID: 65
public class PlayerSpaceHovercraftInput : PlayerVehicleInput<SpaceHovercraftInput>
{
	// Token: 0x060001D9 RID: 473 RVA: 0x000098C9 File Offset: 0x00007AC9
	private static void InputSetup()
	{
		if (PlayerSpaceHovercraftInput.bRewired_Input_Setup)
		{
			return;
		}
		PlayerSpaceHovercraftInput.bRewired_Input_Setup = true;
		PlayerSpaceHovercraftInput.Rewired_Acceleration_id = ReInput.mapping.GetActionId("Acceleration");
		PlayerSpaceHovercraftInput.Rewired_SteerAxis_id = ReInput.mapping.GetActionId("SteerAxis");
	}

	// Token: 0x060001DA RID: 474 RVA: 0x00009904 File Offset: 0x00007B04
	public static SpaceHovercraftInput FillInput(Player player, CameraFocusVehicle cameraFocusVehicle)
	{
		PlayerSpaceHovercraftInput.InputSetup();
		SpaceHovercraftInput spaceHovercraftInput = default(SpaceHovercraftInput);
		spaceHovercraftInput.acceleration = player.GetAxis(PlayerSpaceHovercraftInput.Rewired_Acceleration_id);
		spaceHovercraftInput.sideMovement = player.GetAxis(PlayerSpaceHovercraftInput.Rewired_SteerAxis_id);
		Vector3 rotationEulers = cameraFocusVehicle.GetRotationEulers();
		spaceHovercraftInput.cameraX = rotationEulers.x;
		spaceHovercraftInput.cameraY = rotationEulers.y;
		return spaceHovercraftInput;
	}

	// Token: 0x060001DB RID: 475 RVA: 0x00009964 File Offset: 0x00007B64
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.cameraFocus = base.GetComponent<CameraFocusVehicle>();
	}

	// Token: 0x060001DC RID: 476 RVA: 0x0000997C File Offset: 0x00007B7C
	protected override void HandleInput()
	{
		if (!base.IsNetworkReady())
		{
			return;
		}
		PlayerController driverController = base.GetDriverController();
		if (driverController != null && driverController.IsLocal())
		{
			PlayerControllerInputManager playerControllerInputManager = driverController.GetPlayerControllerInputManager();
			if (playerControllerInputManager != null)
			{
				Player rewiredPlayer = playerControllerInputManager.GetRewiredPlayer();
				if (rewiredPlayer != null)
				{
					SpaceHovercraftInput spaceHovercraftInput = PlayerSpaceHovercraftInput.FillInput(rewiredPlayer, this.cameraFocus);
					base.EnqueueInput(spaceHovercraftInput, false);
				}
			}
		}
	}

	// Token: 0x060001DD RID: 477 RVA: 0x000099CD File Offset: 0x00007BCD
	public PlayerSpaceHovercraftInput()
	{
	}

	// Token: 0x04000183 RID: 387
	private static bool bRewired_Input_Setup;

	// Token: 0x04000184 RID: 388
	private static int Rewired_Acceleration_id;

	// Token: 0x04000185 RID: 389
	private static int Rewired_SteerAxis_id;

	// Token: 0x04000186 RID: 390
	private CameraFocusVehicle cameraFocus;
}
