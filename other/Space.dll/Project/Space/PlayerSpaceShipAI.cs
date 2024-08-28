using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class PlayerSpaceShipAI : MonoBehaviour
{
	// Token: 0x060001F3 RID: 499 RVA: 0x0000A004 File Offset: 0x00008204
	private void Awake()
	{
		this.detectionCheckMask = LayerMask.GetMask(new string[] { "Vehicle", "World", "WorldLarge" });
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.playerVehicle = base.GetComponent<PlayerSpaceShip>();
		PlayerSpaceShip playerSpaceShip = this.playerVehicle;
		playerSpaceShip.onVehicleDestroyed = (Action<PlayerVehicle>)Delegate.Combine(playerSpaceShip.onVehicleDestroyed, new Action<PlayerVehicle>(this.OnVehicleDestroyed));
		this.playerVehicleMovement = base.GetComponent<PlayerSpaceShipMovement>();
		this.actionEnter = base.GetComponentInChildren<ActionEnterExitInteract>();
		this.actionEnter.onDriverChanged += new ActionEnterExitInteract.ActionEnterExitDriverChanged(this.OnDriverChanged);
		this.actionEnter.onPlayerEntered += this.OnPlayerEntered;
		this.actionEnter.onPlayerExited += this.OnPlayerExited;
		this.worldDynamicObject = base.GetComponent<WorldDynamicObject>();
		WorldDynamicObject worldDynamicObject = this.worldDynamicObject;
		worldDynamicObject.onPreDestory = (WorldDynamicObject.OnPreDestory)Delegate.Combine(worldDynamicObject.onPreDestory, new WorldDynamicObject.OnPreDestory(this.OnPreDestory));
		this.worldDynamicObject.SetMoveSceneOnChunkChanged(false);
		this.networkObject = this.playerVehicle.networkObject;
		if (this.targetNodeTest)
		{
			this.SetTargetNode(this.targetNodeTest);
			base.StartCoroutine(this.<Awake>g__TestUpdate|18_0());
		}
		UnitySingleton<SpaceTrafficManager>.Instance.Assign(this);
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0000A157 File Offset: 0x00008357
	private void OnVehicleDestroyed(PlayerVehicle playerVehicle)
	{
		this.ReplaceAllNpcsWithSpawnedDynamicNPCs();
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000A160 File Offset: 0x00008360
	public void ReplaceAllNpcsWithSpawnedDynamicNPCs()
	{
		if (this.npcs != null && this.npcs.Count > 0)
		{
			foreach (PlayerNPCController playerNPCController in this.npcs)
			{
				PlayerNPCVehicle.ReplaceWithSpawnedDynamicNPC(playerNPCController);
			}
			this.npcs.Clear();
		}
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000A1D4 File Offset: 0x000083D4
	private void OnDestroy()
	{
		if (UnitySingleton<SpaceTrafficManager>.InstanceExists)
		{
			UnitySingleton<SpaceTrafficManager>.GetRawInstance().Unassign(this);
		}
		if (this.actionEnter)
		{
			this.actionEnter.onDriverChanged -= new ActionEnterExitInteract.ActionEnterExitDriverChanged(this.OnDriverChanged);
			this.actionEnter.onPlayerEntered -= this.OnPlayerEntered;
			this.actionEnter.onPlayerExited -= this.OnPlayerExited;
		}
		if (this.networkObject != null && this.networkObject.IsServer())
		{
			foreach (PlayerNPCController playerNPCController in this.npcs)
			{
				playerNPCController.networkObject.Destroy();
			}
		}
		if (this.worldDynamicObject)
		{
			WorldDynamicObject worldDynamicObject = this.worldDynamicObject;
			worldDynamicObject.onPreDestory = (WorldDynamicObject.OnPreDestory)Delegate.Remove(worldDynamicObject.onPreDestory, new WorldDynamicObject.OnPreDestory(this.OnPreDestory));
		}
		if (this.playerVehicle)
		{
			this.playerVehicle.RemoveNotAllowedOptimizationHandle(this);
		}
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000A2F0 File Offset: 0x000084F0
	private void Start()
	{
		if (this.networkObject != null)
		{
			this.OnNetworkReady();
			return;
		}
		this.playerVehicle.networkPost.AddCallback(delegate(HawkNetworkObject x)
		{
			this.networkObject = x;
			this.OnNetworkReady();
			base.enabled = false;
			base.enabled = true;
		});
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0000A31D File Offset: 0x0000851D
	private void OnNetworkReady()
	{
		if (this.networkObject.IsServer())
		{
			this.SpawnNPCsInVehicle();
			this.playerVehicle.AddNotAllowedOptimizationHandle(this);
			this.playerVehicleMovement.ServerSetGearsDown(false);
		}
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x0000A34A File Offset: 0x0000854A
	private void OnValidate()
	{
		if (this.targetNodeTest)
		{
			this.SetTargetNode(this.targetNodeTest);
		}
	}

	// Token: 0x060001FA RID: 506 RVA: 0x0000A365 File Offset: 0x00008565
	private bool OnPreDestory(HawkNetworkBehaviour networkBehaviour)
	{
		return true;
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000A368 File Offset: 0x00008568
	private void OnEnable()
	{
		if (this.playerVehicleMovement)
		{
			this.playerVehicleMovement.ServerSetEngineOn(true, true);
			PlayerSpaceShipMovement playerSpaceShipMovement = this.playerVehicleMovement;
			playerSpaceShipMovement.onCollisionEnter = (PlayerVehicleMovement.OnVehicleCollisionEnter)Delegate.Combine(playerSpaceShipMovement.onCollisionEnter, new PlayerVehicleMovement.OnVehicleCollisionEnter(this.OnVehicleCollisionEnter));
		}
		if (this.playerVehicle)
		{
			this.playerVehicle.SetVehicleEnabled(true, true);
		}
		foreach (PlayerNPCController playerNPCController in this.npcs)
		{
			if (playerNPCController)
			{
				VanishComponent.SetVisible(playerNPCController.gameObject, true);
			}
		}
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000A424 File Offset: 0x00008624
	private void OnDisable()
	{
		if (this.playerVehicleMovement)
		{
			PlayerSpaceShipMovement playerSpaceShipMovement = this.playerVehicleMovement;
			playerSpaceShipMovement.onCollisionEnter = (PlayerVehicleMovement.OnVehicleCollisionEnter)Delegate.Remove(playerSpaceShipMovement.onCollisionEnter, new PlayerVehicleMovement.OnVehicleCollisionEnter(this.OnVehicleCollisionEnter));
		}
		foreach (PlayerNPCController playerNPCController in this.npcs)
		{
			if (playerNPCController)
			{
				VanishComponent.SetVisible(playerNPCController.gameObject, false);
			}
		}
		if (this.targetNode)
		{
			this.targetNode.VehicleLeftNode(this);
		}
	}

	// Token: 0x060001FD RID: 509 RVA: 0x0000A4D4 File Offset: 0x000086D4
	private void OnPersistentContentLoaded()
	{
		PersistentContentManager instance = UnitySingleton<PersistentContentManager>.Instance;
		instance.onLoaded = (Action)Delegate.Remove(instance.onLoaded, new Action(this.OnPersistentContentLoaded));
		this.SpawnNPCsInVehicle();
	}

	// Token: 0x060001FE RID: 510 RVA: 0x0000A504 File Offset: 0x00008704
	private void SpawnNPCsInVehicle()
	{
		if (this.networkObject == null || !this.networkObject.IsServer() || !this)
		{
			return;
		}
		if (this.npcs.Count > 0)
		{
			return;
		}
		PlayerNPCVehicle defaultPlayerNPCVehiclePrefab = UnitySingleton<PersistentContentManager>.Instance.GetDefaultPlayerNPCVehiclePrefab();
		if (!defaultPlayerNPCVehiclePrefab)
		{
			PersistentContentManager instance = UnitySingleton<PersistentContentManager>.Instance;
			instance.onLoaded = (Action)Delegate.Combine(instance.onLoaded, new Action(this.OnPersistentContentLoaded));
			return;
		}
		ActionEnterExitInteract actionEnterExitInteract = base.GetComponent<ActionEnterExitInteract>();
		if (actionEnterExitInteract)
		{
			PlayerSeat[] seats = actionEnterExitInteract.GetSeats();
			if (seats != null && seats.Length != 0)
			{
				int num = Random.Range(1, seats.Length);
				for (int i = 0; i < num; i++)
				{
					if (seats[i])
					{
						int seatIndex = i;
						NetworkPrefab.SpawnNetworkPrefab(defaultPlayerNPCVehiclePrefab.gameObject, delegate(HawkNetworkBehaviour npcNetworkBehaviour)
						{
							if (npcNetworkBehaviour != null && this && this.gameObject)
							{
								PlayerNPCVehicle component = npcNetworkBehaviour.GetComponent<PlayerNPCVehicle>();
								if (component)
								{
									this.NPCSetup(component, true, actionEnterExitInteract, seatIndex);
									return;
								}
								npcNetworkBehaviour.networkObject.Destroy();
							}
						}, null, null, null, true, false, false, true);
					}
				}
			}
		}
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0000A624 File Offset: 0x00008824
	private void ServerUpdateNPCSeats()
	{
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		ActionEnterExitInteract component = base.GetComponent<ActionEnterExitInteract>();
		if (!component)
		{
			return;
		}
		int count = this.npcs.Count;
		int seatCount = component.GetSeatCount();
		int num = 0;
		if (seatCount != 0 && count != 0)
		{
			for (int i = 1; i < seatCount; i++)
			{
				PlayerSeat seat = component.GetSeat(i);
				if (seat != null && !seat.IsVehicleSeatOccupied())
				{
					if (this.npcs[num])
					{
						PlayerNPCVehicle component2 = this.npcs[num].GetComponent<PlayerNPCVehicle>();
						if (component2)
						{
							component2.AttachToVehicleSeat(this.playerVehicle, component, i, true);
						}
					}
					num++;
					if (num >= count)
					{
						return;
					}
				}
			}
		}
		for (int j = count - 1; j >= num; j--)
		{
			if (this.npcs[j])
			{
				PlayerNPCVehicle.ReplaceWithSpawnedDynamicNPC(this.npcs[j]);
				this.npcs.RemoveAt(j);
			}
		}
	}

	// Token: 0x06000200 RID: 512 RVA: 0x0000A730 File Offset: 0x00008930
	private void NPCSetup(PlayerNPCVehicle playerNPCVehicle, bool bRandomClothes, ActionEnterExitInteract actionEnterExitInteract, int seatIndex)
	{
		if (actionEnterExitInteract.GetSeatCount() > seatIndex)
		{
			PlayerNPCController playerNPCController = playerNPCVehicle.GetPlayerNPCController();
			if (playerNPCController)
			{
				playerNPCVehicle.AttachToVehicleSeat(this.playerVehicle, actionEnterExitInteract, seatIndex, true);
				if (bRandomClothes)
				{
					playerNPCController.gameObject.AddComponent<CharacterNPCRandomClothes>();
				}
				this.npcs.Add(playerNPCController);
				return;
			}
		}
		else
		{
			HawkNetworkBehaviour component = playerNPCVehicle.GetComponent<HawkNetworkBehaviour>();
			if (component)
			{
				component.networkObject.Destroy();
			}
		}
	}

	// Token: 0x06000201 RID: 513 RVA: 0x0000A79C File Offset: 0x0000899C
	internal void SimulateVehicle()
	{
		if (this.targetNode)
		{
			this.targetNode.VehicleNodeUpdate(this);
		}
		if (this.bCanTravelToNode)
		{
			if (this.targetNode)
			{
				this.SimulateVehicle(this.targetNode);
				return;
			}
		}
		else if (this.targetNode)
		{
			this.bCanTravelToNode = this.targetNode.CanTravelToNode();
			if (this.previousNode)
			{
				this.SimulateVehicle(this.previousNode);
				return;
			}
		}
		else
		{
			this.shipInput.acceleration = 0f;
			this.shipInput.bBoost = false;
		}
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0000A838 File Offset: 0x00008A38
	private void SimulateVehicle(SpaceTrafficNode targetNode)
	{
		Vector3 vector = targetNode.transform.position - base.transform.position;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		float num = 1f;
		if (this.nodeSettings.bUseNodeRotation)
		{
			Vector3 eulerAngles = targetNode.transform.eulerAngles;
			this.shipInput.cameraX = eulerAngles.x;
			this.shipInput.cameraY = eulerAngles.y;
			num = 0.2f;
			this.bOnlySimulateRotation = true;
		}
		else
		{
			this.bOnlySimulateRotation = false;
			Vector3 eulerAngles2 = Quaternion.LookRotation(normalized, Vector3.up).eulerAngles;
			this.shipInput.cameraX = eulerAngles2.x;
			this.shipInput.cameraY = eulerAngles2.y;
			this.shipInput.upDownMovement = 0f;
			if (this.nodeSettings.bOverrideAcceleration)
			{
				this.shipInput.acceleration = this.nodeSettings.overrideAcceleration;
			}
			else
			{
				this.shipInput.acceleration = 0.7f;
			}
			if (Vector3.Dot(base.transform.forward, normalized) < 0.8f)
			{
				this.shipInput.acceleration = 0.3f;
				this.shipInput.bBoost = false;
			}
			if (this.previousNode && Vector3.Dot((targetNode.transform.position - this.previousNode.transform.position).normalized, normalized) < -0.3f)
			{
				if (magnitude < this.playerVehicle.GetOptimizationRadius())
				{
					num = 2.5f;
				}
				else
				{
					this.shipInput.acceleration = 0.3f;
					this.shipInput.bBoost = false;
				}
			}
		}
		this.shipInput.bBoost = this.nodeSettings.bUseBoost;
		if (this.nodeSettings.bLandingGearDown || magnitude <= this.playerVehicle.GetOptimizationRadius() * num)
		{
			this.ReachedNode();
		}
		this.DetectCollisions(normalized);
	}

	// Token: 0x06000203 RID: 515 RVA: 0x0000AA36 File Offset: 0x00008C36
	private void ReachedNode()
	{
		if (this.bReachedTarget)
		{
			return;
		}
		this.bReachedTarget = true;
		this.targetNode.VehicleReachedNode(this);
	}

	// Token: 0x06000204 RID: 516 RVA: 0x0000AA54 File Offset: 0x00008C54
	private void FixedUpdate()
	{
		if (!this.bSimulateAI)
		{
			return;
		}
		if (this.bOnlySimulateRotation && this.targetNode && !this.nodeSettings.bLandingGearDown)
		{
			Vector3 vector = this.targetNode.transform.position - base.transform.position;
			this.rigidbody.AddForce(vector.normalized * 6f - this.rigidbody.velocity, 2);
		}
		this.playerVehicleMovement.ServerSimulateMovement(this.shipInput, !this.bOnlySimulateRotation, this.bOnlySimulateRotation);
	}

	// Token: 0x06000205 RID: 517 RVA: 0x0000AAFC File Offset: 0x00008CFC
	private void DetectCollisions(Vector3 toTargetNorm)
	{
		int num = Physics.SphereCastNonAlloc(base.transform.position, this.playerVehicle.GetOptimizationRadius() * 0.3f, toTargetNorm, this.results, this.playerVehicle.GetOptimizationRadius() * 1.5f, this.detectionCheckMask, 1);
		bool flag = false;
		for (int i = 0; i < num; i++)
		{
			RaycastHit raycastHit = this.results[i];
			if (!(raycastHit.collider.attachedRigidbody == this.rigidbody) && Vector3.Dot(base.transform.forward, (raycastHit.point - base.transform.position).normalized) > -0.3f)
			{
				flag = true;
			}
		}
		if (flag)
		{
			this.shipInput.bBoost = false;
			Vector3 vector = this.rigidbody.velocity;
			vector = this.rigidbody.transform.InverseTransformVector(vector);
			if (Mathf.Abs(vector.z) > 0.1f)
			{
				this.shipInput.acceleration = -Mathf.Sign(vector.z);
				return;
			}
			this.shipInput.acceleration = 0f;
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000AC23 File Offset: 0x00008E23
	private void OnPlayerEntered(PlayerController controller)
	{
		this.ServerUpdateNPCSeats();
		if (this.destroyCoroutine != null)
		{
			base.StopCoroutine(this.destroyCoroutine);
			this.destroyCoroutine = null;
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0000AC48 File Offset: 0x00008E48
	private void OnPlayerExited(PlayerController controller)
	{
		this.ServerUpdateNPCSeats();
		if (this.destroyCoroutine != null)
		{
			base.StopCoroutine(this.destroyCoroutine);
			this.destroyCoroutine = null;
		}
		if (this.actionEnter && this.actionEnter.GetControllersInVehicleCount() == 0)
		{
			this.destroyCoroutine = base.StartCoroutine(this.DestroyWhenThereIsNoPlayersInsideIt());
		}
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000ACA2 File Offset: 0x00008EA2
	private void OnDriverChanged(ActionEnterExitInteract actionInteract, PlayerController previousDriver, PlayerController currentDriver)
	{
		this.worldDynamicObject.SetMoveSceneOnChunkChanged(true);
		if (UnitySingleton<SpaceTrafficManager>.InstanceExists)
		{
			UnitySingleton<SpaceTrafficManager>.GetRawInstance().Unassign(this);
		}
		this.bSimulateAI = false;
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000ACC9 File Offset: 0x00008EC9
	private IEnumerator DestroyWhenThereIsNoPlayersInsideIt()
	{
		yield return new WaitForSeconds(20f);
		foreach (PlayerNPCController playerNPCController in this.npcs)
		{
			if (playerNPCController)
			{
				VanishComponent.VanishAndDestroy(playerNPCController.gameObject);
			}
		}
		Object.Destroy(this);
		this.npcs.Clear();
		yield break;
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0000ACD8 File Offset: 0x00008ED8
	private void OnVehicleCollisionEnter(Collision collision)
	{
		collision.gameObject.GetComponentInParent<PlayerVehicle>();
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000ACEC File Offset: 0x00008EEC
	internal void SetTargetNode(SpaceTrafficNode node)
	{
		this.bReachedTarget = false;
		this.bCanTravelToNode = false;
		if (this.targetNode)
		{
			this.targetNode.VehicleLeftNode(this);
		}
		this.previousNode = this.targetNode;
		this.targetNode = node;
		if (this.targetNode)
		{
			this.bCanTravelToNode = node.CanTravelToNode();
			this.targetNode.VehicleHeadingTowardsNode(this);
		}
	}

	// Token: 0x0600020C RID: 524 RVA: 0x0000AD58 File Offset: 0x00008F58
	public PlayerVehicle GetPlayerVehicle()
	{
		return this.playerVehicle;
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0000AD60 File Offset: 0x00008F60
	internal void ApplyNodeSettings(SpaceTrafficNodeSettings settings)
	{
		if (this.nodeSettings.bLandingGearDown != settings.bLandingGearDown)
		{
			this.playerVehicleMovement.ServerSetGearsDown(settings.bLandingGearDown);
		}
		this.nodeSettings = settings;
	}

	// Token: 0x0600020E RID: 526 RVA: 0x0000AD8D File Offset: 0x00008F8D
	internal bool IsGearsDown()
	{
		return this.nodeSettings.bLandingGearDown;
	}

	// Token: 0x0600020F RID: 527 RVA: 0x0000AD9A File Offset: 0x00008F9A
	private void OnDrawGizmos()
	{
		if (this.targetNode)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(this.playerVehicle.transform.position, this.targetNode.transform.position);
		}
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000ADD8 File Offset: 0x00008FD8
	public PlayerSpaceShipAI()
	{
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000ADFF File Offset: 0x00008FFF
	[CompilerGenerated]
	private IEnumerator <Awake>g__TestUpdate|18_0()
	{
		WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
		for (;;)
		{
			yield return fixedUpdate;
			this.SimulateVehicle();
		}
		yield break;
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000AE0E File Offset: 0x0000900E
	[CompilerGenerated]
	private void <Start>b__22_0(HawkNetworkObject x)
	{
		this.networkObject = x;
		this.OnNetworkReady();
		base.enabled = false;
		base.enabled = true;
	}

	// Token: 0x0400019B RID: 411
	[SerializeField]
	private SpaceTrafficNode targetNodeTest;

	// Token: 0x0400019C RID: 412
	private PlayerSpaceShip playerVehicle;

	// Token: 0x0400019D RID: 413
	private PlayerSpaceShipMovement playerVehicleMovement;

	// Token: 0x0400019E RID: 414
	private HawkNetworkObject networkObject;

	// Token: 0x0400019F RID: 415
	private WorldDynamicObject worldDynamicObject;

	// Token: 0x040001A0 RID: 416
	private ActionEnterExitInteract actionEnter;

	// Token: 0x040001A1 RID: 417
	private List<PlayerNPCController> npcs = new List<PlayerNPCController>();

	// Token: 0x040001A2 RID: 418
	private Coroutine destroyCoroutine;

	// Token: 0x040001A3 RID: 419
	private SpaceTrafficNode previousNode;

	// Token: 0x040001A4 RID: 420
	private SpaceTrafficNode targetNode;

	// Token: 0x040001A5 RID: 421
	private SpaceShipInput shipInput;

	// Token: 0x040001A6 RID: 422
	private Rigidbody rigidbody;

	// Token: 0x040001A7 RID: 423
	private int detectionCheckMask;

	// Token: 0x040001A8 RID: 424
	private bool bOnlySimulateRotation;

	// Token: 0x040001A9 RID: 425
	private bool bReachedTarget;

	// Token: 0x040001AA RID: 426
	private bool bCanTravelToNode;

	// Token: 0x040001AB RID: 427
	private SpaceTrafficNodeSettings nodeSettings;

	// Token: 0x040001AC RID: 428
	private bool bSimulateAI = true;

	// Token: 0x040001AD RID: 429
	private RaycastHit[] results = new RaycastHit[100];

	// Token: 0x0200007B RID: 123
	[CompilerGenerated]
	private sealed class <>c__DisplayClass29_0
	{
		// Token: 0x06000338 RID: 824 RVA: 0x000104F9 File Offset: 0x0000E6F9
		public <>c__DisplayClass29_0()
		{
		}

		// Token: 0x0400029C RID: 668
		public PlayerSpaceShipAI <>4__this;

		// Token: 0x0400029D RID: 669
		public ActionEnterExitInteract actionEnterExitInteract;
	}

	// Token: 0x0200007C RID: 124
	[CompilerGenerated]
	private sealed class <>c__DisplayClass29_1
	{
		// Token: 0x06000339 RID: 825 RVA: 0x00010501 File Offset: 0x0000E701
		public <>c__DisplayClass29_1()
		{
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0001050C File Offset: 0x0000E70C
		internal void <SpawnNPCsInVehicle>b__0(HawkNetworkBehaviour npcNetworkBehaviour)
		{
			if (npcNetworkBehaviour != null && this.CS$<>8__locals1.<>4__this && this.CS$<>8__locals1.<>4__this.gameObject)
			{
				PlayerNPCVehicle component = npcNetworkBehaviour.GetComponent<PlayerNPCVehicle>();
				if (component)
				{
					this.CS$<>8__locals1.<>4__this.NPCSetup(component, true, this.CS$<>8__locals1.actionEnterExitInteract, this.seatIndex);
					return;
				}
				npcNetworkBehaviour.networkObject.Destroy();
			}
		}

		// Token: 0x0400029E RID: 670
		public int seatIndex;

		// Token: 0x0400029F RID: 671
		public PlayerSpaceShipAI.<>c__DisplayClass29_0 CS$<>8__locals1;
	}

	// Token: 0x0200007D RID: 125
	[CompilerGenerated]
	private sealed class <DestroyWhenThereIsNoPlayersInsideIt>d__41 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x0600033B RID: 827 RVA: 0x00010589 File Offset: 0x0000E789
		[DebuggerHidden]
		public <DestroyWhenThereIsNoPlayersInsideIt>d__41(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x0600033C RID: 828 RVA: 0x00010598 File Offset: 0x0000E798
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0001059C File Offset: 0x0000E79C
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			PlayerSpaceShipAI playerSpaceShipAI = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(20f);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			foreach (PlayerNPCController playerNPCController in playerSpaceShipAI.npcs)
			{
				if (playerNPCController)
				{
					VanishComponent.VanishAndDestroy(playerNPCController.gameObject);
				}
			}
			Object.Destroy(playerSpaceShipAI);
			playerSpaceShipAI.npcs.Clear();
			return false;
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600033E RID: 830 RVA: 0x0001064C File Offset: 0x0000E84C
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x0600033F RID: 831 RVA: 0x00010654 File Offset: 0x0000E854
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000340 RID: 832 RVA: 0x0001065B File Offset: 0x0000E85B
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x040002A0 RID: 672
		private int <>1__state;

		// Token: 0x040002A1 RID: 673
		private object <>2__current;

		// Token: 0x040002A2 RID: 674
		public PlayerSpaceShipAI <>4__this;
	}

	// Token: 0x0200007E RID: 126
	[CompilerGenerated]
	private sealed class <<Awake>g__TestUpdate|18_0>d : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x06000341 RID: 833 RVA: 0x00010663 File Offset: 0x0000E863
		[DebuggerHidden]
		public <<Awake>g__TestUpdate|18_0>d(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x06000342 RID: 834 RVA: 0x00010672 File Offset: 0x0000E872
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x06000343 RID: 835 RVA: 0x00010674 File Offset: 0x0000E874
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			PlayerSpaceShipAI playerSpaceShipAI = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				playerSpaceShipAI.SimulateVehicle();
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

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000344 RID: 836 RVA: 0x000106CD File Offset: 0x0000E8CD
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x06000345 RID: 837 RVA: 0x000106D5 File Offset: 0x0000E8D5
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000346 RID: 838 RVA: 0x000106DC File Offset: 0x0000E8DC
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x040002A3 RID: 675
		private int <>1__state;

		// Token: 0x040002A4 RID: 676
		private object <>2__current;

		// Token: 0x040002A5 RID: 677
		public PlayerSpaceShipAI <>4__this;

		// Token: 0x040002A6 RID: 678
		private WaitForFixedUpdate <fixedUpdate>5__2;
	}
}
