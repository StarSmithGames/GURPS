using Game.UI.Windows;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class UIContainerWindow : WindowAnimatedPoolable
	{
		public UnityAction onTakeAll;
		public UnityAction onClose;

		[field: SerializeField] public UIInventory Inventory { get; private set; }

		[field: SerializeField] public Button Close { get; private set; }
		[field: SerializeField] public Button TakeAll { get; private set; }

		private void Start()
		{
			Close.onClick.AddListener(OnClosed);
			TakeAll.onClick.AddListener(OnTakeAll);
		}

		private void OnDestroy()
		{
			Close?.onClick.RemoveAllListeners();
			TakeAll?.onClick.RemoveAllListeners();
		}

		private void OnClosed()
		{
			HidePopup(() => DespawnIt());
			onClose?.Invoke();
		} 

		private void OnTakeAll()
		{
			HidePopup(() => DespawnIt());
			onTakeAll?.Invoke();
		}

		public class Factory : PlaceholderFactory<UIContainerWindow> { }
	}
}