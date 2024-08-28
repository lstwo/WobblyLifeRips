using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x0200005A RID: 90
public class AsteroidDefenceUI : UIPlayerBasedCanvas
{
	// Token: 0x060002B2 RID: 690 RVA: 0x0000E3BE File Offset: 0x0000C5BE
	protected override void Awake()
	{
		base.Awake();
		this.myRectTransform = base.GetComponent<RectTransform>();
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x0000E3D2 File Offset: 0x0000C5D2
	internal void Show(AsteroidDefenceJobMission jobMission)
	{
		this.jobMission = jobMission;
		base.Show();
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x0000E3E1 File Offset: 0x0000C5E1
	private void Update()
	{
		if (this.jobMission)
		{
			this.UpdateArrows();
		}
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x0000E3F8 File Offset: 0x0000C5F8
	private void UpdateArrows()
	{
		List<AsteroidDefenceAsteroid> allAsteroids = this.jobMission.GetAllAsteroids();
		int num = 0;
		PlayerController playerController = base.GetPlayerController();
		if (!playerController)
		{
			return;
		}
		GameplayCamera gameplayCamera = playerController.GetGameplayCamera();
		if (!gameplayCamera)
		{
			return;
		}
		foreach (AsteroidDefenceAsteroid asteroidDefenceAsteroid in allAsteroids)
		{
			if (gameplayCamera.GetUICamera())
			{
				Camera camera = gameplayCamera.GetCamera();
				if (camera && asteroidDefenceAsteroid.gameObject.activeInHierarchy)
				{
					Vector3 position = asteroidDefenceAsteroid.transform.position;
					Vector2 vector = camera.WorldToScreenPoint(position);
					Vector2 vector2;
					if (RectTransformUtility.ScreenPointToLocalPointInRectangle(gameplayCamera.GetCanvasRectTransform(), vector, camera, ref vector2) && !this.myRectTransform.rect.Contains(vector2))
					{
						RectTransform rectTransform = null;
						if (this.arrowPool.Count > num)
						{
							rectTransform = this.arrowPool[num];
							rectTransform.gameObject.SetActive(true);
						}
						else if (this.arrowPrefab)
						{
							rectTransform = Object.Instantiate<RectTransform>(this.arrowPrefab, base.transform, false);
							this.arrowPool.Add(rectTransform);
						}
						if (rectTransform)
						{
							this.UpdateArrow(rectTransform, asteroidDefenceAsteroid, position, vector2);
						}
						num++;
					}
				}
			}
		}
		for (int i = num; i < this.arrowPool.Count; i++)
		{
			this.arrowPool[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x0000E5AC File Offset: 0x0000C7AC
	private void UpdateArrow(RectTransform arrow, AsteroidDefenceAsteroid asteroid, Vector3 asteroidPosition, Vector2 asteroidPositionLocalPoint)
	{
		Vector2 vector = asteroidPositionLocalPoint;
		float num = this.myRectTransform.rect.xMin + this.arrowPrefab.rect.width;
		float num2 = this.myRectTransform.rect.xMax - this.arrowPrefab.rect.width;
		float num3 = this.myRectTransform.rect.yMin + this.arrowPrefab.rect.height;
		float num4 = this.myRectTransform.rect.yMax - this.arrowPrefab.rect.height;
		vector.x = Mathf.Clamp(vector.x, num, num2);
		vector.y = Mathf.Clamp(vector.y, num3, num4);
		arrow.anchoredPosition = vector;
		float num5 = 57.29578f * Mathf.Atan2(asteroidPositionLocalPoint.y - vector.y, asteroidPositionLocalPoint.x - vector.x);
		arrow.transform.localEulerAngles = new Vector3(0f, 0f, num5 + 90f);
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0000E6E0 File Offset: 0x0000C8E0
	internal void SetWave(sbyte waveNumber, LocalizedString waveText)
	{
		if (this.currentWave == waveNumber)
		{
			return;
		}
		this.currentWave = waveNumber;
		if (this.waveAnimator)
		{
			this.waveAnimator.SetTrigger("tWave");
		}
		if (this.waveText)
		{
			this.waveText.text = waveText.GetLocalizedString();
		}
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0000E739 File Offset: 0x0000C939
	public AsteroidDefenceUI()
	{
	}

	// Token: 0x04000229 RID: 553
	[SerializeField]
	private RectTransform arrowPrefab;

	// Token: 0x0400022A RID: 554
	[SerializeField]
	private TextMeshProUGUI waveText;

	// Token: 0x0400022B RID: 555
	[SerializeField]
	private Animator waveAnimator;

	// Token: 0x0400022C RID: 556
	private AsteroidDefenceJobMission jobMission;

	// Token: 0x0400022D RID: 557
	private List<RectTransform> arrowPool = new List<RectTransform>();

	// Token: 0x0400022E RID: 558
	private RectTransform myRectTransform;

	// Token: 0x0400022F RID: 559
	private sbyte currentWave = -1;
}
