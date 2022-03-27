using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.InventorySystem
{
	public class UISlot : MonoBehaviour
	{
		[field: SerializeField] public DragAndDropUISlotComponent DragAndDrop { get; private set; }

		[field: SerializeField] public Image Background { get; protected set; }
		[field: SerializeField] public Image Icon { get; protected set; }
	}
}