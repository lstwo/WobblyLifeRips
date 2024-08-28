using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FMOD.Studio;
using FMODUnity;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class SpaceTube : HawkNetworkBehaviour
{
	// Token: 0x0600016B RID: 363 RVA: 0x00007EFD File Offset: 0x000060FD
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_PLAYSTOP_TUBE_SOUND = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientPlayStopTubeSound), 1);
	}

	// Token: 0x0600016C RID: 364 RVA: 0x00007F1F File Offset: 0x0000611F
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		WobblyPlayerCharacterTrigger wobblyPlayerCharacterTrigger = this.enterTrigger;
		wobblyPlayerCharacterTrigger.onServerPlayerEnter = (WobblyPlayerCharacterTriggerBase.WobblyPlayerCharacterTriggerCallback)Delegate.Combine(wobblyPlayerCharacterTrigger.onServerPlayerEnter, new WobblyPlayerCharacterTriggerBase.WobblyPlayerCharacterTriggerCallback(this.OnEnterTriggerEnter));
	}

	// Token: 0x0600016D RID: 365 RVA: 0x00007F50 File Offset: 0x00006150
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.characterTubeSounds != null)
		{
			foreach (KeyValuePair<uint, EventInstance> keyValuePair in this.characterTubeSounds)
			{
				keyValuePair.Value.stop(1);
				keyValuePair.Value.release();
			}
			this.characterTubeSounds.Clear();
		}
		if (this.enteredCharacters != null)
		{
			foreach (KeyValuePair<PlayerCharacter, IPlayerCharacterAllowedJob> keyValuePair2 in this.enteredCharacters)
			{
				if (keyValuePair2.Key != null)
				{
					this.RemoveEnteredCharacter(keyValuePair2.Key, false);
				}
			}
		}
		if (this.masterCallbacksRef > 0)
		{
			HawkMasterTransformSync.onServerPreSync -= this.OnServerPreSync;
		}
	}

	// Token: 0x0600016E RID: 366 RVA: 0x00008054 File Offset: 0x00006254
	private void OnServerPreSync()
	{
		foreach (RagdollAnimTarget ragdollAnimTarget in this.currentTubeAnimTargets)
		{
			if (ragdollAnimTarget)
			{
				ragdollAnimTarget.ForceApply();
			}
		}
	}

	// Token: 0x0600016F RID: 367 RVA: 0x000080B0 File Offset: 0x000062B0
	private void BakeTubeCameras()
	{
		if (this.tubeCamerasDic == null)
		{
			this.tubeCamerasDic = new Dictionary<Transform, CameraFocus>();
			foreach (SpaceCameraTube spaceCameraTube in this.cameraTubes)
			{
				this.tubeCamerasDic.Add(spaceCameraTube.point, spaceCameraTube.cameraFocus);
			}
		}
	}

	// Token: 0x06000170 RID: 368 RVA: 0x00008100 File Offset: 0x00006300
	private void OnEnterTriggerEnter(PlayerCharacter playerCharacter)
	{
		if (playerCharacter)
		{
			if (playerCharacter.GetComponent<SpaceTubeLock>())
			{
				return;
			}
			if (this.enteredCharacters == null)
			{
				this.enteredCharacters = new Dictionary<PlayerCharacter, IPlayerCharacterAllowedJob>();
			}
			if (!this.enteredCharacters.ContainsKey(playerCharacter))
			{
				this.enteredCharacters.Add(playerCharacter, playerCharacter.gameObject.AddComponent<PlayerCharacterStopAllowedJob>());
				base.StartCoroutine(this.TubeUpdate(playerCharacter));
			}
		}
	}

	// Token: 0x06000171 RID: 369 RVA: 0x0000816C File Offset: 0x0000636C
	private void RemoveEnteredCharacter(PlayerCharacter playerCharacter, bool bRemove = true)
	{
		IPlayerCharacterAllowedJob playerCharacterAllowedJob;
		if (this.enteredCharacters.TryGetValue(playerCharacter, out playerCharacterAllowedJob))
		{
			if (playerCharacterAllowedJob != null && playerCharacterAllowedJob.GetComponent())
			{
				Object.Destroy(playerCharacterAllowedJob.GetComponent());
			}
			if (bRemove)
			{
				this.enteredCharacters.Remove(playerCharacter);
			}
		}
	}

	// Token: 0x06000172 RID: 370 RVA: 0x000081B4 File Offset: 0x000063B4
	private IEnumerator TubeUpdate(PlayerCharacter playerCharacter)
	{
		yield return new WaitForSeconds(0.2f);
		if (!this.enterTrigger.ContainsPlayer(playerCharacter) || !playerCharacter)
		{
			this.RemoveEnteredCharacter(playerCharacter, true);
			yield break;
		}
		if (playerCharacter)
		{
			PlayerController playerController = playerCharacter.GetPlayerController();
			if (playerController)
			{
				PlayerControllerInteractor playerControllerInteractor = playerController.GetPlayerControllerInteractor();
				if (playerControllerInteractor && playerControllerInteractor.HasEnteredAction())
				{
					this.RemoveEnteredCharacter(playerCharacter, true);
					yield break;
				}
			}
		}
		if (this.masterCallbacksRef == 0)
		{
			HawkMasterTransformSync.onServerPreSync += this.OnServerPreSync;
		}
		this.masterCallbacksRef++;
		uint characterNetworkid = playerCharacter.networkObject.GetNetworkID();
		this.networkObject.SendRPC(this.RPC_CLIENT_PLAYSTOP_TUBE_SOUND, 0, new object[] { true, characterNetworkid });
		SpaceTubeLock tubeLock = UnityExtensions.GetOrAddComponent<SpaceTubeLock>(playerCharacter.gameObject);
		this.BakeTubeCameras();
		int currentPointIndex = 0;
		WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
		PlayerBody playerBody = playerCharacter.GetPlayerBody();
		Vector3 previousPosition = playerBody.transform.position;
		Quaternion previousRotation = playerBody.transform.rotation;
		GameObject gameObject = new GameObject(base.gameObject.name + "Fixed Player Joint");
		gameObject.transform.SetPositionAndRotation(previousPosition, previousRotation);
		gameObject.transform.parent = base.transform;
		Rigidbody jointRigidbody = gameObject.gameObject.AddComponent<Rigidbody>();
		jointRigidbody.interpolation = 1;
		jointRigidbody.isKinematic = true;
		ConfigurableJoint joint = gameObject.gameObject.AddComponent<ConfigurableJoint>();
		joint.angularXMotion = 0;
		joint.angularYMotion = 0;
		joint.angularZMotion = 0;
		joint.xMotion = (joint.yMotion = (joint.zMotion = 0));
		joint.connectedBody = playerCharacter.GetHipRigidbody();
		RagdollAnimTarget[] ragdollAnimTargets = playerCharacter.GetComponentsInChildren<RagdollAnimTarget>();
		RagdollPart[] ragdollParts = playerCharacter.GetComponentsInChildren<RagdollPart>();
		if (this.currentTubeAnimTargets == null)
		{
			this.currentTubeAnimTargets = new List<RagdollAnimTarget>();
		}
		this.currentTubeAnimTargets.AddRange(ragdollAnimTargets);
		PlayerController playerController2 = playerCharacter.GetPlayerController();
		if (playerController2)
		{
			PlayerControllerInputManager playerControllerInputManager = playerController2.GetPlayerControllerInputManager();
			if (playerControllerInputManager)
			{
				playerControllerInputManager.DisableGameplayInput(this, true);
			}
		}
		PlayerCharacterAnimController characterAnimController = playerCharacter.GetPlayerCharacterAnimController();
		if (characterAnimController)
		{
			characterAnimController.PlayEmote(0, this.overrideController, false);
		}
		if (playerCharacter)
		{
			RagdollController ragdollController = playerCharacter.GetRagdollController();
			if (ragdollController)
			{
				ragdollController.Ragdoll();
				ragdollController.LockRagdollState();
			}
		}
		float currentSpeed = 0f;
		while (currentPointIndex < this.points.Length)
		{
			Transform point = this.points[currentPointIndex];
			if (point)
			{
				float time = Time.fixedTime;
				Vector3 newPos = point.position;
				Quaternion newRot = point.rotation;
				float num = 0f;
				while (num <= 1f)
				{
					yield return fixedUpdate;
					float num2 = Vector3.Distance(jointRigidbody.position, this.points[this.points.Length - 1].position);
					float num3 = this.tubeSpeedMax;
					if (num2 < this.tubeMaxSpeedDistance)
					{
						num3 = this.tubeSpeedMin;
					}
					CameraFocus cameraFocus;
					if (this.tubeCamerasDic.TryGetValue(point, out cameraFocus) && playerCharacter)
					{
						PlayerController playerController3 = playerCharacter.GetPlayerController();
						if (playerController3 && playerController3.GetPlayerCharacter() == playerCharacter)
						{
							playerController3.SetOwnerCameraFocus(cameraFocus, true);
						}
					}
					currentSpeed = Mathf.Lerp(currentSpeed, num3, Time.fixedDeltaTime);
					float num4 = (previousPosition - newPos).magnitude / currentSpeed;
					num = (Time.fixedTime - time) / num4;
					Vector3 vector = Vector3.Lerp(previousPosition, newPos, num);
					Quaternion quaternion = Quaternion.Slerp(previousRotation, newRot, num);
					jointRigidbody.MovePosition(vector);
					jointRigidbody.MoveRotation(quaternion);
					Rigidbody hipRigidbody = playerCharacter.GetHipRigidbody();
					if (hipRigidbody)
					{
						hipRigidbody.transform.localPosition = (hipRigidbody.position = vector);
						hipRigidbody.transform.localRotation = (hipRigidbody.rotation = quaternion);
					}
					foreach (RagdollAnimTarget ragdollAnimTarget in ragdollAnimTargets)
					{
						if (ragdollAnimTarget)
						{
							ragdollAnimTarget.ForceApply();
						}
					}
					foreach (RagdollPart ragdollPart in ragdollParts)
					{
						if (ragdollPart)
						{
							ragdollPart.ResetPose_PosOnly();
						}
					}
				}
				previousPosition = newPos;
				previousRotation = newRot;
				newPos = default(Vector3);
				newRot = default(Quaternion);
			}
			int i = currentPointIndex;
			currentPointIndex = i + 1;
			point = null;
		}
		if (characterAnimController)
		{
			characterAnimController.StopEmote();
		}
		if (joint)
		{
			Object.Destroy(joint.gameObject);
		}
		this.networkObject.SendRPC(this.RPC_CLIENT_PLAYSTOP_TUBE_SOUND, 0, new object[] { false, characterNetworkid });
		if (playerCharacter)
		{
			RagdollController ragdollController2 = playerCharacter.GetRagdollController();
			if (ragdollController2)
			{
				ragdollController2.UnlockRagdollState();
				if (this.bRagdollOnExit)
				{
					ragdollController2.Ragdoll();
				}
				else
				{
					ragdollController2.Wakeup(true);
				}
			}
			PlayerCharacterMovement playerCharacterMovement = playerCharacter.GetPlayerCharacterMovement();
			if (playerCharacterMovement)
			{
				playerCharacterMovement.SetRotation(playerCharacterMovement.GetHipRigidbody().rotation.eulerAngles.y, 0f);
				playerCharacterMovement.SetPlayerRotation(playerCharacterMovement.GetHipRigidbody().rotation);
			}
		}
		this.currentTubeAnimTargets.RemoveAll((RagdollAnimTarget x) => Array.IndexOf<RagdollAnimTarget>(ragdollAnimTargets, x) != -1);
		float waitTime = Time.time;
		while (Time.time - waitTime < 1f)
		{
			yield return fixedUpdate;
			if (this.bClearVelocityOnExit && playerCharacter)
			{
				PlayerCharacterMovement playerCharacterMovement2 = playerCharacter.GetPlayerCharacterMovement();
				if (playerCharacterMovement2)
				{
					playerCharacterMovement2.GetHipRigidbody().velocity = Vector3.zero;
				}
			}
		}
		if (playerCharacter)
		{
			playerController2 = playerCharacter.GetPlayerController();
			if (playerController2)
			{
				if (playerController2.GetPlayerCharacter() == playerCharacter)
				{
					playerController2.SetOwnerCameraFocusPlayer();
				}
				PlayerControllerInputManager playerControllerInputManager2 = playerController2.GetPlayerControllerInputManager();
				if (playerControllerInputManager2)
				{
					playerControllerInputManager2.EnableGameplayInput(this);
				}
			}
		}
		if (tubeLock)
		{
			Object.Destroy(tubeLock);
		}
		this.masterCallbacksRef--;
		if (this.masterCallbacksRef == 0)
		{
			HawkMasterTransformSync.onServerPreSync -= this.OnServerPreSync;
		}
		this.RemoveEnteredCharacter(playerCharacter, true);
		yield break;
	}

	// Token: 0x06000173 RID: 371 RVA: 0x000081CC File Offset: 0x000063CC
	private void ClientPlayStopTubeSound(HawkNetReader reader, HawkRPCInfo info)
	{
		bool flag = reader.ReadBoolean();
		uint num = reader.ReadUInt32();
		if (this.characterTubeSounds == null)
		{
			this.characterTubeSounds = new Dictionary<uint, EventInstance>();
		}
		EventInstance eventInstance2;
		if (flag)
		{
			if (!this.characterTubeSounds.ContainsKey(num) && !string.IsNullOrEmpty(this.tubeCharacterSound))
			{
				PlayerCharacter playerCharacter = HawkNetworkManager.DefaultInstance.FindNetworkBehaviour<PlayerCharacter>(num);
				if (playerCharacter)
				{
					EventInstance eventInstance = RuntimeManager.CreateInstance(this.tubeCharacterSound);
					RuntimeManager.AttachInstanceToGameObject(eventInstance, playerCharacter.GetPlayerBody().transform, null);
					eventInstance.start();
					this.characterTubeSounds.Add(num, eventInstance);
					return;
				}
			}
		}
		else if (this.characterTubeSounds.TryGetValue(num, out eventInstance2))
		{
			if (this.bTriggerCue)
			{
				eventInstance2.triggerCue();
			}
			else
			{
				eventInstance2.stop(0);
			}
			eventInstance2.release();
			this.characterTubeSounds.Remove(num);
		}
	}

	// Token: 0x06000174 RID: 372 RVA: 0x000082A4 File Offset: 0x000064A4
	public SpaceTube()
	{
	}

	// Token: 0x0400013E RID: 318
	private byte RPC_CLIENT_PLAYSTOP_TUBE_SOUND;

	// Token: 0x0400013F RID: 319
	[SerializeField]
	private WobblyPlayerCharacterTrigger enterTrigger;

	// Token: 0x04000140 RID: 320
	[SerializeField]
	private Transform[] points;

	// Token: 0x04000141 RID: 321
	[SerializeField]
	private RuntimeAnimatorController overrideController;

	// Token: 0x04000142 RID: 322
	[SerializeField]
	private SpaceCameraTube[] cameraTubes;

	// Token: 0x04000143 RID: 323
	[SerializeField]
	[EventRef]
	private string tubeCharacterSound = "event:/Objects_Space/Objects_Space_TravelTube";

	// Token: 0x04000144 RID: 324
	[SerializeField]
	private bool bRagdollOnExit = true;

	// Token: 0x04000145 RID: 325
	[SerializeField]
	private bool bTriggerCue = true;

	// Token: 0x04000146 RID: 326
	[SerializeField]
	private bool bClearVelocityOnExit;

	// Token: 0x04000147 RID: 327
	[SerializeField]
	private float tubeSpeedMax = 50f;

	// Token: 0x04000148 RID: 328
	[SerializeField]
	private float tubeSpeedMin = 10f;

	// Token: 0x04000149 RID: 329
	[SerializeField]
	private float tubeMaxSpeedDistance = 30f;

	// Token: 0x0400014A RID: 330
	private Dictionary<PlayerCharacter, IPlayerCharacterAllowedJob> enteredCharacters;

	// Token: 0x0400014B RID: 331
	private Dictionary<Transform, CameraFocus> tubeCamerasDic;

	// Token: 0x0400014C RID: 332
	private Dictionary<uint, EventInstance> characterTubeSounds;

	// Token: 0x0400014D RID: 333
	private int masterCallbacksRef;

	// Token: 0x0400014E RID: 334
	private List<RagdollAnimTarget> currentTubeAnimTargets;

	// Token: 0x02000073 RID: 115
	[CompilerGenerated]
	private sealed class <>c__DisplayClass24_0
	{
		// Token: 0x0600031A RID: 794 RVA: 0x0000F906 File Offset: 0x0000DB06
		public <>c__DisplayClass24_0()
		{
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0000F90E File Offset: 0x0000DB0E
		internal bool <TubeUpdate>b__0(RagdollAnimTarget x)
		{
			return Array.IndexOf<RagdollAnimTarget>(this.ragdollAnimTargets, x) != -1;
		}

		// Token: 0x04000277 RID: 631
		public RagdollAnimTarget[] ragdollAnimTargets;
	}

	// Token: 0x02000074 RID: 116
	[CompilerGenerated]
	private sealed class <TubeUpdate>d__24 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x0600031C RID: 796 RVA: 0x0000F922 File Offset: 0x0000DB22
		[DebuggerHidden]
		public <TubeUpdate>d__24(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x0600031D RID: 797 RVA: 0x0000F931 File Offset: 0x0000DB31
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x0600031E RID: 798 RVA: 0x0000F934 File Offset: 0x0000DB34
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpaceTube spaceTube = this;
			float num5;
			switch (num)
			{
			case 0:
				this.<>1__state = -1;
				CS$<>8__locals1 = new SpaceTube.<>c__DisplayClass24_0();
				this.<>2__current = new WaitForSeconds(0.2f);
				this.<>1__state = 1;
				return true;
			case 1:
			{
				this.<>1__state = -1;
				if (!spaceTube.enterTrigger.ContainsPlayer(playerCharacter) || !playerCharacter)
				{
					spaceTube.RemoveEnteredCharacter(playerCharacter, true);
					return false;
				}
				if (playerCharacter)
				{
					PlayerController playerController = playerCharacter.GetPlayerController();
					if (playerController)
					{
						PlayerControllerInteractor playerControllerInteractor = playerController.GetPlayerControllerInteractor();
						if (playerControllerInteractor && playerControllerInteractor.HasEnteredAction())
						{
							spaceTube.RemoveEnteredCharacter(playerCharacter, true);
							return false;
						}
					}
				}
				if (spaceTube.masterCallbacksRef == 0)
				{
					HawkMasterTransformSync.onServerPreSync += spaceTube.OnServerPreSync;
				}
				spaceTube.masterCallbacksRef++;
				characterNetworkid = playerCharacter.networkObject.GetNetworkID();
				spaceTube.networkObject.SendRPC(spaceTube.RPC_CLIENT_PLAYSTOP_TUBE_SOUND, 0, new object[] { true, characterNetworkid });
				tubeLock = UnityExtensions.GetOrAddComponent<SpaceTubeLock>(playerCharacter.gameObject);
				spaceTube.BakeTubeCameras();
				currentPointIndex = 0;
				fixedUpdate = new WaitForFixedUpdate();
				PlayerBody playerBody = playerCharacter.GetPlayerBody();
				previousPosition = playerBody.transform.position;
				previousRotation = playerBody.transform.rotation;
				GameObject gameObject = new GameObject(spaceTube.gameObject.name + "Fixed Player Joint");
				gameObject.transform.SetPositionAndRotation(previousPosition, previousRotation);
				gameObject.transform.parent = spaceTube.transform;
				jointRigidbody = gameObject.gameObject.AddComponent<Rigidbody>();
				jointRigidbody.interpolation = 1;
				jointRigidbody.isKinematic = true;
				joint = gameObject.gameObject.AddComponent<ConfigurableJoint>();
				joint.angularXMotion = 0;
				joint.angularYMotion = 0;
				joint.angularZMotion = 0;
				joint.xMotion = (joint.yMotion = (joint.zMotion = 0));
				joint.connectedBody = playerCharacter.GetHipRigidbody();
				CS$<>8__locals1.ragdollAnimTargets = playerCharacter.GetComponentsInChildren<RagdollAnimTarget>();
				ragdollParts = playerCharacter.GetComponentsInChildren<RagdollPart>();
				if (spaceTube.currentTubeAnimTargets == null)
				{
					spaceTube.currentTubeAnimTargets = new List<RagdollAnimTarget>();
				}
				spaceTube.currentTubeAnimTargets.AddRange(CS$<>8__locals1.ragdollAnimTargets);
				PlayerController playerController2 = playerCharacter.GetPlayerController();
				if (playerController2)
				{
					PlayerControllerInputManager playerControllerInputManager = playerController2.GetPlayerControllerInputManager();
					if (playerControllerInputManager)
					{
						playerControllerInputManager.DisableGameplayInput(spaceTube, true);
					}
				}
				characterAnimController = playerCharacter.GetPlayerCharacterAnimController();
				if (characterAnimController)
				{
					characterAnimController.PlayEmote(0, spaceTube.overrideController, false);
				}
				if (playerCharacter)
				{
					RagdollController ragdollController = playerCharacter.GetRagdollController();
					if (ragdollController)
					{
						ragdollController.Ragdoll();
						ragdollController.LockRagdollState();
					}
				}
				currentSpeed = 0f;
				goto IL_0616;
			}
			case 2:
			{
				this.<>1__state = -1;
				float num2 = Vector3.Distance(jointRigidbody.position, spaceTube.points[spaceTube.points.Length - 1].position);
				float num3 = spaceTube.tubeSpeedMax;
				if (num2 < spaceTube.tubeMaxSpeedDistance)
				{
					num3 = spaceTube.tubeSpeedMin;
				}
				CameraFocus cameraFocus;
				if (spaceTube.tubeCamerasDic.TryGetValue(point, out cameraFocus) && playerCharacter)
				{
					PlayerController playerController3 = playerCharacter.GetPlayerController();
					if (playerController3 && playerController3.GetPlayerCharacter() == playerCharacter)
					{
						playerController3.SetOwnerCameraFocus(cameraFocus, true);
					}
				}
				currentSpeed = Mathf.Lerp(currentSpeed, num3, Time.fixedDeltaTime);
				float num4 = (previousPosition - newPos).magnitude / currentSpeed;
				num5 = (Time.fixedTime - time) / num4;
				Vector3 vector = Vector3.Lerp(previousPosition, newPos, num5);
				Quaternion quaternion = Quaternion.Slerp(previousRotation, newRot, num5);
				jointRigidbody.MovePosition(vector);
				jointRigidbody.MoveRotation(quaternion);
				Rigidbody hipRigidbody = playerCharacter.GetHipRigidbody();
				if (hipRigidbody)
				{
					hipRigidbody.transform.localPosition = (hipRigidbody.position = vector);
					hipRigidbody.transform.localRotation = (hipRigidbody.rotation = quaternion);
				}
				foreach (RagdollAnimTarget ragdollAnimTarget in CS$<>8__locals1.ragdollAnimTargets)
				{
					if (ragdollAnimTarget)
					{
						ragdollAnimTarget.ForceApply();
					}
				}
				foreach (RagdollPart ragdollPart in ragdollParts)
				{
					if (ragdollPart)
					{
						ragdollPart.ResetPose_PosOnly();
					}
				}
				break;
			}
			case 3:
			{
				this.<>1__state = -1;
				if (!spaceTube.bClearVelocityOnExit || !playerCharacter)
				{
					goto IL_07A4;
				}
				PlayerCharacterMovement playerCharacterMovement = playerCharacter.GetPlayerCharacterMovement();
				if (playerCharacterMovement)
				{
					playerCharacterMovement.GetHipRigidbody().velocity = Vector3.zero;
					goto IL_07A4;
				}
				goto IL_07A4;
			}
			default:
				return false;
			}
			IL_05C1:
			if (num5 <= 1f)
			{
				this.<>2__current = fixedUpdate;
				this.<>1__state = 2;
				return true;
			}
			previousPosition = newPos;
			previousRotation = newRot;
			newPos = default(Vector3);
			newRot = default(Quaternion);
			IL_05FD:
			int i = currentPointIndex;
			currentPointIndex = i + 1;
			point = null;
			IL_0616:
			if (currentPointIndex >= spaceTube.points.Length)
			{
				if (characterAnimController)
				{
					characterAnimController.StopEmote();
				}
				if (joint)
				{
					Object.Destroy(joint.gameObject);
				}
				spaceTube.networkObject.SendRPC(spaceTube.RPC_CLIENT_PLAYSTOP_TUBE_SOUND, 0, new object[] { false, characterNetworkid });
				if (playerCharacter)
				{
					RagdollController ragdollController2 = playerCharacter.GetRagdollController();
					if (ragdollController2)
					{
						ragdollController2.UnlockRagdollState();
						if (spaceTube.bRagdollOnExit)
						{
							ragdollController2.Ragdoll();
						}
						else
						{
							ragdollController2.Wakeup(true);
						}
					}
					PlayerCharacterMovement playerCharacterMovement2 = playerCharacter.GetPlayerCharacterMovement();
					if (playerCharacterMovement2)
					{
						playerCharacterMovement2.SetRotation(playerCharacterMovement2.GetHipRigidbody().rotation.eulerAngles.y, 0f);
						playerCharacterMovement2.SetPlayerRotation(playerCharacterMovement2.GetHipRigidbody().rotation);
					}
				}
				spaceTube.currentTubeAnimTargets.RemoveAll((RagdollAnimTarget x) => Array.IndexOf<RagdollAnimTarget>(CS$<>8__locals1.ragdollAnimTargets, x) != -1);
				waitTime = Time.time;
			}
			else
			{
				point = spaceTube.points[currentPointIndex];
				if (point)
				{
					time = Time.fixedTime;
					newPos = point.position;
					newRot = point.rotation;
					num5 = 0f;
					goto IL_05C1;
				}
				goto IL_05FD;
			}
			IL_07A4:
			if (Time.time - waitTime >= 1f)
			{
				if (playerCharacter)
				{
					PlayerController playerController2 = playerCharacter.GetPlayerController();
					if (playerController2)
					{
						if (playerController2.GetPlayerCharacter() == playerCharacter)
						{
							playerController2.SetOwnerCameraFocusPlayer();
						}
						PlayerControllerInputManager playerControllerInputManager2 = playerController2.GetPlayerControllerInputManager();
						if (playerControllerInputManager2)
						{
							playerControllerInputManager2.EnableGameplayInput(spaceTube);
						}
					}
				}
				if (tubeLock)
				{
					Object.Destroy(tubeLock);
				}
				spaceTube.masterCallbacksRef--;
				if (spaceTube.masterCallbacksRef == 0)
				{
					HawkMasterTransformSync.onServerPreSync -= spaceTube.OnServerPreSync;
				}
				spaceTube.RemoveEnteredCharacter(playerCharacter, true);
				return false;
			}
			this.<>2__current = fixedUpdate;
			this.<>1__state = 3;
			return true;
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600031F RID: 799 RVA: 0x0001019D File Offset: 0x0000E39D
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x06000320 RID: 800 RVA: 0x000101A5 File Offset: 0x0000E3A5
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000321 RID: 801 RVA: 0x000101AC File Offset: 0x0000E3AC
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000278 RID: 632
		private int <>1__state;

		// Token: 0x04000279 RID: 633
		private object <>2__current;

		// Token: 0x0400027A RID: 634
		public SpaceTube <>4__this;

		// Token: 0x0400027B RID: 635
		public PlayerCharacter playerCharacter;

		// Token: 0x0400027C RID: 636
		private SpaceTube.<>c__DisplayClass24_0 <>8__1;

		// Token: 0x0400027D RID: 637
		private uint <characterNetworkid>5__2;

		// Token: 0x0400027E RID: 638
		private SpaceTubeLock <tubeLock>5__3;

		// Token: 0x0400027F RID: 639
		private int <currentPointIndex>5__4;

		// Token: 0x04000280 RID: 640
		private WaitForFixedUpdate <fixedUpdate>5__5;

		// Token: 0x04000281 RID: 641
		private Vector3 <previousPosition>5__6;

		// Token: 0x04000282 RID: 642
		private Quaternion <previousRotation>5__7;

		// Token: 0x04000283 RID: 643
		private Rigidbody <jointRigidbody>5__8;

		// Token: 0x04000284 RID: 644
		private ConfigurableJoint <joint>5__9;

		// Token: 0x04000285 RID: 645
		private RagdollPart[] <ragdollParts>5__10;

		// Token: 0x04000286 RID: 646
		private PlayerCharacterAnimController <characterAnimController>5__11;

		// Token: 0x04000287 RID: 647
		private float <currentSpeed>5__12;

		// Token: 0x04000288 RID: 648
		private float <waitTime>5__13;

		// Token: 0x04000289 RID: 649
		private Transform <point>5__14;

		// Token: 0x0400028A RID: 650
		private float <time>5__15;

		// Token: 0x0400028B RID: 651
		private Vector3 <newPos>5__16;

		// Token: 0x0400028C RID: 652
		private Quaternion <newRot>5__17;
	}
}
