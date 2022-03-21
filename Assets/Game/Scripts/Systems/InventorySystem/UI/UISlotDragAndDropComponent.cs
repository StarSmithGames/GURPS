using Game.Systems.InventorySystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UISlotDragAndDropComponent : MonoBehaviour,
	IPointerEnterHandler, IPointerExitHandler,
	IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler,
	IPointerClickHandler
{
	public UnityAction<UISlot, PointerEventData> onPointerClick;
	public UnityAction<UISlot, PointerEventData> onPointerEnter;
	public UnityAction<UISlot, PointerEventData> onPointerEXit;
	public UnityAction<UISlot, PointerEventData> onBeginDrag;
	public UnityAction<UISlot, PointerEventData> onDrag;
	public UnityAction<UISlot, PointerEventData> onEndDrag;
	public UnityAction<UISlot, PointerEventData> onDrop;

	[SerializeField] private UISlot owner;

	private void OnDestroy()
	{
		onPointerClick = null;
		onPointerEnter = null;
		onPointerEXit = null;
		onBeginDrag = null;
		onDrag = null;
		onEndDrag = null;
		onDrop = null;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		onPointerEnter?.Invoke(owner, eventData);
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		onPointerEXit?.Invoke(owner, eventData);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		onPointerClick?.Invoke(owner, eventData);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		onBeginDrag?.Invoke(owner, eventData);
	}
	public void OnDrag(PointerEventData eventData)
	{
		onDrag?.Invoke(owner, eventData);
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		onEndDrag?.Invoke(owner, eventData);
	}

	public void OnDrop(PointerEventData eventData)
	{
		onDrop?.Invoke(owner, eventData);
	}
}