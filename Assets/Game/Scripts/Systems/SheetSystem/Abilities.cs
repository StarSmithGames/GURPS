using System.Collections.Generic;
using System.Collections;

using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using Sirenix.Utilities;

namespace Game.Systems.SheetSystem
{
	public class Abilities : Registrator<IAbility>
	{

		public Abilities(AbilitiesSettings settings)
		{

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

		//private static IEnumerable GetScriptableObject()
		//{
		//	return UnityEditor.AssetDatabase.FindAssets("t: ScriptableObject")
		//		.Select((x) => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
		//		.Select((x) => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(x)));
		//}
	}
}