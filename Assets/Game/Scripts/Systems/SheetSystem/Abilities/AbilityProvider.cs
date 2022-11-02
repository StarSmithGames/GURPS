using Game.Entities.Models;

using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Abilities
{
	public class AbilityProvider : Registrator<IAbility>
	{
		private Abilities abilities => null;//model.Sheet.Abilities;
		private ICharacterModel model;

		private DiContainer container;

		public AbilityProvider(DiContainer container, ICharacterModel characterModel)
		{
			this.container = container;

			model = characterModel;

			var abilities = model.Sheet.Settings.abilities.abilities;

			for (int i = 0; i < abilities.Count; i++)
			{
				var ability = container.InstantiatePrefab(abilities[i], Vector3.zero, Quaternion.identity, model.Transform).GetComponent<IAbility>();

				Registrate(ability);
			}
		}

		public void Activate(IAbility ability)
		{
			//var activation = activationFactory.Create(ability);
			//activation.Activate();
		}

		public override bool Registrate(IAbility ability)
		{
			if (base.Registrate(ability))
			{
				ability.Activate();

				return true;
			}

			return false;
		}
	}
}