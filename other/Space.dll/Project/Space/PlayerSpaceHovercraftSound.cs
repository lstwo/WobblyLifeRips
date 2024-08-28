using System;
using System.Runtime.CompilerServices;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class PlayerSpaceHovercraftSound : PlayerVehicleSound
{
	// Token: 0x060001E7 RID: 487 RVA: 0x00009D9E File Offset: 0x00007F9E
	private void OnDestroy()
	{
		if (this.engineInstance.isValid())
		{
			this.engineInstance.stop(1);
			this.engineInstance.release();
		}
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x00009DC8 File Offset: 0x00007FC8
	public void SetEngineOn(bool bEngineOn)
	{
		if (bEngineOn)
		{
			if (!this.engineInstance.isValid() && !string.IsNullOrEmpty(this.engineSound))
			{
				this.engineInstance = base.CreateAudioEvent(this.engineSound, true, true, true);
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

	// Token: 0x060001E9 RID: 489 RVA: 0x00009E44 File Offset: 0x00008044
	public void SetVehicleSpeed(float speed)
	{
		if (this.engineInstance.isValid())
		{
			this.engineInstance.setParameterByName("VehicleSpeed", speed, false);
		}
	}

	// Token: 0x060001EA RID: 490 RVA: 0x00009E68 File Offset: 0x00008068
	internal void PlayCrash(float vehicleSpeed)
	{
		if (!string.IsNullOrEmpty(this.crashSound))
		{
			base.PlayOneShot(this.crashSound, true, true, false, delegate(EventInstance x)
			{
				x.setParameterByName("VehicleSpeed", vehicleSpeed, false);
			});
		}
	}

	// Token: 0x060001EB RID: 491 RVA: 0x00009EAA File Offset: 0x000080AA
	public PlayerSpaceHovercraftSound()
	{
	}

	// Token: 0x04000192 RID: 402
	[SerializeField]
	[EventRef]
	private string engineSound = "event:/Vehicles_Space/LandVehicles/Engine_HoverBaggageVehicle";

	// Token: 0x04000193 RID: 403
	[SerializeField]
	[EventRef]
	private string crashSound = "event:/Impacts/Impacts_CarCrash";

	// Token: 0x04000194 RID: 404
	private EventInstance engineInstance;

	// Token: 0x02000079 RID: 121
	[CompilerGenerated]
	private sealed class <>c__DisplayClass6_0
	{
		// Token: 0x06000330 RID: 816 RVA: 0x00010362 File Offset: 0x0000E562
		public <>c__DisplayClass6_0()
		{
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0001036A File Offset: 0x0000E56A
		internal void <PlayCrash>b__0(EventInstance x)
		{
			x.setParameterByName("VehicleSpeed", this.vehicleSpeed, false);
		}

		// Token: 0x04000297 RID: 663
		public float vehicleSpeed;
	}
}
