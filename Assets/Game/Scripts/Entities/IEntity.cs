using UnityEngine;

public interface IEntity
{
	Transform Transform { get; }
	CharacterController3D Controller { get; }
}