using Game.Systems.SheetSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Entities
{
	[CreateAssetMenu(fileName = "Model", menuName = "Game/Model")]
	public class ModelData : EntityData
	{
		[PropertyOrder(-1)]
		[HideLabel]
		public ModelInformation information;
	}
}