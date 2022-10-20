using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
	[CreateAssetMenu(fileName = "Effect", menuName = "Game/Sheet/Effect")]
	public class EffectData : ScriptableObject
	{
		[HideLabel]
		public Information information;
	}
}