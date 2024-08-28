using System;
using UnityEngine;

// Token: 0x0200000D RID: 13
[Serializable]
internal struct SpaceMechanicWhiteboard
{
	// Token: 0x0600005D RID: 93 RVA: 0x00003A48 File Offset: 0x00001C48
	public void UpdateWhiteboard(SpaceMechanicJobWhiteBoardData data)
	{
		Guid guid;
		if (Guid.TryParse(this.whiteboardGUIDStr, out guid))
		{
			GUIDComponent firstGUIDComponent = UnitySingleton<GUIDComponentManager>.Instance.GetFirstGUIDComponent(guid);
			if (firstGUIDComponent)
			{
				SpaceMechanicJobWhiteBoard component = firstGUIDComponent.GetComponent<SpaceMechanicJobWhiteBoard>();
				if (component)
				{
					component.ServerSendWhiteBoardData(data);
				}
			}
		}
	}

	// Token: 0x0600005E RID: 94 RVA: 0x00003A90 File Offset: 0x00001C90
	public void AssignCallback(Action<GUIDComponent> callback)
	{
		Guid guid;
		if (Guid.TryParse(this.whiteboardGUIDStr, out guid))
		{
			UnitySingleton<GUIDComponentManager>.Instance.AddAssignedCallback(guid, callback);
		}
	}

	// Token: 0x0600005F RID: 95 RVA: 0x00003AB8 File Offset: 0x00001CB8
	public void UnassignCallback(Action<GUIDComponent> callback)
	{
		Guid guid;
		if (UnitySingleton<GUIDComponentManager>.InstanceExists && Guid.TryParse(this.whiteboardGUIDStr, out guid))
		{
			UnitySingleton<GUIDComponentManager>.GetRawInstance().RemoveAssignedCallback(guid, callback);
		}
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00003AE7 File Offset: 0x00001CE7
	public bool IsInBay(SpaceMechanicBays baysMask)
	{
		return (this.bay & baysMask) > (SpaceMechanicBays)0;
	}

	// Token: 0x04000053 RID: 83
	[SerializeField]
	private SpaceMechanicBays bay;

	// Token: 0x04000054 RID: 84
	[SerializeField]
	private string whiteboardGUIDStr;
}
