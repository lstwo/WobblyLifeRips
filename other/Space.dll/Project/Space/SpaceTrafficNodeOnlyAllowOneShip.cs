using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
public class SpaceTrafficNodeOnlyAllowOneShip : SpaceTrafficNodeCallbacks
{
	// Token: 0x060001C2 RID: 450 RVA: 0x000096CC File Offset: 0x000078CC
	private void Awake()
	{
		this.enablePathNode.onVehicleLeftNode += this.EnablePathNode_onVehicleLeftNode;
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x000096E8 File Offset: 0x000078E8
	private void EnablePathNode_onVehicleLeftNode(SpaceTrafficNode node, PlayerSpaceShipAI spaceShipAI)
	{
		this.myNode.AllowAnyTravelToThisNode(this);
		if (this.otherNodesToDisable != null)
		{
			for (int i = 0; i < this.otherNodesToDisable.Length; i++)
			{
				this.otherNodesToDisable[i].AllowAnyTravelToThisNode(this);
			}
		}
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000972C File Offset: 0x0000792C
	public override void VehicleHeadingTowardsNode(PlayerSpaceShipAI spaceShipAI)
	{
		this.myNode.StopAnyTravelToThisNode(this);
		if (this.otherNodesToDisable != null)
		{
			for (int i = 0; i < this.otherNodesToDisable.Length; i++)
			{
				this.otherNodesToDisable[i].StopAnyTravelToThisNode(this);
			}
		}
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000976E File Offset: 0x0000796E
	public override void VehicleLeftNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x00009770 File Offset: 0x00007970
	public override void VehicleReachedNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x00009772 File Offset: 0x00007972
	public override void VehicleNodeUpdate(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x00009774 File Offset: 0x00007974
	public SpaceTrafficNodeOnlyAllowOneShip()
	{
	}

	// Token: 0x04000178 RID: 376
	[Header("The node which re-enables this node")]
	[SerializeField]
	private SpaceTrafficNode enablePathNode;

	// Token: 0x04000179 RID: 377
	[SerializeField]
	private SpaceTrafficNode[] otherNodesToDisable;
}
