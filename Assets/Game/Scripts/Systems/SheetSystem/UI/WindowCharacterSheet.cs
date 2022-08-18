using Game.Managers.InputManager;
using Game.Managers.PartyManager;
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
		[field: SerializeField] public Button Close { get; private set; }
		[field: SerializeField] public UIInventory Inventory { get; private set; }
		[field: SerializeField] public UIEquipment Equipment { get; private set; }

		private UISubCanvas subCanvas;
		private InputManager inputManager;
		private PartyManager partyManager;

		[Inject]
		private void Construct(UISubCanvas subCanvas, InputManager inputManager, PartyManager partyManager)
		{
			this.subCanvas = subCanvas;
			this.inputManager = inputManager;
			this.partyManager = partyManager;
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

		private void Update()
		{
			if (inputManager.GetKeyDown(KeyAction.Inventory))
			{
				if (IsShowing)
				{
					Hide();
				}
				else
				{
					SetSheet(partyManager.PlayerParty.LeaderParty.Sheet as CharacterSheet);
					Show();
				}
			}
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