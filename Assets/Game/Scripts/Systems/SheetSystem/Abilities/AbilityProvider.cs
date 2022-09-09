using Game.Entities.Models;
using System;

using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Abilities
{
	public class AbilityProvider : IInitializable, IDisposable
	{
		private ICharacterModel model;
		private Abilities abilities => model.Sheet.Abilities;

		public AbilityProvider(IEntityModel entityModel)
		{
			model = entityModel as ICharacterModel;
		}

		public void Initialize()
		{
			abilities.onCollectionChanged += OnAbilitiesChanged;
		}

		public void Dispose()
		{
			abilities.onCollectionChanged -= OnAbilitiesChanged;
		}



		private void OnAbilitiesChanged()
		{
			Debug.LogError("Update " + (model as MonoBehaviour).name);
		}
	}
}