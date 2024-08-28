using System;
using HawkNetworking;

// Token: 0x02000014 RID: 20
internal class SpaceMechanicJobShipCleaning : SpaceMechanicJobShipTask
{
	// Token: 0x060000A0 RID: 160 RVA: 0x00004DBC File Offset: 0x00002FBC
	private void Awake()
	{
		this.dirts = base.GetComponentsInChildren<SpaceMechanicJobShipCleaningDirt>();
		this.cleaningPacket.cleanedArray = new bool[this.dirts.Length];
		for (int i = 0; i < this.dirts.Length; i++)
		{
			this.dirts[i].onWaterHit += this.OnWaterHitDirt;
		}
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x00004E19 File Offset: 0x00003019
	public override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_UPDATE_CLEANED = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientUpdateCleaned), 1);
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x00004E3C File Offset: 0x0000303C
	private void OnWaterHitDirt(SpaceMechanicJobShipCleaningDirt dirt)
	{
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		int num = Array.IndexOf<SpaceMechanicJobShipCleaningDirt>(this.dirts, dirt);
		if (this.cleaningPacket.cleanedArray.Length != 0 && num >= 0 && num < this.cleaningPacket.cleanedArray.Length && !this.dirts[num].IsCleaned())
		{
			this.cleaningPacket.cleanedArray[num] = true;
			this.networkObject.SendRPC(this.RPC_CLIENT_UPDATE_CLEANED, true, 6, new object[] { SerializerDeserializerExtensions.SerializeNonAlloc(this.cleaningPacket) });
			SpaceMechanicJobWhiteBoardData whiteBoardData = this.whiteBoardData;
			int num2 = this.cleaningCount;
			this.cleaningCount = num2 + 1;
			whiteBoardData.washedShipPercent = (byte)((float)num2 / (float)(this.dirts.Length - 1) * 100f);
			base.ServerUpdateWhiteboardData();
		}
		if (this.IsTaskComplete())
		{
			base.CompleteTask(true);
		}
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00004F24 File Offset: 0x00003124
	private void ClientUpdateCleaned(HawkNetReader reader, HawkRPCInfo info)
	{
		SerializerDeserializerExtensions.Deserialize<SpaceMechanicJobShipCleaningPacket>(this.cleaningPacket, reader.ReadBytesAndSize());
		for (int i = 0; i < this.cleaningPacket.cleanedArray.Length; i++)
		{
			if (this.cleaningPacket.cleanedArray[i])
			{
				this.dirts[i].ApplyCleaned();
			}
		}
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00004F77 File Offset: 0x00003177
	public override bool IsTaskComplete()
	{
		return this.cleaningCount >= this.dirts.Length;
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00004F8C File Offset: 0x0000318C
	public SpaceMechanicJobShipCleaning()
	{
	}

	// Token: 0x04000085 RID: 133
	private byte RPC_CLIENT_UPDATE_CLEANED;

	// Token: 0x04000086 RID: 134
	private SpaceMechanicJobShipCleaningDirt[] dirts;

	// Token: 0x04000087 RID: 135
	private SpaceMechanicJobShipCleaningPacket cleaningPacket = new SpaceMechanicJobShipCleaningPacket();

	// Token: 0x04000088 RID: 136
	private int cleaningCount;
}
