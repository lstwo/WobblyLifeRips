using System;
using HawkNetworking;
using UnityEngine.Events;

// Token: 0x0200001C RID: 28
public class SpaceMechanicJobShipUnityEvents : HawkNetworkSubBehaviour
{
	// Token: 0x060000DD RID: 221 RVA: 0x00005AF0 File Offset: 0x00003CF0
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		SpaceMechanicJobShip component = base.GetComponent<SpaceMechanicJobShip>();
		component.onServerShipLanded += this.JobShip_onServerShipLanded;
		component.onServerShipTakenOff += this.JobShip_onServerShipTakenOff;
		component.onServerShipFlownAway += this.JobShip_onServerShipFlownAway;
		component.onServerShipCompleted += this.JobShip_onServerShipCompleted;
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00005B51 File Offset: 0x00003D51
	private void JobShip_onServerShipCompleted(SpaceMechanicJobShip obj)
	{
		UnityEvent unityEvent = this.onServerShipCompleted;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00005B63 File Offset: 0x00003D63
	private void JobShip_onServerShipFlownAway(SpaceMechanicJobShip obj)
	{
		UnityEvent unityEvent = this.onServerShipFlownAway;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00005B75 File Offset: 0x00003D75
	private void JobShip_onServerShipTakenOff(SpaceMechanicJobShip obj)
	{
		UnityEvent unityEvent = this.onServerShipTakenOff;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00005B87 File Offset: 0x00003D87
	private void JobShip_onServerShipLanded(SpaceMechanicJobShip obj)
	{
		UnityEvent unityEvent = this.onServerShipLanded;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00005B99 File Offset: 0x00003D99
	public SpaceMechanicJobShipUnityEvents()
	{
	}

	// Token: 0x040000AA RID: 170
	public UnityEvent onServerShipLanded;

	// Token: 0x040000AB RID: 171
	public UnityEvent onServerShipTakenOff;

	// Token: 0x040000AC RID: 172
	public UnityEvent onServerShipFlownAway;

	// Token: 0x040000AD RID: 173
	public UnityEvent onServerShipCompleted;
}
