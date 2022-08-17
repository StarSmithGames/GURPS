using Game.Systems.InventorySystem;
using Game.UI;
using Game.UI.Windows;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.SheetSystem
{
	public class WindowCharacterSheet : WindowBase
	{
		public UnityAction onClose;

		[field: SerializeField] public Button Close { get; private set; }
		[field: SerializeField] public UIInventory Inventory { get; private set; }
		[field: SerializeField] public UIEquipment Equipment { get; private set; }

		private UISubCanvas subCanvas;

		[Inject]
		private void Construct(UISubCanvas subCanvas)
		{
			this.subCanvas = subCanvas;
		}

		private void Start()
		{
			Enable(false);

			subCanvas.WindowsManager.Register(this);

			Close.onClick.AddListener(OnClose);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsManager.UnRegister(this);
			Close?.onClick.RemoveAllListeners();
		}

		public void SetSheet(CharacterSheet sheet)
		{
			Inventory.SetInventory(sheet.Inventory);
			Equipment.SetEquipment(sheet.Equipment);
		}

		private void OnClose()
		{
			onClose?.Invoke();
		}
	}
}