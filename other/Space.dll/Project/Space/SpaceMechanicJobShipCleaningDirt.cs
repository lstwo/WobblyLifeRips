using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

// Token: 0x02000015 RID: 21
[DisallowMultipleComponent]
public class SpaceMechanicJobShipCleaningDirt : MonoBehaviour
{
	// Token: 0x14000007 RID: 7
	// (add) Token: 0x060000A6 RID: 166 RVA: 0x00004FA0 File Offset: 0x000031A0
	// (remove) Token: 0x060000A7 RID: 167 RVA: 0x00004FD8 File Offset: 0x000031D8
	public event Action<SpaceMechanicJobShipCleaningDirt> onWaterHit
	{
		[CompilerGenerated]
		add
		{
			Action<SpaceMechanicJobShipCleaningDirt> action = this.onWaterHit;
			Action<SpaceMechanicJobShipCleaningDirt> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShipCleaningDirt> action3 = (Action<SpaceMechanicJobShipCleaningDirt>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShipCleaningDirt>>(ref this.onWaterHit, action3, action2);
			}
			while (action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<SpaceMechanicJobShipCleaningDirt> action = this.onWaterHit;
			Action<SpaceMechanicJobShipCleaningDirt> action2;
			do
			{
				action2 = action;
				Action<SpaceMechanicJobShipCleaningDirt> action3 = (Action<SpaceMechanicJobShipCleaningDirt>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange<Action<SpaceMechanicJobShipCleaningDirt>>(ref this.onWaterHit, action3, action2);
			}
			while (action != action2);
		}
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x0000500D File Offset: 0x0000320D
	private void OnParticleCollision(GameObject other)
	{
		if (this.bCleaned)
		{
			return;
		}
		if (other.GetComponentInParent<WaterParticle>())
		{
			Action<SpaceMechanicJobShipCleaningDirt> action = this.onWaterHit;
			if (action == null)
			{
				return;
			}
			action(this);
		}
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x00005036 File Offset: 0x00003236
	public void ApplyCleaned()
	{
		if (this.bCleaned)
		{
			return;
		}
		this.bCleaned = true;
		this.cleanedFXInstance.PopPlayPush(0);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060000AA RID: 170 RVA: 0x00005060 File Offset: 0x00003260
	public bool IsCleaned()
	{
		return this.bCleaned;
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00005068 File Offset: 0x00003268
	public SpaceMechanicJobShipCleaningDirt()
	{
	}

	// Token: 0x04000089 RID: 137
	[CompilerGenerated]
	private Action<SpaceMechanicJobShipCleaningDirt> onWaterHit;

	// Token: 0x0400008A RID: 138
	[SerializeField]
	private BaseParticleInstance cleanedFXInstance;

	// Token: 0x0400008B RID: 139
	private bool bCleaned;
}
