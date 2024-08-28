using System;
using System.Collections.Generic;
using HawkNetworking;

// Token: 0x02000018 RID: 24
internal class SpaceMechanicJobShipMissingPartsData : IHawkMessage
{
	// Token: 0x060000C2 RID: 194 RVA: 0x000056F4 File Offset: 0x000038F4
	public void Deserialize(HawkNetReader reader)
	{
		byte b = reader.ReadByte();
		this.partsIndex.Clear();
		for (int i = 0; i < (int)b; i++)
		{
			this.partsIndex.Add(reader.ReadByte());
		}
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00005730 File Offset: 0x00003930
	public void Serialize(HawkNetWriter writer)
	{
		byte b = (byte)this.partsIndex.Count;
		writer.Write(b);
		for (int i = 0; i < (int)b; i++)
		{
			writer.Write(this.partsIndex[i]);
		}
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x0000576F File Offset: 0x0000396F
	public SpaceMechanicJobShipMissingPartsData()
	{
	}

	// Token: 0x0400009F RID: 159
	public List<byte> partsIndex = new List<byte>();
}
