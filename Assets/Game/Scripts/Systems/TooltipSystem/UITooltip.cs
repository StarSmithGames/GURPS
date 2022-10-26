using Cinemachine;

using Game.Systems.SheetSystem;
using Game.UI.CanvasSystem;
using Game.UI.Windows;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;

namespace Game.Systems.TooltipSystem
{
    public class UITooltip : WindowBase
    {
		private RectTransform rectTransform;
		private CinemachineBrain brain;
		private UISubCanvas canvas;

		[Inject]
		private void Construct(CinemachineBrain brain, UISubCanvas canvas)
		{
			this.brain = brain;
			this.canvas = canvas;
		}

		private void Start()
		{
			rectTransform = transform as RectTransform;
			//Enable(false);
		}

		public void SetTarget(IEffect effect)
		{

		}

		public void SetPosition(PointerEventData data)
		{
			//rectTransform.anchoredPosition = brain.OutputCamera.ScreenToWorldPoint(data.position);

		}

		private void Update()
		{
			rectTransform.anchoredPosition = GetPointerPosOnCanvas(canvas.GetComponent<Canvas>(), Input.mousePosition);
		}

		public static Vector3 GetPointerPosOnCanvas(Canvas canvas, Vector2 pointerPos)
		{
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				return Input.mousePosition;
			}
			else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
			{
				if(RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, pointerPos, canvas.worldCamera, out Vector2 pos))
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
	}
}