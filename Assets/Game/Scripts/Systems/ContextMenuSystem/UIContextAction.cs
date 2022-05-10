using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.ContextMenu
{
	public class UIContextAction : PoolableObject
	{
		public UnityAction<UIContextAction> onClicked;

		[field: SerializeField] public Button Button { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }

		public ICommand CurrentCommand { get; private set; }

		private void Start()
		{
			Button.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
			if(Button != null)
			{
				Button.onClick.RemoveListener(OnClick);
			}
		}

		public void SetCommand(ICommand command)
		{
			CurrentCommand = command;
			Text.text = (command as BaseCommand).name;
		}

		private void OnClick()
		{
			onClicked?.Invoke(this);
		}

		public class Factory : PlaceholderFactory<UIContextAction> { }
	}
}