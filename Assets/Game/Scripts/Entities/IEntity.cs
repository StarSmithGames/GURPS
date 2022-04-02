using EPOOutline;

using Game.Entities;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using UnityEngine;

public interface IEntity
{
	Transform Transform { get; }

	Transform CameraPivot { get; }

	ISheet Sheet { get; }

	EntityData EntityData { get; }

	NavigationController Navigation { get; }
	CharacterController3D Controller { get; }

	Markers Markers { get; }
	Outlinable Outlines { get; }

	void Freeze(bool trigger);
}