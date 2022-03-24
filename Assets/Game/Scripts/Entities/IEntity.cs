using Game.Entities;

using UnityEngine;

public interface IEntity
{
	EntityData EntityData { get; }

	Transform Transform { get; }

	NavigationController Navigation { get; }
	CharacterController3D Controller { get; }

	void Freeze();
	void UnFreeze();
}