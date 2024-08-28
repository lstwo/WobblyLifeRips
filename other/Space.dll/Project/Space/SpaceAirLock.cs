using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HawkNetworking;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000021 RID: 33
public class SpaceAirLock : HawkNetworkBehaviour
{
	// Token: 0x060000ED RID: 237 RVA: 0x00005DDE File Offset: 0x00003FDE
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_SET_HAS_AIR = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSetHasAir), 1);
	}

	// Token: 0x060000EE RID: 238 RVA: 0x00005E00 File Offset: 0x00004000
	protected override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		if (networkObject.IsServer())
		{
			this.noAirDoor.doorTrigger.onTriggerEnter.AddListener(new UnityAction<ColliderTriggerEvent, Collider>(this.OnTriggerEnterNoAirDoor));
			this.airDoor.doorTrigger.onTriggerEnter.AddListener(new UnityAction<ColliderTriggerEvent, Collider>(this.OnTriggerEnterAirDoor));
			if (this.touchButtons != null)
			{
				TouchButtonSet[] array = this.touchButtons;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].onButtonPressed.AddListener(new UnityAction(this.OnButtonPressed));
				}
			}
		}
	}

	// Token: 0x060000EF RID: 239 RVA: 0x00005E94 File Offset: 0x00004094
	private void AnimationEvent_AnimatorFinished()
	{
		if (this.animator)
		{
			this.animator.keepAnimatorStateOnDisable = true;
			this.animator.enabled = false;
		}
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x00005EBB File Offset: 0x000040BB
	private void OnTriggerEnterNoAirDoor(ColliderTriggerEvent arg0, Collider collider)
	{
		if (collider.GetComponentInParent<PlayerCharacter>())
		{
			this.StartAirLock(SpaceAirMode.NoAir);
		}
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x00005ED1 File Offset: 0x000040D1
	private void OnTriggerEnterAirDoor(ColliderTriggerEvent arg0, Collider collider)
	{
		if (collider.GetComponentInParent<PlayerCharacter>())
		{
			this.StartAirLock(SpaceAirMode.Air);
		}
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x00005EE7 File Offset: 0x000040E7
	private void StartAirLock(SpaceAirMode target)
	{
		if (this.airLockCoroutine == null)
		{
			this.airLockCoroutine = base.StartCoroutine(this.AirLockEnumerator(target));
		}
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00005F04 File Offset: 0x00004104
	private void OnButtonPressed()
	{
		if (this.airLockCoroutine != null)
		{
			base.StopCoroutine(this.airLockCoroutine);
		}
		if (this.currentMode == SpaceAirMode.Air)
		{
			this.airLockCoroutine = base.StartCoroutine(this.AirLockEnumerator(SpaceAirMode.NoAir));
			return;
		}
		this.airLockCoroutine = base.StartCoroutine(this.AirLockEnumerator(SpaceAirMode.Air));
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x00005F55 File Offset: 0x00004155
	private IEnumerator AirLockEnumerator(SpaceAirMode target)
	{
		if (this.touchButtons != null)
		{
			foreach (TouchButtonSet touchButtonSet in this.touchButtons)
			{
				if (touchButtonSet)
				{
					touchButtonSet.Set(false);
				}
			}
		}
		bool bHasChanged = this.currentMode != target;
		SpaceAirLockDoorData doorWhichWeOpened = null;
		if (target != SpaceAirMode.NoAir)
		{
			if (target == SpaceAirMode.Air)
			{
				this.noAirDoor.door.Set(false);
				yield return new WaitUntil(() => this.noAirDoor.door.HasFinished());
				this.currentMode = target;
				if (bHasChanged)
				{
					this.networkObject.SendRPC(this.RPC_CLIENT_SET_HAS_AIR, true, 6, new object[] { true });
				}
				if (bHasChanged)
				{
					yield return new WaitForSeconds(this.timeToFillWithAir);
					yield return this.ApplyPlayersAirLock();
				}
				this.airDoor.door.Set(true);
				doorWhichWeOpened = this.airDoor;
				yield return new WaitUntil(() => this.airDoor.door.HasFinished());
			}
		}
		else
		{
			this.airDoor.door.Set(false);
			yield return new WaitUntil(() => this.airDoor.door.HasFinished());
			this.currentMode = target;
			if (bHasChanged)
			{
				yield return this.ApplyPlayersAirLock();
				this.networkObject.SendRPC(this.RPC_CLIENT_SET_HAS_AIR, true, 6, new object[] { false });
				yield return new WaitForSeconds(this.timeToFillWithAir);
			}
			this.noAirDoor.door.Set(true);
			doorWhichWeOpened = this.noAirDoor;
			yield return new WaitUntil(() => this.noAirDoor.door.HasFinished());
		}
		if (this.touchButtons != null)
		{
			foreach (TouchButtonSet touchButtonSet2 in this.touchButtons)
			{
				if (touchButtonSet2)
				{
					touchButtonSet2.Set(true);
				}
			}
		}
		for (;;)
		{
			if (this.airLockCoroutine != null && !this.insideAirLockTrigger.ContainsPlayers())
			{
				yield return new WaitForSeconds(3f);
				if (this.airLockCoroutine != null && !this.insideAirLockTrigger.ContainsPlayers())
				{
					if (doorWhichWeOpened == null)
					{
						break;
					}
					doorWhichWeOpened.door.Set(false);
					yield return new WaitUntil(() => doorWhichWeOpened.door.HasFinished());
					if (this.airLockCoroutine != null && !this.insideAirLockTrigger.ContainsPlayers())
					{
						break;
					}
					doorWhichWeOpened.door.Set(true);
					yield return new WaitUntil(() => doorWhichWeOpened.door.HasFinished());
				}
			}
			else
			{
				yield return null;
			}
		}
		this.airLockCoroutine = null;
		yield break;
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x00005F6B File Offset: 0x0000416B
	private IEnumerator ApplyPlayersAirLock()
	{
		yield return null;
		using (List<PlayerCharacter>.Enumerator enumerator = this.insideAirLockTrigger.GetOverlappingPlayers().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				PlayerCharacter playerCharacter = enumerator.Current;
				SpacePlayerCharacter component = playerCharacter.GetComponent<SpacePlayerCharacter>();
				if (component)
				{
					component.ServerSetAirMode(this.currentMode);
				}
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00005F7C File Offset: 0x0000417C
	private void ClientSetHasAir(HawkNetReader reader, HawkRPCInfo info)
	{
		bool flag = reader.ReadBoolean();
		if (this.animator)
		{
			this.animator.enabled = true;
			this.animator.Play(flag ? "Air" : "NoAir");
		}
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00005FC3 File Offset: 0x000041C3
	public SpaceAirLock()
	{
	}

	// Token: 0x040000C3 RID: 195
	private byte RPC_CLIENT_SET_HAS_AIR;

	// Token: 0x040000C4 RID: 196
	[SerializeField]
	private TouchButtonSet[] touchButtons;

	// Token: 0x040000C5 RID: 197
	[SerializeField]
	private SpaceAirMode currentMode;

	// Token: 0x040000C6 RID: 198
	[SerializeField]
	private SpaceAirLockDoorData noAirDoor;

	// Token: 0x040000C7 RID: 199
	[SerializeField]
	private SpaceAirLockDoorData airDoor;

	// Token: 0x040000C8 RID: 200
	[SerializeField]
	private WobblyPlayerCharacterTriggerNonNetworked insideAirLockTrigger;

	// Token: 0x040000C9 RID: 201
	[SerializeField]
	private float timeToFillWithAir = 1f;

	// Token: 0x040000CA RID: 202
	[SerializeField]
	private float timeToGetRidOfAir = 1f;

	// Token: 0x040000CB RID: 203
	[SerializeField]
	private Animator animator;

	// Token: 0x040000CC RID: 204
	private Coroutine airLockCoroutine;

	// Token: 0x02000065 RID: 101
	[CompilerGenerated]
	private sealed class <>c__DisplayClass17_0
	{
		// Token: 0x060002D5 RID: 725 RVA: 0x0000ECE1 File Offset: 0x0000CEE1
		public <>c__DisplayClass17_0()
		{
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000ECE9 File Offset: 0x0000CEE9
		internal bool <AirLockEnumerator>b__0()
		{
			return this.<>4__this.noAirDoor.door.HasFinished();
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0000ED00 File Offset: 0x0000CF00
		internal bool <AirLockEnumerator>b__1()
		{
			return this.<>4__this.airDoor.door.HasFinished();
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0000ED17 File Offset: 0x0000CF17
		internal bool <AirLockEnumerator>b__2()
		{
			return this.<>4__this.airDoor.door.HasFinished();
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000ED2E File Offset: 0x0000CF2E
		internal bool <AirLockEnumerator>b__3()
		{
			return this.<>4__this.noAirDoor.door.HasFinished();
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0000ED45 File Offset: 0x0000CF45
		internal bool <AirLockEnumerator>b__4()
		{
			return this.doorWhichWeOpened.door.HasFinished();
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000ED57 File Offset: 0x0000CF57
		internal bool <AirLockEnumerator>b__5()
		{
			return this.doorWhichWeOpened.door.HasFinished();
		}

		// Token: 0x04000251 RID: 593
		public SpaceAirLock <>4__this;

		// Token: 0x04000252 RID: 594
		public SpaceAirLockDoorData doorWhichWeOpened;
	}

	// Token: 0x02000066 RID: 102
	[CompilerGenerated]
	private sealed class <AirLockEnumerator>d__17 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x060002DC RID: 732 RVA: 0x0000ED69 File Offset: 0x0000CF69
		[DebuggerHidden]
		public <AirLockEnumerator>d__17(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0000ED78 File Offset: 0x0000CF78
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0000ED7C File Offset: 0x0000CF7C
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpaceAirLock spaceAirLock = this;
			switch (num)
			{
			case 0:
			{
				this.<>1__state = -1;
				CS$<>8__locals1 = new SpaceAirLock.<>c__DisplayClass17_0();
				CS$<>8__locals1.<>4__this = this;
				if (spaceAirLock.touchButtons != null)
				{
					foreach (TouchButtonSet touchButtonSet in spaceAirLock.touchButtons)
					{
						if (touchButtonSet)
						{
							touchButtonSet.Set(false);
						}
					}
				}
				bHasChanged = spaceAirLock.currentMode != target;
				CS$<>8__locals1.doorWhichWeOpened = null;
				SpaceAirMode spaceAirMode = target;
				if (spaceAirMode == SpaceAirMode.NoAir)
				{
					spaceAirLock.airDoor.door.Set(false);
					this.<>2__current = new WaitUntil(() => CS$<>8__locals1.<>4__this.airDoor.door.HasFinished());
					this.<>1__state = 5;
					return true;
				}
				if (spaceAirMode == SpaceAirMode.Air)
				{
					spaceAirLock.noAirDoor.door.Set(false);
					this.<>2__current = new WaitUntil(() => CS$<>8__locals1.<>4__this.noAirDoor.door.HasFinished());
					this.<>1__state = 1;
					return true;
				}
				goto IL_02E3;
			}
			case 1:
				this.<>1__state = -1;
				spaceAirLock.currentMode = target;
				if (bHasChanged)
				{
					spaceAirLock.networkObject.SendRPC(spaceAirLock.RPC_CLIENT_SET_HAS_AIR, true, 6, new object[] { true });
				}
				if (bHasChanged)
				{
					this.<>2__current = new WaitForSeconds(spaceAirLock.timeToFillWithAir);
					this.<>1__state = 2;
					return true;
				}
				break;
			case 2:
				this.<>1__state = -1;
				this.<>2__current = spaceAirLock.ApplyPlayersAirLock();
				this.<>1__state = 3;
				return true;
			case 3:
				this.<>1__state = -1;
				break;
			case 4:
				this.<>1__state = -1;
				goto IL_02E3;
			case 5:
				this.<>1__state = -1;
				spaceAirLock.currentMode = target;
				if (bHasChanged)
				{
					this.<>2__current = spaceAirLock.ApplyPlayersAirLock();
					this.<>1__state = 6;
					return true;
				}
				goto IL_0295;
			case 6:
				this.<>1__state = -1;
				spaceAirLock.networkObject.SendRPC(spaceAirLock.RPC_CLIENT_SET_HAS_AIR, true, 6, new object[] { false });
				this.<>2__current = new WaitForSeconds(spaceAirLock.timeToFillWithAir);
				this.<>1__state = 7;
				return true;
			case 7:
				this.<>1__state = -1;
				goto IL_0295;
			case 8:
				this.<>1__state = -1;
				goto IL_02E3;
			case 9:
				this.<>1__state = -1;
				goto IL_0330;
			case 10:
				this.<>1__state = -1;
				if (spaceAirLock.airLockCoroutine == null || spaceAirLock.insideAirLockTrigger.ContainsPlayers())
				{
					goto IL_0330;
				}
				if (CS$<>8__locals1.doorWhichWeOpened != null)
				{
					CS$<>8__locals1.doorWhichWeOpened.door.Set(false);
					this.<>2__current = new WaitUntil(() => CS$<>8__locals1.doorWhichWeOpened.door.HasFinished());
					this.<>1__state = 11;
					return true;
				}
				goto IL_042B;
			case 11:
				this.<>1__state = -1;
				if (spaceAirLock.airLockCoroutine == null || spaceAirLock.insideAirLockTrigger.ContainsPlayers())
				{
					CS$<>8__locals1.doorWhichWeOpened.door.Set(true);
					this.<>2__current = new WaitUntil(() => CS$<>8__locals1.doorWhichWeOpened.door.HasFinished());
					this.<>1__state = 12;
					return true;
				}
				goto IL_042B;
			case 12:
				this.<>1__state = -1;
				goto IL_0330;
			default:
				return false;
			}
			spaceAirLock.airDoor.door.Set(true);
			CS$<>8__locals1.doorWhichWeOpened = spaceAirLock.airDoor;
			this.<>2__current = new WaitUntil(() => CS$<>8__locals1.<>4__this.airDoor.door.HasFinished());
			this.<>1__state = 4;
			return true;
			IL_0295:
			spaceAirLock.noAirDoor.door.Set(true);
			CS$<>8__locals1.doorWhichWeOpened = spaceAirLock.noAirDoor;
			this.<>2__current = new WaitUntil(() => CS$<>8__locals1.<>4__this.noAirDoor.door.HasFinished());
			this.<>1__state = 8;
			return true;
			IL_02E3:
			if (spaceAirLock.touchButtons != null)
			{
				foreach (TouchButtonSet touchButtonSet2 in spaceAirLock.touchButtons)
				{
					if (touchButtonSet2)
					{
						touchButtonSet2.Set(true);
					}
				}
			}
			IL_0330:
			if (spaceAirLock.airLockCoroutine != null && !spaceAirLock.insideAirLockTrigger.ContainsPlayers())
			{
				this.<>2__current = new WaitForSeconds(3f);
				this.<>1__state = 10;
				return true;
			}
			this.<>2__current = null;
			this.<>1__state = 9;
			return true;
			IL_042B:
			spaceAirLock.airLockCoroutine = null;
			return false;
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060002DF RID: 735 RVA: 0x0000F1BC File Offset: 0x0000D3BC
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0000F1C4 File Offset: 0x0000D3C4
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x0000F1CB File Offset: 0x0000D3CB
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000253 RID: 595
		private int <>1__state;

		// Token: 0x04000254 RID: 596
		private object <>2__current;

		// Token: 0x04000255 RID: 597
		public SpaceAirLock <>4__this;

		// Token: 0x04000256 RID: 598
		public SpaceAirMode target;

		// Token: 0x04000257 RID: 599
		private SpaceAirLock.<>c__DisplayClass17_0 <>8__1;

		// Token: 0x04000258 RID: 600
		private bool <bHasChanged>5__2;
	}

	// Token: 0x02000067 RID: 103
	[CompilerGenerated]
	private sealed class <ApplyPlayersAirLock>d__18 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x060002E2 RID: 738 RVA: 0x0000F1D3 File Offset: 0x0000D3D3
		[DebuggerHidden]
		public <ApplyPlayersAirLock>d__18(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000F1E2 File Offset: 0x0000D3E2
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0000F1E4 File Offset: 0x0000D3E4
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpaceAirLock spaceAirLock = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = null;
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			foreach (PlayerCharacter playerCharacter in spaceAirLock.insideAirLockTrigger.GetOverlappingPlayers())
			{
				SpacePlayerCharacter component = playerCharacter.GetComponent<SpacePlayerCharacter>();
				if (component)
				{
					component.ServerSetAirMode(spaceAirLock.currentMode);
				}
			}
			return false;
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x0000F284 File Offset: 0x0000D484
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0000F28C File Offset: 0x0000D48C
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x0000F293 File Offset: 0x0000D493
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000259 RID: 601
		private int <>1__state;

		// Token: 0x0400025A RID: 602
		private object <>2__current;

		// Token: 0x0400025B RID: 603
		public SpaceAirLock <>4__this;
	}
}
