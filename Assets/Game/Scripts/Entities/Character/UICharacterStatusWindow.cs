using Game.Entities;
using Game.Systems.InventorySystem;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.InteractionSystem
{
	public class UICharacterStatusWindow : MonoBehaviour
	{
		public UnityAction onClose;

		[field: SerializeField] public UIButton Close { get; private set; }
		[field: SerializeField] public UIInventory Inventory { get; private set; }
		[field: SerializeField] public UIEquipment Equipment { get; private set; }

		private void Start()
		{
			Close.ButtonPointer.onClick.AddListener(OnClose);
		}

		private void OnDestroy()
		{
			if (Close != null)
			{
				Close.ButtonPointer.onClick.RemoveAllListeners();
			}
		}

		public void SetCharacter(Character character)
		{
			Inventory.SetInventory(character.Inventory);
		}


		private void OnClose()
		{
			onClose?.Invoke();
		}
	}
}