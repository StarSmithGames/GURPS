using EPOOutline;

using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	public interface IEntity : ISheetable, IPathfinderable
	{
		Transform CameraPivot { get; }

		Markers Markers { get; }
		Outlinable Outlines { get; }

		void Freeze(bool trigger);
	}
}