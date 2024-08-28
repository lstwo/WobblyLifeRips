using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class SpaceTrafficNodeVehicleTriggerCheck : SpaceTrafficNodeCallbacks
{
	// Token: 0x060001C9 RID: 457 RVA: 0x0000977C File Offset: 0x0000797C
	public override void VehicleHeadingTowardsNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001CA RID: 458 RVA: 0x0000977E File Offset: 0x0000797E
	public override void VehicleLeftNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001CB RID: 459 RVA: 0x00009780 File Offset: 0x00007980
	public override void VehicleNodeUpdate(PlayerSpaceShipAI spaceShipAI)
	{
		if (this.vehicleTrigger.ContainsVehicles())
		{
			spaceShipAI.SetTargetNode(this.alternativeNode);
		}
	}

	// Token: 0x060001CC RID: 460 RVA: 0x0000979B File Offset: 0x0000799B
	public override void VehicleReachedNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001CD RID: 461 RVA: 0x0000979D File Offset: 0x0000799D
	public override bool CanTravelToNode()
	{
		return !this.vehicleTrigger.ContainsVehicles();
	}

	// Token: 0x060001CE RID: 462 RVA: 0x000097AD File Offset: 0x000079AD
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(base.transform.position, 1f);
	}

	// Token: 0x060001CF RID: 463 RVA: 0x000097CE File Offset: 0x000079CE
	public SpaceTrafficNodeVehicleTriggerCheck()
	{
	}

	// Token: 0x0400017A RID: 378
	[SerializeField]
	private VehicleTriggerEvent vehicleTrigger;

	// Token: 0x0400017B RID: 379
	[SerializeField]
	private SpaceTrafficNode alternativeNode;
}
