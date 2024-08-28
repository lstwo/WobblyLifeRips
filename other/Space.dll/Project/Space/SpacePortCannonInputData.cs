using System;
using HawkNetworking;

// Token: 0x0200002A RID: 42
internal struct SpacePortCannonInputData : IInput, IHawkMessage
{
	// Token: 0x06000151 RID: 337 RVA: 0x000079ED File Offset: 0x00005BED
	public void ResetInput()
	{
		this.bFire = false;
	}

	// Token: 0x06000152 RID: 338 RVA: 0x000079F6 File Offset: 0x00005BF6
	public void Deserialize(HawkNetReader reader)
	{
		this.cameraX = reader.ReadSingle();
		this.cameraY = reader.ReadSingle();
		this.bFire = reader.ReadBoolean();
	}

	// Token: 0x06000153 RID: 339 RVA: 0x00007A1C File Offset: 0x00005C1C
	public void Serialize(HawkNetWriter writer)
	{
		writer.Write(this.cameraX);
		writer.Write(this.cameraY);
		writer.Write(this.bFire);
	}

	// Token: 0x0400012A RID: 298
	public float cameraX;

	// Token: 0x0400012B RID: 299
	public float cameraY;

	// Token: 0x0400012C RID: 300
	public bool bFire;
}
