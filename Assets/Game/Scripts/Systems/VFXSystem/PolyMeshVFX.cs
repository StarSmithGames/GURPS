using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PolyMeshVFX : MonoBehaviour
{
    public PolygonCollider2D polyCollider;
    public MeshFilter filter;

    [OnValueChanged("Refresh")]
    public float radius;
    [OnValueChanged("Refresh")]
    public int n;

    [Button]
    private void Refresh()
	{
        PolyMesh(radius, n);
    }

    void PolyMesh(float radius, int n)
    {
        Mesh mesh = new Mesh();
        filter.mesh = mesh;

        //verticies
        List<Vector3> verticiesList = new List<Vector3> { };
        float x;
        float y;
        for (int i = 0; i < n; i++)
        {
            x = radius * Mathf.Sin((2 * Mathf.PI * i) / n);
            y = radius * Mathf.Cos((2 * Mathf.PI * i) / n);
            verticiesList.Add(new Vector3(x, y, 0f));
        }
        Vector3[] verticies = verticiesList.ToArray();

        //triangles
        List<int> trianglesList = new List<int> { };
        for (int i = 0; i < (n - 2); i++)
        {
            trianglesList.Add(0);
            trianglesList.Add(i + 1);
            trianglesList.Add(i + 2);
        }
        int[] triangles = trianglesList.ToArray();

        //normals
        List<Vector3> normalsList = new List<Vector3> { };
        for (int i = 0; i < verticies.Length; i++)
        {
            normalsList.Add(-Vector3.forward);
        }
        Vector3[] normals = normalsList.ToArray();

        //initialise
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.normals = normals;

		//polyCollider
		if (polyCollider != null)
		{
			polyCollider.pathCount = 1;

            List<Vector2> pathList = new List<Vector2> { };
            for (int i = 0; i < n; i++)
            {
                pathList.Add(new Vector2(verticies[i].x, verticies[i].y));
            }
            Vector2[] path = pathList.ToArray();

            polyCollider.SetPath(0, path);
		}
	}


	private void OnDrawGizmos()
	{
    }
}