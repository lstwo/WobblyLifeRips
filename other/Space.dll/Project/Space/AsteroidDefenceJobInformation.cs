using System;
using HawkNetworking;

// Token: 0x02000007 RID: 7
internal class AsteroidDefenceJobInformation : IHawkMessage
{
	// Token: 0x0600001B RID: 27 RVA: 0x000026D6 File Offset: 0x000008D6
	public void Deserialize(HawkNetReader reader)
	{
		this.health = reader.ReadInt16();
		this.waveNumber = reader.ReadSByte();
	}

	// Token: 0x0600001C RID: 28 RVA: 0x000026F0 File Offset: 0x000008F0
	public void Serialize(HawkNetWriter writer)
	{
		writer.Write(this.health);
		writer.Write(this.waveNumber);
	}

	// Token: 0x0600001D RID: 29 RVA: 0x0000270A File Offset: 0x0000090A
	public AsteroidDefenceJobInformation()
	{
	}

	// Token: 0x0400001A RID: 26
	public short health = 100;

	// Token: 0x0400001B RID: 27
	public sbyte waveNumber = -1;
}
