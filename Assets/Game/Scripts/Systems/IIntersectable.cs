using System.Collections.Generic;

using UnityEngine;

public interface IIntersectable
{
	List<Collider> Intersections { get; }

	Transform Transform { get; }
	bool IsIntersectsColliders { get; }


	void SetMaterial(Material material);
	void ResetMaterial();
}