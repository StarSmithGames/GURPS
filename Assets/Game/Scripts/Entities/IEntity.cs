using EPOOutline;

using Game.Systems.CameraSystem;
using Game.Systems.DamageSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	public interface IEntity :
		ISheetable, IPathfinderable,
		IInteractable, IObservable,
		IDamegeable, IKillable
	{
		MonoBehaviour MonoBehaviour { get; }

		CameraPivot CameraPivot { get; }
	}
}