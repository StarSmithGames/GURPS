using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public sealed class PointerInteractComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
		private bool isInteractable = true;

		public UnityAction<PointerEventData> onPointerClick;
		public UnityAction<PointerEventData> onPointerDown;
		public UnityAction<PointerEventData> onPointerUp;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!IsInteractable) return;

			onPointerClick?.Invoke(eventData);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!IsInteractable) return;

			onPointerDown?.Invoke(eventData);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (!IsInteractable) return;
			
			onPointerUp?.Invoke(eventData);
		}
	}
}