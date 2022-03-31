using UnityEngine;
using Sirenix.OdinInspector;

namespace EPOOutline
{
	[CreateAssetMenu(fileName = "OutlineData", menuName = "Game/Outline")]
	public class OutlineData : ScriptableObject
	{
		[HideLabel]
		public Outlinable.Settings settings;
	}
}