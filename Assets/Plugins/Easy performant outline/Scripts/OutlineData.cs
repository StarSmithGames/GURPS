using UnityEngine;
using Sirenix.OdinInspector;

namespace EPOOutline
{
	[CreateAssetMenu(fileName = "OutlineData", menuName = "Data/Outline")]
	public class OutlineData : ScriptableObject
	{
		[HideLabel]
		public Outlinable.Settings settings;
	}
}