using System;
using UnityEngine;

// Token: 0x02000052 RID: 82
[DisallowMultipleComponent]
public class SpaceGravityRigidbody : MonoBehaviour
{
	// Token: 0x06000261 RID: 609 RVA: 0x0000C5BC File Offset: 0x0000A7BC
	public void IncrementGravityAreaRef()
	{
		this.gravityAreaRef += 1;
	}

	// Token: 0x06000262 RID: 610 RVA: 0x0000C5CD File Offset: 0x0000A7CD
	public void DecrementGravityAreaRef()
	{
		if (this.gravityAreaRef == 0)
		{
			return;
		}
		this.gravityAreaRef -= 1;
		if (this.gravityAreaRef <= 0)
		{
			this.lastGravityArea = null;
		}
	}

	// Token: 0x06000263 RID: 611 RVA: 0x0000C5F7 File Offset: 0x0000A7F7
	public void ApplyVelocity(SpaceGravityArea gravityArea, Vector3 velocity, ForceMode mode)
	{
		this.lastGravityArea = gravityArea;
		this.rigidbody.AddForce(velocity, mode);
	}

	// Token: 0x06000264 RID: 612 RVA: 0x0000C60D File Offset: 0x0000A80D
	public void SetRigidbody(Rigidbody rigidbody)
	{
		this.rigidbody = rigidbody;
	}

	// Token: 0x06000265 RID: 613 RVA: 0x0000C616 File Offset: 0x0000A816
	public Rigidbody GetRigidbody()
	{
		return this.rigidbody;
	}

	// Token: 0x06000266 RID: 614 RVA: 0x0000C61E File Offset: 0x0000A81E
	public SpaceGravityArea GetCurrentGravityArea()
	{
		return this.lastGravityArea;
	}

	// Token: 0x06000267 RID: 615 RVA: 0x0000C626 File Offset: 0x0000A826
	public bool IsInsideOnlyDefaultGravityArea()
	{
		return this.gravityAreaRef == 1;
	}

	// Token: 0x06000268 RID: 616 RVA: 0x0000C631 File Offset: 0x0000A831
	public Vector3 GetCurrentGravity()
	{
		if (this.lastGravityArea)
		{
			return this.lastGravityArea.GetGravity();
		}
		return Physics.gravity;
	}

	// Token: 0x06000269 RID: 617 RVA: 0x0000C651 File Offset: 0x0000A851
	public SpaceGravityRigidbody()
	{
	}

	// Token: 0x040001EC RID: 492
	private ushort gravityAreaRef;

	// Token: 0x040001ED RID: 493
	private Rigidbody rigidbody;

	// Token: 0x040001EE RID: 494
	private SpaceGravityArea lastGravityArea;
}
