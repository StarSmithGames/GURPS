using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public sealed class PointerHandlerComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
		private bool isInteractable = true;

		public UnityAction<PointerEventData> onPointerUp;
		public UnityAction<PointerEventData> onPointerDown;
		public UnityAction<PointerEventData> onPointerClick;
		public UnityAction<PointerEventData> onPointerEnter;
		public UnityAction<PointerEventData> onPointerExit;

		public void OnPointerUp(PointerEventData eventData)
		{
			onPointerUp?.Invoke(eventData);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			onPointerDown?.Invoke(eventData);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			onPointerClick?.Invoke(eventData);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			onPointerEnter?.Invoke(eventData);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			onPointerExit?.Invoke(eventData);
		}
	}
}