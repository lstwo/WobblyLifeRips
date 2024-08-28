using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000044 RID: 68
public class PlayerSpaceHovercraftTrailer : HawkNetworkSubBehaviour
{
	// Token: 0x060001EC RID: 492 RVA: 0x00009EC8 File Offset: 0x000080C8
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.lineRenderer.enabled = false;
		this.playerVehicle = base.GetComponentInParent<PlayerVehicle>();
		PlayerVehicle playerVehicle = this.playerVehicle;
		playerVehicle.onVehicleEnabledChanged = (PlayerVehicle.OnVehicleEnabledDelegate)Delegate.Combine(playerVehicle.onVehicleEnabledChanged, new PlayerVehicle.OnVehicleEnabledDelegate(this.OnVehicleEnabledChanged));
	}

	// Token: 0x060001ED RID: 493 RVA: 0x00009F1B File Offset: 0x0000811B
	private void OnDisable()
	{
		this.updateLineRendererCoroutine = null;
	}

	// Token: 0x060001EE RID: 494 RVA: 0x00009F24 File Offset: 0x00008124
	private void OnVehicleEnabledChanged(bool bVehicleEnabled)
	{
		if (bVehicleEnabled)
		{
			if (this.updateLineRendererCoroutine != null)
			{
				base.StopCoroutine(this.updateLineRendererCoroutine);
			}
			this.updateLineRendererCoroutine = base.StartCoroutine(this.UpdateLineRenderer());
		}
		else
		{
			if (this.updateLineRendererCoroutine != null)
			{
				base.StopCoroutine(this.updateLineRendererCoroutine);
				this.updateLineRendererCoroutine = null;
			}
			this.lineRenderer.enabled = false;
		}
		if (this.animator)
		{
			this.animator.SetBool("bVehicleEnabled", bVehicleEnabled);
		}
	}

	// Token: 0x060001EF RID: 495 RVA: 0x00009FA1 File Offset: 0x000081A1
	private IEnumerator UpdateLineRenderer()
	{
		Vector3[] points = new Vector3[2];
		this.lineRenderer.positionCount = 2;
		this.lineRenderer.useWorldSpace = true;
		for (int i = 0; i < 1; i++)
		{
			points[i] = this.ourTransform.position;
		}
		points[1] = this.targetTransform.position;
		for (;;)
		{
			yield return null;
			Vector3 position = this.ourTransform.position;
			Vector3 position2 = this.targetTransform.position;
			for (int j = 1; j < 1; j++)
			{
				float num = (float)(j / 2);
				Vector3 vector = Vector3.Lerp(position, position2, num);
				points[j] = vector;
			}
			this.lineRenderer.enabled = (position - position2).sqrMagnitude > 0.1f;
			if (this.lineRenderer.enabled)
			{
				points[0] = position;
				points[1] = position2;
				this.lineRenderer.SetPositions(points);
			}
		}
		yield break;
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x00009FB0 File Offset: 0x000081B0
	public PlayerSpaceHovercraftTrailer()
	{
	}

	// Token: 0x04000195 RID: 405
	[SerializeField]
	private Transform targetTransform;

	// Token: 0x04000196 RID: 406
	[SerializeField]
	private Transform ourTransform;

	// Token: 0x04000197 RID: 407
	[SerializeField]
	private LineRenderer lineRenderer;

	// Token: 0x04000198 RID: 408
	[SerializeField]
	private Animator animator;

	// Token: 0x04000199 RID: 409
	private Coroutine updateLineRendererCoroutine;

	// Token: 0x0400019A RID: 410
	private PlayerVehicle playerVehicle;

	// Token: 0x0200007A RID: 122
	[CompilerGenerated]
	private sealed class <UpdateLineRenderer>d__9 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x06000332 RID: 818 RVA: 0x00010380 File Offset: 0x0000E580
		[DebuggerHidden]
		public <UpdateLineRenderer>d__9(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0001038F File Offset: 0x0000E58F
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00010394 File Offset: 0x0000E594
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			PlayerSpaceHovercraftTrailer playerSpaceHovercraftTrailer = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				Vector3 position = playerSpaceHovercraftTrailer.ourTransform.position;
				Vector3 position2 = playerSpaceHovercraftTrailer.targetTransform.position;
				for (int i = 1; i < 1; i++)
				{
					float num2 = (float)(i / 2);
					Vector3 vector = Vector3.Lerp(position, position2, num2);
					points[i] = vector;
				}
				playerSpaceHovercraftTrailer.lineRenderer.enabled = (position - position2).sqrMagnitude > 0.1f;
				if (playerSpaceHovercraftTrailer.lineRenderer.enabled)
				{
					points[0] = position;
					points[1] = position2;
					playerSpaceHovercraftTrailer.lineRenderer.SetPositions(points);
				}
			}
			else
			{
				this.<>1__state = -1;
				points = new Vector3[2];
				playerSpaceHovercraftTrailer.lineRenderer.positionCount = 2;
				playerSpaceHovercraftTrailer.lineRenderer.useWorldSpace = true;
				for (int j = 0; j < 1; j++)
				{
					points[j] = playerSpaceHovercraftTrailer.ourTransform.position;
				}
				points[1] = playerSpaceHovercraftTrailer.targetTransform.position;
			}
			this.<>2__current = null;
			this.<>1__state = 1;
			return true;
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000335 RID: 821 RVA: 0x000104E2 File Offset: 0x0000E6E2
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x06000336 RID: 822 RVA: 0x000104EA File Offset: 0x0000E6EA
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000337 RID: 823 RVA: 0x000104F1 File Offset: 0x0000E6F1
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000298 RID: 664
		private int <>1__state;

		// Token: 0x04000299 RID: 665
		private object <>2__current;

		// Token: 0x0400029A RID: 666
		public PlayerSpaceHovercraftTrailer <>4__this;

		// Token: 0x0400029B RID: 667
		private Vector3[] <points>5__2;
	}
}
