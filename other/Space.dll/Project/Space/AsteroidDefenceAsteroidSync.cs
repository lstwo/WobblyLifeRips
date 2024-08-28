using System;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000002 RID: 2
public struct AsteroidDefenceAsteroidSync : IHawkMessage
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	public void Deserialize(HawkNetReader reader)
	{
		this.position = reader.ReadVector3();
		this.velocity = reader.ReadVector3();
	}

	// Token: 0x06000002 RID: 2 RVA: 0x0000206A File Offset: 0x0000026A
	public void Serialize(HawkNetWriter writer)
	{
		writer.Write(this.position);
		writer.Write(this.velocity);
	}

	// Token: 0x04000001 RID: 1
	public Vector3 position;

	// Token: 0x04000002 RID: 2
	public Vector3 velocity;
}
