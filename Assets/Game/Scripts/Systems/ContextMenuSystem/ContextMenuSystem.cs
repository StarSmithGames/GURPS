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
using Game.Systems.CombatDamageSystem;
using Game.Systems.CommandCenter;
using Game.Systems.SheetSystem.Abilities;

namespace Game.Systems.ContextMenu
{
	public class ContextMenuSystem
	{
		private WindowContextMenu contextMenu;

		private UISubCanvas subCanvas;
		private PartyManager partyManager;
		private GameManager gameManager;

		public ContextMenuSystem(UISubCanvas subCanvas, PartyManager partyManager, GameManager gameManager)
		{
			this.subCanvas = subCanvas;
			this.partyManager = partyManager;
			this.gameManager = gameManager;
		}

		public void SetTarget(IObservable observable)
		{
			if (contextMenu == null)
			{
				contextMenu = subCanvas.WindowsRegistrator.GetAs<WindowContextMenu>();
			}

			List<ContextCommand> commands = new List<ContextCommand>();

			var self = partyManager.PlayerParty.LeaderParty.Model as ICharacterModel;
			bool isSelf = observable == self;

			if (observable is IDamageable damegeable && !isSelf)
			{
				if (self.Sheet.Abilities.Contains<AttackAbility>(out var ability))
				{
					commands.Add(new CommandAttack(self, damegeable) { name = ability.Data.abilityName, type = self.InBattle ? ContextType.Normal : ContextType.Negative });
				}
			}

			if (observable is IActor actor && !isSelf)
			{
				if (actor.IsHasSomethingToSay)
				{
					commands.Add(new CommandTalk(self, actor) { name = "Talk" });
				}
			}
			
			if (observable is IContainer container)
			{
				commands.AddRange(GetContainerCommands(container));
			}
			
			if(observable is IWayPoint wayPoint)
			{
				commands.AddRange(GetWayPointCommands(wayPoint));
			}

			commands.Add(new CommandExamine(observable) { name = "Examine" });

			contextMenu.SetCommands(commands);
		}

		private List<ContextCommand> GetContainerCommands(IContainer container)
		{
			List<ContextCommand> commands = new List<ContextCommand>();

			if (container.IsOpened)
			{
				commands.Add(new CommandCloseContainer(container) { name = "Close" });
			}
			else
			{
				IInteractable interactable = partyManager.PlayerParty.LeaderParty.Model;
				commands.Add(new CommandInteract(interactable, container) { name = "Open" });
			}

			return commands;
		}

		private List<ContextCommand> GetWayPointCommands(IWayPoint wayPoint)
		{
			List<ContextCommand> commands = new List<ContextCommand>();

			IInteractable interactable = partyManager.PlayerParty.LeaderParty.Model;
			commands.Add(new CommandInteract(interactable, wayPoint) { name = "GoTo" });

			return commands;
		}
	}
}