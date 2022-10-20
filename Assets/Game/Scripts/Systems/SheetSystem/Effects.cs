using UnityEngine;

namespace Game.Systems.SheetSystem
{
	public sealed class Effects : Registrator<Effect>
	{
	}

	[System.Serializable]
	public abstract class Effect
	{
	}

	[SearchPath("★Effects/HP Effect")]
	[System.Serializable]
	public class HPEffect : Effect
	{
		[SerializeField] public int test;
	}
}