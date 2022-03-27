using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.InventorySystem
{
	public class UIItemCursor : MonoBehaviour
	{
		[SerializeField] private Image icon;

		public UISlot Slot { get; private set; }

		private Transform parent;

		private void Awake()
		{
			parent = transform.parent;
		}

		public void SetSlot(UISlot slot)
		{
			Slot = slot;
		}

		public void SetIcon(Sprite sprite)
		{
			icon.sprite = sprite;
		}

		public void Dispose()
		{
			transform.parent = parent;
			Slot = null;
			icon.sprite = null;
		}
	}
}