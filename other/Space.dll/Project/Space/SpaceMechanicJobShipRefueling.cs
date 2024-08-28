using System;
using HawkNetworking;
using UnityEngine;

// Token: 0x0200001A RID: 26
internal class SpaceMechanicJobShipRefueling : SpaceMechanicJobShipTask
{
	// Token: 0x060000CD RID: 205 RVA: 0x00005A1A File Offset: 0x00003C1A
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		if (base.IsTaskEnabled())
		{
			this.connector.SetShip(this);
			return;
		}
		this.connector.enabled = false;
	}

	// Token: 0x060000CE RID: 206 RVA: 0x00005A44 File Offset: 0x00003C44
	public void MarkRefueled()
	{
		if (!this.bIsRefueled)
		{
			this.bIsRefueled = true;
			this.whiteBoardData.bRefuelShip = true;
			base.ServerUpdateWhiteboardData();
			base.CompleteTask(true);
		}
	}

	// Token: 0x060000CF RID: 207 RVA: 0x00005A6E File Offset: 0x00003C6E
	public override bool IsTaskComplete()
	{
		return this.bIsRefueled;
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00005A76 File Offset: 0x00003C76
	public SpaceMechanicJobShipRefueling()
	{
	}

	// Token: 0x040000A5 RID: 165
	[SerializeField]
	private SpaceFuelHoseConnector connector;

	// Token: 0x040000A6 RID: 166
	private bool bIsRefueled;
}
