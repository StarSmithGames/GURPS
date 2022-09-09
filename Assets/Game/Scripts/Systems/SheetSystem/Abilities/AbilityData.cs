using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.SheetSystem.Abilities
{
	public abstract class AbilityData : ScriptableObject, ICopyable<IAbility>
	{
		public bool isHasIcon = true;
		[HideLabel]
		[PreviewField(ObjectFieldAlignment.Left, Height = 64)]
		[ShowIf("isHasIcon")]
		public Sprite icon;

		public string abilityName;
		public string description;

		public ActivationType activation;

		public abstract IAbility Copy();
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