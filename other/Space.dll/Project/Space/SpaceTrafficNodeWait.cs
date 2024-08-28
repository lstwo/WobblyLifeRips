using System;
using UnityEngine;

// Token: 0x0200003D RID: 61
[DisallowMultipleComponent]
public class SpaceTrafficNodeWait : MonoBehaviour
{
	// Token: 0x060001D0 RID: 464 RVA: 0x000097D6 File Offset: 0x000079D6
	public float GetWaitTime()
	{
		return Random.Range(this.minWaitTime, this.maxWaitTime);
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x000097E9 File Offset: 0x000079E9
	public SpaceTrafficNodeWait()
	{
	}

	// Token: 0x0400017C RID: 380
	[SerializeField]
	private float minWaitTime;

	// Token: 0x0400017D RID: 381
	[SerializeField]
	private float maxWaitTime;
}
