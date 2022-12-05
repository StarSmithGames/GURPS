using UnityEngine;
using Sirenix.OdinInspector;

namespace EPOOutline
{
	[CreateAssetMenu(fileName = "OutlineData", menuName = "Game/Outline")]
	public class OutlineData : ScriptableObject
	{
		[HideLabel]
		public Outlinable.Settings settings;
		public OutlineType outlineType;
	}

	public enum OutlineType : int
	{
		Character = 0,
		Enemy = 1,

		Object = 10,

		Selected = 20,

		Target = 30,
	}
}