using Game.UI.Windows;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.TooltipSystem
{
	public class UIBattleTooltip : WindowBase
	{
		[field: SerializeField] public RectTransform Tooltip { get; private set; }
		[field: Space]
		[field: SerializeField] public UITooltipRuller Ruller { get; private set; }
		[field: SerializeField] public UITooltipAdditionaMessage AdditionalMessage { get; private set; }
		[field: SerializeField] public UITooltipMessage Message { get; private set; }

		private void Update()
		{
			if (Ruller.Type != TooltipRulerType.None ||
				AdditionalMessage.Type != TooltipAdditionalMessageType.None ||
				Message.Type != TooltipMessageType.None)
			{
				if (!IsShowing)
				{
					Show();
				}

				Vector2 position = Input.mousePosition;
				position += OffsetRightDown(Tooltip);

				Tooltip.anchoredPosition = position;
			}
			else
			{
				if (IsShowing)
				{
					Hide();
				}
			}
		}

		public override void Show(UnityAction callback = null)
		{
			transform.SetAsLastSibling();
			base.Show(callback);
		}

		private Vector2 OffsetRightDown(RectTransform rectTransform) => new Vector2((rectTransform.sizeDelta.x / 2) * 1.5f, -(rectTransform.sizeDelta.y / 2) * 2.5f);
	}
}