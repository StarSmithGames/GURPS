using Game.Entities;
using Game.Systems.DialogueSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Systems.ContextMenu
{
	public interface ICommand
	{
		void Execute();
	}

	public abstract class ContextCommand : ICommand
	{
		public string name;

		public abstract void Execute();
	}

	public abstract class SheetCommand : ICommand
	{
		protected ISheet sheet;

		public SheetCommand(ISheet sheet)
		{
			this.sheet = sheet;
		}

		public abstract void Execute();
	}


	#region ContextCommands
	public class CommandUse : ContextCommand//eat, drink, use spells
	{
		public override void Execute()
		{
		}
	}


	public class CommandTalk : ContextCommand
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
			dialogueSystem.StartDialogue(character, actor);
		}
	}
	public class CommandExamine : ContextCommand
	{
		public CommandExamine(IObservable observable)
		{

		}

		public override void Execute()
		{
		}
	}


	public class CommandOpenContainer : ContextCommand
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
	public class CommandCloseContainer : ContextCommand
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

	public class CommandAttack : ContextCommand
	{
		public override void Execute()
		{
		}
	}


	public class CommandPickUp : ContextCommand
	{
		public override void Execute()
		{
		}
	}

	public class CommandSeparateCharacter : ContextCommand
	{
		public override void Execute()
		{
		}
	}


	public class CommandRob : ContextCommand
	{
		public override void Execute()
		{
		}
	}
	#endregion


	#region SheetCommnads
	public class CommandSetAlignment : SheetCommand
	{
		public Alignment Result { get; private set; }

		private Alignment type = Alignment.TrueNeutral;
		private float percentAlignment = 0;
		private Vector2 vectorAlignment;
		private bool usePercents = false;

		public CommandSetAlignment(ISheet sheet, Vector2 vectorAlignment) : base(sheet)
		{
			this.vectorAlignment = vectorAlignment;

			//Result = vectorAlignment;

			usePercents = false;
		}

		public CommandSetAlignment(ISheet sheet, Alignment type, float percentAlignment) : base(sheet)
		{
			this.type = type;
			this.percentAlignment = percentAlignment;

			Result = type;

			usePercents = true;
		}

		public override void Execute()
		{
			if (usePercents)
			{
				sheet.Characteristics.Alignment.CurrentValue = Vector2.Lerp(sheet.Characteristics.Alignment.CurrentValue, AlignmentCharacteristic.ConvertAligmentToVector2(type), percentAlignment);
			}
			else
			{
				sheet.Characteristics.Alignment.CurrentValue += vectorAlignment;
			}
		}
	}
	#endregion
}