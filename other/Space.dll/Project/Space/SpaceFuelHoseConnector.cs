using System;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class SpaceFuelHoseConnector : HawkNetworkSubBehaviour
{
	// Token: 0x0600010A RID: 266 RVA: 0x00006667 File Offset: 0x00004867
	private void OnTriggerEnter(Collider other)
	{
		if (this.hose)
		{
			return;
		}
		this.hose = other.GetComponentInParent<SpaceFuelHose>();
		if (this.hose)
		{
			this.hose.ServerSetConnector(this);
		}
	}

	// Token: 0x0600010B RID: 267 RVA: 0x0000669C File Offset: 0x0000489C
	private void OnDestroy()
	{
		if (this.hose)
		{
			this.hose.ServerSetConnector(null);
		}
	}

	// Token: 0x0600010C RID: 268 RVA: 0x000066B7 File Offset: 0x000048B7
	public void RefuelShip()
	{
		if (this.shipRefueling)
		{
			this.shipRefueling.MarkRefueled();
		}
	}

	// Token: 0x0600010D RID: 269 RVA: 0x000066D1 File Offset: 0x000048D1
	internal void SetShip(SpaceMechanicJobShipRefueling shipRefueling)
	{
		this.shipRefueling = shipRefueling;
	}

	// Token: 0x0600010E RID: 270 RVA: 0x000066DA File Offset: 0x000048DA
	public Transform GetConnectorTransform()
	{
		return this.connectorTransform;
	}

	// Token: 0x0600010F RID: 271 RVA: 0x000066E2 File Offset: 0x000048E2
	public SpaceFuelHoseConnector()
	{
	}

	// Token: 0x040000E6 RID: 230
	[SerializeField]
	private Transform connectorTransform;

	// Token: 0x040000E7 RID: 231
	private SpaceFuelHose hose;

	// Token: 0x040000E8 RID: 232
	private SpaceMechanicJobShipRefueling shipRefueling;
}
