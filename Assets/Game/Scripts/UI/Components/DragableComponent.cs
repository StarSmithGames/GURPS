using Game.UI.CanvasSystem;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Zenject;

namespace Game.UI
{
	public class DragableComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler
	{
		public UnityAction onOrdered;

		[SerializeField] private bool useOrder = true;
		[field: SerializeField] public RectTransform DragRect { get; private set; }

		private RectTransform canvasRectTransform;

		private Vector2 pointerOffset;
		private bool clampedToLeft;
		private bool clampedToRight;
		private bool clampedToTop;
		private bool clampedToBottom;

		[Inject]
		private void Construct(UISubCanvas canvas)
		{
			canvasRectTransform = canvas.GetComponent<RectTransform>();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (useOrder)
			{
				DragRect.SetAsLastSibling();
				onOrdered?.Invoke();
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(DragRect, eventData.position, eventData.pressEventCamera, out pointerOffset);
		}

		public void OnDrag(PointerEventData eventData)
		{
			//DragRect.anchoredPosition += eventData.delta;

			Vector2 localPointerPosition;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
			{
				DragRect.localPosition = localPointerPosition - pointerOffset;
				ClampWindow();
				Vector2 clampedPosition = DragRect.localPosition;
				if (clampedToRight)
				{
					clampedPosition.x = (canvasRectTransform.rect.width * 0.5f) - (DragRect.rect.width * (1 - DragRect.pivot.x));
				}
				else if (clampedToLeft)
				{
					clampedPosition.x = (-canvasRectTransform.rect.width * 0.5f) + (DragRect.rect.width * DragRect.pivot.x);
				}

				if (clampedToTop)
				{
					clampedPosition.y = (canvasRectTransform.rect.height * 0.5f) - (DragRect.rect.height * (1 - DragRect.pivot.y));
				}
				else if (clampedToBottom)
				{
					clampedPosition.y = (-canvasRectTransform.rect.height * 0.5f) + (DragRect.rect.height * DragRect.pivot.y);
				}

				DragRect.localPosition = clampedPosition;
			}
		}

		private void ClampWindow()
		{
			Vector3[] canvasCorners = new Vector3[4];
			Vector3[] panelRectCorners = new Vector3[4];
			canvasRectTransform.GetWorldCorners(canvasCorners);
			DragRect.GetWorldCorners(panelRectCorners);

			if (panelRectCorners[2].x > canvasCorners[2].x)
			{
				Debug.LogError("Panel is to the right of canvas limits");
				if (!clampedToRight)
				{
					clampedToRight = true;
				}
			}
			else if (clampedToRight)
			{
				clampedToRight = false;
			}
			else if (panelRectCorners[0].x < canvasCorners[0].x)
			{
				Debug.LogError("Panel is to the left of canvas limits");
				if (!clampedToLeft)
				{
					clampedToLeft = true;
				}
			}
			else if (clampedToLeft)
			{
				clampedToLeft = false;
			}

			if (panelRectCorners[2].y > canvasCorners[2].y)
			{
				Debug.LogError("Panel is to the top of canvas limits");
				if (!clampedToTop)
				{
					clampedToTop = true;
				}
			}
			else if (clampedToTop)
			{
				clampedToTop = false;
			}
			else if (panelRectCorners[0].y < canvasCorners[0].y)
			{
				Debug.LogError("Panel is to the bottom of canvas limits");
				if (!clampedToBottom)
				{
					clampedToBottom = true;
				}
			}
			else if (clampedToBottom)
			{
				clampedToBottom = false;
			}
		}
	}
}