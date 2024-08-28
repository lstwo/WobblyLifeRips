using System;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000016 RID: 22
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class SpaceMechanicJobShipFeet : HawkNetworkSubBehaviour
{
	// Token: 0x060000AC RID: 172 RVA: 0x00005070 File Offset: 0x00003270
	public override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_SET_FEET_OUT = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSetFeetOut), 1);
	}

	// Token: 0x060000AD RID: 173 RVA: 0x00005094 File Offset: 0x00003294
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.animator = base.GetComponent<Animator>();
		this.OnFeetUpdated();
		SpaceMechanicJobShip componentInParent = base.GetComponentInParent<SpaceMechanicJobShip>();
		componentInParent.onServerShipPreLanded += this.ServerShipPreLanded;
		componentInParent.onServerShipTakenOff += this.ServerShipTakenOff;
	}

	// Token: 0x060000AE RID: 174 RVA: 0x000050E3 File Offset: 0x000032E3
	private void ServerShipTakenOff(SpaceMechanicJobShip ship)
	{
		this.networkObject.SendRPC(this.RPC_CLIENT_SET_FEET_OUT, true, 6, new object[] { false });
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00005107 File Offset: 0x00003307
	private void ServerShipPreLanded(SpaceMechanicJobShip ship)
	{
		this.networkObject.SendRPC(this.RPC_CLIENT_SET_FEET_OUT, true, 6, new object[] { true });
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x0000512B File Offset: 0x0000332B
	private void OnFeetUpdated()
	{
		this.animator.SetBool("bGearsDown", this.bGearsDown);
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00005143 File Offset: 0x00003343
	private void ClientSetFeetOut(HawkNetReader reader, HawkRPCInfo info)
	{
		this.bGearsDown = reader.ReadBoolean();
		this.OnFeetUpdated();
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x00005157 File Offset: 0x00003357
	public SpaceMechanicJobShipFeet()
	{
	}

	// Token: 0x0400008C RID: 140
	private byte RPC_CLIENT_SET_FEET_OUT;

	// Token: 0x0400008D RID: 141
	private bool bGearsDown;

	// Token: 0x0400008E RID: 142
	private Animator animator;
}
