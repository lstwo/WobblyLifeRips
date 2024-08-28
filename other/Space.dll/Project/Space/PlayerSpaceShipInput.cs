using System;
using HawkNetworking;
using Rewired;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class PlayerSpaceShipInput : PlayerVehicleInput<SpaceShipInput>
{
	// Token: 0x06000219 RID: 537 RVA: 0x0000AF58 File Offset: 0x00009158
	private static void InputSetup()
	{
		if (PlayerSpaceShipInput.bRewired_Input_Setup)
		{
			return;
		}
		PlayerSpaceShipInput.bRewired_Input_Setup = true;
		PlayerSpaceShipInput.Rewired_Acceleration_id = ReInput.mapping.GetActionId("Acceleration");
		PlayerSpaceShipInput.Rewired_SteerAxis_id = ReInput.mapping.GetActionId("SteerAxis");
		PlayerSpaceShipInput.Rewired_Boost_id = ReInput.mapping.GetActionId("VehicleBoost");
		PlayerSpaceShipInput.Rewired_UpDown_Movement_id = ReInput.mapping.GetActionId("VehicleHeight");
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0000AFC4 File Offset: 0x000091C4
	public static SpaceShipInput FillInput(Player player, CameraFocusVehicle cameraFocusVehicle)
	{
		PlayerSpaceShipInput.InputSetup();
		SpaceShipInput spaceShipInput = default(SpaceShipInput);
		spaceShipInput.acceleration = player.GetAxis(PlayerSpaceShipInput.Rewired_Acceleration_id);
		spaceShipInput.sideMovement = player.GetAxis(PlayerSpaceShipInput.Rewired_SteerAxis_id);
		spaceShipInput.bBoost = player.GetButton(PlayerSpaceShipInput.Rewired_Boost_id);
		Vector3 rotationEulers = cameraFocusVehicle.GetRotationEulers();
		spaceShipInput.cameraX = rotationEulers.x;
		spaceShipInput.cameraY = rotationEulers.y;
		spaceShipInput.upDownMovement = player.GetAxis(PlayerSpaceShipInput.Rewired_UpDown_Movement_id);
		return spaceShipInput;
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0000B048 File Offset: 0x00009248
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.cameraFocus = base.GetComponentInChildren<CameraFocusVehicle>();
	}

	// Token: 0x0600021C RID: 540 RVA: 0x0000B060 File Offset: 0x00009260
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
					SpaceShipInput spaceShipInput = PlayerSpaceShipInput.FillInput(rewiredPlayer, this.cameraFocus);
					base.EnqueueInput(spaceShipInput, false);
				}
			}
		}
	}

	// Token: 0x0600021D RID: 541 RVA: 0x0000B0B1 File Offset: 0x000092B1
	public PlayerSpaceShipInput()
	{
	}

	// Token: 0x040001B5 RID: 437
	private static bool bRewired_Input_Setup;

	// Token: 0x040001B6 RID: 438
	private static int Rewired_Acceleration_id;

	// Token: 0x040001B7 RID: 439
	private static int Rewired_SteerAxis_id;

	// Token: 0x040001B8 RID: 440
	private static int Rewired_Boost_id;

	// Token: 0x040001B9 RID: 441
	private static int Rewired_UpDown_Movement_id;

	// Token: 0x040001BA RID: 442
	private CameraFocusVehicle cameraFocus;
}
