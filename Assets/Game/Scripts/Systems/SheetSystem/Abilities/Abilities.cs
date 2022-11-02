using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Abilities
{
	public class Abilities
	{
	}

	[HideLabel]
	[System.Serializable]
	public class AbilitiesSettings
	{
		[ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElement", OnEndListElementGUI = "EndDrawListElement")]
		[AssetSelector(ExpandAllMenuItems = true, IsUniqueList = true, Paths = "Assets/Game/Resources/Assets/Sheet/Abilities")]
		public List<BaseAbility> abilities = new List<BaseAbility>();


		private void BeginDrawListElement(int index)
		{
			GUILayout.BeginHorizontal();
		}

		private void EndDrawListElement(int index)
		{
			if (abilities[index] != null)
			{
				if (abilities[index].name.StartsWith("Base"))
				{
					GUIStyle style = new GUIStyle();
					style.normal.textColor = Color.cyan;
					GUILayout.Label("Base", style);
				}
			}
			GUILayout.EndHorizontal();
		}
	}
}