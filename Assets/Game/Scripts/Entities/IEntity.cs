using EPOOutline;

using Game.Entities;
using Game.Systems.InventorySystem;

using UnityEngine;

public interface IEntity
{
	EntityData EntityData { get; }
	IInventory Inventory { get; }

	Transform Transform { get; }

	NavigationController Navigation { get; }
	CharacterController3D Controller { get; }

	Markers MarkerController { get; }
	Outlinable Outline { get; }

	void Freeze(bool trigger);
}