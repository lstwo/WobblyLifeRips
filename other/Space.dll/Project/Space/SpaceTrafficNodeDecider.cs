using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000039 RID: 57
[RequireComponent(typeof(SpaceTrafficNode))]
public class SpaceTrafficNodeDecider : SpaceTrafficNodeCallbacks
{
	// Token: 0x060001B3 RID: 435 RVA: 0x00009343 File Offset: 0x00007543
	public override void VehicleHeadingTowardsNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x00009345 File Offset: 0x00007545
	public override void VehicleLeftNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x00009347 File Offset: 0x00007547
	public override void VehicleNodeUpdate(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000934C File Offset: 0x0000754C
	public override void VehicleReachedNode(PlayerSpaceShipAI spaceShipAI)
	{
		SpaceTrafficNodeDecider.nodeCache.Clear();
		SpaceTrafficNodeDecider.nodeCache.AddRange(this.potentialNodes);
		for (int i = 0; i < SpaceTrafficNodeDecider.nodeCache.Count; i++)
		{
			int num = Random.Range(0, this.potentialNodes.Length);
			SpaceTrafficNode spaceTrafficNode = SpaceTrafficNodeDecider.nodeCache[i];
			SpaceTrafficNodeDecider.nodeCache[i] = SpaceTrafficNodeDecider.nodeCache[num];
			SpaceTrafficNodeDecider.nodeCache[num] = spaceTrafficNode;
		}
		SpaceTrafficNode spaceTrafficNode2 = null;
		for (int j = 0; j < SpaceTrafficNodeDecider.nodeCache.Count; j++)
		{
			if (SpaceTrafficNodeDecider.nodeCache[j].CanTravelToNode())
			{
				spaceTrafficNode2 = SpaceTrafficNodeDecider.nodeCache[j];
				break;
			}
		}
		if (!spaceTrafficNode2)
		{
			int num2 = Random.Range(0, this.potentialNodes.Length);
			spaceTrafficNode2 = SpaceTrafficNodeDecider.nodeCache[num2];
		}
		if (spaceTrafficNode2)
		{
			spaceShipAI.SetTargetNode(spaceTrafficNode2);
			return;
		}
		Debug.LogError("Node is null " + base.gameObject.name, base.gameObject);
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x00009458 File Offset: 0x00007658
	private void OnDrawGizmos()
	{
		if (this.potentialNodes != null && this.potentialNodes.Length != 0)
		{
			for (int i = 0; i < this.potentialNodes.Length; i++)
			{
				if (!this.myNode)
				{
					this.myNode = base.GetComponent<SpaceTrafficNode>();
				}
				Gizmos.color = this.myNode.GetGizmosColor();
				Gizmos.DrawSphere(base.transform.position, 0.5f);
				Gizmos.color = this.potentialNodes[i].GetGizmosColor();
				Gizmos.DrawLine(base.transform.position, this.potentialNodes[i].transform.position);
			}
			return;
		}
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x00009524 File Offset: 0x00007724
	public SpaceTrafficNodeDecider()
	{
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000952C File Offset: 0x0000772C
	// Note: this type is marked as 'beforefieldinit'.
	static SpaceTrafficNodeDecider()
	{
	}

	// Token: 0x04000173 RID: 371
	[SerializeField]
	private SpaceTrafficNode[] potentialNodes;

	// Token: 0x04000174 RID: 372
	private static List<SpaceTrafficNode> nodeCache = new List<SpaceTrafficNode>();
}
