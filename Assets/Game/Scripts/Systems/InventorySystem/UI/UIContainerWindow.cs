using Game.UI;
using Game.UI.CanvasSystem;
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
		[field: SerializeField] public DragableComponent DragableComponent { get; private set; } 

		[field: SerializeField] public Button Close { get; private set; }
		[field: SerializeField] public Button TakeAll { get; private set; }

		private UISubCanvas subCanvas;

		[Inject]
		private void Constrcut(UISubCanvas subCanvas)
		{
			this.subCanvas = subCanvas;
		}

		private void Start()
		{
			Close.onClick.AddListener(OnClosed);
			TakeAll.onClick.AddListener(OnTakeAll);

			DragableComponent.onOrdered += OnOrdered;
		}

		private void OnDestroy()
		{
			Close?.onClick.RemoveAllListeners();
			TakeAll?.onClick.RemoveAllListeners();

			DragableComponent.onOrdered -= OnOrdered;
		}

		private void OnClosed()
		{
			HidePopup();
			onClose?.Invoke();
		} 

		private void OnTakeAll()
		{
			HidePopup();
			onTakeAll?.Invoke();
		}

		private void OnOrdered()
		{
			//subCanvas.WindowsRegistrator.
		}

		public class Factory : PlaceholderFactory<UIContainerWindow> { }
	}
}