using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	[CreateAssetMenu(fileName = "Model", menuName = "Game/Model")]
	public class ModelData : EntityData
	{
		public ModelInformation information;
	}
}