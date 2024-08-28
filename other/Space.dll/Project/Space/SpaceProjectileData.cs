using System;
using HawkNetworking;
using UnityEngine;

// Token: 0x0200002D RID: 45
internal struct SpaceProjectileData : IHawkMessage
{
	// Token: 0x06000158 RID: 344 RVA: 0x00007AF7 File Offset: 0x00005CF7
	public void Deserialize(HawkNetReader reader)
	{
		this.position = reader.ReadVector3();
		this.rotation = reader.ReadCompressedQuaternion(1);
		this.velocity = reader.ReadVector3();
	}

	// Token: 0x06000159 RID: 345 RVA: 0x00007B1E File Offset: 0x00005D1E
	public void Serialize(HawkNetWriter writer)
	{
		writer.Write(this.position);
		writer.WriteCompressedQuaternion(1, this.rotation);
		writer.Write(this.velocity);
	}

	// Token: 0x0400012F RID: 303
	public Vector3 position;

	// Token: 0x04000130 RID: 304
	public Quaternion rotation;

	// Token: 0x04000131 RID: 305
	public Vector3 velocity;
}
