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

		[Header("Normal button")]
		[field: SerializeField] public ColorBlock normal;
		[Header("Negative button")]
		[field: SerializeField] public ColorBlock negative;

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

		public void SetCommand(ICommand command, ContextType type = ContextType.Normal)
		{
			CurrentCommand = command;
			Text.text = (command as ContextCommand).name;

			Text.color = type == ContextType.Normal ? Color.white : Color.red;
			Button.colors = type == ContextType.Normal ? normal : negative; 
		}

		private void OnClick()
		{
			onClicked?.Invoke(this);
		}

		public class Factory : PlaceholderFactory<UIContextAction> { }
	}

	public enum ContextType
	{
		Normal,
		Negative,
	}
}