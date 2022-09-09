using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
	[CreateAssetMenu(fileName = "AbilityData", menuName = "Game/Sheet/Ability")]
	public class AbilityData : ScriptableObject
	{
		public bool isHasIcon = true;
		[HideLabel]
		[PreviewField(ObjectFieldAlignment.Left, Height = 64)]
		[ShowIf("isHasIcon")]
		public Sprite icon;

		public string abilityName;
		public string description;
		public Requirements requirements;
	}

	[System.Serializable]
	public class Requirements
	{

	}

	[System.Serializable]
	public class Requirement
	{

	}
}