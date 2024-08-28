using System;
using HawkNetworking;
using Rewired;
using UnityEngine;

// Token: 0x0200002B RID: 43
internal class SpacePortCannonInput : InputBufferMonoBehaviour<SpacePortCannonInputData>
{
	// Token: 0x06000154 RID: 340 RVA: 0x00007A42 File Offset: 0x00005C42
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.spacePortCannon = base.GetComponent<SpacePortCannon>();
		this.cameraFocus = base.GetComponentInChildren<CameraFocusVehicle>();
	}

	// Token: 0x06000155 RID: 341 RVA: 0x00007A64 File Offset: 0x00005C64
	protected override void HandleInput()
	{
		PlayerController driverController = this.spacePortCannon.GetDriverController();
		if (driverController && driverController.IsLocal())
		{
			PlayerControllerInputManager playerControllerInputManager = driverController.GetPlayerControllerInputManager();
			if (playerControllerInputManager)
			{
				Player rewiredPlayer = playerControllerInputManager.GetRewiredPlayer();
				if (rewiredPlayer != null)
				{
					Vector3 rotationEulers = this.cameraFocus.GetRotationEulers();
					base.EnqueueInput(new SpacePortCannonInputData
					{
						cameraX = rotationEulers.x,
						cameraY = rotationEulers.y,
						bFire = rewiredPlayer.GetButton("VehicleFire")
					}, false);
				}
			}
		}
	}

	// Token: 0x06000156 RID: 342 RVA: 0x00007AEF File Offset: 0x00005CEF
	public SpacePortCannonInput()
	{
	}

	// Token: 0x0400012D RID: 301
	private SpacePortCannon spacePortCannon;

	// Token: 0x0400012E RID: 302
	private CameraFocusVehicle cameraFocus;
}
