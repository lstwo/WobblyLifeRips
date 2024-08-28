using System;

// Token: 0x0200004F RID: 79
internal interface ISpaceGravityAreaObjectCallback
{
	// Token: 0x0600024D RID: 589
	void OnEntered(SpaceGravityArea gravityArea);

	// Token: 0x0600024E RID: 590
	void OnExited(SpaceGravityArea gravityArea);

	// Token: 0x0600024F RID: 591
	bool IsAllowedInGravityArea(SpaceGravityArea gravityArea);
}
