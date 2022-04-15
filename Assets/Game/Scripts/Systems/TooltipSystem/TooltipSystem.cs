using Cinemachine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.TooltipSystem
{
	public class TooltipSystem : MonoBehaviour
	{
		public bool IsRulerShowing { get; private set; }
		public bool IsMessageShowing { get; private set; }

		[field: SerializeField] public RectTransform Tooltip { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Ruler { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Message { get; private set; }

		private RectTransform ruler;

		private CinemachineBrain brain;

		[Inject]
		private void Construct(CinemachineBrain brain)
		{
			this.brain = brain;
		}

		private void Start()
		{
			EnableRuler(false);
			EnableMessage(false);
		}

		private void Update()
		{
			if (Ruler.gameObject.activeSelf || Message.gameObject.activeSelf)
			{
				Vector2 position = Input.mousePosition;
				position += OffsetRightDown(Tooltip);

				Tooltip.anchoredPosition = position;
			}
		}

		public void EnableRuler(bool trigger)
		{
			IsRulerShowing = trigger;
			Ruler.gameObject.SetActive(trigger);
		}

		public void EnableMessage(bool trigger)
		{
			IsMessageShowing = trigger;
			Message.gameObject.SetActive(trigger);
		}

		public void SetRulerText(string text)
		{
			Ruler.text = text;
		}

		public void SetMessage(TooltipMessageType type)
		{
			switch (type)
			{
				case TooltipMessageType.InvalidTarget:
				{
					Message.text = "Invalid Target";
					Message.color = Color.red;
					break;
				}
				case TooltipMessageType.NotEnoughMovement:
				{
					Message.text = "Not Enough Movement";
					Message.color = Color.yellow;
					break;
				}
				case TooltipMessageType.CanNotReachDesination:
				{
					Message.text = "Can't Reach Destination";
					Message.color = Color.white;
					break;
				}
				case TooltipMessageType.OutOfRange:
				{
					Message.text = "OutOfRange";
					Message.color = Color.red;
					break;
				}
			}
		}

		private Vector2 OffsetRightDown(RectTransform rectTransform) => new Vector2((rectTransform.sizeDelta.x / 2) * 1.5f, -(rectTransform.sizeDelta.y / 2) * 2.5f);
	}

	public enum TooltipMessageType
	{
		InvalidTarget,
		NotEnoughMovement,
		CanNotReachDesination,
		OutOfRange,
	}
}