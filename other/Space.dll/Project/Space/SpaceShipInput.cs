using System;
using HawkNetworking;

// Token: 0x02000049 RID: 73
public struct SpaceShipInput : IInput, IHawkMessage
{
	// Token: 0x06000216 RID: 534 RVA: 0x0000AE93 File Offset: 0x00009093
	public void ResetInput()
	{
		this.acceleration = 0f;
		this.bBoost = false;
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0000AEA8 File Offset: 0x000090A8
	public void Deserialize(HawkNetReader reader)
	{
		this.acceleration = reader.ReadCompressedSingleInput();
		this.cameraX = reader.ReadCompressedAngle();
		this.cameraY = reader.ReadCompressedAngle();
		this.sideMovement = reader.ReadCompressedSingleInput();
		this.upDownMovement = reader.ReadCompressedSingleInput();
		this.bBoost = reader.ReadBoolean();
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0000AF00 File Offset: 0x00009100
	public void Serialize(HawkNetWriter writer)
	{
		writer.WriteCompressedInput(this.acceleration);
		writer.WriteCompressedAngle(this.cameraX);
		writer.WriteCompressedAngle(this.cameraY);
		writer.WriteCompressedInput(this.sideMovement);
		writer.WriteCompressedInput(this.upDownMovement);
		writer.Write(this.bBoost);
	}

	// Token: 0x040001AF RID: 431
	public float acceleration;

	// Token: 0x040001B0 RID: 432
	public float cameraX;

	// Token: 0x040001B1 RID: 433
	public float cameraY;

	// Token: 0x040001B2 RID: 434
	public float sideMovement;

	// Token: 0x040001B3 RID: 435
	public float upDownMovement;

	// Token: 0x040001B4 RID: 436
	public bool bBoost;
}
