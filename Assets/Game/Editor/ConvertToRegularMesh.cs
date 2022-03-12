using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class ConvertToRegularMesh : MonoBehaviour
{
	[MenuItem("CONTEXT/SkinnedMeshRenderer/Convert from skinned mesh to regular mesh")]
	static void Convert(MenuCommand command)
	{
		SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)command.context;
		MeshRenderer meshRenderer = skinnedMeshRenderer.gameObject.AddComponent<MeshRenderer>();
		MeshFilter meshFilter = skinnedMeshRenderer.gameObject.AddComponent<MeshFilter>();

		meshRenderer.sharedMaterial = skinnedMeshRenderer.sharedMaterial;
		meshFilter.sharedMesh = skinnedMeshRenderer.sharedMesh;

		DestroyImmediate(skinnedMeshRenderer);
	}
}