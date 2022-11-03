using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using Game.UI.CanvasSystem;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace Game.HUD
{
	public sealed class UIActionBar : MonoBehaviour
	{
		[field: SerializeField] public Transform Content { get; private set; }

		private List<UISlotAction> slots = new List<UISlotAction>();

		private UISlotAction.Factory actionFactory;
		private PartyManager partyManager;

		[Inject]
		private void Construct(UISlotAction.Factory actionFactory, PartyManager partyManager)
		{
			this.actionFactory = actionFactory;
			this.partyManager = partyManager;
		}

		private void Start()
		{
			Content.DestroyChildren();

			var actionBar = partyManager.PlayerParty.LeaderParty.Sheet.ActionBar;

			CollectionExtensions.Resize(actionBar.Slots, slots,
			() =>
			{
				var action = actionFactory.Create();
				action.transform.SetParent(Content);

				return action;
			},
			() =>
			{
				var slot = slots.Last();

				slot.DespawnIt();

				return slot;
			});

			for (int i = 0; i < slots.Count; i++)
			{
				//Num 0-9
				slots[i].Key.enabled = i < 10;
				slots[i].Key.text = i < 10 ? (i + 1) < 10 ? (i + 1).ToString() : "0" : "";

				slots[i].SetSlot(actionBar.Slots[i]);
			}
		}
	}
}