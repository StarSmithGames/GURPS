using UnityEngine;

namespace Game.Systems.SheetSystem.Effects
{
	[CreateAssetMenu(fileName = "InflictEffect", menuName = "Game/Sheet/Effects/Inflict Effect")]
	public class InflictEffectData : EffectData
	{
		[Space]
		public bool isBlinkOnEnd = true;
	}
}