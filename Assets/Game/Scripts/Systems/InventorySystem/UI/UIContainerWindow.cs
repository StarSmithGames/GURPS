using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class UIContainerWindow : UIPoolableAnimatedWindow
	{
		public UnityAction onTakeAll;
		public UnityAction onClose;

		public UIInventory Inventory => inventory;
		[SerializeField] private UIInventory inventory;

		public Button close;
		public Button takeAll;

		[Inject]
		private void Construct()
		{
			close.onClick.AddListener(OnClosed);
			takeAll.onClick.AddListener(OnTakeAll);
		}

		private void OnDestroy()
		{
			close.onClick.RemoveAllListeners();
			takeAll.onClick.RemoveAllListeners();
		}

		private void OnClosed()
		{
			Popup.PopOut(onComplete: () =>
			{
				Hide();
			});

			onClose?.Invoke();
		} 

		private void OnTakeAll()
		{
			Popup.PopOut(onComplete: () =>
			{
				Hide();
			});

			onTakeAll?.Invoke();
		}

		public class Factory : PlaceholderFactory<UIContainerWindow> { }
	}
}