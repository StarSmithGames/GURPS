using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using Game.UI.CanvasSystem;

using UnityEngine;

using Zenject;

namespace Game.HUD
{
	public sealed class UIActionBar : MonoBehaviour
	{
		[field: SerializeField] public UIInventory Inventory { get; private set; }

		private PartyManager partyManager;

		[Inject]
		private void Construct(PartyManager partyManager)
		{
			this.partyManager = partyManager;
		}

		private void Start()
		{
			Inventory.SetInventory(partyManager.PlayerParty.LeaderParty.Sheet.Actions);
		}
	}
}