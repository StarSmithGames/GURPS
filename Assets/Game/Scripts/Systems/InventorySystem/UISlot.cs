
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Game.Systems.InventorySystem
{
	public class UISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public Image icon;
		public TMPro.TextMeshProUGUI count;

		private RectTransform cursor;

		public void SetItem(Item item)
		{

		}

		public void OnPointerEnter(PointerEventData eventData)
		{
		}
		public void OnPointerExit(PointerEventData eventData)
		{
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if(cursor == null)
			{
				cursor = new GameObject("_Cursor").AddComponent<RectTransform>();
				cursor.transform.parent = transform.root;
			}
		}
		public void OnDrag(PointerEventData eventData)
		{
		}
		public void OnEndDrag(PointerEventData eventData)
		{
		}
	}
}