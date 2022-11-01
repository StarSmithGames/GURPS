using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.InventorySystem
{
	public class UIDragItem : MonoBehaviour
	{
		[SerializeField] private Image icon;

		private Transform parent;

		private void Awake()
		{
			parent = transform.parent;
		}

		public void SetIcon(Sprite sprite)
		{
			icon.sprite = sprite;
		}

		public void Dispose()
		{
			transform.parent = parent;
			icon.sprite = null;
		}
	}
}