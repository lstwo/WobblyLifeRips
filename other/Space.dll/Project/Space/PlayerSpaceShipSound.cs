using System;
using System.Runtime.CompilerServices;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class PlayerSpaceShipSound : PlayerVehicleSound
{
	// Token: 0x06000244 RID: 580 RVA: 0x0000BE8C File Offset: 0x0000A08C
	private void OnDestroy()
	{
		if (this.engineInstance.isValid())
		{
			this.engineInstance.stop(1);
			this.engineInstance.release();
		}
		if (this.boostInstance.isValid())
		{
			this.boostInstance.stop(1);
			this.boostInstance.release();
		}
	}

	// Token: 0x06000245 RID: 581 RVA: 0x0000BEE8 File Offset: 0x0000A0E8
	public void PlayCrash(Vector3 point, float vehicleSpeed)
	{
		if (!string.IsNullOrEmpty(this.crashSound))
		{
			base.PlayOneShotPointAttached(this.crashSound, true, true, point, delegate(EventInstance x)
			{
				x.setParameterByName("VehicleSpeed", vehicleSpeed, false);
			});
		}
	}

	// Token: 0x06000246 RID: 582 RVA: 0x0000BF2C File Offset: 0x0000A12C
	public void SetEngineOn(bool bEngineOn, bool bBypassEngineIgnition = false)
	{
		if (bEngineOn)
		{
			if (!this.engineInstance.isValid() && !string.IsNullOrEmpty(this.engineSound))
			{
				this.engineInstance = base.CreateAudioEvent(this.engineSound, true, true, true);
				if (bBypassEngineIgnition)
				{
					this.engineInstance.setParameterByName("IsEngineOn", Convert.ToSingle(bBypassEngineIgnition), false);
				}
				this.engineInstance.start();
				return;
			}
		}
		else if (this.engineInstance.isValid())
		{
			base.DestroyAudioEvent_TriggerCue(this.engineInstance);
			this.engineInstance.release();
			this.engineInstance.clearHandle();
		}
	}

	// Token: 0x06000247 RID: 583 RVA: 0x0000BFC3 File Offset: 0x0000A1C3
	public void SetAccelerating(bool bAccelerating)
	{
		if (this.engineInstance.isValid())
		{
			this.engineInstance.setParameterByName("EngineLoad", Convert.ToSingle(bAccelerating), false);
		}
	}

	// Token: 0x06000248 RID: 584 RVA: 0x0000BFEA File Offset: 0x0000A1EA
	public void SetVehicleSpeed(float speed)
	{
		if (this.engineInstance.isValid())
		{
			this.engineInstance.setParameterByName("VehicleSpeed", speed, false);
		}
	}

	// Token: 0x06000249 RID: 585 RVA: 0x0000C00C File Offset: 0x0000A20C
	public void SetBoostOn(bool bBoostOn)
	{
		if (bBoostOn)
		{
			if (!this.boostInstance.isValid() && !string.IsNullOrEmpty(this.boostSound))
			{
				this.boostInstance = base.CreateAudioEvent(this.boostSound, true, true, true);
				this.boostInstance.start();
			}
		}
		else if (this.boostInstance.isValid())
		{
			base.DestroyAudioEvent_Stop(this.boostInstance, 0);
			this.boostInstance.release();
			this.boostInstance.clearHandle();
		}
		this.engineInstance.setParameterByName("IsBoosting", Convert.ToSingle(bBoostOn), false);
	}

	// Token: 0x0600024A RID: 586 RVA: 0x0000C0A2 File Offset: 0x0000A2A2
	public PlayerSpaceShipSound()
	{
	}

	// Token: 0x040001DF RID: 479
	[SerializeField]
	[EventRef]
	private string engineSound = "event:/Vehicles_Space/Spaceships/Engine_Spaceship_Speeder";

	// Token: 0x040001E0 RID: 480
	[SerializeField]
	[EventRef]
	private string boostSound = "event:/Vehicles_Space/Spaceships/Spaceship_Speeder_Boost";

	// Token: 0x040001E1 RID: 481
	[SerializeField]
	[EventRef]
	private string crashSound = "event:/Impacts/Impacts_Spaceship_Small";

	// Token: 0x040001E2 RID: 482
	private EventInstance engineInstance;

	// Token: 0x040001E3 RID: 483
	private EventInstance boostInstance;

	// Token: 0x02000080 RID: 128
	[CompilerGenerated]
	private sealed class <>c__DisplayClass6_0
	{
		// Token: 0x0600034B RID: 843 RVA: 0x000106E4 File Offset: 0x0000E8E4
		public <>c__DisplayClass6_0()
		{
		}

		// Token: 0x0600034C RID: 844 RVA: 0x000106EC File Offset: 0x0000E8EC
		internal void <PlayCrash>b__0(EventInstance x)
		{
			x.setParameterByName("VehicleSpeed", this.vehicleSpeed, false);
		}

		// Token: 0x040002A7 RID: 679
		public float vehicleSpeed;
	}
}
