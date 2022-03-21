using Game.Entities;
using Game.Systems.InventorySystem;

using UnityEngine;

public class UICharacterStatusWindow : MonoBehaviour
{
	public UIInventory Inventory => inventory;
	[SerializeField] private UIInventory inventory;

	public void SetCharacter(Character character)
	{
		inventory.SetInventory(character.Inventory);
	}
}