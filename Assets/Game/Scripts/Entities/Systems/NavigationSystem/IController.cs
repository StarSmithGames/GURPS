using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IController
{
	event UnityAction onReachedDestination;

	bool IsHasTarget { get; }
	bool IsGrounded { get; }

	void Freeze(bool trigger);
	void Enable(bool trigger);

	bool SetDestination(Vector3 destination, float maxPathDistance = -1);
	void Stop();

	Vector3 GetVelocity();
}