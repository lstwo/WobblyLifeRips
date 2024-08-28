using System;
using UnityEngine;

// Token: 0x02000003 RID: 3
[Serializable]
internal class AsteroidDefenceHealthState
{
	// Token: 0x06000003 RID: 3 RVA: 0x00002084 File Offset: 0x00000284
	public AsteroidDefenceHealthState()
	{
	}

	// Token: 0x04000003 RID: 3
	[Range(0f, 1f)]
	[Tooltip("Will show the gameobject if the health is above this value and there is no other active health state")]
	public float healthPercentage = 1f;

	// Token: 0x04000004 RID: 4
	public GameObject showGameObject;
}
