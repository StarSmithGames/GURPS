using Game.Entities;
using Game.Systems.DialogueSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.InventorySystem;

using UnityEngine;

namespace Game.Systems.ContextMenu
{
	public interface ICommand
	{
		void Execute();
	}

	public abstract class BaseCommand : ICommand
	{
		public string name;

		public abstract void Execute();
	}

	public class CommandUse : BaseCommand//eat, drink, use spells
	{
		public override void Execute()
		{
		}
	}


	public class CommandTalk : BaseCommand
	{
		private DialogueSystem.DialogueSystem dialogueSystem;
		private Character character;
		private IActor actor;

		public CommandTalk(DialogueSystem.DialogueSystem dialogueSystem, Character character, IActor actor)
		{
			this.dialogueSystem = dialogueSystem;
			this.character = character;
			this.actor = actor;
		}

		public override void Execute()
		{
			dialogueSystem.StartConversation(actor);
		}
	}
	public class CommandExamine : BaseCommand
	{
		public CommandExamine(IObservable observable)
		{

		}

		public override void Execute()
		{
		}
	}


	public class CommandOpenContainer : BaseCommand
	{
		private InteractionSystem.InteractionSystem interactionHandler;
		private Character character;
		private IContainer container;
		public CommandOpenContainer(InteractionSystem.InteractionSystem interactionHandler, Character character, IContainer container)
		{
			this.interactionHandler = interactionHandler;
			this.character = character;
			this.container = container;
		}

		public override void Execute()
		{
			interactionHandler.Interact(character, container);
		}
	}
	public class CommandCloseContainer : BaseCommand
	{
		private IContainer container;

		public CommandCloseContainer(IContainer container)
		{
			this.container = container;
		}

		public override void Execute()
		{
			container.Close();
		}
	}

	public class CommandAttack : BaseCommand
	{
		public override void Execute()
		{
		}
	}


	public class CommandPickUp : BaseCommand
	{
		public override void Execute()
		{
		}
	}

	public class CommandSeparateCharacter : BaseCommand
	{
		public override void Execute()
		{
		}
	}


	public class CommandRob : BaseCommand
	{
		public override void Execute()
		{
		}
	}
}