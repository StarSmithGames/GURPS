using EPOOutline;

using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Entities
{
	public interface IEntity : ISheetable
	{
		Transform Transform { get; }

		Transform CameraPivot { get; }

		NavigationController Navigation { get; }
		CharacterController3D Controller { get; }

		Markers Markers { get; }
		Outlinable Outlines { get; }

		void Freeze(bool trigger);
	}
}