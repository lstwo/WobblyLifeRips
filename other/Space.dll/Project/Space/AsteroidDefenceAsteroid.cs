using System;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class AsteroidDefenceAsteroid : HawkNetworkBehaviour, ISpaceProjectileCallback
{
	// Token: 0x06000004 RID: 4 RVA: 0x00002097 File Offset: 0x00000297
	protected override void Awake()
	{
		base.Awake();
		base.enabled = false;
		base.transform.localScale = Vector3.zero;
	}

	// Token: 0x06000005 RID: 5 RVA: 0x000020B6 File Offset: 0x000002B6
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	// Token: 0x06000006 RID: 6 RVA: 0x000020BE File Offset: 0x000002BE
	private void OnEnable()
	{
		this.timeSinceEnabled = Time.time;
	}

	// Token: 0x06000007 RID: 7 RVA: 0x000020CC File Offset: 0x000002CC
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_SYNC = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSync), 1);
		this.RPC_CLIENT_DESTROY = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientDestroy), 1);
		this.RPC_CLIENT_SYNC_HEALTH = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSyncHealth), 1);
	}

	// Token: 0x06000008 RID: 8 RVA: 0x0000212B File Offset: 0x0000032B
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.rigidbody.interpolation = 0;
		this.collisionMask = LayerMask.NameToLayer("Ignore Raycast");
		this.initalHealth = (float)this.health;
	}

	// Token: 0x06000009 RID: 9 RVA: 0x0000216C File Offset: 0x0000036C
	private void FixedUpdate()
	{
		if (this.networkObject == null)
		{
			return;
		}
		if (this.networkObject.IsServer())
		{
			Vector3 vector = this.rigidbody.position;
			vector += this.syncData.velocity * Time.fixedDeltaTime;
			this.rigidbody.MovePosition(vector);
			if (Time.time - this.timeSinceSynced >= 1f)
			{
				this.timeSinceSynced = Time.time;
				this.syncData.position = vector;
				this.networkObject.SendRPCUnreliable(this.RPC_CLIENT_SYNC, 1, SerializerDeserializerExtensions.SerializeNonAlloc(this.syncData));
			}
		}
		else
		{
			Vector3 vector2 = base.transform.position;
			vector2 += this.syncData.velocity * Time.fixedDeltaTime;
			base.transform.position = vector2;
		}
		base.transform.rotation = Quaternion.LookRotation(this.syncData.velocity, Vector3.up);
		float num = (Time.time - this.timeSinceEnabled) / 0.3f;
		base.transform.localScale = Mathf.Lerp(0f, 1f, num) * Vector3.one;
	}

	// Token: 0x0600000A RID: 10 RVA: 0x000022A8 File Offset: 0x000004A8
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == this.collisionMask)
		{
			return;
		}
		if (this.networkObject != null)
		{
			if (other.isTrigger)
			{
				return;
			}
			if ((other.attachedRigidbody && !other.attachedRigidbody.isKinematic) || other.GetComponentInParent<SpaceStation>())
			{
				this.Explode(other);
			}
		}
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002308 File Offset: 0x00000508
	private void Damage(int damage)
	{
		this.OnDamaged(damage);
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002314 File Offset: 0x00000514
	protected virtual void OnDamaged(int damage)
	{
		this.health -= damage;
		this.networkObject.SendRPC(this.RPC_CLIENT_SYNC_HEALTH, 1, new object[] { this.health });
		this.OnHealthUpdated();
		if (this.health <= 0)
		{
			this.Explode(null);
		}
	}

	// Token: 0x0600000D RID: 13 RVA: 0x0000236B File Offset: 0x0000056B
	private void Explode(Collider other)
	{
		Action<AsteroidDefenceAsteroid, Collider> action = this.onExploded;
		if (action != null)
		{
			action(this, other);
		}
		this.networkObject.SendRPC(this.RPC_CLIENT_DESTROY, 0, Array.Empty<object>());
		this.networkObject.Destroy(1f);
	}

	// Token: 0x0600000E RID: 14 RVA: 0x000023A7 File Offset: 0x000005A7
	protected virtual void OnExploded()
	{
	}

	// Token: 0x0600000F RID: 15 RVA: 0x000023A9 File Offset: 0x000005A9
	bool ISpaceProjectileCallback.OnSpaceProjectileHit(SpaceProjectile projectile)
	{
		this.Damage(projectile.GetBulletDamage());
		return true;
	}

	// Token: 0x06000010 RID: 16 RVA: 0x000023B8 File Offset: 0x000005B8
	private void OnHealthUpdated()
	{
		float num = (float)this.health / this.initalHealth;
		if (this.healthStates != null)
		{
			AsteroidDefenceHealthState asteroidDefenceHealthState = null;
			for (int i = 0; i < this.healthStates.Length; i++)
			{
				if (num > this.healthStates[i].healthPercentage && (asteroidDefenceHealthState == null || this.healthStates[i].healthPercentage > asteroidDefenceHealthState.healthPercentage))
				{
					asteroidDefenceHealthState = this.healthStates[i];
				}
			}
			for (int j = 0; j < this.healthStates.Length; j++)
			{
				if (this.healthStates[j] != asteroidDefenceHealthState)
				{
					this.healthStates[j].showGameObject.SetActive(false);
				}
			}
			if (asteroidDefenceHealthState != null)
			{
				asteroidDefenceHealthState.showGameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00002468 File Offset: 0x00000668
	public void ServerSetData(Transform serverTarget, AsteroidDefenceAsteroidSync asteroidSync, bool bCalculateVelocity = true)
	{
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		if (bCalculateVelocity)
		{
			asteroidSync.velocity *= Random.Range(this.minSpeed, this.maxSpeed);
		}
		this.serverTarget = serverTarget;
		this.syncData = asteroidSync;
		base.enabled = true;
		this.rigidbody.MovePosition(asteroidSync.position);
		this.networkObject.SendRPC(this.RPC_CLIENT_SYNC, 1, new object[] { SerializerDeserializerExtensions.SerializeNonAlloc(asteroidSync) });
		this.timeSinceSynced = Time.time;
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00002512 File Offset: 0x00000712
	private void ClientSyncHealth(HawkNetReader reader, HawkRPCInfo info)
	{
		this.health = reader.ReadInt32();
		this.OnHealthUpdated();
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00002528 File Offset: 0x00000728
	private void ClientDestroy(HawkNetReader reader, HawkRPCInfo info)
	{
		if (this.velocityParticles != null)
		{
			foreach (ParticleSystem particleSystem in this.velocityParticles)
			{
				particleSystem.transform.parent = null;
				particleSystem.main.stopAction = 2;
				particleSystem.Stop();
			}
		}
		this.OnExploded();
		VanishComponent.VanishLocal(base.gameObject);
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00002586 File Offset: 0x00000786
	private void ClientSync(HawkNetReader reader, HawkRPCInfo info)
	{
		this.syncData = SerializerDeserializerExtensions.Deserialize<AsteroidDefenceAsteroidSync>(this.syncData, reader.ReadBytesAndSize());
		base.transform.position = this.syncData.position;
		base.enabled = true;
	}

	// Token: 0x06000015 RID: 21 RVA: 0x000025BC File Offset: 0x000007BC
	public short GetDamage()
	{
		return this.damage;
	}

	// Token: 0x06000016 RID: 22 RVA: 0x000025C4 File Offset: 0x000007C4
	public AsteroidDefenceAsteroid()
	{
	}

	// Token: 0x04000005 RID: 5
	private byte RPC_CLIENT_SYNC;

	// Token: 0x04000006 RID: 6
	private byte RPC_CLIENT_DESTROY;

	// Token: 0x04000007 RID: 7
	private byte RPC_CLIENT_SYNC_HEALTH;

	// Token: 0x04000008 RID: 8
	public Action<AsteroidDefenceAsteroid, Collider> onExploded;

	// Token: 0x04000009 RID: 9
	[SerializeField]
	private ParticleSystem[] velocityParticles;

	// Token: 0x0400000A RID: 10
	[SerializeField]
	private int health = 100;

	// Token: 0x0400000B RID: 11
	[SerializeField]
	private AsteroidDefenceHealthState[] healthStates;

	// Token: 0x0400000C RID: 12
	[SerializeField]
	private short damage = 20;

	// Token: 0x0400000D RID: 13
	[SerializeField]
	private float minSpeed = 50f;

	// Token: 0x0400000E RID: 14
	[SerializeField]
	private float maxSpeed = 100f;

	// Token: 0x0400000F RID: 15
	protected AsteroidDefenceAsteroidSync syncData;

	// Token: 0x04000010 RID: 16
	protected Transform serverTarget;

	// Token: 0x04000011 RID: 17
	private float timeSinceEnabled;

	// Token: 0x04000012 RID: 18
	private Rigidbody rigidbody;

	// Token: 0x04000013 RID: 19
	private int collisionMask;

	// Token: 0x04000014 RID: 20
	private float timeSinceSynced;

	// Token: 0x04000015 RID: 21
	private float initalHealth;
}
