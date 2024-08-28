using System;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class PlayerSpaceHovercraftMovement : PlayerVehicleMovement
{
	// Token: 0x060001DE RID: 478 RVA: 0x000099D5 File Offset: 0x00007BD5
	protected override void Awake()
	{
		base.Awake();
		this.hovercraftSound = base.GetComponent<PlayerSpaceHovercraftSound>();
	}

	// Token: 0x060001DF RID: 479 RVA: 0x000099E9 File Offset: 0x00007BE9
	public override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_PLAY_CRASH = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientPlayCrash), 1);
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x00009A0B File Offset: 0x00007C0B
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		if (PlayerSpaceHovercraftMovement.CrashMask == 0)
		{
			PlayerSpaceHovercraftMovement.CrashMask = ~LayerMask.GetMask(new string[] { "Ragdoll" });
		}
		this.hovercraftInput = base.GetComponent<PlayerSpaceHovercraftInput>();
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x00009A40 File Offset: 0x00007C40
	protected override void OnVehicleEnabledChanged(bool bVehicleEnabled)
	{
		base.OnVehicleEnabledChanged(bVehicleEnabled);
		if (this.animator)
		{
			this.animator.SetBool("bVehicleEnabled", bVehicleEnabled);
		}
		this.hovercraftSound.SetEngineOn(bVehicleEnabled);
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x00009A74 File Offset: 0x00007C74
	protected override void FixedUpdate_Special()
	{
		this.vehicleSpeed = this.previousVelocity.magnitude;
		this.hovercraftSound.SetVehicleSpeed(this.vehicleSpeed);
		base.FixedUpdate_Special();
		if (this.networkObject.IsServer())
		{
			this.ServerSimulateMovement(this.hovercraftInput.GetLatestInput(), this.hovercraftInput.IsControlsEnabled());
		}
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x00009AD4 File Offset: 0x00007CD4
	private void ServerSimulateMovement(SpaceHovercraftInput input, bool bInputEnabled)
	{
		float num = 0f;
		float num2 = 0f;
		Vector3 eulerAngles = this.rigidbody.rotation.eulerAngles;
		if (bInputEnabled)
		{
			num = input.acceleration;
			num2 = input.sideMovement;
			eulerAngles.x = 0f;
			eulerAngles.y = input.cameraY;
			eulerAngles.z = 0f;
		}
		Quaternion quaternion = Quaternion.AngleAxis(eulerAngles.y, Vector3.up);
		quaternion *= Quaternion.AngleAxis(eulerAngles.x, Vector3.right);
		quaternion *= Quaternion.AngleAxis(eulerAngles.z, Vector3.forward);
		Vector3 vector = HawkMathUtils.ComputeTorque(this.rigidbody, quaternion, this.rotateFrequency, 1f);
		this.rigidbody.AddTorque(vector);
		bool flag = Mathf.Abs(input.acceleration) > 0f || Mathf.Abs(input.sideMovement) > 0f;
		if (Mathf.Abs(num2) > 0.01f)
		{
			Vector3 vector2 = Vector3.Cross(Vector3.up, base.transform.forward);
			vector2.y = 0f;
			vector2.Normalize();
			this.rigidbody.AddForce(vector2 * num2 * this.sideAcceleration, 5);
		}
		if (num > 0f)
		{
			this.rigidbody.AddForce(base.transform.forward * num * this.forwardAcceleration, 5);
		}
		else
		{
			this.rigidbody.AddForce(base.transform.forward * num * this.backAcceleration, 5);
		}
		if (flag)
		{
			Vector3 velocity = this.rigidbody.velocity;
			velocity.y = 0f;
			this.rigidbody.drag = velocity.magnitude / this.topSpeed;
			return;
		}
		this.rigidbody.drag = Mathf.Lerp(this.rigidbody.drag, 0.5f, Time.fixedDeltaTime * 5f);
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x00009CD4 File Offset: 0x00007ED4
	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		if (this.networkObject == null)
		{
			return;
		}
		ContactPoint contactPoint;
		float num;
		if (PlayerVehicleRoadMovement.HasCrashed(collision, base.transform, PlayerSpaceHovercraftMovement.CrashMask, ref contactPoint, ref num))
		{
			this.networkObject.SendRPCUnreliable(this.RPC_CLIENT_PLAY_CRASH, 0, new object[] { contactPoint.point, num });
		}
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x00009D38 File Offset: 0x00007F38
	private void ClientPlayCrash(HawkNetReader reader, HawkRPCInfo info)
	{
		reader.ReadVector3();
		float num = reader.ReadSingle();
		this.hovercraftSound.PlayCrash(num);
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x00009D5F File Offset: 0x00007F5F
	public PlayerSpaceHovercraftMovement()
	{
	}

	// Token: 0x04000187 RID: 391
	private byte RPC_CLIENT_PLAY_CRASH;

	// Token: 0x04000188 RID: 392
	private static int CrashMask;

	// Token: 0x04000189 RID: 393
	[Header("Settings")]
	[SerializeField]
	private float topSpeed = 20f;

	// Token: 0x0400018A RID: 394
	[SerializeField]
	private float forwardAcceleration = 100f;

	// Token: 0x0400018B RID: 395
	[SerializeField]
	private float backAcceleration = 50f;

	// Token: 0x0400018C RID: 396
	[SerializeField]
	private float sideAcceleration = 50f;

	// Token: 0x0400018D RID: 397
	[SerializeField]
	private float rotateFrequency = 2f;

	// Token: 0x0400018E RID: 398
	[SerializeField]
	private Animator animator;

	// Token: 0x0400018F RID: 399
	private PlayerSpaceHovercraftInput hovercraftInput;

	// Token: 0x04000190 RID: 400
	private PlayerSpaceHovercraftSound hovercraftSound;

	// Token: 0x04000191 RID: 401
	private float vehicleSpeed;
}
