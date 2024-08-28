using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HawkNetworking;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x0200000B RID: 11
public class AsteroidMiningMonster : HawkNetworkBehaviour, ISpaceProjectileCallback
{
	// Token: 0x06000049 RID: 73 RVA: 0x000034F0 File Offset: 0x000016F0
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_SET_AWAKE = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSetAwake), 1);
		this.RPC_CLIENT_ATTACK = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientAttack), 1);
		this.RPC_CLIENT_KNOCKOUT = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientKnockout), 1);
	}

	// Token: 0x0600004A RID: 74 RVA: 0x0000354F File Offset: 0x0000174F
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		if (AsteroidMiningMonster.CollisionMask == 0)
		{
			AsteroidMiningMonster.CollisionMask = LayerMask.GetMask(new string[] { "World", "Building", "WorldLarge", "Default" });
		}
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00003590 File Offset: 0x00001790
	private void FixedUpdate()
	{
		if (Time.fixedTime - this.checkTime >= 0.3f)
		{
			this.checkTime = Time.fixedTime;
			this.UpdateTarget();
		}
		if (this.currentTarget)
		{
			Vector3 vector = base.transform.InverseTransformPoint(this.currentTarget.position);
			Quaternion quaternion = Quaternion.AngleAxis(Mathf.Atan2(vector.x, vector.z) * 57.29578f, Vector3.up);
			this.monsterTransform.localRotation = Quaternion.Slerp(this.monsterTransform.localRotation, quaternion, 10f);
			if (this.bReadyToFire)
			{
				this.ServerTriggerAttack();
			}
		}
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00003638 File Offset: 0x00001838
	private void UpdateTarget()
	{
		List<PlayerCharacter> playerCharacters = UnitySingleton<GameInstance>.Instance.GetPlayerCharacters();
		this.currentTarget = null;
		float num = float.MaxValue;
		foreach (PlayerCharacter playerCharacter in playerCharacters)
		{
			Vector3 vector = playerCharacter.GetPlayerPosition() - this.eyesTransform.position;
			float magnitude = vector.magnitude;
			RaycastHit raycastHit;
			if (magnitude <= this.radius && magnitude < num && !Physics.Raycast(this.eyesTransform.position, vector.normalized, ref raycastHit, magnitude, AsteroidMiningMonster.CollisionMask, 1))
			{
				num = magnitude;
				this.currentTarget = playerCharacter.GetPlayerBody().GetHead().transform;
				this.targetPosition = this.currentTarget.position;
			}
		}
		this.ServerSetAwake(this.currentTarget);
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00003728 File Offset: 0x00001928
	private void ServerSetAwake(bool bAwake)
	{
		if (this.bAwake == bAwake)
		{
			return;
		}
		if (this.bKnockedOut)
		{
			return;
		}
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		this.networkObject.SendRPC(this.RPC_CLIENT_SET_AWAKE, 6, new object[]
		{
			HawkNetworkManager.DefaultInstance.GetTimestep(),
			bAwake
		});
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00003791 File Offset: 0x00001991
	private void ServerTriggerAttack()
	{
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		this.networkObject.SendRPC(this.RPC_CLIENT_ATTACK, 0, Array.Empty<object>());
	}

	// Token: 0x0600004F RID: 79 RVA: 0x000037C0 File Offset: 0x000019C0
	private void ServerKnockout()
	{
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		this.networkObject.ClearBuffered(this.RPC_CLIENT_SET_AWAKE);
		this.networkObject.SendRPC(this.RPC_CLIENT_KNOCKOUT, 0, Array.Empty<object>());
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00003800 File Offset: 0x00001A00
	bool ISpaceProjectileCallback.OnSpaceProjectileHit(SpaceProjectile projectile)
	{
		if (!this.bKnockedOut && this.bAwake)
		{
			this.ServerKnockout();
		}
		return true;
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00003819 File Offset: 0x00001A19
	private void AnimationEvent_PostKnockout()
	{
		this.bKnockedOut = false;
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00003822 File Offset: 0x00001A22
	private void AnimationEvent_ReadyToFire()
	{
		this.bReadyToFire = true;
	}

	// Token: 0x06000053 RID: 83 RVA: 0x0000382C File Offset: 0x00001A2C
	private void AnimationEvent_Fire()
	{
		if (this.networkObject != null && this.networkObject.IsServer() && this.projectileSpawn)
		{
			NetworkPrefab.SpawnNetworkPrefab(this.projectilePrefab, delegate(HawkNetworkBehaviour x)
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
							velocity = (this.targetPosition - this.eyesTransform.position).normalized * this.projectileSpeed
						}, true);
					}
				}
			}, new Vector3?(this.projectileSpawn.position), new Quaternion?(this.projectileSpawn.rotation), null, true, false, false, true);
		}
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00003898 File Offset: 0x00001A98
	private void ClientKnockout(HawkNetReader reader, HawkRPCInfo info)
	{
		if (this.animator)
		{
			this.animator.SetTrigger("tKnockout");
			this.animator.SetBool("bAwake", false);
		}
		this.bReadyToFire = false;
		this.bAwake = false;
		this.bKnockedOut = true;
	}

	// Token: 0x06000055 RID: 85 RVA: 0x000038E8 File Offset: 0x00001AE8
	private void ClientAttack(HawkNetReader reader, HawkRPCInfo info)
	{
		if (this.animator)
		{
			this.animator.SetTrigger("tAttack");
		}
		this.bReadyToFire = false;
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00003910 File Offset: 0x00001B10
	private void ClientSetAwake(HawkNetReader reader, HawkRPCInfo info)
	{
		ulong num = reader.ReadUInt64();
		if (this.awakeTimestep >= num)
		{
			return;
		}
		this.awakeTimestep = num;
		this.bAwake = reader.ReadBoolean();
		if (this.animator)
		{
			this.animator.SetBool("bAwake", this.bAwake);
		}
		if (!this.bAwake)
		{
			this.bReadyToFire = false;
		}
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00003973 File Offset: 0x00001B73
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x06000058 RID: 88 RVA: 0x00003995 File Offset: 0x00001B95
	public AsteroidMiningMonster()
	{
	}

	// Token: 0x06000059 RID: 89 RVA: 0x000039B4 File Offset: 0x00001BB4
	[CompilerGenerated]
	private void <AnimationEvent_Fire>b__28_0(HawkNetworkBehaviour x)
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
					velocity = (this.targetPosition - this.eyesTransform.position).normalized * this.projectileSpeed
				}, true);
			}
		}
	}

	// Token: 0x04000040 RID: 64
	private const float CheckInterval = 0.3f;

	// Token: 0x04000041 RID: 65
	private static int CollisionMask;

	// Token: 0x04000042 RID: 66
	private byte RPC_CLIENT_SET_AWAKE;

	// Token: 0x04000043 RID: 67
	private byte RPC_CLIENT_ATTACK;

	// Token: 0x04000044 RID: 68
	private byte RPC_CLIENT_KNOCKOUT;

	// Token: 0x04000045 RID: 69
	[SerializeField]
	private Animator animator;

	// Token: 0x04000046 RID: 70
	[SerializeField]
	private AssetReference projectilePrefab;

	// Token: 0x04000047 RID: 71
	[SerializeField]
	private Transform projectileSpawn;

	// Token: 0x04000048 RID: 72
	[SerializeField]
	private float projectileSpeed = 10f;

	// Token: 0x04000049 RID: 73
	[SerializeField]
	private Transform monsterTransform;

	// Token: 0x0400004A RID: 74
	[SerializeField]
	private float radius = 100f;

	// Token: 0x0400004B RID: 75
	[SerializeField]
	private Transform eyesTransform;

	// Token: 0x0400004C RID: 76
	private float checkTime;

	// Token: 0x0400004D RID: 77
	private Transform currentTarget;

	// Token: 0x0400004E RID: 78
	private Vector3 targetPosition;

	// Token: 0x0400004F RID: 79
	private bool bAwake;

	// Token: 0x04000050 RID: 80
	private bool bReadyToFire;

	// Token: 0x04000051 RID: 81
	private bool bKnockedOut;

	// Token: 0x04000052 RID: 82
	private ulong awakeTimestep;
}
