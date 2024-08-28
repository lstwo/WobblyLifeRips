using System;
using HawkNetworking;

// Token: 0x02000040 RID: 64
public struct SpaceHovercraftInput : IInput, IHawkMessage
{
	// Token: 0x060001D6 RID: 470 RVA: 0x00009858 File Offset: 0x00007A58
	public void ResetInput()
	{
		this.acceleration = 0f;
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x00009865 File Offset: 0x00007A65
	public void Deserialize(HawkNetReader reader)
	{
		this.acceleration = reader.ReadCompressedSingleInput();
		this.cameraX = reader.ReadCompressedAngle();
		this.cameraY = reader.ReadCompressedAngle();
		this.sideMovement = reader.ReadCompressedSingleInput();
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x00009897 File Offset: 0x00007A97
	public void Serialize(HawkNetWriter writer)
	{
		writer.WriteCompressedInput(this.acceleration);
		writer.WriteCompressedAngle(this.cameraX);
		writer.WriteCompressedAngle(this.cameraY);
		writer.WriteCompressedInput(this.sideMovement);
	}

	// Token: 0x0400017F RID: 383
	public float acceleration;

	// Token: 0x04000180 RID: 384
	public float cameraX;

	// Token: 0x04000181 RID: 385
	public float cameraY;

	// Token: 0x04000182 RID: 386
	public float sideMovement;
}
