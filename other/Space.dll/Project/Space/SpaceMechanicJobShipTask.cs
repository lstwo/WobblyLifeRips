using System;
using HawkNetworking;
using UnityEngine;

// Token: 0x0200001B RID: 27
internal abstract class SpaceMechanicJobShipTask : HawkNetworkSubBehaviour
{
	// Token: 0x060000D1 RID: 209 RVA: 0x00005A7E File Offset: 0x00003C7E
	protected virtual void OnDestroy()
	{
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00005A80 File Offset: 0x00003C80
	public void CompleteTask(bool bCompleteSubSets = true)
	{
		SpaceMechanicJobShip component = base.GetComponent<SpaceMechanicJobShip>();
		if (component)
		{
			component.TaskCompleted(this, bCompleteSubSets);
		}
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00005AA4 File Offset: 0x00003CA4
	public virtual void OnServerShipPreLanded()
	{
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00005AA6 File Offset: 0x00003CA6
	public virtual void OnServerShipLanded()
	{
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00005AA8 File Offset: 0x00003CA8
	public virtual void OnServerShipTakenOff()
	{
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00005AAA File Offset: 0x00003CAA
	public virtual void OnServerShipFlownAway()
	{
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00005AAC File Offset: 0x00003CAC
	protected virtual void OnInitalizeWhiteboard()
	{
	}

	// Token: 0x060000D8 RID: 216
	public abstract bool IsTaskComplete();

	// Token: 0x060000D9 RID: 217 RVA: 0x00005AAE File Offset: 0x00003CAE
	public bool IsTaskEnabled()
	{
		return this.bEnabled;
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00005AB6 File Offset: 0x00003CB6
	protected void ServerUpdateWhiteboardData()
	{
		Action action = this.serverSendWhiteboardDataCallback;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00005AC8 File Offset: 0x00003CC8
	internal void SetWhiteboardData(SpaceMechanicJobWhiteBoardData whiteBoardData, Action serverSendWhiteboardDataCallback)
	{
		this.whiteBoardData = whiteBoardData;
		this.serverSendWhiteboardDataCallback = serverSendWhiteboardDataCallback;
		this.OnInitalizeWhiteboard();
	}

	// Token: 0x060000DC RID: 220 RVA: 0x00005ADE File Offset: 0x00003CDE
	protected SpaceMechanicJobShipTask()
	{
	}

	// Token: 0x040000A7 RID: 167
	[SerializeField]
	private bool bEnabled = true;

	// Token: 0x040000A8 RID: 168
	protected SpaceMechanicJobWhiteBoardData whiteBoardData;

	// Token: 0x040000A9 RID: 169
	private Action serverSendWhiteboardDataCallback;
}
