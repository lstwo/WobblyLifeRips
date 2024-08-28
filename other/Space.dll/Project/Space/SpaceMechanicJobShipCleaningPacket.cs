using System;
using HawkNetworking;

// Token: 0x02000013 RID: 19
internal class SpaceMechanicJobShipCleaningPacket : IHawkMessage
{
	// Token: 0x0600009D RID: 157 RVA: 0x00004D44 File Offset: 0x00002F44
	public void Deserialize(HawkNetReader reader)
	{
		if (this.cleanedArray == null)
		{
			return;
		}
		for (int i = 0; i < this.cleanedArray.Length; i++)
		{
			this.cleanedArray[i] = reader.ReadBoolean();
		}
	}

	// Token: 0x0600009E RID: 158 RVA: 0x00004D7C File Offset: 0x00002F7C
	public void Serialize(HawkNetWriter writer)
	{
		if (this.cleanedArray == null)
		{
			return;
		}
		for (int i = 0; i < this.cleanedArray.Length; i++)
		{
			writer.Write(this.cleanedArray[i]);
		}
	}

	// Token: 0x0600009F RID: 159 RVA: 0x00004DB3 File Offset: 0x00002FB3
	public SpaceMechanicJobShipCleaningPacket()
	{
	}

	// Token: 0x04000084 RID: 132
	public bool[] cleanedArray;
}
