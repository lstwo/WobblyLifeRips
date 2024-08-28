using System;
using HawkNetworking;
using UnityEngine;

// Token: 0x0200002E RID: 46
internal class SpaceProjectile : HawkNetworkBehaviour
{
	// Token: 0x0600015A RID: 346 RVA: 0x00007B45 File Offset: 0x00005D45
	protected override void Awake()
	{
		base.Awake();
		base.enabled = false;
	}

	// Token: 0x0600015B RID: 347 RVA: 0x00007B54 File Offset: 0x00005D54
	private void OnEnable()
	{
		this.timeSinceEnabled = Time.time;
	}

	// Token: 0x0600015C RID: 348 RVA: 0x00007B61 File Offset: 0x00005D61
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_SYNC = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSync), 1);
	}

	// Token: 0x0600015D RID: 349 RVA: 0x00007B83 File Offset: 0x00005D83
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.rigidbody.interpolation = 0;
	}

	// Token: 0x0600015E RID: 350 RVA: 0x00007BA4 File Offset: 0x00005DA4
	private void FixedUpdate()
	{
		if (this.networkObject.IsServer())
		{
			Vector3 vector = this.rigidbody.position;
			vector += this.syncData.velocity * Time.fixedDeltaTime;
			this.rigidbody.MovePosition(vector);
			if (Time.time - this.timeSinceSynced >= 1f)
			{
				this.timeSinceSynced = Time.time;
				this.syncData.position = vector;
				this.ServerSetData(this.syncData, false);
			}
		}
		else
		{
			Vector3 vector2 = base.transform.position;
			vector2 += this.syncData.velocity * Time.fixedDeltaTime;
			base.transform.position = vector2;
		}
		float num = (Time.time - this.timeSinceEnabled) / 0.1f;
		base.transform.localScale = Mathf.Lerp(1f, this.largeScale, num) * Vector3.one;
	}

	// Token: 0x0600015F RID: 351 RVA: 0x00007C98 File Offset: 0x00005E98
	public void ServerSetData(SpaceProjectileData bulletSync, bool bReliable = true)
	{
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		this.syncData = bulletSync;
		this.timeSinceSynced = Time.time;
		base.enabled = true;
		this.rigidbody.MovePosition(bulletSync.position);
		this.rigidbody.MoveRotation(bulletSync.rotation);
		if (bReliable)
		{
			this.networkObject.SendRPC(this.RPC_CLIENT_SYNC, 1, new object[] { SerializerDeserializerExtensions.SerializeNonAlloc(bulletSync) });
			return;
		}
		this.networkObject.SendRPCUnreliable(this.RPC_CLIENT_SYNC, 1, SerializerDeserializerExtensions.SerializeNonAlloc(bulletSync));
	}

	// Token: 0x06000160 RID: 352 RVA: 0x00007D48 File Offset: 0x00005F48
	private void OnTriggerEnter(Collider other)
	{
		if (Time.time - this.timeSinceEnabled < 0.1f)
		{
			return;
		}
		ISpaceProjectileCallback componentInParent = other.GetComponentInParent<ISpaceProjectileCallback>();
		if (other.isTrigger && componentInParent == null)
		{
			return;
		}
		if (componentInParent != null)
		{
			if (componentInParent.OnSpaceProjectileHit(this))
			{
				VanishComponent.VanishAndDestroy(base.gameObject);
				return;
			}
		}
		else
		{
			VanishComponent.VanishAndDestroy(base.gameObject);
		}
	}

	// Token: 0x06000161 RID: 353 RVA: 0x00007DA0 File Offset: 0x00005FA0
	private void ClientSync(HawkNetReader reader, HawkRPCInfo info)
	{
		this.syncData = SerializerDeserializerExtensions.Deserialize<SpaceProjectileData>(this.syncData, reader.ReadBytesAndSize());
		base.transform.SetPositionAndRotation(this.syncData.position, this.syncData.rotation);
		base.enabled = true;
	}

	// Token: 0x06000162 RID: 354 RVA: 0x00007DEC File Offset: 0x00005FEC
	public int GetBulletDamage()
	{
		return this.bulletDamage;
	}

	// Token: 0x06000163 RID: 355 RVA: 0x00007DF4 File Offset: 0x00005FF4
	public SpaceProjectile()
	{
	}

	// Token: 0x04000132 RID: 306
	private const float BulletGrowSeconds = 0.1f;

	// Token: 0x04000133 RID: 307
	private const float SyncInterval = 1f;

	// Token: 0x04000134 RID: 308
	private byte RPC_CLIENT_SYNC;

	// Token: 0x04000135 RID: 309
	[SerializeField]
	private float largeScale = 1f;

	// Token: 0x04000136 RID: 310
	[SerializeField]
	private int bulletDamage = 100;

	// Token: 0x04000137 RID: 311
	private SpaceProjectileData syncData;

	// Token: 0x04000138 RID: 312
	protected float timeSinceEnabled;

	// Token: 0x04000139 RID: 313
	private float timeSinceSynced;

	// Token: 0x0400013A RID: 314
	private Rigidbody rigidbody;
}
