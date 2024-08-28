using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class SpacePlayerCharacterSpawnPoint : PlayerCharacterSpawnPoint
{
	// Token: 0x060001D2 RID: 466 RVA: 0x000097F4 File Offset: 0x000079F4
	public override void OnPlayerCharacterSpawned(PlayerCharacter playerCharacter)
	{
		base.OnPlayerCharacterSpawned(playerCharacter);
		SpacePlayerCharacter component = playerCharacter.GetComponent<SpacePlayerCharacter>();
		if (component)
		{
			component.ServerSetAirMode(this.airMode);
		}
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x00009823 File Offset: 0x00007A23
	public SpacePlayerCharacterSpawnPoint()
	{
	}

	// Token: 0x0400017E RID: 382
	[SerializeField]
	private SpaceAirMode airMode;
}
