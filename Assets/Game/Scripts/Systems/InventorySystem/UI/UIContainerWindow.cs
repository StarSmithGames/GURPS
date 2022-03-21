using Sirenix.OdinInspector;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class UIContainerWindow : WindowBasePoolable<UIContainerWindow>
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
			close.onClick.AddListener(OnClose);
			takeAll.onClick.AddListener(OnTakeAll);
		}

		private void OnDestroy()
		{
			close.onClick.RemoveAllListeners();
			takeAll.onClick.RemoveAllListeners();
		}

		private void OnTakeAll()
		{
			OnClose();

			onTakeAll?.Invoke();
		}

		private void OnClose()
		{
			Hide();
			DespawnIt();

			onClose?.Invoke();
		}
	}
}