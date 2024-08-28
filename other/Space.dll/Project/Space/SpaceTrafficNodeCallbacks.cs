using System;
using UnityEngine;

// Token: 0x02000038 RID: 56
public abstract class SpaceTrafficNodeCallbacks : MonoBehaviour
{
	// Token: 0x060001AC RID: 428
	public abstract void VehicleNodeUpdate(PlayerSpaceShipAI spaceShipAI);

	// Token: 0x060001AD RID: 429
	public abstract void VehicleHeadingTowardsNode(PlayerSpaceShipAI spaceShipAI);

	// Token: 0x060001AE RID: 430
	public abstract void VehicleReachedNode(PlayerSpaceShipAI spaceShipAI);

	// Token: 0x060001AF RID: 431
	public abstract void VehicleLeftNode(PlayerSpaceShipAI spaceShipAI);

	// Token: 0x060001B0 RID: 432 RVA: 0x0000932F File Offset: 0x0000752F
	internal void SetNode(SpaceTrafficNode node)
	{
		this.myNode = node;
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x00009338 File Offset: 0x00007538
	public virtual bool CanTravelToNode()
	{
		return true;
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000933B File Offset: 0x0000753B
	protected SpaceTrafficNodeCallbacks()
	{
	}

	// Token: 0x04000172 RID: 370
	protected SpaceTrafficNode myNode;
}
