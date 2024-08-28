using System;
using System.Collections.Generic;
using HawkNetworking;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000051 RID: 81
[DisallowMultipleComponent]
public class SpaceGravityArea : MonoBehaviour
{
	// Token: 0x06000252 RID: 594 RVA: 0x0000C17C File Offset: 0x0000A37C
	public SpaceGravityArea()
	{
		this.onNetworkBehaviourDestoryedCallback = new Action<HawkNetworkBehaviour>(this.OnNetworkBehaviourDestroyed);
	}

	// Token: 0x06000253 RID: 595 RVA: 0x0000C1E8 File Offset: 0x0000A3E8
	private void Awake()
	{
		this.newGravity = this.gravity - Physics.gravity;
		if (this.colliderTrigger)
		{
			this.colliderTrigger.onTriggerEnter.AddListener(new UnityAction<ColliderTriggerEvent, Collider>(this.OnColliderTriggerEnter));
			this.colliderTrigger.onTriggerExit.AddListener(new UnityAction<ColliderTriggerEvent, Collider>(this.OnColliderTriggerExit));
		}
	}

	// Token: 0x06000254 RID: 596 RVA: 0x0000C250 File Offset: 0x0000A450
	private void OnColliderTriggerEnter(ColliderTriggerEvent triggerEvent, Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if (attachedRigidbody && !attachedRigidbody.isKinematic)
		{
			ISpaceGravityAreaObjectCallback componentInParent = attachedRigidbody.GetComponentInParent<ISpaceGravityAreaObjectCallback>();
			if (componentInParent != null && !componentInParent.IsAllowedInGravityArea(this))
			{
				return;
			}
			HawkNetworkBehaviour componentInParent2 = attachedRigidbody.GetComponentInParent<HawkNetworkBehaviour>();
			if (componentInParent2)
			{
				if (!this.enteredBehaviours.ContainsKey(componentInParent2))
				{
					this.enteredBehaviours.Add(componentInParent2, 1);
					this.OnEntered(componentInParent2);
				}
				else
				{
					Dictionary<HawkNetworkBehaviour, int> dictionary = this.enteredBehaviours;
					HawkNetworkBehaviour hawkNetworkBehaviour = componentInParent2;
					int num = dictionary[hawkNetworkBehaviour] + 1;
					dictionary[hawkNetworkBehaviour] = num;
				}
				if (!this.enteredBodies.ContainsKey(attachedRigidbody))
				{
					SpaceGravityRigidbody orAddComponent = UnityExtensions.GetOrAddComponent<SpaceGravityRigidbody>(attachedRigidbody.gameObject);
					orAddComponent.SetRigidbody(attachedRigidbody);
					this.enteredBodies.Add(attachedRigidbody, 1);
					this.OnEntered(orAddComponent);
					if (componentInParent != null)
					{
						componentInParent.OnEntered(this);
						return;
					}
				}
				else
				{
					Dictionary<Rigidbody, int> dictionary2 = this.enteredBodies;
					Rigidbody rigidbody = attachedRigidbody;
					int num = dictionary2[rigidbody] + 1;
					dictionary2[rigidbody] = num;
				}
			}
		}
	}

	// Token: 0x06000255 RID: 597 RVA: 0x0000C340 File Offset: 0x0000A540
	private void OnColliderTriggerExit(ColliderTriggerEvent triggerEvent, Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if (attachedRigidbody && !attachedRigidbody.isKinematic)
		{
			HawkNetworkBehaviour componentInParent = attachedRigidbody.GetComponentInParent<HawkNetworkBehaviour>();
			if (componentInParent)
			{
				if (this.enteredBehaviours.ContainsKey(componentInParent))
				{
					Dictionary<HawkNetworkBehaviour, int> dictionary = this.enteredBehaviours;
					HawkNetworkBehaviour hawkNetworkBehaviour = componentInParent;
					int num = dictionary[hawkNetworkBehaviour] - 1;
					dictionary[hawkNetworkBehaviour] = num;
					if (num <= 0)
					{
						this.enteredBehaviours.Remove(componentInParent);
						this.OnExited(componentInParent);
					}
				}
				if (this.enteredBodies.ContainsKey(attachedRigidbody))
				{
					Dictionary<Rigidbody, int> dictionary2 = this.enteredBodies;
					Rigidbody rigidbody = attachedRigidbody;
					int num = dictionary2[rigidbody] - 1;
					dictionary2[rigidbody] = num;
					if (num <= 0)
					{
						SpaceGravityRigidbody component = attachedRigidbody.gameObject.GetComponent<SpaceGravityRigidbody>();
						this.enteredBodies.Remove(attachedRigidbody);
						this.OnExited(component);
						ISpaceGravityAreaObjectCallback componentInParent2 = attachedRigidbody.GetComponentInParent<ISpaceGravityAreaObjectCallback>();
						if (componentInParent2 != null)
						{
							componentInParent2.OnExited(this);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000256 RID: 598 RVA: 0x0000C41F File Offset: 0x0000A61F
	private void OnTriggerEnter(Collider other)
	{
		this.OnColliderTriggerEnter(null, other);
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0000C429 File Offset: 0x0000A629
	private void OnTriggerExit(Collider other)
	{
		this.OnColliderTriggerExit(null, other);
	}

	// Token: 0x06000258 RID: 600 RVA: 0x0000C433 File Offset: 0x0000A633
	private void OnEntered(HawkNetworkBehaviour networkBehaviour)
	{
		networkBehaviour.onDestroy.AddCallback(this.onNetworkBehaviourDestoryedCallback);
	}

	// Token: 0x06000259 RID: 601 RVA: 0x0000C446 File Offset: 0x0000A646
	private void OnExited(HawkNetworkBehaviour networkBehaviour)
	{
		networkBehaviour.onDestroy.RemoveCallback(this.onNetworkBehaviourDestoryedCallback);
	}

	// Token: 0x0600025A RID: 602 RVA: 0x0000C45C File Offset: 0x0000A65C
	private void OnNetworkBehaviourDestroyed(HawkNetworkBehaviour networkBehaviour)
	{
		this.rigidbodiesCache.Clear();
		networkBehaviour.GetComponentsInChildren<Rigidbody>(true, this.rigidbodiesCache);
		this.enteredBehaviours.Remove(networkBehaviour);
		this.OnExited(networkBehaviour);
		foreach (Rigidbody rigidbody in this.rigidbodiesCache)
		{
			this.enteredBodies.Remove(rigidbody);
			SpaceGravityRigidbody component = rigidbody.GetComponent<SpaceGravityRigidbody>();
			this.OnExited(component);
		}
	}

	// Token: 0x0600025B RID: 603 RVA: 0x0000C4F0 File Offset: 0x0000A6F0
	private void OnEntered(SpaceGravityRigidbody gravityRigidbody)
	{
		this.gravityRigidbodies.AddLast(gravityRigidbody);
		gravityRigidbody.IncrementGravityAreaRef();
	}

	// Token: 0x0600025C RID: 604 RVA: 0x0000C505 File Offset: 0x0000A705
	private void OnExited(SpaceGravityRigidbody gravityRigidbody)
	{
		if (this.gravityRigidbodies.Remove(gravityRigidbody))
		{
			gravityRigidbody.DecrementGravityAreaRef();
		}
	}

	// Token: 0x0600025D RID: 605 RVA: 0x0000C51B File Offset: 0x0000A71B
	private void FixedUpdate()
	{
		this.UpdateGravity();
	}

	// Token: 0x0600025E RID: 606 RVA: 0x0000C524 File Offset: 0x0000A724
	protected virtual void UpdateGravity()
	{
		foreach (SpaceGravityRigidbody spaceGravityRigidbody in this.gravityRigidbodies)
		{
			Rigidbody rigidbody = spaceGravityRigidbody.GetRigidbody();
			if (!rigidbody.IsSleeping() && rigidbody.useGravity && rigidbody.velocity.sqrMagnitude > 0.001f)
			{
				spaceGravityRigidbody.ApplyVelocity(this, this.newGravity, 5);
			}
		}
	}

	// Token: 0x0600025F RID: 607 RVA: 0x0000C5AC File Offset: 0x0000A7AC
	public Vector3 GetNewGravity()
	{
		return this.newGravity;
	}

	// Token: 0x06000260 RID: 608 RVA: 0x0000C5B4 File Offset: 0x0000A7B4
	public Vector3 GetGravity()
	{
		return this.gravity;
	}

	// Token: 0x040001E4 RID: 484
	[SerializeField]
	private Vector3 gravity = new Vector3(0f, -19.62f, 0f);

	// Token: 0x040001E5 RID: 485
	[SerializeField]
	private ColliderTriggerEvent colliderTrigger;

	// Token: 0x040001E6 RID: 486
	private Dictionary<HawkNetworkBehaviour, int> enteredBehaviours = new Dictionary<HawkNetworkBehaviour, int>();

	// Token: 0x040001E7 RID: 487
	private Dictionary<Rigidbody, int> enteredBodies = new Dictionary<Rigidbody, int>();

	// Token: 0x040001E8 RID: 488
	protected LinkedList<SpaceGravityRigidbody> gravityRigidbodies = new LinkedList<SpaceGravityRigidbody>();

	// Token: 0x040001E9 RID: 489
	private Action<HawkNetworkBehaviour> onNetworkBehaviourDestoryedCallback;

	// Token: 0x040001EA RID: 490
	private Vector3 newGravity;

	// Token: 0x040001EB RID: 491
	private List<Rigidbody> rigidbodiesCache = new List<Rigidbody>();
}
