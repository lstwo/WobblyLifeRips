using System;
using HawkNetworking;

// Token: 0x0200001D RID: 29
internal class SpaceMechanicJobWhiteBoardData : IHawkMessage
{
	// Token: 0x060000E3 RID: 227 RVA: 0x00005BA1 File Offset: 0x00003DA1
	public void Deserialize(HawkNetReader reader)
	{
		this.bOpen = reader.ReadBoolean();
		this.partsFixed = reader.ReadByte();
		this.partsFixedOutOf = reader.ReadByte();
		this.bRefuelShip = reader.ReadBoolean();
		this.washedShipPercent = reader.ReadByte();
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00005BDF File Offset: 0x00003DDF
	public void Serialize(HawkNetWriter writer)
	{
		writer.Write(this.bOpen);
		writer.Write(this.partsFixed);
		writer.Write(this.partsFixedOutOf);
		writer.Write(this.bRefuelShip);
		writer.Write(this.washedShipPercent);
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00005C1D File Offset: 0x00003E1D
	public SpaceMechanicJobWhiteBoardData()
	{
	}

	// Token: 0x040000AE RID: 174
	public bool bOpen;

	// Token: 0x040000AF RID: 175
	public byte partsFixed;

	// Token: 0x040000B0 RID: 176
	public byte partsFixedOutOf;

	// Token: 0x040000B1 RID: 177
	public bool bRefuelShip;

	// Token: 0x040000B2 RID: 178
	public byte washedShipPercent;
}
