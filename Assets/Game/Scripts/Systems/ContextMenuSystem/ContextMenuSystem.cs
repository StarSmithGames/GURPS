using System.Collections.Generic;

using Game.Systems.InventorySystem;
using Game.Systems.DialogueSystem;
using Game.UI;
using Game.Managers.PartyManager;
using Game.Entities.Models;

namespace Game.Systems.ContextMenu
{
	public class ContextMenuSystem
	{
		private WindowContextMenu contextMenu;

		private UISubCanvas subCanvas;
		private PartyManager partyManager;

		public ContextMenuSystem(UISubCanvas subCanvas, PartyManager partyManager)
		{
			this.subCanvas = subCanvas;
			this.partyManager = partyManager;
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
					commands.Add(new CommandOpenContainer(partyManager.PlayerParty.LeaderParty.Model as ICharacterModel, container) { name = "Open" });
				}

				commands.Add(new CommandAttack() { name = "Attack" });
				commands.Add(new CommandExamine(observable) { name = "Examine" });
			}
			else if (observable is IActor actor)
			{
				if (actor.IsHaveSomethingToSay)
				{
					//AddCommand(new CommandTalk(dialogueSystem, characterManager.CurrentParty.LeaderParty, actor) { name = "Talk" });
				}
				commands.Add(new CommandAttack() { name = "Attack", type = ContextType.Negative });
				commands.Add(new CommandExamine(observable) { name = "Examine" });
			}

			contextMenu.SetCommands(commands);
		}
	}
}