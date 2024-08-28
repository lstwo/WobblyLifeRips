using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FMODUnity;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000029 RID: 41
[RequireComponent(typeof(SpacePortCannonInput))]
public class SpacePortCannon : HawkNetworkBehaviour
{
	// Token: 0x06000142 RID: 322 RVA: 0x000074D3 File Offset: 0x000056D3
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_FIRE = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientFire), 1);
	}

	// Token: 0x06000143 RID: 323 RVA: 0x000074F8 File Offset: 0x000056F8
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.transformSync = base.GetComponentInChildren<HawkTransformSync>();
		if (this.transformSync)
		{
			this.transformSync.enabled = false;
		}
		this.animator.enabled = false;
		this.orbAnimator.enabled = false;
		this.cannonInput = base.GetComponent<SpacePortCannonInput>();
		this.cannonInput.enabled = false;
		this.rigidbody = base.GetComponentInChildren<Rigidbody>();
		this.actionInteract.onDriverChanged += new ActionEnterExitInteract.ActionEnterExitDriverChanged(this.OnDriverChanged);
		this.showOnlyToDriverRenderer.gameObject.SetActive(false);
	}

	// Token: 0x06000144 RID: 324 RVA: 0x00007598 File Offset: 0x00005798
	private void OnDriverChanged(ActionEnterExitInteract actionInteract, PlayerController previousDriver, PlayerController currentDriver)
	{
		if (this.serverFixedUpdateCoroutine != null)
		{
			base.StopCoroutine(this.serverFixedUpdateCoroutine);
		}
		if (this.transformSync)
		{
			this.transformSync.enabled = currentDriver;
		}
		this.animator.enabled = currentDriver;
		this.orbAnimator.enabled = currentDriver && currentDriver.IsLocal();
		this.SetIgnoreColliders(previousDriver, false);
		this.SetIgnoreColliders(currentDriver, true);
		this.cannonInput.enabled = currentDriver && currentDriver.IsLocal();
		if (currentDriver)
		{
			this.serverFixedUpdateCoroutine = base.StartCoroutine(this.ServerFixedUpdate());
		}
		if (previousDriver && previousDriver.IsLocal())
		{
			this.OnLocalExited(previousDriver);
		}
		if (currentDriver && currentDriver.IsLocal())
		{
			this.OnLocalEntered(currentDriver);
		}
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00007678 File Offset: 0x00005878
	private void OnLocalEntered(PlayerController playerController)
	{
		if (this.showOnlyToDriverRenderer)
		{
			this.showOnlyToDriverRenderer.gameObject.SetActive(true);
			this.showOnlyToDriverRenderer.Show(playerController);
		}
	}

	// Token: 0x06000146 RID: 326 RVA: 0x000076A4 File Offset: 0x000058A4
	private void OnLocalExited(PlayerController playerController)
	{
		if (this.showOnlyToDriverRenderer)
		{
			this.showOnlyToDriverRenderer.gameObject.SetActive(false);
			this.showOnlyToDriverRenderer.Hide(playerController);
		}
	}

	// Token: 0x06000147 RID: 327 RVA: 0x000076D0 File Offset: 0x000058D0
	private void SetIgnoreColliders(PlayerController playerController, bool bIgnore)
	{
		if (!playerController || this.ignorePlayerColliders == null)
		{
			return;
		}
		PlayerCharacter playerCharacter = playerController.GetPlayerCharacter();
		if (playerCharacter)
		{
			Rigidbody hipRigidbody = playerCharacter.GetHipRigidbody();
			if (hipRigidbody)
			{
				foreach (Collider collider in hipRigidbody.GetComponentsInChildren<Collider>())
				{
					Collider[] array = this.ignorePlayerColliders;
					for (int j = 0; j < array.Length; j++)
					{
						Physics.IgnoreCollision(array[j], collider, bIgnore);
					}
				}
			}
		}
	}

	// Token: 0x06000148 RID: 328 RVA: 0x0000774E File Offset: 0x0000594E
	private IEnumerator ServerFixedUpdate()
	{
		WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
		for (;;)
		{
			yield return fixedUpdate;
			this.ServerSimulateMovement(this.cannonInput.GetLatestInput(), this.cannonInput.IsControlsEnabled());
		}
		yield break;
	}

	// Token: 0x06000149 RID: 329 RVA: 0x00007760 File Offset: 0x00005960
	private void ServerSimulateMovement(SpacePortCannonInputData inputData, bool bControlsEnabled)
	{
		if (!bControlsEnabled)
		{
			return;
		}
		Quaternion quaternion = Quaternion.Euler(Mathf.Clamp(inputData.cameraX, this.xAngleMin, this.xAngleMax), inputData.cameraY, 0f);
		Quaternion quaternion2 = this.rigidbody.rotation;
		quaternion2 = Quaternion.Slerp(quaternion2, quaternion, Time.fixedDeltaTime * 10f);
		if (inputData.bFire && Time.time - this.rateOfFireTime >= this.rateOfFire)
		{
			Vector3 eulerAngles = quaternion2.eulerAngles;
			eulerAngles.x -= this.recoilAmountAngle;
			quaternion2.eulerAngles = eulerAngles;
			this.rateOfFireTime = Time.time;
			if (this.bulletSpawn)
			{
				NetworkPrefab.SpawnNetworkPrefab(this.bulletPrefab.gameObject, delegate(HawkNetworkBehaviour x)
				{
					if (x)
					{
						SpaceProjectile spaceProjectile = x as SpaceProjectile;
						if (spaceProjectile)
						{
							spaceProjectile.ServerSetData(new SpaceProjectileData
							{
								position = x.transform.position,
								rotation = x.transform.rotation,
								velocity = this.bulletSpawn.transform.forward * this.bulletSpeed
							}, true);
							this.networkObject.SendRPC(this.RPC_CLIENT_FIRE, 0, Array.Empty<object>());
						}
					}
				}, new Vector3?(this.bulletSpawn.position), new Quaternion?(this.bulletSpawn.rotation), null, true, true, false, true);
			}
		}
		this.rigidbody.MoveRotation(quaternion2);
	}

	// Token: 0x0600014A RID: 330 RVA: 0x0000785D File Offset: 0x00005A5D
	public void PlayOrbAlarm()
	{
		if (this.orbAnimator)
		{
			this.orbAnimator.CrossFade("Alarm", 0.1f, 0, 0f);
		}
	}

	// Token: 0x0600014B RID: 331 RVA: 0x00007887 File Offset: 0x00005A87
	public void PlayAnimation(string state, int layer = 0)
	{
		if (this.animator)
		{
			this.animator.Play(state, layer);
		}
	}

	// Token: 0x0600014C RID: 332 RVA: 0x000078A3 File Offset: 0x00005AA3
	private void ClientFire(HawkNetReader reader, HawkRPCInfo info)
	{
		if (!string.IsNullOrEmpty(this.fireSound))
		{
			RuntimeManager.PlayOneShot(this.fireSound, base.transform.position);
		}
		this.PlayAnimation(this.animationStateFire, 0);
	}

	// Token: 0x0600014D RID: 333 RVA: 0x000078D5 File Offset: 0x00005AD5
	public PlayerController GetDriverController()
	{
		if (this.actionInteract)
		{
			return this.actionInteract.GetDriverController();
		}
		return null;
	}

	// Token: 0x0600014E RID: 334 RVA: 0x000078F1 File Offset: 0x00005AF1
	public ActionEnterExitInteract GetActionEnterExitInteract()
	{
		return this.actionInteract;
	}

	// Token: 0x0600014F RID: 335 RVA: 0x000078FC File Offset: 0x00005AFC
	public SpacePortCannon()
	{
	}

	// Token: 0x06000150 RID: 336 RVA: 0x0000795C File Offset: 0x00005B5C
	[CompilerGenerated]
	private void <ServerSimulateMovement>b__27_0(HawkNetworkBehaviour x)
	{
		if (x)
		{
			SpaceProjectile spaceProjectile = x as SpaceProjectile;
			if (spaceProjectile)
			{
				spaceProjectile.ServerSetData(new SpaceProjectileData
				{
					position = x.transform.position,
					rotation = x.transform.rotation,
					velocity = this.bulletSpawn.transform.forward * this.bulletSpeed
				}, true);
				this.networkObject.SendRPC(this.RPC_CLIENT_FIRE, 0, Array.Empty<object>());
			}
		}
	}

	// Token: 0x04000116 RID: 278
	private byte RPC_CLIENT_FIRE;

	// Token: 0x04000117 RID: 279
	[SerializeField]
	private ActionEnterExitInteract actionInteract;

	// Token: 0x04000118 RID: 280
	[SerializeField]
	private float xAngleMin = -90f;

	// Token: 0x04000119 RID: 281
	[SerializeField]
	private float xAngleMax = -3f;

	// Token: 0x0400011A RID: 282
	[SerializeField]
	private Collider[] ignorePlayerColliders;

	// Token: 0x0400011B RID: 283
	[SerializeField]
	private SpaceProjectile bulletPrefab;

	// Token: 0x0400011C RID: 284
	[SerializeField]
	private Transform bulletSpawn;

	// Token: 0x0400011D RID: 285
	[SerializeField]
	private float bulletSpeed = 10f;

	// Token: 0x0400011E RID: 286
	[SerializeField]
	private float rateOfFire = 1f;

	// Token: 0x0400011F RID: 287
	[SerializeField]
	private float recoilAmountAngle = 5f;

	// Token: 0x04000120 RID: 288
	[SerializeField]
	private PlayerVisibilityRenderer showOnlyToDriverRenderer;

	// Token: 0x04000121 RID: 289
	[SerializeField]
	[EventRef]
	private string fireSound = "event:/Objects_Space/AsteroidDefenceJob/Objects_Space_AsteroidDefenceJob_CannonFire";

	// Token: 0x04000122 RID: 290
	[SerializeField]
	private Animator animator;

	// Token: 0x04000123 RID: 291
	[SerializeField]
	private Animator orbAnimator;

	// Token: 0x04000124 RID: 292
	[SerializeField]
	private string animationStateFire = "Cannon Reload";

	// Token: 0x04000125 RID: 293
	private SpacePortCannonInput cannonInput;

	// Token: 0x04000126 RID: 294
	private Coroutine serverFixedUpdateCoroutine;

	// Token: 0x04000127 RID: 295
	private Rigidbody rigidbody;

	// Token: 0x04000128 RID: 296
	private float rateOfFireTime;

	// Token: 0x04000129 RID: 297
	private HawkTransformSync transformSync;

	// Token: 0x02000072 RID: 114
	[CompilerGenerated]
	private sealed class <ServerFixedUpdate>d__26 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x06000314 RID: 788 RVA: 0x0000F86D File Offset: 0x0000DA6D
		[DebuggerHidden]
		public <ServerFixedUpdate>d__26(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0000F87C File Offset: 0x0000DA7C
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0000F880 File Offset: 0x0000DA80
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpacePortCannon spacePortCannon = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				spacePortCannon.ServerSimulateMovement(spacePortCannon.cannonInput.GetLatestInput(), spacePortCannon.cannonInput.IsControlsEnabled());
			}
			else
			{
				this.<>1__state = -1;
				fixedUpdate = new WaitForFixedUpdate();
			}
			this.<>2__current = fixedUpdate;
			this.<>1__state = 1;
			return true;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000317 RID: 791 RVA: 0x0000F8EF File Offset: 0x0000DAEF
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0000F8F7 File Offset: 0x0000DAF7
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000319 RID: 793 RVA: 0x0000F8FE File Offset: 0x0000DAFE
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000273 RID: 627
		private int <>1__state;

		// Token: 0x04000274 RID: 628
		private object <>2__current;

		// Token: 0x04000275 RID: 629
		public SpacePortCannon <>4__this;

		// Token: 0x04000276 RID: 630
		private WaitForFixedUpdate <fixedUpdate>5__2;
	}
}
