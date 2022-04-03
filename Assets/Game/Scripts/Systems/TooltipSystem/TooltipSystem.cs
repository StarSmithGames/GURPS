using Cinemachine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class TooltipSystem : MonoBehaviour
{
    public bool IsShowing { get; private set; }

	[field: SerializeField] public TMPro.TextMeshProUGUI Ruler { get; private set; }

	private RectTransform ruler;

	private CinemachineBrain brain;

	[Inject]
	private void Construct(CinemachineBrain brain)
	{
		this.brain = brain;
	}

	private void Update()
	{
		if (IsShowing)
		{
			Vector2 position = Input.mousePosition;
			position += OffsetRightDown(Ruler.rectTransform);

			Ruler.rectTransform.anchoredPosition = position;
		}
	}

	public void EnableRuler(bool trigger)
	{
		IsShowing = trigger;
		Ruler.gameObject.SetActive(trigger);
	}

	public void SetRulerText(string text)
	{
		Ruler.text = text;
	}

	private Vector2 OffsetRightDown(RectTransform rectTransform) => new Vector2((rectTransform.sizeDelta.x / 2) * 1.5f, -(rectTransform.sizeDelta.y / 2) * 2.5f);
}