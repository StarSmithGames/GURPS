using Cinemachine;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using Game.UI.CanvasSystem;
using Game.UI.Windows;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Systems.TooltipSystem
{
    public class UITooltip : WindowBase
    {
		private RectTransform rectTransform;
		private Canvas canvas;

		[Inject]
		private void Construct(UISubCanvas canvas)
		{
			this.canvas = canvas.GetComponent<Canvas>();
		}

		private void Start()
		{
			rectTransform = transform as RectTransform;
			rectTransform.pivot = new Vector2(0, 1);

			Enable(false);
		}

		private void Update()
		{
			if (IsShowing)
			{
				UpdatePositionFromMouse();
			}
		}

		public override void Show(UnityAction callback = null)
		{
			Enable(true);
		}

		public override void Hide(UnityAction callback = null)
		{
			Enable(false);
		}

		public void EnterTarget(IEffect effect)
		{
			Show();
		}

		public void EnterTarget(UISlot slot)
		{
			if (!slot.IsEmpty)
			{
				Show();
			}
		}

		public void ExitTarget()
		{
			if (IsShowing)
			{
				Hide();
			}
		}

		public void ExitTarget(UISlot slot)
		{
			ExitTarget();
		}

		public void UpdatePositionFromMouse()
		{
			UpdatePosition(GetPositionOnCanvas());
		}



		private void UpdatePosition(Vector3 position)
		{
			UpdatePivot(position);
			rectTransform.position = position;
		}
		private void UpdatePivot(Vector3 position)
		{
			var width = rectTransform.sizeDelta.x;
			var height = rectTransform.sizeDelta.y;

			Vector3 pivot = rectTransform.pivot;//base 0 1

			pivot.x = position.x < (Screen.width - width) ? 0 : 1;
			pivot.y = position.y < (height) ? 0 : 1;

			rectTransform.pivot = pivot;

		}

		private Vector3 GetPositionOnCanvas()
		{
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				return Input.mousePosition;
			}
			else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
			{
				if(RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 pos))
				{
					return canvas.transform.TransformPoint(pos);
				}
			}
			else
			{
				if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector3 globalMousePos))
				{
					return globalMousePos;
				}
			};

			return Vector2.zero;
		}

		private void Test()
		{
			//Vector2 localMousePosition = imgRectTransform.InverseTransformPoint(Input.mousePosition);
			//if (imgRectTransform.rect.Contains(localMousePosition))
			//{
			//	return true;
			//}
		}
		private void ClampWindow()
		{
			var lastPos = rectTransform.position;
			var pos = GetPositionOnCanvas();

			var width = rectTransform.sizeDelta.x;
			var height = rectTransform.sizeDelta.y;

			if (pos.x < 0 || (pos.x + width) > Screen.width)
			{
				pos = new Vector3(lastPos.x, pos.y, pos.z);
			}

			if (pos.y < height || pos.y > Screen.height)
			{
				pos = new Vector3(pos.x, lastPos.y, pos.z);
			}

			rectTransform.position = pos;
		}

	}
}