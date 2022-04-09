using EPOOutline;

using Game.Systems.CameraSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Entities
{
	public interface IEntity : ISheetable, IPathfinderable, IInteractable, IObservable, IAnimatable, IDamegeable
	{
		public event UnityAction<IEntity> onDeath;

		GameObject GameObject { get; }

		CameraPivot CameraPivot { get; }

		Markers Markers { get; }
		Outlinable Outlines { get; }

		void Freeze(bool trigger);
	}
}