using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.SheetSystem
{
	public abstract class EffectData : ScriptableObject
	{
		[HideLabel]
		public Information information;

		[SerializeReference] public List<EffectType> enchantments = new List<EffectType>();
	}


	[System.Serializable]
	public abstract class EffectType { }

	[System.Serializable]
	public sealed class AddHealthPoints : EffectType
	{
		public float add;
	}
}