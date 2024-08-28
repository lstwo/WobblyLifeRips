using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FMOD.Studio;
using FMODUnity;
using HawkNetworking;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class SpaceFuelPump : HawkNetworkSubBehaviour
{
	// Token: 0x06000110 RID: 272 RVA: 0x000066EA File Offset: 0x000048EA
	public override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_SET_FUEL_AMOUNT = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientSetFuelAmount), 1);
	}

	// Token: 0x06000111 RID: 273 RVA: 0x0000670C File Offset: 0x0000490C
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		this.OnHoseConnectedChanged(this.hose, this.hose.IsConnected());
		SpaceFuelHose spaceFuelHose = this.hose;
		spaceFuelHose.onConnectedCallback = (SpaceFuelHose.SpaceFuelHoseConnectedCallback)Delegate.Combine(spaceFuelHose.onConnectedCallback, new SpaceFuelHose.SpaceFuelHoseConnectedCallback(this.OnHoseConnectedChanged));
		this.pumpHandle.onPumpCallback += this.OnPumpCallback;
	}

	// Token: 0x06000112 RID: 274 RVA: 0x00006775 File Offset: 0x00004975
	private void OnDestroy()
	{
		if (this.pumpingInstance.isValid())
		{
			this.pumpingInstance.stop(1);
			this.pumpingInstance.release();
		}
	}

	// Token: 0x06000113 RID: 275 RVA: 0x000067A0 File Offset: 0x000049A0
	private void OnHoseConnectedChanged(SpaceFuelHose hose, bool bConnected)
	{
		this.showWhenConnected.gameObject.SetActive(bConnected);
		this.showWhenDisconnected.gameObject.SetActive(!bConnected);
		if (this.networkObject.IsServer())
		{
			this.ServerUpdateFuelAmount(0f);
		}
		this.CheckHandle();
	}

	// Token: 0x06000114 RID: 276 RVA: 0x000067F0 File Offset: 0x000049F0
	private void CheckHandle()
	{
		this.pumpHandle.ServerSetAllowedToUse(this.hose.GetConnectedConnector() && this.currentFuel < 1f);
	}

	// Token: 0x06000115 RID: 277 RVA: 0x00006820 File Offset: 0x00004A20
	private void OnPumpCallback(bool bPumpedIn)
	{
		if (this.networkObject.IsServer() && this.hose && this.hose.GetConnectedConnector() && this.currentFuel < 1f)
		{
			this.currentFuel += this.pumpIncrements;
			this.currentFuel = Mathf.Clamp01(this.currentFuel);
			if (this.currentFuel >= 1f)
			{
				this.CheckHandle();
				if (this.serverUnHookPumpCoroutine != null)
				{
					base.StopCoroutine(this.serverUnHookPumpCoroutine);
				}
				this.serverUnHookPumpCoroutine = base.StartCoroutine(this.ServerUnhookPumpDelay());
			}
			this.ServerUpdateFuelAmount(this.currentFuel);
		}
	}

	// Token: 0x06000116 RID: 278 RVA: 0x000068D5 File Offset: 0x00004AD5
	private IEnumerator PumpingSound()
	{
		if (!this.pumpingInstance.isValid() && !string.IsNullOrEmpty(this.pumpingSound))
		{
			this.pumpingInstance = RuntimeManager.CreateInstance(this.pumpingSound);
			this.pumpingInstance.set3DAttributes(RuntimeUtils.To3DAttributes(base.transform.position));
			this.pumpingInstance.start();
		}
		while (this.pumpSeconds > 0f)
		{
			yield return null;
			this.pumpSeconds -= Time.deltaTime;
		}
		if (this.pumpingInstance.isValid())
		{
			this.pumpingInstance.stop(0);
			this.pumpingInstance.release();
			this.pumpingInstance.clearHandle();
		}
		this.pumpSoundCoroutine = null;
		yield break;
	}

	// Token: 0x06000117 RID: 279 RVA: 0x000068E4 File Offset: 0x00004AE4
	private IEnumerator ServerUnhookPumpDelay()
	{
		yield return new WaitForSeconds(1f);
		if (this.hose)
		{
			SpaceFuelHoseConnector connectedConnector = this.hose.GetConnectedConnector();
			if (connectedConnector)
			{
				connectedConnector.RefuelShip();
			}
			this.hose.ServerSetConnector(null);
		}
		yield break;
	}

	// Token: 0x06000118 RID: 280 RVA: 0x000068F4 File Offset: 0x00004AF4
	private void OnFuelAmountChanged(float currentFuel)
	{
		if (this.fuelIndicator)
		{
			this.fuelIndicator.gameObject.SetActive(currentFuel > 0f);
			this.fuelIndicator.transform.localScale = new Vector3(1f, currentFuel, 1f);
		}
	}

	// Token: 0x06000119 RID: 281 RVA: 0x00006948 File Offset: 0x00004B48
	private void ServerUpdateFuelAmount(float currentFuel)
	{
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		this.networkObject.SendRPC(this.RPC_CLIENT_SET_FUEL_AMOUNT, true, 6, new object[] { currentFuel, true });
	}

	// Token: 0x0600011A RID: 282 RVA: 0x00006998 File Offset: 0x00004B98
	private void ClientSetFuelAmount(HawkNetReader reader, HawkRPCInfo info)
	{
		this.currentFuel = reader.ReadSingle();
		this.OnFuelAmountChanged(this.currentFuel);
		if (this.currentFuel <= 0f)
		{
			if (this.pumpingInstance.isValid())
			{
				this.pumpingInstance.stop(1);
				this.pumpingInstance.release();
				this.pumpingInstance.clearHandle();
			}
			if (this.pumpSoundCoroutine != null)
			{
				base.StopCoroutine(this.pumpSoundCoroutine);
				this.pumpSoundCoroutine = null;
				return;
			}
		}
		else
		{
			this.pumpSeconds += 1f;
			if (this.pumpSoundCoroutine == null)
			{
				this.pumpSoundCoroutine = base.StartCoroutine(this.PumpingSound());
			}
		}
	}

	// Token: 0x0600011B RID: 283 RVA: 0x00006A43 File Offset: 0x00004C43
	public SpaceFuelPump()
	{
	}

	// Token: 0x040000E9 RID: 233
	private byte RPC_CLIENT_SET_FUEL_AMOUNT;

	// Token: 0x040000EA RID: 234
	[SerializeField]
	private SpaceFuelHose hose;

	// Token: 0x040000EB RID: 235
	[SerializeField]
	private GameObject showWhenConnected;

	// Token: 0x040000EC RID: 236
	[SerializeField]
	private GameObject showWhenDisconnected;

	// Token: 0x040000ED RID: 237
	[SerializeField]
	private SpaceFuelPumpPump pumpHandle;

	// Token: 0x040000EE RID: 238
	[SerializeField]
	private float pumpIncrements = 0.25f;

	// Token: 0x040000EF RID: 239
	[SerializeField]
	private GameObject fuelIndicator;

	// Token: 0x040000F0 RID: 240
	[SerializeField]
	[EventRef]
	private string pumpingSound = "event:/Objects_Space/SpaceMechanicJob/Objects_Space_SpaceMechanicJob_FuelMachine_Pumping";

	// Token: 0x040000F1 RID: 241
	private float currentFuel;

	// Token: 0x040000F2 RID: 242
	private Coroutine serverUnHookPumpCoroutine;

	// Token: 0x040000F3 RID: 243
	private EventInstance pumpingInstance;

	// Token: 0x040000F4 RID: 244
	private float pumpSeconds;

	// Token: 0x040000F5 RID: 245
	private Coroutine pumpSoundCoroutine;

	// Token: 0x0200006B RID: 107
	[CompilerGenerated]
	private sealed class <PumpingSound>d__19 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x060002F4 RID: 756 RVA: 0x0000F394 File Offset: 0x0000D594
		[DebuggerHidden]
		public <PumpingSound>d__19(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000F3A3 File Offset: 0x0000D5A3
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0000F3A8 File Offset: 0x0000D5A8
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpaceFuelPump spaceFuelPump = this;
			if (num != 0)
			{
				if (num != 1)
				{
					return false;
				}
				this.<>1__state = -1;
				spaceFuelPump.pumpSeconds -= Time.deltaTime;
			}
			else
			{
				this.<>1__state = -1;
				if (!spaceFuelPump.pumpingInstance.isValid() && !string.IsNullOrEmpty(spaceFuelPump.pumpingSound))
				{
					spaceFuelPump.pumpingInstance = RuntimeManager.CreateInstance(spaceFuelPump.pumpingSound);
					spaceFuelPump.pumpingInstance.set3DAttributes(RuntimeUtils.To3DAttributes(spaceFuelPump.transform.position));
					spaceFuelPump.pumpingInstance.start();
				}
			}
			if (spaceFuelPump.pumpSeconds <= 0f)
			{
				if (spaceFuelPump.pumpingInstance.isValid())
				{
					spaceFuelPump.pumpingInstance.stop(0);
					spaceFuelPump.pumpingInstance.release();
					spaceFuelPump.pumpingInstance.clearHandle();
				}
				spaceFuelPump.pumpSoundCoroutine = null;
				return false;
			}
			this.<>2__current = null;
			this.<>1__state = 1;
			return true;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x0000F497 File Offset: 0x0000D697
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0000F49F File Offset: 0x0000D69F
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x0000F4A6 File Offset: 0x0000D6A6
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000261 RID: 609
		private int <>1__state;

		// Token: 0x04000262 RID: 610
		private object <>2__current;

		// Token: 0x04000263 RID: 611
		public SpaceFuelPump <>4__this;
	}

	// Token: 0x0200006C RID: 108
	[CompilerGenerated]
	private sealed class <ServerUnhookPumpDelay>d__20 : IEnumerator<object>, IEnumerator, IDisposable
	{
		// Token: 0x060002FA RID: 762 RVA: 0x0000F4AE File Offset: 0x0000D6AE
		[DebuggerHidden]
		public <ServerUnhookPumpDelay>d__20(int <>1__state)
		{
			this.<>1__state = <>1__state;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0000F4BD File Offset: 0x0000D6BD
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0000F4C0 File Offset: 0x0000D6C0
		bool IEnumerator.MoveNext()
		{
			int num = this.<>1__state;
			SpaceFuelPump spaceFuelPump = this;
			if (num == 0)
			{
				this.<>1__state = -1;
				this.<>2__current = new WaitForSeconds(1f);
				this.<>1__state = 1;
				return true;
			}
			if (num != 1)
			{
				return false;
			}
			this.<>1__state = -1;
			if (spaceFuelPump.hose)
			{
				SpaceFuelHoseConnector connectedConnector = spaceFuelPump.hose.GetConnectedConnector();
				if (connectedConnector)
				{
					connectedConnector.RefuelShip();
				}
				spaceFuelPump.hose.ServerSetConnector(null);
			}
			return false;
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060002FD RID: 765 RVA: 0x0000F53F File Offset: 0x0000D73F
		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0000F547 File Offset: 0x0000D747
		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060002FF RID: 767 RVA: 0x0000F54E File Offset: 0x0000D74E
		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.<>2__current;
			}
		}

		// Token: 0x04000264 RID: 612
		private int <>1__state;

		// Token: 0x04000265 RID: 613
		private object <>2__current;

		// Token: 0x04000266 RID: 614
		public SpaceFuelPump <>4__this;
	}
}
