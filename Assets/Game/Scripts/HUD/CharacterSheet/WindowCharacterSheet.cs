using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using Game.UI.CanvasSystem;
using Game.UI.Windows;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.HUD
{
	public class WindowCharacterSheet : WindowBase
	{
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

			subCanvas.WindowsRegistrator.Registrate(this);

			Close.onClick.AddListener(OnClose);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsRegistrator.UnRegistrate(this);
			Close?.onClick.RemoveAllListeners();
		}

		public void SetSheet(CharacterSheet sheet)
		{
			Inventory.SetInventory(sheet.Inventory);
			Equipment.SetEquipment(sheet.Equipment);
		}

		private void OnClose()
		{
			Hide();
		}
	}
}