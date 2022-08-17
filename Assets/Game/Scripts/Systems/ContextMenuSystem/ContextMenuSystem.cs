using System.Collections.Generic;

using Game.Systems.InventorySystem;
using Game.Systems.DialogueSystem;
using Game.UI;
using Game.Managers.PartyManager;
using Game.Entities.Models;
using Game.Map;
using Game.Managers.GameManager;
using Game.Systems.InteractionSystem;
using Game.Entities;

namespace Game.Systems.ContextMenu
{
	public class ContextMenuSystem
	{
		private WindowContextMenu contextMenu;

		private UISubCanvas subCanvas;
		private PartyManager partyManager;
		private GameManager gameManager;
		private IPlayer player;

		public ContextMenuSystem(UISubCanvas subCanvas, PartyManager partyManager, GameManager gameManager, IPlayer player)
		{
			this.subCanvas = subCanvas;
			this.partyManager = partyManager;
			this.gameManager = gameManager;
			this.player = player;
		}

		public void SetTarget(IObservable observable)
		{
			if (contextMenu == null)
			{
				contextMenu = subCanvas.WindowsManager.GetAs<WindowContextMenu>();
			}

			List<ContextCommand> commands = new List<ContextCommand>();

			if (observable is IContainer container)
			{
				if (container.IsOpened)
				{
					commands.Add(new CommandCloseContainer(container) { name = "Close" });
				}
				else
				{
					IInteractable interactable = gameManager.CurrentGameLocation == GameLocation.Location ? partyManager.PlayerParty.LeaderParty.Model as ICharacterModel : player.RTSModel as IInteractable;
					commands.Add(new CommandInteract(interactable, container) { name = "Open" });
				}

				commands.Add(new CommandAttack() { name = "Attack" });
				commands.Add(new CommandExamine(observable) { name = "Examine" });
			}
			else if (observable is ICharacterModel)
			{
				//if (actor.IsHaveSomethingToSay)
				//{
				//	//AddCommand(new CommandTalk(dialogueSystem, characterManager.CurrentParty.LeaderParty, actor) { name = "Talk" });
				//}
				commands.Add(new CommandAttack() { name = "Attack", type = ContextType.Negative });
				commands.Add(new CommandExamine(observable) { name = "Examine" });
			}
			else if(observable is IWayPoint wayPoint)
			{
				IInteractable interactable = gameManager.CurrentGameLocation == GameLocation.Location ? partyManager.PlayerParty.LeaderParty.Model as ICharacterModel : player.RTSModel as IInteractable;
				commands.Add(new CommandInteract(interactable, wayPoint) { name = "GoTo"});
			}

			contextMenu.SetCommands(commands);
		}
	}
}