using System;
using System.Collections.Generic;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000019 RID: 25
internal class SpaceMechanicJobShipMissingParts : SpaceMechanicJobShipTask
{
	// Token: 0x060000C5 RID: 197 RVA: 0x00005782 File Offset: 0x00003982
	public override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_UPDATE_PARTS = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientUpdateParts), 1);
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x000057A4 File Offset: 0x000039A4
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.missingParts = base.GetComponentsInChildren<SpaceMechanicJobShipMissingPart>();
		if (networkObject.IsServer())
		{
			this.maxParts = (byte)Mathf.Min(this.missingParts.Length, (int)this.maxParts);
			List<SpaceMechanicJobShipMissingPart> list = new List<SpaceMechanicJobShipMissingPart>(this.missingParts);
			for (int i = 0; i < list.Count; i++)
			{
				SpaceMechanicJobShipMissingPart spaceMechanicJobShipMissingPart = list[i];
				int num = Random.Range(0, list.Count);
				SpaceMechanicJobShipMissingPart spaceMechanicJobShipMissingPart2 = list[num];
				list[i] = spaceMechanicJobShipMissingPart2;
				list[num] = spaceMechanicJobShipMissingPart;
			}
			for (int j = 0; j < (int)this.maxParts; j++)
			{
				this.partsData.partsIndex.Add((byte)Array.IndexOf<SpaceMechanicJobShipMissingPart>(this.missingParts, list[j]));
				SpaceMechanicJobShipMissingPart spaceMechanicJobShipMissingPart3 = list[j];
				spaceMechanicJobShipMissingPart3.onServerComplete = (Action<SpaceMechanicJobShipMissingPart>)Delegate.Combine(spaceMechanicJobShipMissingPart3.onServerComplete, new Action<SpaceMechanicJobShipMissingPart>(this.OnServerPartCompleted));
			}
			for (int k = (int)this.maxParts; k < list.Count; k++)
			{
				list[k].ServerSetStage(SpaceMechanicJobShipMissingPart.ShipMissingPartStage.Repaired);
			}
			networkObject.SendRPC(this.RPC_CLIENT_UPDATE_PARTS, true, 6, new object[] { SerializerDeserializerExtensions.SerializeNonAlloc(this.partsData) });
		}
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x000058E6 File Offset: 0x00003AE6
	protected override void OnInitalizeWhiteboard()
	{
		base.OnInitalizeWhiteboard();
		if (this.whiteBoardData != null)
		{
			this.whiteBoardData.partsFixedOutOf = (byte)this.partsData.partsIndex.Count;
		}
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00005912 File Offset: 0x00003B12
	private void OnServerPartCompleted(SpaceMechanicJobShipMissingPart part)
	{
		SpaceMechanicJobWhiteBoardData whiteBoardData = this.whiteBoardData;
		whiteBoardData.partsFixed += 1;
		base.ServerUpdateWhiteboardData();
		if (this.IsTaskComplete())
		{
			base.CompleteTask(true);
		}
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x00005940 File Offset: 0x00003B40
	public override bool IsTaskComplete()
	{
		if (!this.bRecievedParts)
		{
			return false;
		}
		for (int i = 0; i < this.partsData.partsIndex.Count; i++)
		{
			if (!this.missingParts[(int)this.partsData.partsIndex[i]].IsTaskComplete())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060000CA RID: 202 RVA: 0x00005994 File Offset: 0x00003B94
	public override void OnServerShipLanded()
	{
		base.OnServerShipLanded();
		for (int i = 0; i < this.partsData.partsIndex.Count; i++)
		{
			this.missingParts[(int)this.partsData.partsIndex[i]].OnServerShipLanded();
		}
	}

	// Token: 0x060000CB RID: 203 RVA: 0x000059DF File Offset: 0x00003BDF
	private void ClientUpdateParts(HawkNetReader reader, HawkRPCInfo info)
	{
		SerializerDeserializerExtensions.Deserialize<SpaceMechanicJobShipMissingPartsData>(this.partsData, reader.ReadBytesAndSize());
		this.bRecievedParts = true;
		this.OnInitalizeWhiteboard();
	}

	// Token: 0x060000CC RID: 204 RVA: 0x00005A00 File Offset: 0x00003C00
	public SpaceMechanicJobShipMissingParts()
	{
	}

	// Token: 0x040000A0 RID: 160
	private byte RPC_CLIENT_UPDATE_PARTS;

	// Token: 0x040000A1 RID: 161
	[SerializeField]
	private byte maxParts = 3;

	// Token: 0x040000A2 RID: 162
	private SpaceMechanicJobShipMissingPart[] missingParts;

	// Token: 0x040000A3 RID: 163
	private SpaceMechanicJobShipMissingPartsData partsData = new SpaceMechanicJobShipMissingPartsData();

	// Token: 0x040000A4 RID: 164
	private bool bRecievedParts;
}
