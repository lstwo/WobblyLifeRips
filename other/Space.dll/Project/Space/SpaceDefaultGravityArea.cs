using System;
using UnityEngine;

// Token: 0x02000050 RID: 80
public class SpaceDefaultGravityArea : SpaceGravityArea
{
	// Token: 0x06000250 RID: 592 RVA: 0x0000C0E0 File Offset: 0x0000A2E0
	protected override void UpdateGravity()
	{
		Vector3 newGravity = base.GetNewGravity();
		foreach (SpaceGravityRigidbody spaceGravityRigidbody in this.gravityRigidbodies)
		{
			if (spaceGravityRigidbody.IsInsideOnlyDefaultGravityArea())
			{
				Rigidbody rigidbody = spaceGravityRigidbody.GetRigidbody();
				if (!rigidbody.IsSleeping() && rigidbody.useGravity && rigidbody.velocity.sqrMagnitude > 0.001f)
				{
					spaceGravityRigidbody.ApplyVelocity(this, newGravity, 5);
				}
			}
		}
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0000C174 File Offset: 0x0000A374
	public SpaceDefaultGravityArea()
	{
	}
}
