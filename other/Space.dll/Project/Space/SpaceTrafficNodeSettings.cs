using System;
using UnityEngine;

// Token: 0x02000036 RID: 54
[Serializable]
internal struct SpaceTrafficNodeSettings
{
	// Token: 0x04000164 RID: 356
	public bool bOverrideAcceleration;

	// Token: 0x04000165 RID: 357
	[Range(0f, 1f)]
	public float overrideAcceleration;

	// Token: 0x04000166 RID: 358
	public bool bUseBoost;

	// Token: 0x04000167 RID: 359
	public bool bLandingGearDown;

	// Token: 0x04000168 RID: 360
	public bool bUseNodeRotation;
}
