using EPOOutline;

using Game.Entities;
using Game.Systems.InventorySystem;

using UnityEngine;

public interface IEntity
{
	Transform Transform { get; }

	Transform CameraPivot { get; }

	EntityData EntityData { get; }
	IInventory Inventory { get; }

	NavigationController Navigation { get; }
	CharacterController3D Controller { get; }

	Markers Markers { get; }
	Outlinable Outlines { get; }

	void Freeze(bool trigger);
}