using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200003A RID: 58
[RequireComponent(typeof(SpaceTrafficNode))]
public class SpaceTrafficNodeNext : SpaceTrafficNodeCallbacks
{
	// Token: 0x060001BA RID: 442 RVA: 0x00009538 File Offset: 0x00007738
	private void Awake()
	{
		this.bHasWaitNode = this.waitNode;
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000954B File Offset: 0x0000774B
	public override void VehicleHeadingTowardsNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0000954D File Offset: 0x0000774D
	public override void VehicleLeftNode(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0000954F File Offset: 0x0000774F
	public override void VehicleReachedNode(PlayerSpaceShipAI spaceShipAI)
	{
		if (this.bHasWaitNode)
		{
			base.StartCoroutine(this.VehicleReachedNode_Wait(spaceShipAI));
			return;
		}
		spaceShipAI.SetTargetNode(this.nextNode);
	}

	// Token: 0x060001BE RID: 446 RVA: 0x00009574 File Offset: 0x00007774
	public override void VehicleNodeUpdate(PlayerSpaceShipAI spaceShipAI)
	{
	}

	// Token: 0x060001BF RID: 447 RVA: 0x00009576 File Offset: 0x00007776
	private IEnumerator VehicleReachedNode_Wait(PlayerSpaceShipAI spaceShipAI)
	{
		float time = Time.time;
		float seconds = this.waitNode.GetWaitTime();
		while (Time.time - time <= seconds)
		{
			yield return null;
		}
		if (spaceShipAI)
		{
			spaceShipAI.SetTargetNode(this.nextNode);
		}
		yield break;
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x0000958C File Offset: 0x0000778C
	private void OnDrawGizmos()
	{
		if (this.nextNode)
		{
			if (!this.myNode)
			{
				this.myNode = base.GetComponent<SpaceTrafficNode>();
			}
			Gizmos.color = this.myNode.GetGizmosColor();
			Gizmos.DrawSphere(base.transform.position, 0.5f);
			Gizmos.color = this.nextNode.GetGizmosColor();
			Vector3 normalized = (this.nextNode.transform.position - base.transform.position).normalized;
			Vector3 vector = Vector3.Cross(Vector3.up, normalized);
			Gizmos.DrawLine(base.transform.position, this.nextNode.transform.position);
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(base.transform.position, base.transform.position - normalized - vector);
			Gizmos.DrawLine(base.transform.position, base.transform.position - normalized + vector);
			return;
		}
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x000096C4 File Offset: 0x000078C4
	public SpaceTrafficNodeNext()
	{
	}

	// Token: 0x04000175 RID: 373
	[SerializeField]
	private SpaceTrafficNode nextNode;

	// Token: 0x04000176 RID: 374
	[SerializeField]
	private SpaceTrafficNodeWait waitNode;

	// Token: 0x04000177 RID: 375
	private bool bHasWaitNode;

	// Token: 0x02000078 RID: 120
	[CompilerGenerated]
	private sealed class <VehicleReachedNode_Wait>d__8 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x0600032A RID: 810 RVA: 0x000102A4 File Offset: 0x0000E4A4
		[DebuggerHidden]
		public <VehicleReachedNode_Wait>d__8(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x0600032B RID: 811 RVA: 0x000102B3 File Offset: 0x0000E4B3
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x0600032C RID: 812 RVA: 0x000102B8 File Offset: 0x0000E4B8
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpaceTrafficNodeNext spaceTrafficNodeNext = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
			}
			else
			{
				this.<>1__state = -1;
				time = Time.time;
				seconds = spaceTrafficNodeNext.waitNode.GetWaitTime();
			}
			if (Time.time - time > seconds)
			{
				if (spaceShipAI)
				{
					spaceShipAI.SetTargetNode(spaceTrafficNodeNext.nextNode);
				}
				return false;
			}
			this.<>2__current = null;
			this.<>1__state = 1;
			return true;
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600032D RID: 813 RVA: 0x0001034B File Offset: 0x0000E54B
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00010353 File Offset: 0x0000E553
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600032F RID: 815 RVA: 0x0001035A File Offset: 0x0000E55A
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000291 RID: 657
		private int <>1__state;

		// Token: 0x04000292 RID: 658
		private object <>2__current;

		// Token: 0x04000293 RID: 659
		public SpaceTrafficNodeNext <>4__this;

		// Token: 0x04000294 RID: 660
		public PlayerSpaceShipAI spaceShipAI;

		// Token: 0x04000295 RID: 661
		private float <time>5__2;

		// Token: 0x04000296 RID: 662
		private float <seconds>5__3;
	}
}
