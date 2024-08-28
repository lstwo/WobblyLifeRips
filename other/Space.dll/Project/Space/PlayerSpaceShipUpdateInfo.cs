using System;
using HawkNetworking;

// Token: 0x0200004B RID: 75
internal class PlayerSpaceShipUpdateInfo : IHawkMessage
{
	// Token: 0x0600021E RID: 542 RVA: 0x0000B0BC File Offset: 0x000092BC
	public void Deserialize(HawkNetReader reader)
	{
		bool flag;
		bool flag2;
		bool flag3;
		bool flag4;
		bool flag5;
		reader.ReadMask(ref this.bBoosting, ref this.bAccelerating, ref this.bGearsDown, ref flag, ref flag2, ref flag3, ref flag4, ref flag5);
	}

	// Token: 0x0600021F RID: 543 RVA: 0x0000B0EC File Offset: 0x000092EC
	public void Serialize(HawkNetWriter writer)
	{
		writer.WriteMask(this.bBoosting, this.bAccelerating, this.bGearsDown, false, false, false, false, false);
	}

	// Token: 0x06000220 RID: 544 RVA: 0x0000B116 File Offset: 0x00009316
	public PlayerSpaceShipUpdateInfo()
	{
	}

	// Token: 0x040001BB RID: 443
	public bool bBoosting;

	// Token: 0x040001BC RID: 444
	public bool bAccelerating;

	// Token: 0x040001BD RID: 445
	public bool bGearsDown = true;
}
