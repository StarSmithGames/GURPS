using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.DialogueSystem
{
	public class UIChoice : PoolableObject
	{
		public UnityAction<UIChoice> onButtonClick;

		[field: SerializeField] public Button Background { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }

		private void Start()
		{
			Background.onClick.AddListener(OnButtonClicked);
		}

		private void OnDestroy()
		{
			if (Background != null)
			{
				Background.onClick.RemoveListener(OnButtonClicked);
			}
		}


		private void OnButtonClicked()
		{
			onButtonClick?.Invoke(this);
		}

		public class Factory : PlaceholderFactory<UIChoice> { }
	}
}