using Game.Managers.InputManager;
using Game.Systems.CommandCenter;
using Game.UI;
using Game.UI.CanvasSystem;
using Game.UI.Windows;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.ContextMenu
{
    public class WindowContextMenu : WindowBase
    {
		private List<UIContextAction> actions = new List<UIContextAction>();

		private UISubCanvas subCanvas;
		private UIContextAction.Factory contextMenuFactory;
		private Cinemachine.CinemachineBrain brain;
		private InputManager inputManager;

		[Inject]
        private void Construct(UISubCanvas subCanvas,
			UIContextAction.Factory contextMenuFactory,
			Cinemachine.CinemachineBrain brain,
			InputManager inputManager)
		{
            this.subCanvas = subCanvas;
			this.contextMenuFactory = contextMenuFactory;
			this.brain = brain;
			this.inputManager = inputManager;
		}

		private void Start()
		{
			subCanvas.WindowsRegistrator.Registrate(this);

			transform.DestroyChildren();
			Enable(false);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsRegistrator.UnRegistrate(this);
		}

		private void Update()
		{
			if (IsShowing)
			{
				//Check all inputs
				if (inputManager.IsScroolWheelDown() || inputManager.IsScroolWheelUp() || (inputManager.IsAnyKeyDown() && !inputManager.IsRightMouseButtonDown() && !inputManager.IsLeftMouseButtonDown()))
				{
					Hide();
				}
				else if (inputManager.IsLeftMouseButtonDown())//Click Outside
				{
					if (!RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, GetMouseCoordsInCanvas(brain.OutputCamera, false)))
					{
						Hide();
					}
				}
			}
		}

		public void SetCommands(List<ContextCommand> commands)
		{
			CollectionExtensions.Resize(commands, actions,
			() =>
			{
				var action = contextMenuFactory.Create();

				action.onClicked += OnActionClicked;
				action.transform.SetParent(transform);
				return action;
			},
			() =>
			{
				var action = actions.Last();

				action.onClicked -= OnActionClicked;
				action.DespawnIt();
				return action;
			});

			int diff = commands.Count - actions.Count;

			for (int i = 0; i < actions.Count; i++)
			{
				actions[i].SetCommand(commands[i]);
			}

			if(actions.Count > 0)
			{
				Show();
			}
		}

		public override void Show(UnityAction callback = null)
		{
			transform.position = GetMouseCoordsInCanvas(brain.OutputCamera, false);

			base.Show(callback);
		}

		public override void Hide(UnityAction callback = null)
		{
			base.Hide(() =>
			{
				Dispose();
				callback?.Invoke();
			});
		}

		private void Dispose()
		{
			for (int i = actions.Count - 1; i >= 0; i--)
			{
				actions[i].onClicked -= OnActionClicked;
				actions[i].DespawnIt();
				actions.Remove(actions[i]);
			}
		}

		private Vector3 GetMouseCoordsInCanvas(Camera camera, bool worldCanvas)
		{
			if (!worldCanvas) { return Input.mousePosition; }

			// If the canvas render mode is in World Space,
			// We need to convert the mouse position into this rect coords.
			RectTransformUtility.ScreenPointToWorldPointInRectangle(
				transform as RectTransform,
				Input.mousePosition,
				camera,
				out Vector3 mousePosition);
			return mousePosition;
		}

		private void OnActionClicked(UIContextAction action)
		{
			action.ContextCommand.Execute();
			Hide();
		}
	}
}