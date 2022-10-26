using Game.Entities;
using Game.Managers.PartyManager;
using Game.Systems.CombatDamageSystem;
using Game.Systems.DialogueSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using System.Collections.Generic;

using UnityEngine;

namespace Game.Systems.CommandCenter
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
	public class CommandAttack : ContextCommand
	{
		private ICombatable initiator;
		private IDamageable damageable;

		public CommandAttack(ICombatable initiator, IDamageable damageable)
		{
			this.initiator = initiator;
			this.damageable = damageable;
		}

		public override void Execute()
		{
			Combator.ABCombat(initiator, damageable);
		}
	}

	public class CommandTalk : ContextCommand
	{
		private IActor initiator;
		private IActor actor;

		public CommandTalk(IActor initiator, IActor actor)
		{
			this.initiator = initiator;
			this.actor = actor;
		}

		public override void Execute()
		{
			Talker.ABTalk(initiator, actor);
		}
	}

	public class CommandExamine : ContextCommand
	{
		public CommandExamine(IObservable observable)
		{

		}

		public CommandExamine(Item observable)
		{

		}

		public override void Execute()
		{
		}
	}

	public class CommandInteract : ContextCommand
	{
		private IInteractable initiator;
		private IInteractable interactable;

		public CommandInteract(IInteractable initiator, IInteractable interactable)
		{
			this.initiator = initiator;
			this.interactable = interactable;
		}

		public override void Execute()
		{
			Interactor.ABInteraction(initiator, interactable);
		}
	}

	public class CommandCrackingContainer : ContextCommand
	{
		private IInteractable initiator;
		private IContainer container;

		public CommandCrackingContainer(IInteractable initiator, IContainer container)
		{
			this.initiator = initiator;
			this.container = container;
		}

		public override void Execute()
		{
			initiator.ExecuteInteraction(new BaseInteraction(container.InteractionPoint, container.UnLock));
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


	public class CommandConsume : ContextCommand
	{
		private ICharacter character;
		private ConsumableItemData data;

		public CommandConsume(ICharacter character, Item item)
		{
			this.character = character;
			data = item.GetItemData<ConsumableItemData>();
		}

		public override void Execute()
		{
			if(data != null)
			{
				character.Effects.Apply(data.effects);
			}
		}

		public static void Execute(ICharacter character, Item item)
		{
			character.Effects.Apply(item.GetItemData<ConsumableItemData>().effects);
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

	#region PartyManagerCommands
	public interface IPartyManagerCommand : ICommand
	{
		void Execute(Party party);
	}

	public class CommandAddCompanionInPlayerParty : IPartyManagerCommand
	{
		private Character companion;

		public CommandAddCompanionInPlayerParty(Character companion)
		{
			this.companion = companion;
		}

		public void Execute() { }

		public void Execute(Party party)
		{
			party.AddCharacter(companion);
		}
	}
	#endregion

	public enum ContextType
	{
		Normal,
		Negative,
	}
}