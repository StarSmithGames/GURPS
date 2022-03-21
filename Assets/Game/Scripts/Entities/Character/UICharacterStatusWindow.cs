using Game.Systems.InventorySystem;

using UnityEngine;

public class UICharacterStatusWindow : MonoBehaviour
{
	public UIInventory Inventory => inventory;
	[SerializeField] private UIInventory inventory;
}