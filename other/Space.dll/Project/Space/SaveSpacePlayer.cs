using System;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000057 RID: 87
[Serializable]
internal class SaveSpacePlayer : IHawkMessage, ISerializationCallbackReceiver
{
	// Token: 0x06000288 RID: 648 RVA: 0x0000CD5E File Offset: 0x0000AF5E
	public void Deserialize(HawkNetReader reader)
	{
		this.currentHelmetGuid = reader.ReadString();
		this.currentHelmetColor = reader.ReadColor();
	}

	// Token: 0x06000289 RID: 649 RVA: 0x0000CD78 File Offset: 0x0000AF78
	public void OnAfterDeserialize()
	{
		if (this.money <= 0)
		{
			this.money = 0;
		}
	}

	// Token: 0x0600028A RID: 650 RVA: 0x0000CD8A File Offset: 0x0000AF8A
	public void OnBeforeSerialize()
	{
	}

	// Token: 0x0600028B RID: 651 RVA: 0x0000CD8C File Offset: 0x0000AF8C
	public void Serialize(HawkNetWriter writer)
	{
		writer.Write(this.currentHelmetGuid);
		writer.Write(this.currentHelmetColor);
	}

	// Token: 0x0600028C RID: 652 RVA: 0x0000CDA6 File Offset: 0x0000AFA6
	public SaveSpacePlayer()
	{
	}

	// Token: 0x040001FE RID: 510
	public const string GUID = "6b31dacc-6295-467a-a1df-4b3ccff9537f";

	// Token: 0x040001FF RID: 511
	public int money;

	// Token: 0x04000200 RID: 512
	public string currentHelmetGuid;

	// Token: 0x04000201 RID: 513
	public Color currentHelmetColor = Color.white;
}
