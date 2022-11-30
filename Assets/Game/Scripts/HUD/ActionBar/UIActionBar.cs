using Game.Managers.PartyManager;

using Sirenix.OdinInspector;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.HUD
{
	public sealed class UIActionBar : MonoBehaviour
	{
		[field: SerializeField] public Transform Content { get; private set; }
		[SerializeField] private List<UISlotAction> slots = new List<UISlotAction>();

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
			var actionBar = partyManager.PlayerParty.LeaderParty.Sheet.ActionBar;

			Assert.IsTrue(actionBar.Slots.Count == slots.Count);

			for (int i = 0; i < slots.Count; i++)
			{
				//Num 0-9
				slots[i].Key.enabled = i < 10;
				slots[i].Key.text = i < 10 ? (i + 1) < 10 ? (i + 1).ToString() : "0" : "";

				slots[i].SetSlot(actionBar.Slots[i]);
			}
		}


		[Button(DirtyOnClick = true)]
		private void GetSlots()
		{
			slots = GetComponentsInChildren<UISlotAction>().ToList();
		}
	}
}