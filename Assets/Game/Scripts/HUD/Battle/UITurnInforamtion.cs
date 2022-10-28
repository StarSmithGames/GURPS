using Game.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Game.HUD
{
	public class UITurnInforamtion : WindowBase
	{
		[field: SerializeField] public Image Back { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }

		[SerializeField] private Sprite playerBackground;
		[SerializeField] private Sprite enemyBackground;

		private void Start()
		{
			Enable(false);
		}

		public UITurnInforamtion SetText(string text, TurnInformationBackground background = TurnInformationBackground.Player)
		{
			Text.text = text;

			Back.sprite = background == TurnInformationBackground.Player ? playerBackground : enemyBackground;

			return this;
		}
	}

	public enum TurnInformationBackground
	{
		Player,
		Enemy,
	}
}