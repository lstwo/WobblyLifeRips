using System;
using HawkNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x0200001E RID: 30
internal class SpaceMechanicJobWhiteBoard : HawkNetworkBehaviour
{
	// Token: 0x060000E6 RID: 230 RVA: 0x00005C25 File Offset: 0x00003E25
	protected override void RegisterRPCs(HawkNetworkObject networkObject)
	{
		base.RegisterRPCs(networkObject);
		this.RPC_CLIENT_UPDATE_WHITEBOARD = networkObject.RegisterRPC(new HawkNetworkObject.RPCCallback(this.ClientUpdateWhiteboard), 1);
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00005C48 File Offset: 0x00003E48
	private void UpdateWhiteBoard()
	{
		this.partsFixedText.text = this.partsFixedLocalized.GetLocalizedString(new object[]
		{
			this.data.partsFixed,
			this.data.partsFixedOutOf
		});
		this.refuelShipText.text = this.refuelShipLocalized.GetLocalizedString();
		this.refuelShip_Done.SetActive(this.data.bRefuelShip);
		this.refuelShip_NotDone.SetActive(!this.data.bRefuelShip);
		this.shipWashedText.text = this.shipWashedLocalized.GetLocalizedString(new object[] { this.data.washedShipPercent });
		if (this.animator)
		{
			this.animator.SetBool("bOpen", this.data.bOpen);
		}
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00005D34 File Offset: 0x00003F34
	public void ServerSendWhiteBoardData(SpaceMechanicJobWhiteBoardData data)
	{
		if (this.networkObject == null || !this.networkObject.IsServer())
		{
			return;
		}
		if (data == null)
		{
			this.data.bOpen = false;
		}
		else
		{
			this.data = data;
		}
		this.networkObject.SendRPC(this.RPC_CLIENT_UPDATE_WHITEBOARD, true, 7, new object[] { SerializerDeserializerExtensions.SerializeNonAlloc(this.data) });
		this.UpdateWhiteBoard();
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00005DA1 File Offset: 0x00003FA1
	private void ClientUpdateWhiteboard(HawkNetReader reader, HawkRPCInfo info)
	{
		SerializerDeserializerExtensions.Deserialize<SpaceMechanicJobWhiteBoardData>(this.data, reader.ReadBytesAndSize());
		this.UpdateWhiteBoard();
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00005DBB File Offset: 0x00003FBB
	public SpaceMechanicJobWhiteBoardData GetWhiteBoardData()
	{
		return this.data;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00005DC3 File Offset: 0x00003FC3
	public SpaceMechanicJobWhiteBoard()
	{
	}

	// Token: 0x040000B3 RID: 179
	private byte RPC_CLIENT_UPDATE_WHITEBOARD;

	// Token: 0x040000B4 RID: 180
	[SerializeField]
	private Animator animator;

	// Token: 0x040000B5 RID: 181
	[SerializeField]
	private LocalizedString partsFixedLocalized;

	// Token: 0x040000B6 RID: 182
	[SerializeField]
	private LocalizedString refuelShipLocalized;

	// Token: 0x040000B7 RID: 183
	[SerializeField]
	private LocalizedString shipWashedLocalized;

	// Token: 0x040000B8 RID: 184
	[SerializeField]
	private TextMeshProUGUI partsFixedText;

	// Token: 0x040000B9 RID: 185
	[SerializeField]
	private TextMeshProUGUI refuelShipText;

	// Token: 0x040000BA RID: 186
	[SerializeField]
	private TextMeshProUGUI shipWashedText;

	// Token: 0x040000BB RID: 187
	[SerializeField]
	private GameObject refuelShip_Done;

	// Token: 0x040000BC RID: 188
	[SerializeField]
	private GameObject refuelShip_NotDone;

	// Token: 0x040000BD RID: 189
	private SpaceMechanicJobWhiteBoardData data = new SpaceMechanicJobWhiteBoardData();
}
