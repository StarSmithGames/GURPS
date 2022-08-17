using Game.Entities;
using Game.Entities.Models;
using Game.Systems.DialogueSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using System.Collections.Generic;

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

		public ContextType type = ContextType.Normal;

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
		private CharacterModel character;
		private IActor actor;

		public CommandTalk(DialogueSystem.DialogueSystem dialogueSystem, CharacterModel character, IActor actor)
		{
			this.dialogueSystem = dialogueSystem;
			this.character = character;
			this.actor = actor;
		}

		public override void Execute()
		{
			//dialogueSystem.StartDialogue(character, actor);
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
		private IInteractable initiator;
		private IContainer container;
		public CommandOpenContainer(IInteractable initiator, IContainer container)
		{
			this.initiator = initiator;
			this.container = container;
		}

		public override void Execute()
		{
			Interactor.ABInteraction(initiator, container);
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
		public AlignmentType Target { get; private set; }
		public AlignmentType Current { get; private set; }
		public AlignmentType Forecast { get; private set; }
		public Vector2 ForecastAlignment { get; private set; }

		private float percentAlignment = 0;
		private Vector2 vectorAlignment;
		private bool usePercents = false;

		public CommandSetAlignment(ISheet sheet, Vector2 vectorAlignment) : base(sheet)
		{
			this.vectorAlignment = vectorAlignment;

			Target = Alignment.ConvertVector2ToAlignment(vectorAlignment);
			Current = (sheet.Characteristics.Alignment as AlignmentCharacteristic).AlignmentType;
			usePercents = false;
			Calculate();

			Forecast = Alignment.ConvertVector2ToAlignment(ForecastAlignment);
		}

		public CommandSetAlignment(ISheet sheet, AlignmentType type, float percentAlignment) : base(sheet)
		{
			this.percentAlignment = percentAlignment;

			Target = type;
			Current = (sheet.Characteristics.Alignment as AlignmentCharacteristic).AlignmentType;
			usePercents = true;
			Calculate();

			Forecast = Alignment.ConvertVector2ToAlignment(ForecastAlignment);
		}

		public override void Execute()
		{
			sheet.Characteristics.Alignment.CurrentValue = ForecastAlignment;
		}

		private void Calculate()
		{
			if (usePercents)
			{
				ForecastAlignment = Vector2.Lerp(sheet.Characteristics.Alignment.CurrentValue, Alignment.ConvertAlignmentToVector2(Target), percentAlignment);
			}
			else
			{
				ForecastAlignment = sheet.Characteristics.Alignment.CurrentValue + vectorAlignment;
			}
		}
	}

	public class CommandAddItems : SheetCommand
	{
		public List<Item> Items { get; private set; }

		public CommandAddItems(ISheet sheet, List<Item> items) : base(sheet)
		{
			this.Items = items;

			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].useRandom)
				{
					Items[i].Randomize();
				}
			}
		}

		public override void Execute()
		{
			for (int i = 0; i < Items.Count; i++)
			{
				sheet.Inventory.Add(Items[i]);
			}
		}
	}

	public class CommandAddExperience : SheetCommand
	{
		public int exp = 1;
		public int level = 0;

		public CommandAddExperience(ISheet sheet, int addExp, int addLevel = 0) : base(sheet)
		{
			exp = addExp;
			level = addLevel;
		}

		public override void Execute()
		{
			sheet.Characteristics.Level.CurrentValue += level;
			sheet.Characteristics.Experience.CurrentValue += exp;
		}

		public bool IsLevelChanged()
		{
			var lastLevel = sheet.Characteristics.Level.CurrentValue;

			return lastLevel != (sheet.Characteristics.Level.CurrentValue + level);
		}
	}
	#endregion


	public enum ContextType
	{
		Normal,
		Negative,
	}
}