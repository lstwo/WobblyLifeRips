using System;
using System.Runtime.CompilerServices;
using System.Threading;
using HawkNetworking;
using Rewired;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x0200004C RID: 76
internal class PlayerSpaceShipMovement : PlayerVehicleMovement, ISpaceGravityAreaObjectCallback
{
	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06000221 RID: 545 RVA: 0x0000B128 File Offset: 0x00009328
	// (remove) Token: 0x06000222 RID: 546 RVA: 0x0000B160 File Offset: 0x00009360
	public event PlayerSpaceShipMovement.SpaceShipMovementGearsDown onGearsDownChanged
	{
		[CompilerGenerated]
		add
		{
			PlayerSpaceShipMovement.SpaceShipMovementGearsDown spaceShipMovementGearsDown = this.onGearsDownChanged;
			PlayerSpaceShipMovement.SpaceShipMovementGearsDown spaceShipMovementGearsDown2;
			do
			{
				spaceShipMovementGearsDown2 = spaceShipMovementGearsDown;
				PlayerSpaceShipMovement.SpaceShipMovementGearsDown spaceShipMovementGearsDown3 = (PlayerSpaceShipMovement.SpaceShipMovementGearsDown)Delegate.Combine(spaceShipMovementGearsDown2, value);
				spaceShipMovementGearsDown = Interlocked.CompareExchange<PlayerSpaceShipMovement.SpaceShipMovementGearsDown>(ref this.onGearsDownChanged, spaceShipMovementGearsDown3, spaceShipMovementGearsDown2);
			}
			while (spaceShipMovementGearsDown != spaceShipMovementGearsDown2);
		}
		[CompilerGenerated]
		remove
		{
			PlayerSpaceShipMovement.SpaceShipMovementGearsDown spaceShipMovementGearsDown = this.onGearsDownChanged;
			PlayerSpaceShipMovement.SpaceShipMovementGearsDown spaceShipMovementGearsDown2;
			do
			{
				spaceShipMovementGearsDown2 = spaceShipMovementGearsDown;
				PlayerSpaceShipMovement.SpaceShipMovementGearsDown spaceShipMovementGearsDown3 = (PlayerSpaceShipMovement.SpaceShipMovementGearsDown)Delegate.Remove(spaceShipMovementGearsDown2, value);
				spaceShipMovementGearsDown = Interlocked.CompareExchange<PlayerSpaceShipMovement.SpaceShipMovementGearsDown>(ref this.onGearsDownChanged, spaceShipMovementGearsDown3, spaceShipMovementGearsDown2);
			}
			while (spaceShipMovementGearsDown != spaceShipMovementGearsDown2);
		}
	}

	// Token: 0x06000223 RID: 547 RVA: 0x0000B195 File Offset: 0x00009395
	protected override void Awake()
	{
		base.Awake();
		this.shipSound = base.GetComponent<PlayerSpaceShipSound>();
	}

	// Token: 0x06000224 RID: 548 RVA: 0x0000B1AC File Offset: 0x000093AC
	public override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_UPDATE_VEHICLE_INFO = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientUpdateVehicleInfo), 1);
		this.RPC_SERVER_SET_GEARS_DOWN = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ServerSetGearsDown), new HawkNetworkObject.RPCValidateCallback(this.ServerSetGearsDownValidate));
		this.RPC_CLIENT_PLAY_CRASH = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientPlayCrash), 1);
		this.RPC_CLIENT_SET_ENGINE_ON = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSetEngineOn), 1);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000B230 File Offset: 0x00009430
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.playerVehicleInput = base.GetComponent<PlayerVehicleInput<SpaceShipInput>>();
		this.spaceShip = base.GetComponent<PlayerSpaceShip>();
		PlayerSpaceShip playerSpaceShip = this.spaceShip;
		playerSpaceShip.onPlayerDriverChanged = (ActionEnterExitInteract.ActionEnterExitDriverChanged)Delegate.Combine(playerSpaceShip.onPlayerDriverChanged, new ActionEnterExitInteract.ActionEnterExitDriverChanged(this.OnPlayerDriverChanged));
		this.gearsAction = new InputHintAction(this.GearsToggleInput);
		if (PlayerSpaceShipMovement.CrashMask == 0)
		{
			PlayerSpaceShipMovement.CrashMask = ~LayerMask.GetMask(new string[] { "Ragdoll" });
		}
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000B2B5 File Offset: 0x000094B5
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.spaceShip)
		{
			this.StopControlling(this.spaceShip.GetDriverPlayerController());
		}
	}

	// Token: 0x06000227 RID: 551 RVA: 0x0000B2DB File Offset: 0x000094DB
	void ISpaceGravityAreaObjectCallback.OnEntered(SpaceGravityArea gravityArea)
	{
		this.UpdateGravityAreaGravity();
	}

	// Token: 0x06000228 RID: 552 RVA: 0x0000B2E3 File Offset: 0x000094E3
	void ISpaceGravityAreaObjectCallback.OnExited(SpaceGravityArea gravityArea)
	{
		this.UpdateGravityAreaGravity();
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0000B2EC File Offset: 0x000094EC
	private void OnPlayerDriverChanged(ActionEnterExitInteract actionInteract, PlayerController previousDriver, PlayerController currentDriver)
	{
		this.StopControlling(previousDriver);
		this.StartUpdateControlling(currentDriver);
		if (this.networkObject != null && this.networkObject.IsServer() && !currentDriver && !this.vehicleInfo.bGearsDown)
		{
			this.networkObject.SendRPC(this.RPC_SERVER_SET_GEARS_DOWN, 3, new object[] { !this.vehicleInfo.bGearsDown });
		}
	}

	// Token: 0x0600022A RID: 554 RVA: 0x0000B360 File Offset: 0x00009560
	private void StartUpdateControlling(PlayerController playerController)
	{
		if (!playerController || !playerController.IsLocal())
		{
			return;
		}
		PlayerControllerInputHint playerControllerInputHint = playerController.GetPlayerControllerInputHint();
		if (playerControllerInputHint)
		{
			playerControllerInputHint.AddHint(this, "Vehicle_Special_1", this.vehicleInfo.bGearsDown ? this.inputHint_GearsUp.GetLocalizedString() : this.inputHint_GearsDown.GetLocalizedString(), this.gearsAction, 0, false);
		}
	}

	// Token: 0x0600022B RID: 555 RVA: 0x0000B3C8 File Offset: 0x000095C8
	private void StopControlling(PlayerController playerController)
	{
		if (!playerController || !playerController.IsLocal())
		{
			return;
		}
		PlayerControllerInputHint playerControllerInputHint = playerController.GetPlayerControllerInputHint();
		if (playerControllerInputHint)
		{
			playerControllerInputHint.RemoveHint(this, "Vehicle_Special_1");
		}
	}

	// Token: 0x0600022C RID: 556 RVA: 0x0000B401 File Offset: 0x00009601
	protected virtual void Reset()
	{
	}

	// Token: 0x0600022D RID: 557 RVA: 0x0000B404 File Offset: 0x00009604
	protected override void OnVehicleEnabledChanged(bool bVehicleEnabled)
	{
		base.OnVehicleEnabledChanged(bVehicleEnabled);
		if (!bVehicleEnabled)
		{
			this.vehicleInfo.bAccelerating = false;
			this.vehicleInfo.bBoosting = false;
			this.UpdateVisuals();
			this.ServerSetEngineOn(false, false);
			this.rigidbody.drag = 0f;
		}
		else
		{
			this.gravityRigidbody = UnityExtensions.GetOrAddComponent<SpaceGravityRigidbody>(base.gameObject);
			this.ServerSetEngineOn(true, false);
		}
		this.UpdateGravityAreaGravity();
	}

	// Token: 0x0600022E RID: 558 RVA: 0x0000B474 File Offset: 0x00009674
	private void UpdateGravityAreaGravity()
	{
		if (this.playerVehicle.IsVehicleEnabled())
		{
			this.rigidbody.useGravity = true;
			return;
		}
		if (this.gravityRigidbody)
		{
			this.rigidbody.useGravity = !this.gravityRigidbody.IsInsideOnlyDefaultGravityArea();
			return;
		}
		this.rigidbody.useGravity = true;
	}

	// Token: 0x0600022F RID: 559 RVA: 0x0000B4D0 File Offset: 0x000096D0
	public void ServerSetEngineOn(bool bEngineOn, bool bBypassEngineIgnition = false)
	{
		if (this.bEngineOn == bEngineOn)
		{
			return;
		}
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		this.networkObject.SendRPC(this.RPC_CLIENT_SET_ENGINE_ON, true, 6, new object[] { bEngineOn, bBypassEngineIgnition });
	}

	// Token: 0x06000230 RID: 560 RVA: 0x0000B528 File Offset: 0x00009728
	protected override void Update_Special()
	{
		base.Update_Special();
		this.UpdateVisuals();
	}

	// Token: 0x06000231 RID: 561 RVA: 0x0000B538 File Offset: 0x00009738
	protected override void FixedUpdate_Special()
	{
		this.vehicleSpeed = this.previousVelocity.magnitude;
		this.shipSound.SetVehicleSpeed(this.vehicleSpeed);
		base.FixedUpdate_Special();
		if (this.networkObject.IsServer() && this.playerVehicle.GetDriverPlayerController())
		{
			this.ServerSimulateMovement(this.playerVehicleInput.GetLatestInput(), this.playerVehicleInput.IsControlsEnabled(), false);
			if (Time.fixedTime - this.timeSinceSendVehicleInfo >= 0.05f)
			{
				this.timeSinceSendVehicleInfo = Time.fixedDeltaTime;
				this.ServerSendVehicleInfo();
			}
		}
	}

	// Token: 0x06000232 RID: 562 RVA: 0x0000B5CD File Offset: 0x000097CD
	private void UpdateVisuals()
	{
		this.UpdateBoostingVisuals();
		this.UpdateAcceleratingVisuals();
	}

	// Token: 0x06000233 RID: 563 RVA: 0x0000B5DC File Offset: 0x000097DC
	private void UpdateAcceleratingVisuals()
	{
		bool flag = this.vehicleInfo.bAccelerating;
		if (this.bTurnAccelerateFXOffOnBoost)
		{
			bool bBoosting = this.vehicleInfo.bBoosting;
			bool flag2 = Time.time - this.timeSinceBoostActivated >= this.boostDelay;
			if (bBoosting && flag2)
			{
				flag = false;
			}
		}
		if (flag)
		{
			if (this.accelerateParticles != null)
			{
				BaseParticle[] array = this.accelerateParticles;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Play();
				}
			}
		}
		else if (this.accelerateParticles != null)
		{
			BaseParticle[] array = this.accelerateParticles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop();
			}
		}
		this.shipSound.SetAccelerating(this.vehicleInfo.bAccelerating);
	}

	// Token: 0x06000234 RID: 564 RVA: 0x0000B68C File Offset: 0x0000988C
	private void UpdateBoostingVisuals()
	{
		bool bBoosting = this.vehicleInfo.bBoosting;
		bool flag = Time.time - this.timeSinceBoostActivated >= this.boostDelay;
		if (bBoosting && flag)
		{
			if (this.boostParticles != null)
			{
				BaseParticle[] array = this.boostParticles;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Play();
				}
			}
		}
		else if (this.boostParticles != null)
		{
			BaseParticle[] array = this.boostParticles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop();
			}
		}
		if (bBoosting && !flag)
		{
			if (this.boostDelayParticles != null)
			{
				BaseParticle[] array = this.boostDelayParticles;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Play();
				}
			}
		}
		else if (this.boostDelayParticles != null)
		{
			BaseParticle[] array = this.boostDelayParticles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop();
			}
		}
		this.shipSound.SetBoostOn(this.vehicleInfo.bBoosting);
	}

	// Token: 0x06000235 RID: 565 RVA: 0x0000B778 File Offset: 0x00009978
	internal void ServerSimulateMovement(SpaceShipInput input, bool bInputEnabled, bool bOnlySimulateRotationWhenInputDisabled = false)
	{
		if (this.vehicleInfo.bGearsDown)
		{
			return;
		}
		SpaceGravityArea currentGravityArea = this.gravityRigidbody.GetCurrentGravityArea();
		if (currentGravityArea)
		{
			Vector3 vector = -currentGravityArea.GetGravity();
			this.rigidbody.AddForce(vector, 5);
		}
		else
		{
			this.rigidbody.AddForce(-Physics.gravity, 5);
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float magnitude = this.rigidbody.velocity.magnitude;
		Vector3 eulerAngles = this.rigidbody.rotation.eulerAngles;
		if (bInputEnabled)
		{
			num = input.acceleration;
			num2 = input.sideMovement;
			eulerAngles.x = input.cameraX;
			eulerAngles.y = input.cameraY;
			num3 = input.upDownMovement;
		}
		else if (bOnlySimulateRotationWhenInputDisabled)
		{
			eulerAngles.x = input.cameraX;
			eulerAngles.y = input.cameraY;
		}
		bool flag = input.bBoost && num > 0f;
		this.vehicleInfo.bBoosting = flag;
		this.vehicleInfo.bAccelerating = input.acceleration > 0f;
		Vector3 vector2 = this.rigidbody.velocity;
		vector2 = this.rigidbody.transform.InverseTransformVector(vector2);
		float num4 = Mathf.Clamp(vector2.x / (this.sideAcceleration * 0.5f), -1f, 1f);
		if (Mathf.Abs(num2) > 0f)
		{
			eulerAngles.z = -num4 * 25f;
		}
		else
		{
			eulerAngles.z = num4 * 25f;
		}
		Quaternion quaternion = Quaternion.AngleAxis(eulerAngles.y, Vector3.up);
		quaternion *= Quaternion.AngleAxis(eulerAngles.x, Vector3.right);
		quaternion *= Quaternion.AngleAxis(eulerAngles.z, Vector3.forward);
		Vector3 vector3 = HawkMathUtils.ComputeTorque(this.rigidbody, quaternion, 2f, 1f);
		this.rigidbody.AddTorque(vector3);
		if (bOnlySimulateRotationWhenInputDisabled)
		{
			this.rigidbody.drag = Mathf.Lerp(this.rigidbody.drag, 0.5f, Time.fixedDeltaTime * 5f);
			return;
		}
		if (Mathf.Abs(input.acceleration) > 0f || Mathf.Abs(input.sideMovement) > 0f)
		{
			this.rigidbody.drag = magnitude / this.topSpeed;
		}
		else
		{
			this.rigidbody.drag = Mathf.Lerp(this.rigidbody.drag, 0.5f, Time.fixedDeltaTime * 5f);
		}
		if (Mathf.Abs(num2) > 0.01f)
		{
			Vector3 vector4 = Vector3.Cross(Vector3.up, base.transform.forward);
			vector4.y = 0f;
			vector4.Normalize();
			this.rigidbody.AddForce(vector4 * num2 * this.sideAcceleration, 5);
		}
		this.rigidbody.AddForce(Vector3.up * num3 * this.upDownAcceleration, 5);
		if (num <= 0f)
		{
			this.rigidbody.AddForce(base.transform.forward * num * this.backAcceleration, 5);
			return;
		}
		if (flag && Time.time - this.timeSinceBoostActivated >= this.boostDelay)
		{
			this.rigidbody.AddForce(base.transform.forward * num * this.boostAcceleration, 5);
			return;
		}
		this.rigidbody.AddForce(base.transform.forward * num * this.forwardAcceleration, 5);
	}

	// Token: 0x06000236 RID: 566 RVA: 0x0000BB2F File Offset: 0x00009D2F
	private void ServerSendVehicleInfo()
	{
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		this.OnVehicleInfoUpdated();
		this.networkObject.SendRPCUnreliable(this.RPC_CLIENT_UPDATE_VEHICLE_INFO, 1, SerializerDeserializerExtensions.SerializeNonAlloc(this.vehicleInfo));
	}

	// Token: 0x06000237 RID: 567 RVA: 0x0000BB70 File Offset: 0x00009D70
	private void GearsToggleInput(InputHintData hintData, Player player)
	{
		if (player.GetButtonDown(hintData.actionName) && this.networkObject != null)
		{
			this.networkObject.SendRPC(this.RPC_SERVER_SET_GEARS_DOWN, 3, new object[] { !this.vehicleInfo.bGearsDown });
		}
	}

	// Token: 0x06000238 RID: 568 RVA: 0x0000BBC4 File Offset: 0x00009DC4
	private void OnVehicleInfoUpdated()
	{
		if (this.bPreviousGearsDown != this.vehicleInfo.bGearsDown)
		{
			this.bPreviousGearsDown = this.vehicleInfo.bGearsDown;
			this.StartUpdateControlling(this.spaceShip.GetDriverPlayerController());
			PlayerSpaceShipMovement.SpaceShipMovementGearsDown spaceShipMovementGearsDown = this.onGearsDownChanged;
			if (spaceShipMovementGearsDown != null)
			{
				spaceShipMovementGearsDown(this, this.vehicleInfo.bGearsDown);
			}
		}
		if (this.bPreviousBoost != this.vehicleInfo.bBoosting)
		{
			this.bPreviousBoost = this.vehicleInfo.bBoosting;
			this.timeSinceBoostActivated = Time.time;
		}
	}

	// Token: 0x06000239 RID: 569 RVA: 0x0000BC54 File Offset: 0x00009E54
	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		if (this.networkObject == null)
		{
			return;
		}
		ContactPoint contactPoint;
		float num;
		if (PlayerVehicleRoadMovement.HasCrashed(collision, base.transform, PlayerSpaceShipMovement.CrashMask, ref contactPoint, ref num))
		{
			this.networkObject.SendRPCUnreliable(this.RPC_CLIENT_PLAY_CRASH, 0, new object[] { contactPoint.point, num });
		}
	}

	// Token: 0x0600023A RID: 570 RVA: 0x0000BCB8 File Offset: 0x00009EB8
	private void ClientSetEngineOn(HawkNetReader reader, HawkRPCInfo info)
	{
		this.bEngineOn = reader.ReadBoolean();
		bool flag = reader.ReadBoolean();
		if (this.shipSound)
		{
			this.shipSound.SetEngineOn(this.bEngineOn, flag);
		}
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0000BCF8 File Offset: 0x00009EF8
	private void ClientPlayCrash(HawkNetReader reader, HawkRPCInfo info)
	{
		Vector3 vector = reader.ReadVector3();
		float num = reader.ReadSingle();
		this.shipSound.PlayCrash(vector, num);
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000BD20 File Offset: 0x00009F20
	private void ClientUpdateVehicleInfo(HawkNetReader reader, HawkRPCInfo info)
	{
		SerializerDeserializerExtensions.Deserialize<PlayerSpaceShipUpdateInfo>(this.vehicleInfo, reader.ReadBytesAndSize());
		this.OnVehicleInfoUpdated();
	}

	// Token: 0x0600023D RID: 573 RVA: 0x0000BD3A File Offset: 0x00009F3A
	private void ServerSetGearsDown(HawkNetReader reader, HawkRPCInfo info)
	{
		this.vehicleInfo.bGearsDown = reader.ReadBoolean();
		this.playerVehicle.SetVehicleEnabled(!this.vehicleInfo.bGearsDown, true);
		this.ServerSendVehicleInfo();
	}

	// Token: 0x0600023E RID: 574 RVA: 0x0000BD70 File Offset: 0x00009F70
	private bool ServerSetGearsDownValidate(HawkRPCValidateInfo validateInfo)
	{
		return validateInfo.sender.IsHost || (this.spaceShip.GetDriverPlayerController() && this.spaceShip.GetDriverPlayerController().networkObject.GetOwner() == validateInfo.sender);
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000BDBC File Offset: 0x00009FBC
	bool ISpaceGravityAreaObjectCallback.IsAllowedInGravityArea(SpaceGravityArea gravityArea)
	{
		return true;
	}

	// Token: 0x06000240 RID: 576 RVA: 0x0000BDC0 File Offset: 0x00009FC0
	internal void ServerSetGearsDown(bool bGearsDown)
	{
		if (this.vehicleInfo.bGearsDown == bGearsDown)
		{
			return;
		}
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		this.networkObject.SendRPC(this.RPC_SERVER_SET_GEARS_DOWN, 3, new object[] { bGearsDown });
	}

	// Token: 0x06000241 RID: 577 RVA: 0x0000BE13 File Offset: 0x0000A013
	public float GetVehicleSpeed()
	{
		return this.vehicleSpeed;
	}

	// Token: 0x06000242 RID: 578 RVA: 0x0000BE1B File Offset: 0x0000A01B
	internal PlayerSpaceShipUpdateInfo GetVehicleUpdateInfo()
	{
		return this.vehicleInfo;
	}

	// Token: 0x06000243 RID: 579 RVA: 0x0000BE24 File Offset: 0x0000A024
	public PlayerSpaceShipMovement()
	{
	}

	// Token: 0x040001BE RID: 446
	private const string ActionID_ToggleGears = "Vehicle_Special_1";

	// Token: 0x040001BF RID: 447
	private const float UpdateVehicleInfoInterval = 0.05f;

	// Token: 0x040001C0 RID: 448
	private byte RPC_CLIENT_UPDATE_VEHICLE_INFO;

	// Token: 0x040001C1 RID: 449
	private byte RPC_SERVER_SET_GEARS_DOWN;

	// Token: 0x040001C2 RID: 450
	private byte RPC_CLIENT_PLAY_CRASH;

	// Token: 0x040001C3 RID: 451
	private byte RPC_CLIENT_SET_ENGINE_ON;

	// Token: 0x040001C4 RID: 452
	private static int CrashMask;

	// Token: 0x040001C5 RID: 453
	[CompilerGenerated]
	private PlayerSpaceShipMovement.SpaceShipMovementGearsDown onGearsDownChanged;

	// Token: 0x040001C6 RID: 454
	[Header("Settings")]
	[SerializeField]
	private float topSpeed = 20f;

	// Token: 0x040001C7 RID: 455
	[SerializeField]
	private float forwardAcceleration = 100f;

	// Token: 0x040001C8 RID: 456
	[SerializeField]
	private float backAcceleration = 50f;

	// Token: 0x040001C9 RID: 457
	[SerializeField]
	private float sideAcceleration = 50f;

	// Token: 0x040001CA RID: 458
	[SerializeField]
	private float upDownAcceleration = 10f;

	// Token: 0x040001CB RID: 459
	[Header("Boost")]
	[SerializeField]
	private BaseParticle[] boostDelayParticles;

	// Token: 0x040001CC RID: 460
	[SerializeField]
	private BaseParticle[] boostParticles;

	// Token: 0x040001CD RID: 461
	[SerializeField]
	private BaseParticle[] accelerateParticles;

	// Token: 0x040001CE RID: 462
	[SerializeField]
	private float boostAcceleration = 300f;

	// Token: 0x040001CF RID: 463
	[SerializeField]
	private float boostDelay;

	// Token: 0x040001D0 RID: 464
	[SerializeField]
	private bool bTurnAccelerateFXOffOnBoost;

	// Token: 0x040001D1 RID: 465
	[Header("Text")]
	[SerializeField]
	private LocalizedString inputHint_GearsUp;

	// Token: 0x040001D2 RID: 466
	[SerializeField]
	private LocalizedString inputHint_GearsDown;

	// Token: 0x040001D3 RID: 467
	private PlayerSpaceShip spaceShip;

	// Token: 0x040001D4 RID: 468
	private PlayerVehicleInput<SpaceShipInput> playerVehicleInput;

	// Token: 0x040001D5 RID: 469
	private PlayerSpaceShipSound shipSound;

	// Token: 0x040001D6 RID: 470
	private PlayerSpaceShipUpdateInfo vehicleInfo = new PlayerSpaceShipUpdateInfo();

	// Token: 0x040001D7 RID: 471
	private float timeSinceSendVehicleInfo;

	// Token: 0x040001D8 RID: 472
	private float vehicleSpeed;

	// Token: 0x040001D9 RID: 473
	private float timeSinceBoostActivated;

	// Token: 0x040001DA RID: 474
	private bool bPreviousGearsDown = true;

	// Token: 0x040001DB RID: 475
	private bool bPreviousBoost;

	// Token: 0x040001DC RID: 476
	private bool bEngineOn;

	// Token: 0x040001DD RID: 477
	private SpaceGravityRigidbody gravityRigidbody;

	// Token: 0x040001DE RID: 478
	private InputHintAction gearsAction;

	// Token: 0x0200007F RID: 127
	// (Invoke) Token: 0x06000348 RID: 840
	public delegate void SpaceShipMovementGearsDown(PlayerSpaceShipMovement movement, bool bGearsDown);
}
