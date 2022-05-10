using System.Collections.Generic;

using UnityEngine;
using System.Linq;
using Zenject;
using Game.Systems.InventorySystem;
using Game.Entities;
using Game.Managers.InputManager;
using Game.Managers.CharacterManager;
using Game.Systems.InteractionSystem;

namespace Game.Systems.ContextMenu
{
	public class ContextMenuHandler : ITickable, IInitializable
	{
		public bool IsShowing { get; private set; }

		private UIContextMenu contextMenu;

		private List<UIContextAction> commands = new List<UIContextAction>();

		private UIContextAction.Factory contextMenuFactory;
		private Cinemachine.CinemachineBrain brain;
		private InputManager inputManager;
		private CharacterManager characterManager;
		private InteractionHandler interactionHandler;

		public ContextMenuHandler(
			UIContextAction.Factory contextMenuFactory,
			UIManager uiManager,
			Cinemachine.CinemachineBrain brain,
			InputManager inputManager,
			CharacterManager characterManager,
			InteractionHandler interactionHandler)
		{
			this.contextMenuFactory = contextMenuFactory;
			this.contextMenu = uiManager.ContextMenu;
			this.brain = brain;
			this.inputManager = inputManager;
			this.characterManager = characterManager;
			this.interactionHandler = interactionHandler;
		}

		public void Initialize()
		{
			Enable(false);
			contextMenu.transform.DestroyChildren();
		}

		public void Tick()
		{
			if (IsShowing)
			{
				//Check all inputs
				if(inputManager.IsScroolWheelDown() || inputManager.IsScroolWheelUp() || (inputManager.IsAnyKeyDown() && !inputManager.IsRightMouseButtonDown() && !inputManager.IsLeftMouseButtonDown()))
				{
					Hide();
				}
				else if (inputManager.IsLeftMouseButtonDown())//Click Outside
				{
					if(!RectTransformUtility.RectangleContainsScreenPoint(contextMenu.transform as RectTransform, GetMouseCoordsInCanvas(brain.OutputCamera, false)))
					{
						Hide();
					}
				}
			}
		}

		public ContextMenuHandler SetTarget(IObservable observable)
		{
			if(commands.Count > 0)
			{
				Dispose();
			}

			if(observable is IContainer container)
			{
				if (container.IsOpened)
				{
					AddCommand(new CommandCloseContainer(container) { name = "Close" });
				}
				else
				{
					AddCommand(new CommandOpenContainer(interactionHandler, characterManager.CurrentParty.LeaderParty, container) { name = "Open" });
				}

				AddCommand(new CommandAttack() { name = "Attack" });
				AddCommand(new CommandExamine(observable) { name = "Examine" });
			}
			else if (observable is Character character)
			{
				AddCommand(new CommandAttack() { name = "Attack" }, ContextType.Negative);
				AddCommand(new CommandExamine(observable) { name = "Examine" });
			}

			return this;
		}

		public void Show()
		{
			IsShowing = true;

			contextMenu.transform.position = GetMouseCoordsInCanvas(brain.OutputCamera, false);
			Enable(true);
		}

		public void Hide()
		{
			if (IsShowing)
			{
				IsShowing = false;

				Enable(false);

				Dispose();
			}
		}

		private void Enable(bool trigger)
		{
			contextMenu.gameObject.SetActive(trigger);
		}

		private void AddCommand(ICommand command, ContextType type = ContextType.Normal)
		{
			var action = contextMenuFactory.Create();
			action.onClicked += OnActionClicked;
			action.SetCommand(command, type);

			action.transform.SetParent(contextMenu.transform);

			commands.Add(action);
		}

		private void Dispose()
		{
			for (int i = commands.Count - 1; i >= 0; i--)
			{
				commands[i].onClicked -= OnActionClicked;
				commands[i].DespawnIt();
				commands.Remove(commands[i]);
			}
		}

		private Vector3 GetMouseCoordsInCanvas(Camera camera, bool worldCanvas)
		{
			if (!worldCanvas) { return Input.mousePosition; }

			// If the canvas render mode is in World Space,
			// We need to convert the mouse position into this rect coords.
			RectTransformUtility.ScreenPointToWorldPointInRectangle(
				contextMenu.transform as RectTransform,
				Input.mousePosition,
				camera,
				out Vector3 mousePosition);
			return mousePosition;
		}



		private void OnActionClicked(UIContextAction action)
		{
			Hide();
			action.CurrentCommand.Execute();
		}
	}
}