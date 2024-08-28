using System;
using FMOD.Studio;
using HawkNetworking;

// Token: 0x02000054 RID: 84
public class SpacePlayerCharacterSound : HawkNetworkSubBehaviour
{
	// Token: 0x06000278 RID: 632 RVA: 0x0000C9D8 File Offset: 0x0000ABD8
	public override void NetworkStart(HawkNetworkObject networkObject)
	{
		base.NetworkStart(networkObject);
		PlayerCharacterSound playerCharacterSound = base.GetComponent<PlayerCharacter>().GetPlayerCharacterSound();
		PlayerCharacterSound playerCharacterSound2 = playerCharacterSound;
		playerCharacterSound2.onPlayLandedOneStep = (Action<EventInstance>)Delegate.Combine(playerCharacterSound2.onPlayLandedOneStep, new Action<EventInstance>(this.OnPlayLandedOneStep));
		PlayerCharacterSound playerCharacterSound3 = playerCharacterSound;
		playerCharacterSound3.onPlayStepOneShot = (Action<EventInstance>)Delegate.Combine(playerCharacterSound3.onPlayStepOneShot, new Action<EventInstance>(this.OnPlayStepOneShot));
		this.step_IsMuffledParameter = playerCharacterSound.GetParameterID_StepSound("IsMuffled");
		this.landed_IsMuffledParameter = playerCharacterSound.GetParameterID_LandedSound("IsMuffled");
		this.spaceCharacter = base.GetComponent<SpacePlayerCharacter>();
	}

	// Token: 0x06000279 RID: 633 RVA: 0x0000CA6C File Offset: 0x0000AC6C
	private void OnPlayStepOneShot(EventInstance eventInstance)
	{
		SpaceAirMode? currentAirMode = this.spaceCharacter.GetCurrentAirMode();
		if (currentAirMode != null && currentAirMode.Value == SpaceAirMode.NoAir)
		{
			eventInstance.setParameterByID(this.step_IsMuffledParameter, 1f, false);
		}
	}

	// Token: 0x0600027A RID: 634 RVA: 0x0000CAAC File Offset: 0x0000ACAC
	private void OnPlayLandedOneStep(EventInstance eventInstance)
	{
		SpaceAirMode? currentAirMode = this.spaceCharacter.GetCurrentAirMode();
		if (currentAirMode != null && currentAirMode.Value == SpaceAirMode.NoAir)
		{
			eventInstance.setParameterByID(this.landed_IsMuffledParameter, 1f, false);
		}
	}

	// Token: 0x0600027B RID: 635 RVA: 0x0000CAEB File Offset: 0x0000ACEB
	public SpacePlayerCharacterSound()
	{
	}

	// Token: 0x040001F7 RID: 503
	private PARAMETER_ID landed_IsMuffledParameter;

	// Token: 0x040001F8 RID: 504
	private PARAMETER_ID step_IsMuffledParameter;

	// Token: 0x040001F9 RID: 505
	private SpacePlayerCharacter spaceCharacter;
}
