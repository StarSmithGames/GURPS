using EPOOutline;

using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	public interface IEntity : ISheetable, IPathfinderable, IInteractable, IObservable, IAnimatable
	{
		Transform CameraPivot { get; }

		Markers Markers { get; }
		Outlinable Outlines { get; }

		void Freeze(bool trigger);
	}
}