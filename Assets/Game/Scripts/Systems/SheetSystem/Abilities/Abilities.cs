using System.Collections.Generic;

using Sirenix.OdinInspector;

namespace Game.Systems.SheetSystem.Abilities
{
	public class Abilities : Registrator<IAbility>
	{
		public Abilities(AbilitiesSettings settings)
		{
			for (int i = 0; i < settings.baseAbilities.Count; i++)
			{
				Registrate(settings.baseAbilities[i].Copy());
			}
		}
	}

	[HideLabel]
	[System.Serializable]
	public class AbilitiesSettings
	{
		[ListDrawerSettings(Expanded = true)]
		[HorizontalGroup("Abilities")]
		[VerticalGroup("Abilities/Left")]
		public List<AbilityData> baseAbilities = new List<AbilityData>();
		[ListDrawerSettings(Expanded = true)]
		[VerticalGroup("Abilities/Right")]
		public List<AbilityData> abilities = new List<AbilityData>();
	}
}