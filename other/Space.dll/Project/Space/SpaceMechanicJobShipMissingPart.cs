using System;
using FMODUnity;
using HawkNetworking;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

// Token: 0x02000017 RID: 23
internal class SpaceMechanicJobShipMissingPart : HawkNetworkSubBehaviour
{
	// Token: 0x060000B3 RID: 179 RVA: 0x0000515F File Offset: 0x0000335F
	public override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_SET_STAGE = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSetStage), 1);
		this.RPC_CLIENT_HIT_BROKEN = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientHitBroken), 1);
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x0000519C File Offset: 0x0000339C
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.repairedPartTrigger.onTriggerEnter.AddListener(new UnityAction<ColliderTriggerEvent, Collider>(this.OnRepairPartTrigger));
		this.brokenPartOnShip.onCollisionEnter.AddListener(new UnityAction<CollisionEvent, Collision>(this.OnBrokenPartCollisionEnter));
		this.OnStageUpdated(this.currentStage);
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x000051F4 File Offset: 0x000033F4
	public void OnInitalizeWhiteboard(SpaceMechanicJobWhiteBoardData whiteBoardData)
	{
		whiteBoardData.partsFixedOutOf += 1;
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00005205 File Offset: 0x00003405
	private void OnDestroy()
	{
		if (this.spawnedPart)
		{
			VanishComponent.VanishAndDestroy(this.spawnedPart.gameObject);
		}
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x00005224 File Offset: 0x00003424
	private void OnBrokenPartCollisionEnter(CollisionEvent arg0, Collision collision)
	{
		HandTool componentInParent = collision.collider.GetComponentInParent<HandTool>();
		if (componentInParent && componentInParent.IsToolType(4) && componentInParent.TryHit())
		{
			byte b = this.brokenHitRequirements - 1;
			this.brokenHitRequirements = b;
			if (b <= 0)
			{
				this.ServerSetStage(SpaceMechanicJobShipMissingPart.ShipMissingPartStage.KnockedOff);
			}
			if (collision.contactCount > 0)
			{
				this.networkObject.SendRPCUnreliable(this.RPC_CLIENT_HIT_BROKEN, 0, collision.GetContact(0).point);
			}
		}
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x000052A4 File Offset: 0x000034A4
	private void OnRepairPartTrigger(ColliderTriggerEvent arg0, Collider collider)
	{
		HawkNetworkBehaviour componentInParent = collider.GetComponentInParent<HawkNetworkBehaviour>();
		if (componentInParent && this.repairedPartPrefab != null && this.repairedPartPrefab.AssetGUID == componentInParent.GetAssetIdRaw())
		{
			VanishComponent.VanishAndDestroy(componentInParent.gameObject);
			this.ServerSetStage(SpaceMechanicJobShipMissingPart.ShipMissingPartStage.Repaired);
		}
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x000052F4 File Offset: 0x000034F4
	internal void ServerSetStage(SpaceMechanicJobShipMissingPart.ShipMissingPartStage stage)
	{
		if (this.currentStage == stage)
		{
			return;
		}
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		if (stage == SpaceMechanicJobShipMissingPart.ShipMissingPartStage.KnockedOff && this.brokenPartPrefab != null && this.brokenPartPrefab.RuntimeKeyIsValid())
		{
			NetworkPrefab.SpawnNetworkPrefab(this.brokenPartPrefab, new Action<HawkNetworkBehaviour>(this.OnBrokenPartSpawned), new Vector3?(this.brokenPartOnShip.transform.position), new Quaternion?(this.brokenPartOnShip.transform.rotation), null, true, true, false, true);
		}
		this.networkObject.SendRPC(this.RPC_CLIENT_SET_STAGE, true, 7, new object[] { (byte)stage });
		this.OnStageUpdated(stage);
		if (this.IsTaskComplete())
		{
			Action<SpaceMechanicJobShipMissingPart> action = this.onServerComplete;
			if (action == null)
			{
				return;
			}
			action(this);
		}
	}

	// Token: 0x060000BA RID: 186 RVA: 0x000053C4 File Offset: 0x000035C4
	public void OnServerShipLanded()
	{
		if (!this.spawnedPart && this.repairedPartPrefab != null && this.repairedPartPrefab.RuntimeKeyIsValid() && !this.bTriedSpawningPart)
		{
			Transform transform = null;
			for (int i = 0; i < this.potentialPartSpawnsGuids.Length; i++)
			{
				int num = Random.Range(0, this.potentialPartSpawnsGuids.Length);
				string text = this.potentialPartSpawnsGuids[i];
				string text2 = this.potentialPartSpawnsGuids[num];
				this.potentialPartSpawnsGuids[i] = text2;
				this.potentialPartSpawnsGuids[num] = text2;
			}
			for (int j = 0; j < this.potentialPartSpawnsGuids.Length; j++)
			{
				Guid guid;
				if (Guid.TryParse(this.potentialPartSpawnsGuids[j], out guid))
				{
					GUIDComponent firstGUIDComponent = UnitySingleton<GUIDComponentManager>.Instance.GetFirstGUIDComponent(guid);
					if (firstGUIDComponent != null)
					{
						transform = firstGUIDComponent.transform;
						if (transform)
						{
							break;
						}
					}
				}
			}
			if (transform)
			{
				this.bTriedSpawningPart = true;
				NetworkPrefab.SpawnNetworkPrefab(this.repairedPartPrefab, new Action<HawkNetworkBehaviour>(this.OnRepairedPartSpawned), new Vector3?(transform.position), new Quaternion?(transform.rotation), null, true, true, false, true);
				return;
			}
			Debug.LogError("Something has gone wrong - No part spawn point. Going to auto complete it for safe measures");
			this.ServerSetStage(SpaceMechanicJobShipMissingPart.ShipMissingPartStage.Repaired);
		}
	}

	// Token: 0x060000BB RID: 187 RVA: 0x000054F3 File Offset: 0x000036F3
	private void OnRepairedPartSpawned(HawkNetworkBehaviour networkBehaviour)
	{
		if (this)
		{
			this.spawnedPart = networkBehaviour;
			return;
		}
		if (networkBehaviour)
		{
			networkBehaviour.networkObject.Destroy();
		}
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00005518 File Offset: 0x00003718
	private void OnBrokenPartSpawned(HawkNetworkBehaviour networkBehaviour)
	{
		if (networkBehaviour)
		{
			Rigidbody component = networkBehaviour.GetComponent<Rigidbody>();
			if (component)
			{
				component.AddForce(Vector3.up * 10f, 2);
			}
			VanishComponent.VanishAndDestroy(networkBehaviour.gameObject, 5f);
		}
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00005564 File Offset: 0x00003764
	private void OnStageUpdated(SpaceMechanicJobShipMissingPart.ShipMissingPartStage stage)
	{
		this.currentStage = stage;
		switch (stage)
		{
		case SpaceMechanicJobShipMissingPart.ShipMissingPartStage.None:
			this.brokenPartOnShip.gameObject.SetActive(true);
			this.repairedPartOnShip.gameObject.SetActive(false);
			this.repairedPartTrigger.gameObject.SetActive(false);
			return;
		case SpaceMechanicJobShipMissingPart.ShipMissingPartStage.KnockedOff:
			this.brokenPartOnShip.gameObject.SetActive(false);
			this.repairedPartOnShip.gameObject.SetActive(false);
			this.repairedPartTrigger.gameObject.SetActive(true);
			if (!string.IsNullOrEmpty(this.knockedOffSoundOneShot))
			{
				RuntimeManager.PlayOneShot(this.knockedOffSoundOneShot, this.brokenPartOnShip.transform.position);
				return;
			}
			break;
		case SpaceMechanicJobShipMissingPart.ShipMissingPartStage.Repaired:
			this.brokenPartOnShip.gameObject.SetActive(false);
			this.repairedPartOnShip.gameObject.SetActive(true);
			this.repairedPartTrigger.gameObject.SetActive(false);
			if (!string.IsNullOrEmpty(this.repairedSoundOneShot))
			{
				RuntimeManager.PlayOneShot(this.repairedSoundOneShot, this.brokenPartOnShip.transform.position);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060000BE RID: 190 RVA: 0x00005676 File Offset: 0x00003876
	private void ClientHitBroken(HawkNetReader reader, HawkRPCInfo info)
	{
		if (this.brokenHitPrefab)
		{
			UnitySingleton<ParticleManager>.Instance.PopPlayPush(this.brokenHitPrefab, reader.ReadVector3(), Quaternion.identity, 1f, 0);
		}
	}

	// Token: 0x060000BF RID: 191 RVA: 0x000056A8 File Offset: 0x000038A8
	private void ClientSetStage(HawkNetReader reader, HawkRPCInfo info)
	{
		SpaceMechanicJobShipMissingPart.ShipMissingPartStage shipMissingPartStage = (SpaceMechanicJobShipMissingPart.ShipMissingPartStage)reader.ReadByte();
		this.OnStageUpdated(shipMissingPartStage);
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x000056C3 File Offset: 0x000038C3
	public bool IsTaskComplete()
	{
		return this.currentStage == SpaceMechanicJobShipMissingPart.ShipMissingPartStage.Repaired;
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x000056CE File Offset: 0x000038CE
	public SpaceMechanicJobShipMissingPart()
	{
	}

	// Token: 0x0400008F RID: 143
	public Action<SpaceMechanicJobShipMissingPart> onServerComplete;

	// Token: 0x04000090 RID: 144
	private byte RPC_CLIENT_SET_STAGE;

	// Token: 0x04000091 RID: 145
	private byte RPC_CLIENT_HIT_BROKEN;

	// Token: 0x04000092 RID: 146
	[SerializeField]
	private CollisionEvent brokenPartOnShip;

	// Token: 0x04000093 RID: 147
	[SerializeField]
	private GameObject repairedPartOnShip;

	// Token: 0x04000094 RID: 148
	[SerializeField]
	private ColliderTriggerEvent repairedPartTrigger;

	// Token: 0x04000095 RID: 149
	[SerializeField]
	private AssetReference brokenPartPrefab;

	// Token: 0x04000096 RID: 150
	[SerializeField]
	private AssetReference repairedPartPrefab;

	// Token: 0x04000097 RID: 151
	[SerializeField]
	private string[] potentialPartSpawnsGuids;

	// Token: 0x04000098 RID: 152
	[SerializeField]
	private byte brokenHitRequirements = 3;

	// Token: 0x04000099 RID: 153
	[SerializeField]
	private BaseParticle brokenHitPrefab;

	// Token: 0x0400009A RID: 154
	[SerializeField]
	[EventRef]
	private string knockedOffSoundOneShot = "event:/Objects_Space/SpaceMechanicJob/Objects_Space_SpaceMechanicJob_BreakOffPart_Metal";

	// Token: 0x0400009B RID: 155
	[SerializeField]
	[EventRef]
	private string repairedSoundOneShot = "event:/Objects_Space/SpaceMechanicJob/Objects_Space_SpaceMechanicJob_PlacePart_Metal";

	// Token: 0x0400009C RID: 156
	private SpaceMechanicJobShipMissingPart.ShipMissingPartStage currentStage;

	// Token: 0x0400009D RID: 157
	private HawkNetworkBehaviour spawnedPart;

	// Token: 0x0400009E RID: 158
	private bool bTriedSpawningPart;

	// Token: 0x02000064 RID: 100
	internal enum ShipMissingPartStage : byte
	{
		// Token: 0x0400024E RID: 590
		None,
		// Token: 0x0400024F RID: 591
		KnockedOff,
		// Token: 0x04000250 RID: 592
		Repaired
	}
}
