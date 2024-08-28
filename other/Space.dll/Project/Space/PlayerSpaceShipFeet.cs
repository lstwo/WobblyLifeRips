using System;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class PlayerSpaceShipFeet : HawkNetworkSubBehaviour
{
	// Token: 0x06000213 RID: 531 RVA: 0x0000AE2C File Offset: 0x0000902C
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		PlayerSpaceShipMovement componentInParent = base.GetComponentInParent<PlayerSpaceShipMovement>();
		componentInParent.onGearsDownChanged += this.OnGearsDownChanged;
		this.OnGearsDownChanged(componentInParent, componentInParent.GetVehicleUpdateInfo().bGearsDown);
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0000AE6B File Offset: 0x0000906B
	private void OnGearsDownChanged(PlayerSpaceShipMovement movement, bool bGearsDown)
	{
		if (this.feetAnimator)
		{
			this.feetAnimator.SetBool("bGearsDown", bGearsDown);
		}
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000AE8B File Offset: 0x0000908B
	public PlayerSpaceShipFeet()
	{
	}

	// Token: 0x040001AE RID: 430
	[SerializeField]
	private Animator feetAnimator;
}
