using System;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class SpaceRenderFarAway : MonoBehaviour
{
	// Token: 0x06000164 RID: 356 RVA: 0x00007E10 File Offset: 0x00006010
	private void Start()
	{
		this.normalPosition = base.transform.position;
		if (UnitySingleton<GameplayCameraManager>.Instance)
		{
			UnitySingleton<GameplayCameraManager>.GetRawInstance().onCameraPreCull.AddCallback(new Action<GameplayCamera>(this.OnCameraPreCull));
			UnitySingleton<GameplayCameraManager>.GetRawInstance().onCameraPostRender.AddCallback(new Action<GameplayCamera>(this.OnCameraPostRender));
		}
	}

	// Token: 0x06000165 RID: 357 RVA: 0x00007E70 File Offset: 0x00006070
	private void OnDestroy()
	{
		if (UnitySingleton<GameplayCameraManager>.InstanceExists)
		{
			UnitySingleton<GameplayCameraManager>.GetRawInstance().onCameraPreCull.RemoveCallback(new Action<GameplayCamera>(this.OnCameraPreCull));
			UnitySingleton<GameplayCameraManager>.GetRawInstance().onCameraPostRender.RemoveCallback(new Action<GameplayCamera>(this.OnCameraPostRender));
		}
	}

	// Token: 0x06000166 RID: 358 RVA: 0x00007EAF File Offset: 0x000060AF
	private void OnCameraPostRender(GameplayCamera gameplayCamera)
	{
		base.transform.position = this.normalPosition;
	}

	// Token: 0x06000167 RID: 359 RVA: 0x00007EC2 File Offset: 0x000060C2
	private void OnCameraPreCull(GameplayCamera gameplayCamera)
	{
		base.transform.position = this.normalPosition + gameplayCamera.transform.position;
	}

	// Token: 0x06000168 RID: 360 RVA: 0x00007EE5 File Offset: 0x000060E5
	public SpaceRenderFarAway()
	{
	}

	// Token: 0x0400013B RID: 315
	private Vector3 normalPosition;
}
