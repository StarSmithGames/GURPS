using System.Collections.Generic;
using Game.Systems.InventorySystem;
using Game.Systems.DialogueSystem;
using Game.Managers.PartyManager;
using Game.Entities.Models;
using Game.Map;
using Game.Systems.CombatDamageSystem;
using Game.Systems.CommandCenter;
using Game.Systems.SheetSystem.Abilities;
using Game.UI.CanvasSystem;

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
				contextMenu = subCanvas.WindowsRegistrator.GetAs<WindowContextMenu>();
			}

			List<ContextCommand> commands = new List<ContextCommand>();

			var self = partyManager.PlayerParty.LeaderParty.Model;
			var isSelf = observable == self;

			if (observable is IDamageable damegeable && !isSelf)
			{
				//if (self.Sheet.Abilities.ContainsType<AttackAbility>(out var ability))
				//{
				//	commands.Add(new CommandAttack(self, damegeable) { name = ability.abilityDisplayName, type = self.InBattle ? ContextType.Normal : ContextType.Negative });
				//}
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
				commands.AddRange(GetContainerCommands(self, container));
			}
			
			if(observable is IWayPoint wayPoint)
			{
				commands.AddRange(GetWayPointCommands(self, wayPoint));
			}

			commands.Add(new CommandExamine(observable) { name = "Examine" });

			contextMenu.SetCommands(commands);
		}

		private List<ContextCommand> GetContainerCommands(ICharacterModel leader, IContainer container)
		{
			List<ContextCommand> commands = new List<ContextCommand>();

			if (container.IsOpened)
			{
				commands.Add(new CommandCloseContainer(container) { name = "Close" });
			}
			else
			{
				commands.Add(new CommandInteract(leader, container) { name = "Open" });
			}

			return commands;
		}

		private List<ContextCommand> GetWayPointCommands(ICharacterModel leader, IWayPoint wayPoint)
		{
			List<ContextCommand> commands = new List<ContextCommand>();

			commands.Add(new CommandInteract(leader, wayPoint) { name = "GoTo" });

			return commands;
		}
	}
}